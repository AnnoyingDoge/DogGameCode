/* Credit for the base of this code goes to a tutorial by Board to Bits Games on youtube. Linked here: https://www.youtube.com/watch?v=7KiK0Aqtmzc
 * Originally sourced from this tutorial, the code here is heavily modified.
 */

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
        holdWall = !(coll.holdLeftWall == coll.holdRightWall);
        float y = controls.Player.Movement.ReadValue<Vector2>().y;
        //Betterjump here, essentially it controls the fall speed (gravity) of the player depending on their jump input (held vs unheld) and if they are in the "falling" portion of the jump
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
        }

        if (holdWall && rb.velocity.y < 0 && !wallJumping)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (wallMultiplier - 1) * Time.deltaTime * rb.gravityScale;
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
