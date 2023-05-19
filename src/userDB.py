from flask import Flask, jsonify, request
from flask_restx import Resource, Api, Namespace
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

from dbModule import Database, userdb

UserDB = Namespace(
    name="UserDB",
    description="user db 중계 api",
)

@UserDB.route('/load_settings')
class Load_Settings(Resource):
    def post(self):
        db = Database(userdb)
        # Get JSON data from request
        data = request.get_json()

        sql = "SELECT wallet_id, nickname from user where wallet_id=%s"
        row = db.executeAll(sql, data['wallet_id'])
        
        print(row)

        return row
    
@UserDB.route('/save_settings')
class Save_Settings(Resource):
    def post(self):
        db = Database(userdb)
        # Get JSON data from request
        data = request.get_json()

        sql = ""
        db.execute(sql)
        db.commit()

        return "success"