from flask import Flask
from flask_restx import Resource, Api

from relay import Relay
from gameDB import GameDB
from nftDB import NftDB
from logDB import LogDB
app = Flask(__name__)
api = Api(app)

api.add_namespace(Relay, '/send')
api.add_namespace(GameDB, '/gamedb')
api.add_namespace(NftDB, '/nftdb')
api.add_namespace(LogDB, '/logdb')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5000', debug=True)
