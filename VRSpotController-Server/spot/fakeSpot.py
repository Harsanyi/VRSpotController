from spot.spotState import *
import time
from math import sin
from numpy import *
from bosdyn.client.math_helpers import *

class FakeSpot():
    debugImages = {}
    pose : int = 0
    power : int = 0
    position : Vec3 = Vec3(0,0,0)
    rotation : Quat = Quat(0,0,0,1)
    armPosition : Vec3 = Vec3(0,0,0)
    armRotation : Quat = Quat(0,0,0,1)

    def __init__(self):
        self.loadDebugImages()

    def loadDebugImage(self,name:str):
        try:
            with open(name+".jpg", "rb") as f:
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
        img = self.debugImages[source]
        if img is None:
            print(f"No debug image for source: {source}")
            return None
        return img
    
    def GetBattery(self):
        return sin(time.time()*5)*0.5+0.5
    
    def GetState(self) -> SpotState:
        state : SpotState = SpotState()
        state.battery = self.GetBattery()
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