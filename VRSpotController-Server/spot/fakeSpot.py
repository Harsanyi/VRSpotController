from spot.spotState import SpotState
import time
from math import sin
from pathlib import Path
from bosdyn.client.math_helpers import Vec3, Quat

from enum import Enum

from vector import vector2, vector3, vector4

class PowerState(Enum):
    UNKNOWN = 0
    OFF = 1
    ON = 2
    POWERING_ON = 3
    POWERING_OFF = 4
    ERROR = 5

class Pose(Enum):
    UNKNOWN = 0
    NOT_READY = 1
    TRANSITION = 2
    STANDING = 3
    STEPPING = 4

class FakeSpot():
    speed : float = 0.1

    def __init__(self):
        self.debugImages = {}
        self.pose = Pose.NOT_READY.value
        self.power = PowerState.OFF.value
        self.standing = Pose.NOT_READY.value
        self.moving = False
        self.position = Vec3(0, 0, 0)
        self.rotation = Quat(w=1, x=0, y=0, z=0)
        self.armPosition = Vec3(0, 0, 0)
        self.armRotation = Quat(w=1, x=0, y=0, z=0)
        self.gripper = 0.0
        self.loadDebugImages()

    def loadDebugImage(self,name:str):
        try:
            with (Path(__file__).resolve().parent.parent / (name + ".jpg")).open("rb") as f:
                self.debugImages[name] = f.read()
                print(f"Loaded debug image: {name}")
        except Exception as e:
            print(f"Error loading debug image {name}: {e}")

    def loadDebugImages(self):
        self.loadDebugImage("back_fisheye_image")
        self.loadDebugImage("frontleft_fisheye_image")
        self.loadDebugImage("frontright_fisheye_image")
        self.loadDebugImage("left_fisheye_image")
        self.loadDebugImage("right_fisheye_image")
        self.loadDebugImage("hand_color_image")

    def GetImage(self, source:str):
        img = self.debugImages.get(source)
        if img is None:
            print(f"No debug image for source: {source}")
            return None
        return img
    
    def GetBattery(self):
        return int((sin(time.time() * 0.2) * 0.5 + 0.5) * 100)
    
    def GetState(self) -> SpotState:
        state : SpotState = SpotState()
        state.battery = self.GetBattery()
        self.UpdatePose()
        state.poseState = self.pose
        state.powerState = self.power
        
        state.posX = self.position.x
        state.posY = self.position.y
        state.posZ = self.position.z

        state.rotX = self.rotation.x
        state.rotY = self.rotation.y
        state.rotZ = self.rotation.z
        state.rotW = self.rotation.w

        state.armPosX = self.armPosition.x
        state.armPosY = self.armPosition.y
        state.armPosZ = self.armPosition.z

        state.armRotW = self.armRotation.w
        state.armRotX = self.armRotation.x
        state.armRotY = self.armRotation.y
        state.armRotZ = self.armRotation.z

        return state
    
    def PowerOn(self):
        if(self.power == PowerState.OFF.value):
            self.power = PowerState.POWERING_ON.value
            time.sleep(2)
            self.power = PowerState.ON.value
        else:
            print("PowerOn called but Spot is not in OFF state. "+str(self.power))             
                
    def DropLease(self):
        if(self.power == PowerState.ON.value):
            self.power = PowerState.POWERING_OFF.value
            time.sleep(2)
            self.power = PowerState.OFF.value
        else:
            print("DropLease called but Spot is not in ON state. "+str(self.power))

    def UpdatePose(self):
        if self.moving:
            self.pose = Pose.STEPPING.value
            return 
        self.pose = self.standing

    def StandUp(self):
        if self.standing == Pose.STANDING.value:
            return
        
        self.standing = Pose.TRANSITION.value
        time.sleep(2)
        self.standing = Pose.STANDING.value
      
    def SitDown(self):
        if self.standing == Pose.NOT_READY.value:
            return
        self.standing = Pose.TRANSITION.value
        time.sleep(2)
        self.standing = Pose.NOT_READY.value

    def Move(self, delta:vector2, yaw:float):
        self.position = Vec3(self.position.x + delta.x * self.speed,
                             self.position.y + delta.y * self.speed, self.position.z)
        self.rotation = Quat.from_yaw(yaw * self.speed) * self.rotation
        self.moving = delta.Magnitude() > 0.01 or abs(yaw) > 0.01

    def Pitch(self, pitch:float):
        self.rotation = Quat.from_pitch(pitch) * self.rotation

    def SetArm(self, pos:vector3, rot:vector4):
        self.armPosition = pos
        self.armRotation = Quat(w=rot.w, x=rot.x, y=rot.y, z=rot.z)

    def SetGripper(self, open:float):
        self.gripper = max(0.0, min(open, 1.0))
