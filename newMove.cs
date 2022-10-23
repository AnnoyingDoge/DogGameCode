using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class newMove : MonoBehaviour
{
    //Script handling player movement.
    //I think this script is like... objectively bad. It needs some work.
    //Nearly every float/bool here has some case where it will need to be/could be referenced from another class.
    //Thus, a lot of public stuff. Not the best implementation for sure.

    //Classes
    private Collision coll;
    private betterJump bJump;
    private dogAnimator animScript;
    private betterJump jumpScript;
    private InputMaster controls;
    //Components
    public Rigidbody2D PlayerRB2D;
    public GameObject dashParticleObject;
    public GameObject dashAfterImageObject;

    [Header("Controller Settings")]
    public float deadZone;

    [Header("Movement Settings")]
    public int speed;
    //This float is used in a lerp to build momentum
    public float yVel;
    [Space]
    public float wallJumpVel;
    public float wallJumpTime;
    public float playerIncomingVel;
    [Space]
    public float climbSpeed;
    [Space]
    public float dashSpeed;
    public float dashTime;
    
    [Header("Lerp 't' values")]
    public float speedUp;
    public float airMobilityMultiplier;
    public float highSpeedAdjustmentMultiplier;
    public float wallJumpMult;

    [Header("Movement Interactables")]
    public float springVel;

    [Header("Booleans")]
    public bool isGrounded;
    public bool jumping;
    public bool canJump;
    public bool canMove;
    public bool canDash;
    public bool canWallJump;
    public bool dashing;
    public bool holdWall;
    public bool setWallVel;
    public bool canCancelDash;
    public bool springing;
    public bool reverseGravity;
    public bool climbing;
    public bool climbCheck;

    [Header("Debug Data")]
    [SerializeField]
    private float x;
    [SerializeField]
    private float y;
    [SerializeField]
    private float gravScale;
    [SerializeField]
    private Vector2 EightDirVector;
    //dashDirection is public as animator may need to utilize to set proper dash animation.
    public Vector2 dashDirection;
    private Vector2 freezeVel;

    //Store coroutines so we can stop it if we want to.
    private Coroutine dashRoutine;
    private Coroutine dashParticleRoutine;
    private Coroutine dashAfterImageRoutine;
    private Coroutine wallJumpRoutine;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Jump.performed += context => Jump();
        controls.Player.Dash.performed += context => startDash();
    }
    // Start is called before the first frame update
    void Start()
    {

        speed = 8;
        yVel = 10;
        wallJumpVel = yVel;
        
        dashSpeed = 25;
        dashTime = 0.2f;
        deadZone = 0.5f;
        wallJumpTime = 0.2f;

        //airMobilityMultiplier = 100;
        //highSpeedAdjustmentMultiplier = 100;
        //wallJumpMult = 2.0f;

        canMove = true;

        PlayerRB2D = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        bJump = GetComponent<betterJump>();
        animScript = GetComponent<dogAnimator>();
        jumpScript = GetComponent<betterJump>();

        gravScale = PlayerRB2D.gravityScale;
        springing = false;

        reverseGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = coll.onGround;
        //changed from getaxis to getaxisraw
        x = controls.Player.Movement.ReadValue<Vector2>().x;
        y = controls.Player.Movement.ReadValue<Vector2>().y;
        EightDirVector = EightDirConversion();

        //if(canMove && !dashing)
        //{
        //    moveXY();
        //}
        if (reverseGravity != coll.reverseGravity)
        {
            reverseGravity = coll.reverseGravity;
            StartCoroutine(gravityReverser());
        }

        //if (Input.GetButtonDown("Jump"))
        //{
        //    //Jump
        //    if (coll.onGround && canMove)
        //    {
        //        if (dashing)
        //        {
        //            PlayerRB2D.velocity += Vector2.up * yVel * Mathf.Sign(gravScale);
        //        }
        //        else
        //        {
        //            PlayerRB2D.velocity += Vector2.up * yVel * Mathf.Sign(gravScale);
        //        }

                
                
        //        jumping = true;
        //    }
        //}
        if (coll.onGround && PlayerRB2D.velocity.y == 0)
        {
            jumping = false;
        }

        //if (Input.GetButtonDown("Jump") && dashing)
        //{
        //    //CancelDash();
        //}

        if (!coll.onLeftWall && !coll.onRightWall)
        {
            playerIncomingVel = PlayerRB2D.velocity.y;
            setWallVel = true;
        }
        //else if ((coll.onLeftWall || coll.onRightWall) && Input.GetButton("Jump") && setWallVel)
        //{
        //    PlayerRB2D.velocity = new Vector2(PlayerRB2D.velocity.x, playerIncomingVel);
        //    setWallVel = false;

        //}
        else
        {
            playerIncomingVel = 0;
        }

        //if (Input.GetButtonDown("Jump") && !dashing && (coll.onLeftWall || coll.onRightWall) && !coll.onGround) //&& canWallJump)
        //{
        //    wallJump();
        //}

        if (coll.onGround)
        {
            if (canCancelDash || !dashing)
            {
                canDash = true;
            }
            canWallJump = true;
        }

        //if(Input.GetButtonDown("Fire2") && canDash && (!dashing || canCancelDash)) //&& (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0))
        //{
        //    if(dashing)
        //    {
        //        canCancelDash = false;
        //        StopCoroutine(dashRoutine);
        //        StopCoroutine(dashParticleRoutine);
        //        StopCoroutine(dashAfterImageRoutine);
        //    }

        //    Dash(EightDirVector);
        //    dashParticleRoutine = StartCoroutine(dashParticles());
        //    //can prob remove below line
        //    canDash = false;
        //    //Dash(new Vector2(x, y)); //new Vector3(0, 0, 0));
        //}
        if(coll.botSpring && !springing)
        {
            springing = true;
            jumpScript.enabled = false;
            PlayerRB2D.velocity = new Vector2(0, springVel);
            StartCoroutine(springTime());

            //PlayerRB2D.velocity = Vector2.zero;
            //PlayerRB2D.velocity += new Vector2(0, springVel);
        }
        //if(springing)
        //{
        //    PlayerRB2D.velocity = new Vector2(0, springVel);
        //}
    }

    private void FixedUpdate()
    {
        if (canMove && !dashing && !((coll.onLeftWall || coll.onRightWall) && controls.Player.Climb.IsPressed()))
        {
            moveXY();
        }
        if ((coll.onLeftWall || coll.onRightWall) && controls.Player.Climb.IsPressed())
        {
            wallClimb();
            climbing = true;
            PlayerRB2D.gravityScale = 0;
        }
        else
        {
            climbing = false;
        }
        if(climbing != climbCheck)
        {
            climbCheck = climbing;
            if(!climbing)
            {
                PlayerRB2D.gravityScale = gravScale;
            }
        }
    }
    #region input
    private Vector2 EightDirConversion()
    {
        float EightX;
        float EightY;
        if (Mathf.Abs(controls.Player.Movement.ReadValue<Vector2>().x) > deadZone)
        {
            //I think... this could just mathf.sign... but it's fine.
            EightX = controls.Player.Movement.ReadValue<Vector2>().x / Mathf.Abs(controls.Player.Movement.ReadValue<Vector2>().x);
        }
        else
        {
            EightX = 0;
        }
        if (Mathf.Abs(controls.Player.Movement.ReadValue<Vector2>().y) > deadZone)
        {
            EightY = controls.Player.Movement.ReadValue<Vector2>().y / Mathf.Abs(controls.Player.Movement.ReadValue<Vector2>().y);
        }
        else
        {
            EightY = 0;
        }
        return new Vector2(EightX, EightY);
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion

    #region basic movement
    private void moveXY()
    {
        //Every. Single. Line. Is bad. Using a Lerp like this means we never really meet our desired speed. It's really silly.
        if(coll.onGround && Mathf.Abs(PlayerRB2D.velocity.x) <= speed)
        {
            //PlayerRB2D.velocity = new Vector2(x * speed, PlayerRB2D.velocity.y);
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), speedUp);
            //PlayerRB2D.velocity = new Vector2(x * speed, PlayerRB2D.velocity.y);
        }
        
            
        
        //fix, need to be able to "glide" onto platforms (fixed i think)
        //grab vel before wall hit and add to player?

        //fix is this check on x. Don't allow player to add vel in direction of wall.
        else if (!coll.onGround && coll.onLeftWall && x > 0)
        {
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), airMobilityMultiplier);
        }
        else if (!coll.onGround && coll.onRightWall && x < 0)
        {
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), airMobilityMultiplier);
        }
        else if(bJump.wallJumping)
        {
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), wallJumpMult);
        }

        else if (Mathf.Abs(PlayerRB2D.velocity.x) > speed)
        {
            //something like this seems fun
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), highSpeedAdjustmentMultiplier);
        }
        //last case, we want high speed to replace airmobility in those cases.
        else if (!coll.onGround && !bJump.wallJumping && !coll.onLeftWall && !coll.onRightWall)
        {
            PlayerRB2D.velocity = Vector2.Lerp(PlayerRB2D.velocity, new Vector2(EightDirVector.x * speed, PlayerRB2D.velocity.y), airMobilityMultiplier);
        }

    }
    public void Jump()
    {
        if (coll.onGround && canMove)
        {
            if (dashing)
            {
                PlayerRB2D.velocity += Vector2.up * yVel * Mathf.Sign(gravScale);
                //cancel dash?
            }
            else
            {
                PlayerRB2D.velocity += Vector2.up * yVel * Mathf.Sign(gravScale);
            }
            jumping = true;
        }
        if (!dashing && (coll.onLeftWall || coll.onRightWall) && !coll.onGround) //&& canWallJump)
        {
            wallJump();
        }
    }
    #endregion

    #region wall handling
    private void wallJump()
    {
        //needs fixing for left to right wall jumps
        canWallJump = false;
        bJump.wallJumping = true;
        if(coll.onLeftWall)
        {
            PlayerRB2D.velocity = Vector2.zero;
            PlayerRB2D.velocity += (Vector2.up * Mathf.Sign(gravScale) + Vector2.right) * wallJumpVel;
        }
        if(coll.onRightWall)
        {
            PlayerRB2D.velocity = Vector2.zero;
            PlayerRB2D.velocity += (Vector2.up * Mathf.Sign(gravScale) + Vector2.left) * wallJumpVel;
        }
        wallJumpRoutine = StartCoroutine(wallJumpCoroutine());
    }

    private void wallClimb()
    {
        float climbVal;
        climbVal = EightDirVector.y / (Mathf.Abs(EightDirVector.y));
        if(!bJump.wallJumping)
        {
            if(EightDirVector.y == 0)
            {
                PlayerRB2D.velocity = Vector2.zero;
            }
            else
            {
                PlayerRB2D.velocity = new Vector2(0, climbVal * climbSpeed);
            }
        }
    }

    //Unused method lol
    private void cancelWallJump()
    {
        StopCoroutine(wallJumpRoutine);
        bJump.wallJumping = false;
        canWallJump = true;
    }

    IEnumerator wallJumpCoroutine() //, Vector3 dashVector)
    {
        yield return new WaitForSeconds(wallJumpTime);
        bJump.wallJumping = false;
        canWallJump = true;
    }
    #endregion

    #region dashing
    private void startDash()
    {
        if (canDash && (!dashing || canCancelDash)) //&& (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0))
        {
            if (dashing)
            {
                canCancelDash = false;
                StopCoroutine(dashRoutine);
                StopCoroutine(dashParticleRoutine);
                StopCoroutine(dashAfterImageRoutine);
            }

            Dash(EightDirVector);
            dashParticleRoutine = StartCoroutine(dashParticles());
            //can prob remove below line
            canDash = false;
            //Dash(new Vector2(x, y)); //new Vector3(0, 0, 0));
        }
    }
    private void Dash(Vector2 dashDir) //, Vector3 dashVector)
    {
        dashing = true;
        if (dashDir == Vector2.zero)
        {
            if(animScript.facingRight)
            {
                dashDir = Vector2.right;
            }
            else
            {
                dashDir = Vector2.left;
            }
        }
        bJump.enabled = false;
        PlayerRB2D.velocity = (Vector2.zero);
        PlayerRB2D.velocity += (dashDir.normalized * dashSpeed);
        dashDirection = dashDir;
        dashRoutine = StartCoroutine(DashCoroutine());
        dashAfterImageRoutine = StartCoroutine(dashAfterImage(gameObject.GetComponent<SpriteRenderer>()));
    }

    
    //EndDash is referenced by other classes, hence it being public.
    public void EndDash(bool doSlowDown)
    {
        if (dashing)
        {
            //canDash = false;
            //PlayerRB2D.velocity = (Vector2.zero);

            //maybe uncomment below line
            //PlayerRB2D.velocity = new Vector2(x * speed, 0);
            if(doSlowDown)
            {
                //PlayerRB2D.velocity = new Vector2(x * speed, 0);
                //PlayerRB2D.velocity = new Vector2(0, 0);
            }
            PlayerRB2D.velocity = new Vector2(PlayerRB2D.velocity.x, 0);

            //PlayerRB2D.velocity = new Vector2(x * speed, y * speed);
            PlayerRB2D.gravityScale = gravScale;
            dashing = false;
            bJump.enabled = true;

            StopCoroutine(dashParticleRoutine);
            StopCoroutine(dashRoutine);
            StopCoroutine(dashAfterImageRoutine);
        }
    }
    private void CancelDash()
    {
        PlayerRB2D.gravityScale = gravScale;
        dashing = false;
        bJump.enabled = true;

        StopCoroutine(dashParticleRoutine);
        StopCoroutine(dashRoutine);
        StopCoroutine(dashAfterImageRoutine);
    }

    private IEnumerator DashCoroutine() //, Vector3 dashVector)
    {

        PlayerRB2D.gravityScale = 0;
        canCancelDash = false;

        yield return new WaitForSeconds(dashTime / 2);

        canCancelDash = true;

        yield return new WaitForSeconds(dashTime / 2);
        canCancelDash = false;

        EndDash(true);
    }
    private IEnumerator dashParticles()
    {
        float particleTime = 0.05f * Random.Range(1.0f, 2.0f);

        for (int i = 0; i < (dashTime / particleTime); i++)
        {
            Instantiate(dashParticleObject, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(particleTime);
        }
    }
    private IEnumerator dashAfterImage(SpriteRenderer currentSprite)
    {
        //shader that we'll use to make our afterimage look different
        Shader GUIShader;
        //the GUI text shader allows us to just set the sprite as one color
        GUIShader = Shader.Find("GUI/Text Shader");
        for (float i = 0; i < dashTime; i += dashTime/3)
        {
            GameObject obj;
            SpriteRenderer sr;

            //create our afterimage object
            obj = Instantiate(dashAfterImageObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), Quaternion.identity);

            //Scale and sprite set
            obj.transform.localScale = gameObject.transform.localScale;
            sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = currentSprite.sprite;

            //some hacky renderer stuff to make the sprite single color
            sr.material.shader = GUIShader;
            
            //white with 50% opacity
            sr.color = new Color (1, 1, 1, 0.5f);

            
            sr.flipY = (Mathf.Sign(gravScale) == -1);

            //Wait to loop again
            yield return new WaitForSeconds(dashTime / 3);
        }
        yield return null;
    }
    #endregion


    private IEnumerator springTime()
    {
        yield return new WaitForSeconds(0.1f);
        springing = false;
        jumpScript.enabled = true;
    }

    private IEnumerator gravityReverser()
    {
        while(dashing)
        {
            yield return null;
        }
        PlayerRB2D.gravityScale *= -1;
        gravScale = PlayerRB2D.gravityScale;
        yield return null;
    }

    private void freezePlayer()
    {
        freezeVel = PlayerRB2D.velocity;
        PlayerRB2D.velocity = Vector2.zero;
        PlayerRB2D.bodyType = RigidbodyType2D.Static;

    }

    private void unFreezePlayer()
    {
        PlayerRB2D.bodyType = RigidbodyType2D.Dynamic;
        PlayerRB2D.velocity = freezeVel;
    }
}
