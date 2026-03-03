from vector2 import vector2

class HelloRequest:
    name : str

    def __init__(self, name : str):
        self.name = name

class MoveRequest:
    move : vector2

    def __init__(self, move):
        self.move = vector2(move['x'], move['y'])

class StandRequest:
    stand : bool

    def __init__(self, stand:bool):
        self.stand = stand

class PowerOnRequest:
    powerOn : bool

    def __init__(self, powerOn:bool):
        self.powerOn = powerOn