using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class betterJump : MonoBehaviour
{
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float downMultiplier = 2f;
    public float wallMultiplier = 1.5f;

    //Other classes will interface with these booleans.
    public bool holdWall;
    public bool wallJumping;

    Rigidbody2D rb;

    //Instances of other classes.
    private Collision coll;
    private newMove moveScript;
    private InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
    }
    // Start is called before the first frame update
    void Start()
    {
        wallJumping = false;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        moveScript = GetComponent<newMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //holdWall = !coll.holdLeftWall.Equals(coll.holdRightWall);
        holdWall = !(coll.holdLeftWall == coll.holdRightWall);
        float y = controls.Player.Movement.ReadValue<Vector2>().y;
        if (!holdWall)
        {
            if (rb.velocity.y * Mathf.Sign(rb.gravityScale) < 0 && y > -0.9)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime * rb.gravityScale;
            }
            //if released jump, fall faster to lower jump height
            else if (rb.velocity.y * Mathf.Sign(rb.gravityScale) > 0 && !controls.Player.Jump.IsPressed())
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime * rb.gravityScale;
            }
            else if (rb.velocity.y * Mathf.Sign(rb.gravityScale) < 0 && y < -0.9 && !coll.onGround)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier * downMultiplier) * Time.deltaTime * rb.gravityScale;
            }

            //if (rb.velocity.y > 0)
            //{
            //}
            //else
            //{
            //}
        }

        if (holdWall && rb.velocity.y < 0 && !wallJumping)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (wallMultiplier - 1) * Time.deltaTime * rb.gravityScale;
        }


        //tweak 2 to something else probably!
        //breaks wallJump (fixed)
        //else if (holdWall && rb.velocity.y > 2 && !wallJumping && !moveScript.dashing)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, 0);
        //}
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
