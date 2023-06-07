using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : MonoBehaviour
{
    public Rigidbody rigidBody;
    public Animator toyAnimator;
    public float freeMoveSpeed;
    float reDirCnt;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ToyRandMove(true));
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
    public IEnumerator ToyRandMove(bool infinite)
    {
        reDirCnt = 0;
        Vector3 dir = new Vector3(0, 0, 0);
        while (infinite)
        {
            if (Player.instance.controller.activeSelf)
            {
                if (!toyAnimator.GetBool("toyOn"))
                    toyAnimator.SetBool("toyOn", true);
                if (reDirCnt < 0)
                {
                    dir = new Vector3(Random.Range(-1f, 1f) * Random.Range(0.5f * freeMoveSpeed, freeMoveSpeed), rigidBody.velocity.y, Random.Range(-1f, 1f) * Random.Range(0.5f * freeMoveSpeed, freeMoveSpeed));
                    transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan(dir.x / dir.z), 0);
                    if (dir.x > 0 && dir.z > 0)
                        transform.rotation *= Quaternion.Euler(1, -1, 1);
                    /*if (dir.x < 0 && dir.z > 0)
                        transform.rotation *= Quaternion.Euler(1, 1, 1);
                    else if (dir.x < 0 && dir.z < 0)
                        transform.rotation *= Quaternion.Euler(1, -1, 1);*/
                    else if (dir.x > 0 && dir.z < 0)
                        transform.rotation *= Quaternion.Euler(1, -1, 1);
                    reDirCnt = Random.Range(0, 3f);
                }
                rigidBody.velocity = new Vector3(dir.x, rigidBody.velocity.y, dir.z);
                //transform.position += dir;
            }
            else
            {
                toyAnimator.SetBool("toyOn", false);
            }
            yield return new WaitForFixedUpdate();
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
