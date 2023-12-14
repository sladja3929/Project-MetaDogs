#!/src/.venv/bin/python3

from flask import Flask, jsonify, request
import requests

app = Flask(__name__)

@app.route('/receive', methods=['POST'])
def recieve():
    return "Hello World!"

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5001', debug=True)
