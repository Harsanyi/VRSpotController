import socket
from requests import *
from responses import *
from spot import *
from spot.fakeSpot import FakeSpot
from spot.spot import *

class Server:
    spot : Spot = None

    def __init__(self, debug:bool=False):
        self.spot = FakeSpot() if debug else Spot()

    def printHostName(self):
        hostname = socket.gethostname()
        print("Hostname:", hostname)


    def hello(self,req:HelloRequest):
        resp = Response(True, f"Hello, {req.name}")
        return resp
    
    def move(self,req:MoveRequest):
        try:
            self.spot.Move(req.move,req.yaw)
            resp = Response(True, f"Moving to {req.move.x}, {req.move.y}!")
        except Exception as e:
            resp = Response(False, f"Error:{e}")
        return resp
    
    def pitch(self, req:PitchRequest):
        try:
            self.spot.Pitch(req.pitch)
            resp = Response(True, f"Pitch to {req.pitch}!")
        except Exception as e:
            resp = Response(False, f"Error:{e}")
        return resp
    
    def stand(self,req:StandRequest):
        print(f"Got stand request. Stand:{req.stand}")

        try:
            self.spot.StandUp() if req.stand else self.spot.SitDown()
            resp = Response(True, "Spot now standing.")
        except Exception as e:
            resp = Response(False, f"Error:{e}")

        return resp
    
    def powerOn(self,req:PowerOnRequest):
        print(f"Got Power on request:{req.powerOn}")

        try:
            self.spot.PowerOn() if req.powerOn else self.spot.DropLease()
            resp = Response(True, "Spot now powered.")
        except Exception as e:
            resp = Response(False, f"Error:{e}")
        return resp
    
    def getImage(self, req:ImageRequest):
        try:
            return self.spot.GetImage(req.source)
        except Exception as e:
            print(e)
            return None
            
    def getState(self):
        resp : StateResponse = StateResponse()
        try:
            state = self.spot.GetState()
            resp.state = state.__dict__
        except Exception as e:
            print(e)
        finally:
            return resp
        
    def setGrip(self, req:GripRequest):
        try:
            self.spot.SetGripper(req.open)
            return Response(True, f"Grip set to {req.open}")
        except Exception as e:
            print(e)
            return Response(False, "Grip failed to set.")
        
    def setArm(self, req:ArmRequest):
        try:
            self.spot.SetArm(vector3(req.posX, req.posY, req.posZ),vector4(req.rotX, req.rotY, req.rotZ, req.rotW))
            return Response(True, f"Arm setted to location.")
        except Exception as e:
            print(e)
            return Response(False, e)

