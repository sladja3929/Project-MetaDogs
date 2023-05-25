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
result_path = "/mlagent/usr_model"

@MLAgent.route('/create_model')
class Create_Model(Resource):
    def post(self):
        # Get pet_token
        data = request.get_json()

        run_id = f"{result_path}/{data['pet_token']}/model_{data['gesture_id']}"

        #첫 학습
        cmd = f"mlagents-learn {model_name} --run-id={run_id} --env={env_name} --no-graphics"
        os.system(cmd)

@MLAgent.route('/learning')
class Learning(Resource):
    def post(self):
        # Get pet_token
        data = request.get_json()

        old_run_id = f"{result_path}/{data['pet_token']}/model_{data['gesture_id']}"
        new_run_id = old_run_id + "_new" #새 오닉스 파일이 저장될 폴더

        #재학습
        cmd = f"mlagents-learn {model_name}  --initialize-from={old_run_id} --run-id={new_run_id} --env={env_name} --no-graphics"
        os.system(cmd)

        #기존 모델 삭제
        cmd = f"rm -rf {old_run_id}"
        os.system(cmd)

        #새 모델 이름 변경
        cmd = f"mv {new_run_id} {old_run_id}"
        os.system(cmd)

        #오닉스 이름: yaml파일과 동일