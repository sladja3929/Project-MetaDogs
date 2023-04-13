#!/src/.venv/bin/python3

from flask import Flask, jsonify, request
import requests
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

app = Flask(__name__)

@app.route('/send', methods=['POST'])
def send():
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

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5000', debug=True)
