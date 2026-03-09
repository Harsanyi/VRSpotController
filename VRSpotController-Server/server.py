import socket
from requests import *
from responses import *
from spot import *
from spotState import *

class Server:
    spot : Spot = None
    debug : bool = False
    debugImages = {}

    def __init__(self, debug:bool=False):
        self.spot = Spot()
        self.debug = debug

        if self.debug:
            self.loadDebugImages()

    def loadDebugImage(self,name:str):
        try:
            with open(name+".jpg", "rb") as f:
                Server.debugImages[name] = f.read()
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

    def printHostName(self):
        hostname = socket.gethostname()
        print("Hostname:", hostname)


    def hello(self,req:HelloRequest):
        resp = Response(True, f"Hello, {req.name}")
        return resp
    
    def move(self,req:MoveRequest):
        if self.debug:
            resp = Response(True, f"Moving to {req.move.x}, {req.move.y}!")
            return resp

        try:
            self.spot.Move(req.move,1)
            resp = Response(True, f"Moving to {req.move.x}, {req.move.y}!")
        except Exception as e:
            resp = Response(False, f"Error:{e}")
        return resp
    
    def stand(self,req:StandRequest):
        print(f"Got stand request. Stand:{req.stand}")

        if self.debug:
            resp = Response(True, f"Spot now standing.")
            return resp

        try:
            self.spot.StandUp() if req.stand else self.spot.SitDown()
            resp = Response(True, "Spot now standing.")
        except Exception as e:
            resp = Response(False, f"Error:{e}")

        return resp
    
    def powerOn(self,req:PowerOnRequest):
        print(f"Got Power on request:{req.powerOn}")

        if self.debug:
            resp = Response(True, f"Spot now powered.")
            return resp

        try:
            self.spot.PowerOn() if req.powerOn else self.spot.DropLease()
            resp = Response(True, "Spot now powered.")
        except Exception as e:
            resp = Response(False, f"Error:{e}")
        return resp
    
    def getImage(self, req:ImageRequest):
        if self.debug:
            if req.source in Server.debugImages:
                return Server.debugImages[req.source]
            else:
                print(f"Debug image {req.source} not found.")
                return None

        try:
            imgs = self.spot.GetImages([req.source])
            return imgs[0].data
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

