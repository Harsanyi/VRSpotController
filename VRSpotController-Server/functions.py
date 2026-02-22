import socket

def printHostName():
    hostname = socket.gethostname()
    print("Hostname:", hostname)