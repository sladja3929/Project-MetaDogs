using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Canvas canvas;
    public Transform canvasPos;
    public GameObject laser;
    public bool vrMode;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
                laser.SetActive(false);
            }
            else
            {
                //canvas.transform.SetParent(transform);
                canvas.transform.position = canvasPos.position;
                canvas.transform.rotation = canvasPos.rotation;
                canvas.transform.rotation *= Quaternion.Euler(0, 1, 0);
                //canvas.transform.SetParent(transform.parent);
                canvas.gameObject.SetActive(true);
                laser.SetActive(true);
            }
        }
    }
    void FixedUpdate()
    {

    }
}
