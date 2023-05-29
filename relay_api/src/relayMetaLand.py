from flask import Flask, jsonify, request
from flask_restx import Resource, Api, Namespace
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

from kasModule import nft_list_url, x_chain_id, authorization, contract_address
from dbModule import Database

RelayMetaLand = Namespace(
    name="RelayMetaLand",
    description="메타랜드 중계 api",
)

@RelayMetaLand.route('/pet_sync_db')
class Pet_Sync_DB(Resource):
    def post(self):      
        #세션 설정  
        retries = 3
        backoff_factor = 0.3
        status_forcelist = (500, 400)
    
        retry = Retry(
        total=retries,
        read=retries,
        connect=retries,
        backoff_factor=backoff_factor,
        status_forcelist=status_forcelist
        )

        session = requests.Session()
        adapter = HTTPAdapter(max_retries=retry)
        session.mount("http://", adapter)
        session.mount("https://", adapter)

        headers = {
            'x-chain-id': x_chain_id,
            'Authorization': authorization
        }
        
        # Send JSON data to external server
        response = session.get(nft_list_url, headers=headers, timeout=3)
        data = response.json()

        if response.status_code == 200:
            # Access the JSON response content as a dictionary
            data = response.json()

            extras = []
            db = Database()
            #메타랜드 컬렉션에 속한 토큰만 추출
            for item in data['items']:
                if item['contractAddress'] == contract_address:        

                    #pet property 추출
                    pet_name = "tmpName"
                    pet_age = 4
                    pet_sex = "f"
                    pet_emotion = "1.1"

                    #db user.user에 항목 추가
                    sql = "INSERT INTO user.user(wallet_id) SELECT %s \
                        FROM DUAL WHERE NOT EXISTS \
                        (SELECT wallet_id FROM user.user WHERE wallet_id=%s)"
                    db.execute(sql, (item['lastTransfer']['transferFrom'], item['lastTransfer']['transferFrom']))
                    db.commit()

                    #db nft.pet에 항목 추가
                    sql = "INSERT INTO nft.pet(wallet_id, pet_token, pet_name, pet_age, pet_sex, pet_emotion) \
                        VALUES(%s, %s ,%s, %s, %s, %s) \
                        ON DUPLICATE KEY UPDATE wallet_id=%s, pet_token=%s, pet_name=%s, pet_age=%s, pet_sex=%s, pet_emotion=%s"
                    db.execute(sql, (item['lastTransfer']['transferFrom'], item['extras']['tokenId'], pet_name, pet_age, pet_sex, pet_emotion, item['lastTransfer']['transferFrom'], item['extras']['tokenId'], pet_name, pet_age, pet_sex, pet_emotion))
                    db.commit()

                    #db log.aimodel에 항목 추가
                    sql = "INSERT INTO log.aimodel(pet_token) SELECT %s \
                        FROM DUAL WHERE NOT EXISTS \
                        (SELECT pet_token FROM log.aimodel WHERE pet_token=%s)"
                    db.execute(sql, (item['extras']['tokenId'], item['extras']['tokenId']))
                    db.commit()

            return "success"

        else:
            print('Request failed with status code:', response.status_code)
            return response.status_code
