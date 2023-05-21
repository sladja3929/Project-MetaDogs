from flask import Flask, jsonify, request
from flask_restx import Resource, Api, Namespace
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry
import os, sys

MLAgent = Namespace(
    name="MLAgent",
    description="강화학습 api",
)

@MLAgent.route('/create_model')
class Create_Model(Resource):
    def post(self):
        env_name = "/mlagent/Project-MetaDogs/Linux-build/AITest.x86_64"
        run_id = "test-2"
        model_name = "/mlagent/AItest.yaml"

        cmd = f"mlagents-learn {model_name} --run-id={run_id} --env={env_name} --no-graphics"
        os.system(cmd)
