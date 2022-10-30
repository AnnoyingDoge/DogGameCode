using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class roomChange : MonoBehaviour
{
    //This script is a bit of a mess, but it handles dynamic room changes.
    //The code itself makes everything pretty easy to set up in editor. 

    [Header("Exit Settings")]
    public int roomOffsetX;
    public int roomOffsetY;
    public string sceneToActivate;

    //Given in R,L,U,D
    public string doorDir;
    

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
    }
    
    //Call camera changes after physics and position updates.
    private void LateUpdate()
    {
        if (gameObject.scene == SceneManager.GetActiveScene() && cam.orthographicSize != desiredCameraSize)
        {
            //Just a note, SmoothDamp accounts for Time.deltaTime, so no need for FixedUpdate or multiplying by Time.deltaTime
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredCameraSize, ref vel, smoothTime);
        }
    }

    private void FixedUpdate()
    {
        //this checks if we can transition, and does it if player collides with transition to next room.
        if (playerColl && !transitioning && gameObject.scene == SceneManager.GetActiveScene())
        {
            transitioning = true;
            transitionScene();
            StartCoroutine(transitionWait());
        }
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
        //Wait for the player to be in a different scene than the object (new scene does not contain transition object)
        while(playerColl)
        {
            yield return null;
        }

        transitioning = false;
    }

}
