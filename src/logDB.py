from flask import Flask, jsonify, request
from flask_restx import Resource, Api, Namespace
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

from dbModule import Database, logdb

LogDB = Namespace(
    name="LogDB",
    description="log db 중계 api",
)

@LogDB.route('/load_settings')
class Load_Settings(Resource):
    def post(self):
        # Get JSON data from request
        data = request.get_json()
        
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

        url = "http://jsonplaceholder.typicode.com/users"

        # Send JSON data to external server
        response = session.get(url, timeout=3)

        # Return response from external server to client
        return jsonify(response.json())
