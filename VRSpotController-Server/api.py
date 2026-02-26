from flask import Flask, request
from requests import *
from functions import *
from responses import Response
from ends import END
from spot import *

app = Flask(__name__)

@app.route(END.HOME)
def home():
    return 'Server is running!'

@app.route(END.HELLO, methods=['POST'])
def hello():
    print("Got hello request:"+str(request.json))
    data = HelloRequest(**request.json)
    resp = Response(True, f"Hello, {data.name}")
    print("Sending response:"+resp.message)
    return resp.str()

@app.route(END.MOVE, methods=['POST'])
def move():
    data = MoveRequest(**request.json)
    resp = Response(True, f"Moving to {data.move.x}, {data.move.y}!")
    return resp.str()

printHostName()
app.run(host="127.0.0.1", port=5000, debug=True)


