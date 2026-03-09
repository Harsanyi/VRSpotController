import argparse
import os
import sys
import time
from bosdyn.api import image_pb2

import bosdyn.client
import bosdyn.client.lease
import bosdyn.client.util
import bosdyn.geometry
from bosdyn.api import trajectory_pb2
from bosdyn.api.spot import robot_command_pb2 as spot_command_pb2
from bosdyn.client import math_helpers
from bosdyn.client.frame_helpers import GRAV_ALIGNED_BODY_FRAME_NAME, ODOM_FRAME_NAME, get_a_tform_b
from bosdyn.client.image import ImageClient
from bosdyn.client.robot_command import RobotCommandBuilder, RobotCommandClient, blocking_stand
from bosdyn.client.robot_state import RobotStateClient
from bosdyn.util import seconds_to_duration
from bosdyn.client.util import authenticate

import bosdyn.client
from bosdyn.client.image import ImageClient
from bosdyn.api import image_pb2
import numpy as np
#import cv2
from bosdyn.client.robot_command import (
    RobotCommandClient,
    RobotCommandBuilder,
    blocking_stand,
    blocking_sit
)
import bosdyn.client.lease
from bosdyn.client.lease import LeaseKeepAlive
from bosdyn.client.power import PowerClient, power_on_motors
from bosdyn.client.robot_command import RobotCommandBuilder
from bosdyn.util import seconds_to_duration
import time
from vector2 import vector2
from bosdyn.api import robot_state_pb2
from spotState import *
from bosdyn.api import power_pb2

class Spot:
    SPOT_IP = "***REMOVED***"
    #SPOT_IP = "***REMOVED***"
    USERNAME = "***REMOVED***"
    PASSWORD = "***REMOVED***"

    robot = None
    lease = None
    lease_alive = None
    timeout = 10
    lease_cl = None
    
    def GetRobot(self):
        if self.robot != None:
            return self.robot
        
        sdk = bosdyn.client.create_standard_sdk("SpotImageClient")
        self.robot = sdk.create_robot(self.SPOT_IP)
        self.robot.authenticate(self.USERNAME, self.PASSWORD)
        self.robot.time_sync.wait_for_sync()
        print("Robot Loggged in.")
        return self.robot

    def GetLease(self):
        if self.lease != None:
            return self.lease
        
        self.GetRobot()
        self.lease_cl = self.robot.ensure_client(bosdyn.client.lease.LeaseClient.default_service_name)
        try:
            self.lease = self.lease_cl.acquire(resource='body')
        except bosdyn.client.lease.ResourceAlreadyClaimedError:
            self.lease = self.lease_cl.take(resource='body')
        
        print("Lease Created.")
        return self.lease
    
    def PowerOn(self):
        self.GetLeaseAlive()
        power_cl = self.robot.ensure_client(PowerClient.default_service_name)
        power_on_motors(power_cl, timeout_sec=self.timeout)
    
    def GetLeaseAlive(self):
        if self.lease_alive != None:
            return self.lease_alive
        
        self.GetLease()
        self.lease_alive = LeaseKeepAlive(self.lease_cl, return_at_exit=True)
        print("Get Lease Alive.")

    def DropLease(self):
        if self.lease_alive == None:
            return
        
        self.lease_alive.shutdown()
        self.lease_alive = None
        self.lease = None
        print("Drop Lease.")

    def GetState(self) -> SpotState:
        baseState = self.GetBaseState()

        state : SpotState = SpotState()
        state.battery = Spot.GetBattery(baseState)
        state.poseState = Spot.GetPoseState(baseState)
        state.powerState = Spot.GetPowerState(baseState)
        
        pos = Spot.GetPosition(baseState)
        state.posX = pos.x
        state.posY = pos.y
        state.posZ = pos.z
        
        #state.rotation = Spot.GetRotation(baseState)
        return state

    def GetBaseState(self):
        self.GetRobot()

        state_client = self.robot.ensure_client(RobotStateClient.default_service_name)
        state = state_client.get_robot_state()
        return state

    @staticmethod
    def GetBattery(state) -> float:
        return state.battery_states[0].charge_percentage.value
    
    @staticmethod
    def GetPowerState(state):
        return state.power_state.motor_power_state
    
    @staticmethod
    def GetPoseState(state):
        return state.behavior_state.state
    
    @staticmethod
    def GetTransform(state):
        snapshot = state.kinematic_state.transforms_snapshot
        return snapshot.child_to_parent_edge_map["vision"].parent_tform_child

    @staticmethod
    def GetRotation(state):
        return Spot.GetTransform(state).rotation
        
    @staticmethod
    def GetPosition(state):
        return Spot.GetTransform(state).position

    def GetImages(self, source:list[str]):
        self.GetRobot()
        client = self.robot.ensure_client(ImageClient.default_service_name)
        image_responses = client.get_image_from_sources(source)

        images:list = []
        for resp in image_responses:
            img = resp.shot.image
            images.append(img)

        return images
    
    def ListCameras(self):
        self.GetRobot()
        client = self.robot.ensure_client(ImageClient.default_service_name)
        sources = client.list_image_sources()
        return [str(source.name) for source in sources]

    @staticmethod
    def SaveImage(img, path):
        if img.format == image_pb2.Image.FORMAT_JPEG:
            with open(path, "wb") as f:
                f.write(img.data)
            print(f"Image Saved: {path}")

    def GetCommandClient(self):
        self.GetLeaseAlive()
        return self.robot.ensure_client(RobotCommandClient.default_service_name)

    def StandUp(self):
        cmd = self.GetCommandClient()
        blocking_stand(cmd, timeout_sec=self.timeout)
        print("Spot Standing.")

    def SitDown(self):
        cmd = self.GetCommandClient()
        blocking_sit(cmd, timeout_sec=self.timeout)
        print("Spot Sitting.")
        
    def Move(self, delta:vector2, speed):
        self.GetLeaseAlive()

        cmd_cl = self.GetCommandClient()
        cmd = RobotCommandBuilder.synchro_velocity_command(
            v_x=delta.x,
            v_y=delta.y,
            v_rot=0,
            params=None,
            locomotion_hint=spot_command_pb2.HINT_AUTO
        )

        duration = delta.Magnitude() / speed
        try:
            cmd_cl.robot_command(cmd, end_time_secs=time.time()+duration)
            time.sleep(duration)

            stopCmd = RobotCommandBuilder.synchro_velocity_command(
                v_x=0,
                v_y=0,
                v_rot=0,
                params=None,
                locomotion_hint=spot_command_pb2.HINT_AUTO
            )
            cmd_cl.robot_command(stopCmd)
        except Exception as e:
            print(f"Move error: {e}")
            try:
                stop_cmd = RobotCommandBuilder.synchro_velocity_command(v_x=0.0, v_y=0.0, v_rot=0.0)
                cmd_cl.robot_command(stop_cmd)
            except:
                pass
            return False
        
class ImgSource:
    BACK = "back_fisheye_image"
    FRONT_LEFT = "frontleft_fisheye_image"
    FRONT_RIGHT = "frontright_fisheye_image"
    LEFT = "left_fisheye_image"
    RIGHT = "right_fisheye_image"
    HAND = "hand_color_image"

    ALL = [BACK,FRONT_LEFT,FRONT_RIGHT,LEFT,RIGHT,HAND]

    ROTATION_ANGLE = {
        BACK: 0,
        FRONT_LEFT: -78,
        FRONT_RIGHT: -102,
        LEFT: 0,
        RIGHT: 180
    }


    