using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dogAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Animator anim;

    [SerializeField]
    private AnimationClip fallLoopR;
    [SerializeField]
    private AnimationClip fallLoopL;
    [SerializeField]
    private List<AnimationClip> fallAnims;

    //Classes
    private newMove moveScript;
    private Collision coll;
    private InputMaster controls;

    public bool facingRight;
    private bool falling;

    private Vector2 dashVect;

    public float scaleX;
    public float scaleY;
    public float scaleZ;

    public float stretchX;
    public float stretchY;

    //Multiplier, relates to vel
    public float jumpStretchMult;
    //Multiplier, relates to dash vector
    public float dashStretchMultX;
    public float dashStretchMultY;

    private void Awake()
    {
        controls = new InputMaster();
    }
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        moveScript = gameObject.GetComponent<newMove>();
        coll = gameObject.GetComponent<Collision>();

        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        //print(anim.GetCurrentAnimatorClipInfo(0)[0].clip);
        fallCheck();
        dashVect = moveScript.dashDirection.normalized;
        transform.localScale = new Vector3(scaleX * stretchX, scaleY * stretchY, scaleZ);

        //We really don't want him to do a fall animation stretch while dashing, it looks crazy!
        //Also, if he's 'onGround' we need to stop doing this or we get some funky repeated landings.
        if(!coll.onGround && !moveScript.dashing)
        {
            stretchY = 1 + (-1 * rb.velocity.y * jumpStretchMult * Mathf.Sign(rb.gravityScale));
            stretchX = 1 + (rb.velocity.y * jumpStretchMult * Mathf.Sign(rb.gravityScale));
        }
        else if(moveScript.dashing)
        {
            stretchX = 1 + (Mathf.Abs(dashVect.x) * dashStretchMultX);
            stretchY = 1 + (Mathf.Abs(dashVect.y) * dashStretchMultY);
        }
        //So yeah in those cases don't stretch
        else
        {
            stretchY = 1;
            stretchX = 1;
        }

        if (controls.Player.Movement.ReadValue<Vector2>().x < -0.1)
        {
            facingRight = false;
            //spriteRenderer.flipX = true;
        }
        else if(controls.Player.Movement.ReadValue<Vector2>().x > 0.1)
        {
            facingRight = true;
            //spriteRenderer.flipX = false;
        }
        anim.SetBool("facingRight", facingRight);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("Jumping", moveScript.jumping);
        anim.SetFloat("yVel", rb.velocity.y * Mathf.Sign(rb.gravityScale));
        anim.SetBool("Grounded", moveScript.isGrounded);
        anim.SetBool("Dashing", moveScript.dashing);
        anim.SetBool("FallingAnim", falling);

        if(rb.gravityScale < 0)
        {
            spriteRenderer.flipY = true;
        }
        else if (rb.gravityScale > 0)
        {
            spriteRenderer.flipY = false;
        }
    }

    private void fallCheck()
    {
        if(anim.GetCurrentAnimatorClipInfo(0).Length == 0)
        {
            falling = false;
        }
        else if(fallAnims.Contains(anim.GetCurrentAnimatorClipInfo(0)[0].clip))
        {
            falling = true;
        }
        else
        {
            falling = false;
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
