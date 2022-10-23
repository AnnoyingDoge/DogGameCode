using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class roomChange : MonoBehaviour
{
    [Header("Exit Settings")]
    public int roomOffsetX;
    public int roomOffsetY;
    public string sceneToActivate;

    //Given in R,L,U,D
    public string doorDir;
    public float fadeSpeed;
    

    [Header("Other Stuff")]
    [SerializeField]
    private bool waiting;
    public bool transitioning;
    public bool playerColl;

    public GameObject cameraBlocker;
    public Color cameraColor;

    public Collider2D coll;

    public LayerMask playerLayer;

    public Vector2 collBox;

    //Camera Stuff
    [Header("Camera")]
    public float desiredCameraSize = 5.0f;
    private Camera cam;
    public float smoothTime = 0.3f;
    private float vel = 0.0f;

    //private sceneLoader sceneLoaderScript;
    //public GameObject sceneManager;
    // Start is called before the first frame update
    void Start()
    {
        transitioning = false;
        coll = gameObject.GetComponent<Collider2D>();
        collBox = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        cameraBlocker = GameObject.FindGameObjectWithTag("cameraBlocker");
        cameraColor = new Color(cameraBlocker.GetComponent<SpriteRenderer>().color.r, cameraBlocker.GetComponent<SpriteRenderer>().color.g, cameraBlocker.GetComponent<SpriteRenderer>().color.b, cameraBlocker.GetComponent<SpriteRenderer>().color.a);
        //StartCoroutine(loadScene());
        //sceneLoaderScript = sceneManager.GetComponent<sceneLoader>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        playerColl = Physics2D.OverlapBox(transform.position, collBox, 0, playerLayer);
        cameraBlocker.GetComponent<SpriteRenderer>().color = cameraColor;

        if (gameObject.scene == SceneManager.GetActiveScene() && cam.orthographicSize != desiredCameraSize)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredCameraSize, ref vel, smoothTime);
        }
    }

    private void FixedUpdate()
    {
        if (playerColl && !transitioning && gameObject.scene == SceneManager.GetActiveScene())
        {
            transitioning = true;
            transitionScene();
            StartCoroutine(transitionWait());
        }
    }

    private void fadeColor()
    {
        if (cameraColor.a < 1)
        {
            cameraColor = new Color(cameraColor.r, cameraColor.g, cameraColor.b, cameraColor.a + fadeSpeed);
        }
    }

    private void unfadeColor()
    {

    }
    
    private void transitionScene()
    {
        //move camera/pause player, set active scene, etc.
        if(SceneManager.GetSceneByName(sceneToActivate).isLoaded)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToActivate));
        }
        else
        {
            SceneManager.LoadScene(sceneToActivate, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToActivate));
        }

        //print(SceneManager.GetActiveScene().name);
    }

    IEnumerator transitionWait()
    {
        while(playerColl)
        {
            yield return null;
        }

        transitioning = false;
    }

    //private IEnumerator loadScene()
    //{
    //    Scene scene;
    //    scene = SceneManager.GetSceneByName(sceneToActivate);
    //    yield return null;

    //    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToActivate);
    //    asyncOperation.allowSceneActivation = false;
    //    waiting = true;

    //    while (waiting)
    //    {
    //        yield return null;
    //    }

    //    print(sceneToActivate);
    //    asyncOperation.allowSceneActivation = true;
    //    yield return null;
    //}
}
