from vector2 import vector2

class HelloRequest:
    name : str

    def __init__(self, name : str):
        self.name = name

class MoveRequest:
    move : vector2

    def __init__(self, move):
        self.move = vector2(move['x'], move['y'])