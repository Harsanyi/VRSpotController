import socket
from requests import *
from responses import *

class server:
    def printHostName():
        hostname = socket.gethostname()
        print("Hostname:", hostname)

    def hello(req:HelloRequest):
        resp = Response(True, f"Hello, {req.name}")
        return resp
    
    def move(req:MoveRequest):
        resp = Response(True, f"Moving to {req.move.x}, {req.move.y}!")
        return resp
