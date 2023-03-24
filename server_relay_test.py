#!/src/.venv/bin/python3

from flask import Flask, jsonify, request
import requests

app = Flask(__name__)

@app.route('/send', methods=['POST'])
def send():
    # Get JSON data from request
    data = request.get_json()

    # Send JSON data to external server
    response = requests.post('localhost:5001/receive', json=data)

    # Return response from external server to client
    return jsonify(response.json())

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5000', debug=True)
