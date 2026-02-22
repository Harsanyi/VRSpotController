import json

class Response:
    success : bool = True
    message : str = ""

    def str(self):
        return json.dumps(self.__dict__)