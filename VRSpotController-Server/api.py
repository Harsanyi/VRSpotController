from flask import Flask, request
from requests import *
from server import *
from responses import Response
from ends import END
from spot import *

app = Flask(__name__)

server = Server()

@app.route(END.HOME)
def home():
    return 'Server is running!'

@app.route(END.HELLO, methods=['POST'])
def hello():
    req = HelloRequest(**request.json)
    return server.hello(req).str()

@app.route(END.MOVE, methods=['POST'])
def move():
    req = MoveRequest(**request.json)
    return server.move(req).str()

@app.route(END.STAND, methods=["POST"])
def stand():
    req = StandRequest(**request.json)
    return server.stand(req).str()

@app.route(END.POWER, methods=["POST"])
def powerOn():
    req = PowerOnRequest(**request.json)
    return server.powerOn(req).str()

server.printHostName()
app.run(host="127.0.0.1", port=5000, debug=True)


