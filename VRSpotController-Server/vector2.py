import math

class vector2:
    x : float
    y : float

    def __init__(self, x, y):
        self.x = x
        self.y = y

    def Magnitude(self):
        return math.sqrt(self.x*self.x + self.y*self.y)