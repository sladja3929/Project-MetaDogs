from flask import Flask, jsonify, request, Response
from flask_restx import Resource, Api, Namespace
import requests
from requests_toolbelt.multipart.encoder import MultipartEncoder
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry
from io import BytesIO, StringIO
import os, sys

MLAgent = Namespace(
    name="MLAgent",
    description="강화학습 api",
)

env_name = "/mlagent/Project-MetaDogs/Linux-build/AITest.x86_64"
model_name = "/mlagent/Project-MetaDogs/AItest.yaml"

@MLAgent.route('/create_model')
class Create_Model(Resource):
    def post(self):
        # Get pet_token
        data = request.get_json()
        model_int = f"model_{data['gesture_id']}"

        run_id = data['pet_token'] + "_model_" + data['gesture_id']

        cmd = f"mlagents-learn {model_name} --run-id={run_id} --env={env_name} --no-graphics"
        os.system(cmd)

@MLAgent.route('/learning')
class Learning(Resource):
    def post(self):
        # Get data from request
        model = request.files['model']
        pet_token = request.form['pet_token']
        gesture_id = request.form['gesture_id']

        run_id = pet_token + "_model_" + gesture_id

        old_run_id = "" #db에서 가져온 오닉스 파일 저장하는 폴더 이름
        new_run_id = "" #새 오닉스 파일이 저장될 폴더

        cmd = f"mlagents-learn {model_name}  --initialize-from={old_run_id} --run-id={new_run_id} --env={env_name} --no-graphics"
        os.system(cmd) #학습할때마다 뉴가 올드로 이름 바뀌어야 함

        #오닉스 이름: yaml파일과 동일