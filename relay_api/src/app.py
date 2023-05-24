from flask import Flask
from flask_restx import Resource, Api

from relayMetaLand import RelayMetaLand
from relayDatabase import RelayDatabase

app = Flask(__name__)
api = Api(app)

api.add_namespace(RelayMetaLand, '/metaland')
api.add_namespace(RelayDatabase, '/db')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5000', debug=True)
