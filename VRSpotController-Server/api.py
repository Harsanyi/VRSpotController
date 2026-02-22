from flask import Flask, request
from datetime import datetime
from models import User

app = Flask(__name__)

@app.route('/')
def home():
    return 'Server is running!'

@app.route('/time')
def getTime():
    now = datetime.now()
    print(now)
    return str(now)

@app.route('/hello', methods=['POST'])
def hello():
    data = User(**request.json)
    print(data.name)
    return data.name

