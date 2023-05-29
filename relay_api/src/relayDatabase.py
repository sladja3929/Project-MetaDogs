from flask import Flask, jsonify, request, Response
from flask_restx import Resource, Api, Namespace
import requests
from requests_toolbelt.multipart.encoder import MultipartEncoder
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry
from io import BytesIO, StringIO

from dbModule import Database

RelayDatabase = Namespace(
    name="RelayDatabase",
    description="데이터베이스 중계 api",
)

@RelayDatabase.route('/login')
class First_Login(Resource):
    def post(self):
        db = Database()
        #get JSON data
        data = request.get_json()

        sql = "SELECT wallet_id FROM user.user WHERE wallet_id=%s"
        row = db.executeOne(sql, data['wallet_id'])

        #nft 데이터가 없을 경우
        if row is None:
            return "no nft data"
        
        sql = "SELECT nickname FROM user.user WHERE wallet_id=%s"
        row = db.executeOne(sql, data['wallet_id'])

        #nft 데이터는 있지만 첫 접속일 경우
        if row is None:
            file = open("save.txt", "r")
            sql = "INSERT INTO user.gamedata(wallet_id, ui_save) \
                VALUES(%s, %s)"
            db.execute(sql, (data['wallet_id'], file.read()))
            db.commit()
            file.close()

            return "write nickname"
        
        return jsonify(row)



@RelayDatabase.route('/load_pet_list')
class Load_Pet_List(Resource):
    def post(self):
        db = Database()
        # Get JSON data from request
        data = request.get_json()        

        #지갑 주소로 펫 NFT 목록 가져옴
        sql = "SELECT pet_token FROM nft.pet WHERE wallet_id=%s"
        row = db.executeAll(sql, data['wallet_id'])
        
        #펫이 없을 경우
        if db.cursor.rowcount == 0:
            return -1

        return jsonify(row)

@RelayDatabase.route('/load_settings')
class Load_Settings(Resource):
    def post(self):
        db = Database()
        # get wallet_id
        data = request.get_json()

        #db에서 가져오기
        sql = "SELECT ui_save FROM user.gamedata WHERE wallet_id=%s"
        row = db.executeOne(sql, data['wallet_id'])

        file_stream = StringIO(row['ui_save'])
        response = Response(
            file_stream.getvalue(),
            mimetype='text/plain',
            content_type='application/octet-stream'
        )
        response.headers["Content-Disposition"] = "attachment; filename=ui_save.txt"
        return response
    
@RelayDatabase.route('/load_pet_property')
class Load_Pet_Property(Resource):
    def post(self):
        db = Database()
        # Get pet_token
        data = request.get_json()

        #db에서 가져오기
        sql = "SELECT pet_token, pet_name, pet_age, pet_sex, pet_emotion FROM nft.pet WHERE pet_token=%s"
        row = db.executeOne(sql, data['pet_token'])

        if row is None:
            return "no data"

        return jsonify(row)
    
@RelayDatabase.route('/load_pet_texture')
class Load_Pet_Texture(Resource):
    def post(self):
        db = Database()

        # Get pet_token
        data = request.get_json()

        #db에서 가져오기
        sql = "SELECT pet_texture FROM nft.pet WHERE pet_token=%s"
        row = db.executeOne(sql, data['pet_token'])

        file_stream = BytesIO(row['pet_texture'])
        response = Response(
            file_stream.getvalue(),
            mimetype='image/png',
            content_type='application/octet-stream'
        )
        response.headers["Content-Disposition"] = "attachment; filename=pet_texture.png"

        return response

@RelayDatabase.route('/load_ai_model')
class Load_AI_Model(Resource):
    def post(self):
        db = Database()

        # Get pet_token
        data = request.get_json()
        model_int = f"model_{data['gesture_id']}"

        #db에서 가져오기
        sql = f"SELECT {model_int} FROM log.aimodel"
        sql += " WHERE pet_token=%s"
        row = db.executeOne(sql, data['pet_token'])

        file_stream = BytesIO(row[model_int])
        response = Response(
            file_stream.getvalue(),
            mimetype='application/onnx',
            content_type='application/octet-stream'
        )
        response.headers["Content-Disposition"] = f"attachment; filename=model{data['gesture_id']}.onnx"

        return response
    
@RelayDatabase.route('/save_settings')
class Save_Settings(Resource):
    def post(self):
        # Get data from request
        savedata = request.files['savedata']
        wallet_id = request.form['wallet_id']
            
        if savedata:
            db = Database()
            sql = "UPDATE user.gamedata SET ui_save=%s WHERE wallet_id=%s" 
            row = db.execute(sql, (savedata.read(), wallet_id))
            db.commit()

        return "success"
    
@RelayDatabase.route('/save_pet_property')
class Save_Pet_Property(Resource):
    def post(self):
        # Get data from request
        pet_token = request.form['pet_token']
        pet_name = request.form['pet_name']
        pet_age = request.form['pet_age']
        pet_sex = request.form['pet_sex']
        pet_emotion = request.form['pet_emotion']
        pet_texture = request.files['pet_texture']
            
        if pet_texture:
            db = Database()
            sql = "UPDATE nft.pet \
                SET pet_name=%s, pet_age=%s, pet_sex=%s, pet_emotion=%s, pet_texture=%s \
                WHERE pet_token=%s" 
            row = db.execute(sql, (pet_name, pet_age, pet_sex, pet_emotion, pet_texture.read(), pet_token))
            db.commit()

        return "success"
    
@RelayDatabase.route('/save_ai_model')
class Save_AI_Model(Resource):
    def post(self):
        # Get data from request
        model = request.files['model']
        pet_token = request.form['pet_token']
        gesture_id = request.form['gesture_id']

        if model:
            db = Database()
            sql = f"UPDATE log.aimodel SET model_{gesture_id}"
            sql += "=%s WHERE pet_token=%s" 
            row = db.execute(sql, (model.read(), pet_token))
            db.commit()

        return "success"
    
