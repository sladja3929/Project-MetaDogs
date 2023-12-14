

from flask import Flask
from flask_restx import Resource, Api

from mlagent import MLAgent

app = Flask(__name__)
api = Api(app)

api.add_namespace(MLAgent, '/mlagent')


if __name__ == '__main__':
    app.run(host='0.0.0.0', port='2000', debug=True)