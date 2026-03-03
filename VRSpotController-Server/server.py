import socket
from requests import *
from responses import *
from spot import *

class Server:
    spot : Spot = None

    def __init__(self):
        self.spot = Spot()

    def printHostName(self):
        hostname = socket.gethostname()
        print("Hostname:", hostname)

    def hello(self,req:HelloRequest):
        resp = Response(True, f"Hello, {req.name}")
        return resp
    
    def move(self,req:MoveRequest):
        try:
            self.spot.Move(req.move,1)
            resp = Response(True, f"Moving to {req.move.x}, {req.move.y}!")
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
        print(f"Got image request {req.source}")
        try:
            imgs = self.spot.GetImages([req.source])
            return imgs[0].data
        except Exception as e:
            print(e)
            return None
    
    def getImages(self):
        try:
            imgs = self.spot.GetImages(ImgSource.ALL)
            imgDatas : ImageData = []
            for i in range(0,len(imgs)):
                data = ImageData(imgs[i].data,f"{ImgSource.ALL[i]}.jpg")
                imgDatas.append(data)
        
        except Exception as e:
            print(e)

