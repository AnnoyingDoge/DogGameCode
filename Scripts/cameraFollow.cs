using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraFollow : MonoBehaviour
{
    //Camera follow script.
    //Lags slightly behind player (intentionally) and stops at designated borders on each room.

    [Header("Main Settings")]
    public float camSpeed = 20.0f;
    public float smoothTime = 0.3f;


    [Header("Misc.")]
    public bool leftMax;
    public bool rightMax;
    public bool topMax;
    public bool botMax;

    private float x;
    private float y;

    private float halfWidth;
    private float halfHeight;
    public GameObject player;
    public GameObject mainCamera;
    public Camera cam;

    [SerializeField]
    public BoxCollider2D coll;
    [SerializeField]
    private Collider2D leftColl;
    [SerializeField]
    private Collider2D rightColl;
    [SerializeField]
    private Collider2D upColl;
    [SerializeField]
    private Collider2D downColl;

    [SerializeField]
    private Vector3 velocity = Vector3.zero;


    public ContactFilter2D cF;

    [SerializeField]
    private List<Collider2D> resultsList;

    public Scene activeScene;

    // Start is called before the first frame update
    void Start()
    {
        //Grab collider object and create a contact 'filter' (no filter so it just reports every contacted object)
        coll = gameObject.GetComponent<BoxCollider2D>();
        cF = new ContactFilter2D().NoFilter();
        //Create an empty list to set resultsList to for now
        resultsList = new List<Collider2D>();

        //Find the game objects we want to track/move
        player = GameObject.FindGameObjectWithTag("player");
        //mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = Camera.main.gameObject;
        cam = Camera.main;
        
        //Check active scene, used so that we can ignore any objects in pre-loaded scenes
        activeScene = SceneManager.GetActiveScene();

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;

        coll.size = new Vector2(halfWidth * 2, halfHeight * 2);
    }

    // Update is called once per frame
    void Update()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;

        coll.size = new Vector2(halfWidth * 2, halfHeight * 2);
        //Move the camera controller to the position of the player constantly
        transform.position = player.transform.position;

        //Get all overlapping colliders, put them into resultsList.
        coll.OverlapCollider(cF, resultsList);

        foreach (Collider2D collided in resultsList)
        {
            //print(collided);

            //only activates if collision object is within the active scene
            //bug if collider collides with nothing, causes potential camera freeze? fixed below.
            if (collided.gameObject.scene == SceneManager.GetActiveScene())
            {
                filterResults(collided);
            }
            else
            {
                filterResults(gameObject.GetComponent<Collider2D>());
            }

        }
        if(resultsList.Count == 0)
        {
            //This is cheating. We're passing through the object's own collider to the filter if the list is empty, which allows us to just always run the filterResults script.
            //Don't ask why I did it this way. (I'm just lazy and this is an edge case.)
            filterResults(gameObject.GetComponent<Collider2D>());
        }

        //After filtering the results, 'moveCamera' is called.
        //moveCamera is a bad name. It doesn't actually move it. It just tells it where it will move when we call LateUpdate.
        moveCamera();


        //On a scene change, we want to ensure that the camera boarders 'reset,' so we call a function once.
        //We compare the variable 'activeScene' to the actual active scene to do so.
        if (activeScene != SceneManager.GetActiveScene())
        {
            activeScene = SceneManager.GetActiveScene();
            clearCollisions();
        }

    }


    private void LateUpdate()
    {
        //Move the camera smoothly towards the desired position. It lags slightly behind the player, but catches up completely unlike Vector3.lerp. 
        //LateUpdate is utilized, as it will move to the desired position after all physics updates on a given frame.
        //Without LateUpdate, the camera is slightly de-synced from the player, causing a ghosting effect.
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, new Vector3(x, y, mainCamera.transform.position.z), ref velocity, smoothTime);
    }

    //This code below was the old system. Lerp is kind of bad here, as it kind of asymptotically approaches the position (kind of). FixedUpdate almost looks okay, unlike the completely broken Update, but is still bad.
    //private void FixedUpdate()
    //{
    //    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(x, y, mainCamera.transform.position.z), Time.fixedDeltaTime * camSpeed);
    //    //mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
    //}

    private void filterResults(Collider2D collision)
    {
        //Just go through and see which border is touched.
        switch (collision.gameObject.tag)
        {
            case "leftMax":
                leftMax = true;
                leftColl = collision;
                break;
            case "rightMax":
                rightMax = true;
                rightColl = collision;
                break;
            case "upMax":
                topMax = true;
                upColl = collision;
                break;
            case "downMax":
                botMax = true;
                downColl = collision;
                break;
        }

        //If our resultsList is empty, we're no longer at any maximums (edge case described in the self-passthrough lines).
        if (resultsList.Count == 0)
        {
            leftMax = false;
            rightMax = false;
            topMax = false;
            botMax = false;
        }

        //Save the collider information of current maximums.
        else
        {
            leftMax = resultsList.Contains(leftColl);
            rightMax = resultsList.Contains(rightColl);
            topMax = resultsList.Contains(upColl);
            botMax = resultsList.Contains(downColl);
        }
    }

    private void clearCollisions()
    {
        //All this does is clear the saved collision information. It's inefficient/lazy code, but it only runs on scene transitions so it's okay.
        if (upColl != null && upColl.gameObject.scene != SceneManager.GetActiveScene())
        {
            upColl = null;
        }
        if (downColl != null && downColl.gameObject.scene != SceneManager.GetActiveScene())
        {
            downColl = null;
        }
        if (leftColl != null && leftColl.gameObject.scene != SceneManager.GetActiveScene())
        {
            leftColl = null;
        }
        if (rightColl != null && rightColl.gameObject.scene != SceneManager.GetActiveScene())
        {
            rightColl = null;
        }
    }

    private void moveCamera()
    {
        //smthn about if touching a border, cant inc. in that direction.
        //should rework this code such that it's a simple x can't inc/dec thing


        if(leftMax && leftColl != null) //|| rightMax !
        {
            x = leftColl.gameObject.transform.position.x + halfWidth;
        }
        else if(rightMax && rightColl != null)
        {
            x = rightColl.gameObject.transform.position.x - halfWidth;
        }
        else
        {
            x = player.transform.position.x;
        }
        
        if(topMax && upColl != null)
        {
            y = upColl.gameObject.transform.position.y - halfHeight;
        }
        else if(botMax && downColl!= null)
        {
            y = downColl.gameObject.transform.position.y + halfHeight;
        }
        else
        {
            y = player.transform.position.y;
        }

    }

}
