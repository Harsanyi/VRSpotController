import json
from spot.spotState import SpotState

class Response:
    success : bool = True
    message : str = ""

    def __init__(self, success : bool = True, message : str = ""):
        self.success = success
        self.message = message

    def str(self):
        return json.dumps(self.__dict__)
    
class ImageData:
    data : bytes
    name : str

    def __init__(self, data:bytes, name:str):
        self.data = data
        self.name = name

class ImageResponse:
    images : ImageData

class StateResponse(Response):
    state : SpotState
