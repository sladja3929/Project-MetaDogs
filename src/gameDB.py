from flask import Flask, jsonify, request
from flask_restx import Resource, Api, Namespace
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

from dbModule import Database, gamedb

GameDB = Namespace(
    name="GameDB",
    description="게임 db 중계 api",
)

@GameDB.route('/load_settings')
class Load_Settings(Resource):
    def post(self):
        # Get JSON data from request
        data = request.get_json()

        sql = "SELECT savefile"
        row = Database.executeALL(sql)

        return row
    
@GameDB.route('/save_settings')
class Save_Settings(Resource):
    def post(self):
        # Get JSON data from request
        data = request.get_json()

        sql = ""
        Database.execute(sql)
        Database.commit()

        return "success"