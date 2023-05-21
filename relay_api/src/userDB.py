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

@UserDB.route('/load_pet_list')
class Load_Pet_List(Resource):
    def post(self):
        db = Database()
        # Get JSON data from request
        data = request.get_json()

        sql = "SELECT wallet_id, nickname FROM user.user WHERE wallet_id=%s"
        row = db.executeOne(sql, data['wallet_id'])
        
        #user db에 지갑 주소가 없을 경우 (첫 접속)
        if row is None:
            sql = "INSERT INTO user.user(wallet_id, nickname) values(%s ,'a')"
            db.execute(sql, data['wallet_id'])
            db.commit()

        #해당 지갑 주소로 메타랜드에 있는 사용자 NFT 목록 db에 갱신
        #미구현

        #지갑 주소로 펫 NFT 목록 가져옴
        sql = "SELECT pet_token FROM nft.pet WHERE wallet_id=%s"
        row = db.executeAll(sql, data['wallet_id'])
        
        #펫이 없을 경우
        if db.cursor.rowcount == 0:
            return -1

        return row

@UserDB.route('/load_settings')
class Load_Settings(Resource):
    def post(self):
        db = Database()
        # Get JSON data from request
        data = request.get_json()

        sql = "SELECT ui_save, object_save FROM user.gamedata WHERE wallet_id=%s"
        row = db.executeOne(sql, data['wallet_id'])
        
        if row is None:
            return "no data"

        return row
    
@UserDB.route('/load_pet_property')
class Load_Pet_Property(Resource):
    def post(self):
        db = Database()
        # Get JSON data from request
        data = request.get_json()

        sql = "SELECT pet_name, pet_age, pet_sex FROM nft.pet WHERE pet_token=%s"
        row = db.executeOne(sql, data['pet_token'])
        
        if row is None:
            return "no data"

        return row

@UserDB.route('/test')
class Test(Resource):
    def post(self):
        db = Database()

        # Get data from request
        file = request.files['file']
        json_data = request.form['data']
            
        if file:
            sql = "UPDATE log.aimodel SET model=%s WHERE pet_token=%s" 
            row = db.execute(sql, (file.read(), 1234))
            db.commit()

        #print(type(file))

        return "good"

@UserDB.route('/save_settings')
class Save_Settings(Resource):
    def post(self):
        db = Database()

        # Get data from request
        file = request.files['file']
        data = request.form['wallet_id']
            
        if file:
            sql = "UPDATE user.gamedata SET ui_save=%s WHERE wallet_id=%s" 
            row = db.execute(sql, (file.read(), data))
            db.commit()

        print(data)

        return "good"