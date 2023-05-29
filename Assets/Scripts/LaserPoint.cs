using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserPoint : MonoBehaviour
{
    private LineRenderer laser;        // 레이저
    private RaycastHit Collided_object; // 충돌된 객체
    private GameObject currentObject;   // 가장 최근에 충돌한 객체를 저장하기 위한 객체

    public Shader lit;
    public MeshRenderer targetSphere;
    public float raycastDistance = 100f; // 레이저 포인터 감지 거리

    // Start is called before the first frame update
    void Start()
    {
        // 스크립트가 포함된 객체에 라인 렌더러라는 컴포넌트를 넣고있다.
        laser = this.gameObject.AddComponent<LineRenderer>();

        // 라인이 가지개될 색상 표현
        Material material = new Material(lit);
        material.color = new Color(1, 1, 0.5f, 0.5f);
        laser.material = material;
        // 레이저의 꼭지점은 2개가 필요 더 많이 넣으면 곡선도 표현 할 수 있다.
        laser.positionCount = 2;
        // 레이저 굵기 표현
        laser.startWidth = 0.01f;
        laser.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        laser.SetPosition(0, transform.position); // 첫번째 시작점 위치
                                                   // 업데이트에 넣어 줌으로써, 플레이어가 이동하면 이동을 따라가게 된다.
                                                   //  선 만들기(충돌 감지를 위한)
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);

        // 충돌 감지 시
        if (Physics.Raycast(transform.position, transform.forward, out Collided_object, raycastDistance))
        {
            laser.SetPosition(1, Collided_object.point);

            targetSphere.gameObject.SetActive(true);
            targetSphere.transform.position = Collided_object.point;
            targetSphere.material.color = laser.material.color;

            // 충돌 객체의 태그가 Button인 경우
            if (Collided_object.collider.gameObject.CompareTag("Button"))
            {
                //if (OVRInput.Get())
                // 오큘러스 고 리모콘에 큰 동그라미 부분을 누를 경우
                if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    EffectManager.instance.PlayEffect(2);
                    // 버튼에 등록된 onClick 메소드를 실행한다.
                    Collided_object.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                }

                else
                {
                    Collided_object.collider.gameObject.GetComponent<Button>().OnPointerEnter(null);
                    currentObject = Collided_object.collider.gameObject;
                }
            }
            else if(currentObject != null)
            {
                currentObject.GetComponent<Button>().OnPointerExit(null);
                currentObject = null;
            }
        }

        else
        {
            // 레이저에 감지된 것이 없기 때문에 레이저 초기 설정 길이만큼 길게 만든다.
            laser.SetPosition(1, transform.position + (transform.forward * raycastDistance));
            targetSphere.gameObject.SetActive(false);

            // 최근 감지된 오브젝트가 Button인 경우
            // 버튼은 현재 눌려있는 상태이므로 이것을 풀어준다.
            if (currentObject != null)
            {
                currentObject.GetComponent<Button>().OnPointerExit(null);
                currentObject = null;
            }

        }

    }

    private void LateUpdate()
    {
        // 버튼을 누를 경우        
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            laser.material.color = new Color(1, 0, 0, 0.5f);
            //EffectManager.instance.PlayEffect(2);
        }

        // 버튼을 뗄 경우          
        else if (OVRInput.GetUp(OVRInput.Button.One) || Input.GetKeyUp(KeyCode.Mouse0))
        {
            laser.material.color = new Color(1, 1, 0.5f, 0.5f);
            //EffectManager.instance.PlayEffect(2);
        }
    }

    public void Clicked()
    {
        //Debug.Log(1);
    }
}

