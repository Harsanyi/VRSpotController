import argparse
import os

from flask import Flask, make_response, request
from api_requests import (
    HelloRequest, MoveRequest, PitchRequest, StandRequest, PowerOnRequest,
    ImageRequest, GripRequest, ArmRequest,
)
from ends import END
from responses import Response
from server import Server


def create_app(debug=False):
    """Create the HTTP bridge; debug selects the hardware-free mock backend."""
    app = Flask(__name__)
    server = Server(debug=debug)

    @app.route(END.HOME)
    def home():
        return 'Server is running!'

    @app.route(END.HELLO, methods=['POST'])
    def hello():
        return server.hello(HelloRequest(**request.json)).str()

    @app.route(END.MOVE, methods=['POST'])
    def move():
        return server.move(MoveRequest(**request.json)).str()

    @app.route(END.PITCH, methods=['POST'])
    def pitch():
        return server.pitch(PitchRequest(**request.json)).str()

    @app.route(END.STAND, methods=['POST'])
    def stand():
        return server.stand(StandRequest(**request.json)).str()

    @app.route(END.POWER, methods=['POST'])
    def power_on():
        return server.powerOn(PowerOnRequest(**request.json)).str()

    @app.route(END.IMAGE, methods=['POST'])
    def get_image():
        req = ImageRequest(**request.json)
        img = server.getImage(req)
        if img is None:
            return Response(False, 'Image unavailable.').str(), 404
        resp = make_response(img)
        resp.headers.set('Content-Type', 'image/jpeg')
        resp.headers.set('Content-Disposition', 'attachment', filename=f'{req.source}.jpg')
        return resp

    @app.route(END.STATE, methods=['POST'])
    def get_state():
        return server.getState().str()

    @app.route(END.GRIP, methods=['POST'])
    def set_grip():
        return server.setGrip(GripRequest(**request.json)).str()

    @app.route(END.ARM, methods=['POST'])
    def set_arm():
        return server.setArm(ArmRequest(**request.json)).str()

    return app


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Unity to Spot HTTP bridge')
    parser.add_argument('--mock', action='store_true', help='Use FakeSpot without robot hardware')
    args = parser.parse_args()
    app = create_app(debug=args.mock)
    debug = os.getenv('FLASK_DEBUG', '0').lower() in ('1', 'true', 'yes')
    app.run(host='0.0.0.0', port=5000, debug=debug, use_reloader=False)