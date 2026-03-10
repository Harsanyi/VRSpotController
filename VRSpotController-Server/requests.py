from vector import *

class HelloRequest:
    name : str

    def __init__(self, name : str):
        self.name = name

class MoveRequest:
    move : vector2
    yaw : float

    def __init__(self, move, yaw):
        self.move = vector2(move['x'], move['y'])
        self.yaw = yaw

class PitchRequest:
    pitch : float

    def __init__(self, pitch):
        self.pitch = pitch

class YawRequest:
    value : float
    
    def __init__(self, value):
        self.value = value

class StandRequest:
    stand : bool

    def __init__(self, stand:bool):
        self.stand = stand

class PowerOnRequest:
    powerOn : bool

    def __init__(self, powerOn:bool):
        self.powerOn = powerOn

class ImageRequest:
    source : str

    def __init__(self, source:str):
        self.source = source

class GripRequest:
    open : float

    def __init__(self, open:float):
        self.open = open

class ArmRequest:
    posX : float
    posY : float
    posZ : float
    rotX : float
    rotY : float
    rotZ : float
    rotW : float

    def __init__(self, posX:float, posY:float, posZ:float, rotX:float, rotY:float, rotZ:float, rotW:float):
        self.posX = posX
        self.posY = posY
        self.posZ = posZ
        self.rotX = rotX
        self.rotY = rotY
        self.rotZ = rotZ
        self.rotW = rotW