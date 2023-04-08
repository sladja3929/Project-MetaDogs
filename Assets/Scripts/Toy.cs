using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float freeMoveSpeed;
    float reDirCnt;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ToyRandMove(60));
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {

    }

    public IEnumerator ToyRandMove(float t)
    {
        reDirCnt = 0;
        Vector3 dir = new Vector3(0, 0, 0);
        while (t > 0)
        {
            if (reDirCnt < 0)
            {
                dir = new Vector3(Random.Range(-1f, 1f) * Random.Range(0f, freeMoveSpeed), rigidBody.velocity.y, Random.Range(-1f, 1f) * Random.Range(0f, freeMoveSpeed));
                transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan(dir.x/dir.z) + 90, 0);
                reDirCnt = Random.Range(0, 3f);
            }
            rigidBody.velocity = dir;
            //transform.position += dir;
            yield return new WaitForFixedUpdate();
            t -= Time.fixedDeltaTime;
            reDirCnt -= Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            reDirCnt = 0;
        }
    }
}
