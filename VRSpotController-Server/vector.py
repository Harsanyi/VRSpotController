import math

class vector2:
    x : float
    y : float

    def __init__(self, x, y):
        self.x = x
        self.y = y

    def Magnitude(self):
        return math.sqrt(self.x*self.x + self.y*self.y)
    
class vector3:
    x : float
    y : float
    z : float

    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z

    def Magnitude(self):
        return math.sqrt(self.x*self.x + self.y*self.y + self.z*self.z) 
    
class vector4:
    x : float
    y : float
    z : float
    w : float

    def __init__(self, x, y, z, w):
        self.x = x
        self.y = y
        self.z = z
        self.w = w

    def Magnitude(self):
        return math.sqrt(self.x*self.x + self.y*self.y + self.z*self.z + self.w * self.w) 