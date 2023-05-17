from flask import Flask
from flask_restx import Resource, Api

from relay import Relay
from database import Database
app = Flask(__name__)
api = Api(app)

api.add_namespace(Relay, '/send')
api.add_namespace(Database, '/db')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5000', debug=True)
