using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collision : MonoBehaviour
{
    //This is our collision script! It gives us a lot more control than using Unity's "onCollision" methods.
    //The access levels of this stuff is a lot like the movement script. We need a lot of this information elsewhere, so it's mostly public.
    //This script is based on an idea from a Mix and Jam video on Celeste's movement. Link to video here: https://www.youtube.com/watch?v=STyY26a_dPY
    //It doesn't use their code, but it functions similarly to their "Collision.cs" file.
    //That can be found here: https://github.com/mixandjam/Celeste-Movement/blob/master/Assets/Scripts/Collision.cs


    public bool onGround;
    public bool onLeftWall;
    public bool onRightWall;
    public bool holdLeftWall;
    public bool holdRightWall;
    public bool dead;
    public bool botSpring;
    public bool reverseGravity;

    public float distFromGround;
    public float distFromWall;

    public Collider2D coll;
    public Rigidbody2D rb;
    private InputMaster controls;

    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask transitionLayer;
    public LayerMask killLayer;
    public LayerMask springLayer;

    private Vector3 downOffset;
    private Vector3 leftOffset;
    private Vector3 rightOffset;
    private Vector3 sideOffset;

    private Vector2 downBox;
    private Vector2 sideBox;

    public GameObject transitionObject;

    private void Awake()
    {
        controls = new InputMaster();
    }

    // Start is called before the first frame update
    void Start()
    {
        coll = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        distFromGround = coll.bounds.extents.y;
        distFromWall = coll.bounds.extents.x;

        downOffset = new Vector3(0, coll.bounds.extents.y, 0);
        leftOffset = new Vector3(-coll.bounds.extents.x, 0, 0);
        rightOffset = new Vector3(coll.bounds.extents.x, 0, 0);
        sideOffset = new Vector3(coll.bounds.extents.x, 0, 0);

        downBox = new Vector2((coll.bounds.extents.x - 0.01f) * 2, 0.1f);
        
        //sidebox might break later, make sure it's okay!
        sideBox = new Vector2(0.1f, (coll.bounds.extents.y - 0.01f) * 2);
    }

    // Update is called once per frame
    void Update()
    {
        float x = controls.Player.Movement.ReadValue<Vector2>().x;
        if (rb.gravityScale < 0)
        {
            downOffset = new Vector3(0, coll.bounds.extents.y, 0) * -1;
        }
        //if this is just an else statement it creates a bug, ground detection becomes both top and bottom of body when dashing
        else if(rb.gravityScale > 0)
        {
            downOffset = new Vector3(0, coll.bounds.extents.y, 0);
        }

        //set all the collision booleans
        onGround = Physics2D.OverlapBox(coll.bounds.center - downOffset, downBox, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox(coll.bounds.center - sideOffset, sideBox, 0, wallLayer);
        onRightWall = Physics2D.OverlapBox(coll.bounds.center + sideOffset, sideBox, 0, wallLayer);
        dead = Physics2D.OverlapBox(coll.bounds.center, coll.bounds.size, 0, killLayer);

        //movement modifiers
        botSpring = Physics2D.OverlapBox(coll.bounds.center - downOffset, downBox, 0, springLayer);

        if (!onGround && onRightWall && x > 0)
        {
            holdRightWall = true;
        }
        else if (!onGround && onLeftWall && x < 0)
        {
            holdLeftWall = true;
        }
        else
        {
            holdLeftWall = false;
            holdRightWall = false;
        }

        if(Physics2D.OverlapBox(coll.bounds.center, coll.bounds.size, 0, transitionLayer) && Physics2D.OverlapBox(coll.bounds.center, coll.bounds.size, 0, transitionLayer).gameObject.scene == SceneManager.GetActiveScene())
        {
            transitionObject = Physics2D.OverlapBox(coll.bounds.center, coll.bounds.size, 0, transitionLayer).gameObject;
        }

        //Last thing to do is re-draw our collision boxes. This ensures that any change performed to the player doesn't break everything.
        //Except not this one. Because gravity scale check above. downOffset = new Vector3(0, coll.bounds.extents.y, 0);
        leftOffset = new Vector3(-coll.bounds.extents.x, 0, 0);
        rightOffset = new Vector3(coll.bounds.extents.x, 0, 0);
        sideOffset = new Vector3(coll.bounds.extents.x, 0, 0);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(coll.bounds.center - downOffset, downBox);
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(coll.bounds.center - sideOffset, sideBox);
            Gizmos.DrawCube(coll.bounds.center + sideOffset, sideBox);
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(coll.bounds.center, coll.bounds.size);
        }
    }

    //This is old, (mostly) unused, and bad
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == ("townperson"))
        {
            //print(collision.gameObject.name + "enter");
            collision.gameObject.GetComponent<TownPersonHandler>().touchingPlayer = true;
        }
        if(collision.gameObject.tag == ("gravReverse"))
        {
            reverseGravity = true;
        }
        if (collision.gameObject.tag == ("gravNormal"))
        {
            reverseGravity = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("townperson"))
        {
            //print(collision.gameObject.name + "leave");
            collision.gameObject.GetComponent<TownPersonHandler>().touchingPlayer = false;
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
