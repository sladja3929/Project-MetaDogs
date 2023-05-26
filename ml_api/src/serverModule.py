import requests
from requests_toolbelt.multipart.encoder import MultipartEncoder
from requests import Session
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry

#host="203.250.148.33:20080" #외부에서 돌릴 경우
host="https://metadogs-relay-api" #도커 컨테이너를 네트워크로 묶고 컨테이너 이름을 호스트로 사용
save_ai_model = "/db/save_ai_model"

def SyncModelToDB(pet_token, gesture_id, file_path):
    #multipart/form-data 생성
    m = MultipartEncoder(
        fields={
            'pet_token': pet_token,
            'gesture_id': str(gesture_id),
            'model': (f"model_{gesture_id}", open(file_path, 'rb'), 'application/onnx')
        }
    )

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

    response = session.post(host+save_ai_model, data=m, headers={'Content-Type': m.content_type}, timeout=3)
    
    #relay api에 전송
    #r = requests.post(host+save_ai_model, data=m, headers={'Content-Type': m.content_type})
