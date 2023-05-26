from flask import Flask, jsonify, request, Response, send_file
from flask_restx import Resource, Api, Namespace
import os, sys

from serverModule import SyncModelToDB

MLAgent = Namespace(
    name="MLAgent",
    description="강화학습 api",
)

env_path = "/mlagent/Project-MetaDogs/Linux-build/AITest.x86_64"
model_name = "AITest"
model_path = f"/mlagent/{model_name}.yaml"
result_path = "/mlagent/pet_model"
#오닉스 이름: yaml파일과 동일

@MLAgent.route('/learning')
class Learning(Resource):
    def post(self):
        # Get data
        data = request.get_json()

        pet_token_path = f"{result_path}/{data['pet_token']}"
        old_run_id = f"{pet_token_path}/model_{data['gesture_id']}"
        new_run_id = old_run_id + "_new" #새 오닉스 파일이 저장될 폴더
        

        if not os.path.isdir(pet_token_path):
            #토큰으로 폴더 생성
            cmd = f"mkdir {pet_token_path}"
            system_return = os.system(cmd)  #0 리턴하면 성공

        if not os.path.isdir(old_run_id):
            #첫 학습
            cmd = f"mlagents-learn {model_path} --run-id={old_run_id} --env={env_path} --no-graphics"
            system_return = os.system(cmd)

        else:
            #재학습
            cmd = f"mlagents-learn {model_path}  --initialize-from={old_run_id} --run-id={new_run_id} --env={env_path} --no-graphics"
            system_return = os.system(cmd)

            #기존 모델 삭제
            cmd = f"rm -rf {old_run_id}"
            system_return = os.system(cmd)

            #새 모델 이름 변경
            cmd = f"mv {new_run_id} {old_run_id}"
            system_return = os.system(cmd)
        

        #오닉스 db에 저장
        SyncModelToDB(data['pet_token'], data['gesture_id'], f"{old_run_id}/{model_name}.onnx")

        #오닉스 파일 리턴
        try:
             return send_file(f"{old_run_id}/{model_name}.onnx", download_name=f"model_{data['gesture_id']}.onnx")
        
        except Exception as e:
            return str(e)