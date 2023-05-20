using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutSceneManager : MonoBehaviour
{
    public SpriteRenderer[] circles = new SpriteRenderer[8];
    public GameObject dog;
    float spinSpeed;
    public Camera cam;
    public ParticleSystem bars;
    public ParticleSystem sparkles;
    public AudioSource audioSource;
    public Animator circleAnimator;
    public Animator titleAnimator;
    public bool prepare;
    public bool start;
    bool fin;
    float timer;
    bool[] switches = new bool[7];

    public TextMeshProUGUI text;
    public TextMeshProUGUI title;

    public Animator tutorialAnimator;
    public GameObject[] tutorialObjects = new GameObject[4];

    public TextMeshProUGUI[] creditTexts;
    public RawImage fade;
    public Button selectButton;

    bool nextPage;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !prepare)
        {
            OnClickSelect();
            /*
            start = true;
            text.enabled = true;
            cam.GetComponent<Animator>().SetTrigger("start");
            bars.Play();*/
        }
    }

    private void FixedUpdate()
    {
        if (nextPage) return;

        if (!start) return;
        if (!audioSource.isPlaying)
            audioSource.Play();
        timer += Time.fixedDeltaTime;

        if (fin) dog.transform.position += new Vector3(0, 10 * Time.fixedDeltaTime, 0);
        
        for (int i = 0; i < circles.Length; i++)
            if (i % 2 == 0)
                circles[i].transform.Rotate(Vector3.forward * Time.fixedDeltaTime * 75);
            else
                circles[i].transform.Rotate(Vector3.forward * Time.fixedDeltaTime * -75);

        if (cam.fieldOfView != 0)
            spinSpeed = 60 * 180 / cam.fieldOfView;
        dog.transform.Rotate(Vector3.up * Time.fixedDeltaTime * spinSpeed);

        if (timer > 3 && !switches[0])
        {
            switches[0] = true;
            circleAnimator.SetTrigger("start");
        }
        if ((Input.GetKeyDown(KeyCode.I) || timer > 7f) && !switches[1])
        {
            switches[1] = true;
            sparkles.gameObject.SetActive(true);
        }
        if ((Input.GetKeyDown(KeyCode.O) || timer > 15) && !switches[2])
        {
            switches[2] = true;
            CloseUpStart();
        }
        if ((Input.GetKeyDown(KeyCode.P) || timer > 25f) && !switches[3])
        {
            switches[3] = true;
            fin = true;
            cam.GetComponent<Animator>().SetTrigger("fin");
        }
        if (timer > 31.8f && !switches[4])
        {
            switches[4] = true;
            text.enabled = false;
            title.enabled = true;
        }
        if (timer > 36f && !switches[5])
        {
            switches[5] = true;
            titleAnimator.SetTrigger("move");
        }
        if (timer > 38f && !switches[6])
        {
            switches[6] = true;
            tutorialAnimator.SetTrigger("tutorial");
            StartCoroutine(Tutorial());
        }
    }

    public void CloseUpStart()
    {
        StartCoroutine(CloseUp());
    }
    public IEnumerator CloseUp()
    {
        float t = 3f;
        while (t > 0)
        {
            cam.fieldOfView -= Time.deltaTime * 59.5f / 3f;
            t -= Time.deltaTime;
            yield return null;
        }
        dog.GetComponent<Animator>().SetTrigger("now");
        while (t < 2)
        {
            cam.fieldOfView += Time.deltaTime * 59.5f / 2;
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = 60;
    }
    public IEnumerator Tutorial()
    {
        float timer = 0;
        while (timer < 8)
        {
            timer += Time.deltaTime;
            tutorialObjects[0].transform.localPosition += new Vector3(20f * Time.deltaTime, 0, 0);
            yield return null;
        }
        while (timer < 16)
        {
            timer += Time.deltaTime;
            tutorialObjects[1].transform.localPosition -= new Vector3(20f * Time.deltaTime, 0, 0);
            yield return null;
        }
        while (timer < 24)
        {
            timer += Time.deltaTime;
            tutorialObjects[2].transform.localPosition += new Vector3(20f * Time.deltaTime, 0, 0);
            yield return null;
        }
        while (timer < 32)
        {
            timer += Time.deltaTime;
            tutorialObjects[3].transform.localPosition -= new Vector3(20f * Time.deltaTime, 0, 0);
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        nextPage = true;
        cam.GetComponent<Animator>().enabled = false;
        cam.fieldOfView = 13;
        dog.GetComponent<Animator>().SetTrigger("next");
        dog.transform.rotation = Quaternion.Euler(0, 0, 0);
        titleAnimator.gameObject.SetActive(false);
        cam.transform.rotation = Quaternion.Euler(0, 180, 0);
        cam.transform.position = dog.transform.position + new Vector3 (0.1f, 0.48f, 0.9f);
        creditTexts[0].enabled = true;

        yield return new WaitForSeconds(2f);

        //cam.transform.rotation = Quaternion.Euler(0, 180, 0);
        cam.transform.position = dog.transform.position + new Vector3(-0.1f, 0.48f, 0.9f);
        creditTexts[0].enabled = false;
        creditTexts[1].enabled = true;

        yield return new WaitForSeconds(2f);

        cam.fieldOfView = 25;
        cam.transform.rotation = Quaternion.Euler(0, -90, 0);
        cam.transform.position = dog.transform.position + new Vector3(3, 0.36f, 0);
        creditTexts[1].enabled = false;
        creditTexts[2].enabled = true;

        yield return new WaitForSeconds(2f);

        cam.transform.rotation = Quaternion.Euler(0, 90, 0);
        cam.transform.position = dog.transform.position + new Vector3(-3, 0.36f, 0);
        creditTexts[2].enabled = false;
        creditTexts[3].enabled = true;

        yield return new WaitForSeconds(2f);

        cam.transform.rotation = Quaternion.Euler(-45, 135, 0);
        cam.transform.position = dog.transform.position + new Vector3(-0.3f, -0.1f, 0.45f);
        creditTexts[3].enabled = false;
        //creditTexts[4].enabled = true;

        yield return new WaitForSeconds(2f);

        cam.transform.rotation = Quaternion.Euler(-45, -135, 0);
        cam.transform.position = dog.transform.position + new Vector3(0.3f, -0.1f, 0.45f);
        //creditTexts[4].enabled = false;
        //creditTexts[5].enabled = true;

        yield return new WaitForSeconds(2f);

        cam.fieldOfView = 30;
        cam.transform.rotation = Quaternion.Euler(20, 0, 0);
        cam.transform.position = dog.transform.position + new Vector3(0, 1.64f, -3);
        //creditTexts[5].enabled = false;

        yield return new WaitForSeconds(1f);

        while (fade.color.a < 1)
        {
            audioSource.volume -= 0.07f * Time.deltaTime;
            fade.transform.localScale += new Vector3(10 * Time.deltaTime, 10 * Time.deltaTime, 0);
            fade.color += new Color(0, 0, 0, 0.1f * Time.deltaTime);
            cam.fieldOfView += Time.deltaTime * 10;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickSelect()
    {
        if (prepare) return;
        prepare = true;
        selectButton.gameObject.SetActive(false);
        text.enabled = true;
        StartCoroutine(CutSceneStart());
    }
    public IEnumerator CutSceneStart()
    {
        float timer = 0;
        while (timer < 3f)
        {
            if (cam.fieldOfView != 0)
                spinSpeed = 60 * 180 / cam.fieldOfView;
            dog.transform.Rotate(Vector3.up * Time.fixedDeltaTime * spinSpeed);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        start = true;
        cam.GetComponent<Animator>().SetTrigger("start");
        bars.Play();
    }
}
