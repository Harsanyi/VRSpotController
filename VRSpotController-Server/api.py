from flask import Flask, make_response, request
from requests import *
from server import *
from responses import Response
from ends import END
from spot import *
import logging
from flask.json.provider import DefaultJSONProvider
from dataclasses import is_dataclass, asdict

app = Flask(__name__)
log = logging.getLogger('werkzeug')
log.setLevel(logging.ERROR)

server = Server(False)

class JSONProvider(DefaultJSONProvider):
    def default(self,obj):
        if is_dataclass(obj):
            return asdict(obj)
        return super().default(obj)
    
app.json_provider_class = JSONProvider

@app.route(END.HOME)
async def home():
    return 'Server is running!'

@app.route(END.HELLO, methods=['POST'])
async def hello():
    req = HelloRequest(**request.json)
    return server.hello(req).str()

@app.route(END.MOVE, methods=['POST'])
async def move():
    req = MoveRequest(**request.json)
    return server.move(req).str()

@app.route(END.STAND, methods=["POST"])
async def stand():
    req = StandRequest(**request.json)
    return server.stand(req).str()

@app.route(END.POWER, methods=["POST"])
async def powerOn():
    req = PowerOnRequest(**request.json)
    return server.powerOn(req).str()

@app.route(END.IMAGE, methods=["POST"])
async def getImage():
    req = ImageRequest(**request.json)
    img = server.getImage(req)
    if(img == None):
        return Response(False,"Image not got!").str()
    
    resp = make_response(img)
    resp.headers.set('Content-Type', 'image/jpeg')
    resp.headers.set(
        'Content-Disposition', 'attachment', filename=f'{req.source}.jpg')
    return resp

@app.route(END.STATE, methods=["POST"])
async def getState():
    return server.getState().str()

server.printHostName()
app.run(host="127.0.0.1", port=5000, debug=True)


