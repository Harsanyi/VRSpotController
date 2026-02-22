from flask import Flask, request
from requests import *
from functions import *
from responses import Response
from ends import END

app = Flask(__name__)

@app.route(END.HOME)
def home():
    return 'Server is running!'

@app.route(END.HELLO, methods=['POST'])
def hello():
    print("Got hello request!")
    data = HelloRequest(**request.json)
    resp = Response()
    resp.success = True
    resp.message = f"Hello, {data.name}!"
    print("Sending response:"+resp.message)
    return resp.str()

printHostName()
app.run(host="127.0.0.1", port=5000, debug=True)


