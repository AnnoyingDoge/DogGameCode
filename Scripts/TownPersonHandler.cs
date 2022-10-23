using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPersonHandler : MonoBehaviour
{
    public bool touchingPlayer;
    public bool drawPrompt;
    public bool isTalking;
    public bool goToPoint;
    public bool moveRight;

    public int promptDrawCounter;
    public int drawSpeed;

    public float promptTransparency;
    public float boxTransparency;
    public float textColorR;
    public float textColorG;
    public float textColorB;

    public GameObject textPrompt;
    private GameObject player;

    public Color textPromptColor;

    private Vector2 pointToGo;

    private newMove moveScript;
    private Collision collPlayer;
    private Rigidbody2D playerRB;
    private dogAnimator playerAnim;
    public InputMaster controls;

    Vector2 y;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Jump.performed += context => DialogueInteract();
    }

    // Start is called before the first frame update
    void Start()
    {
        isTalking = false;
        drawPrompt = false;
        touchingPlayer = false;

        promptTransparency = 0f;
        drawSpeed = 10;

        player = GameObject.FindGameObjectWithTag("player").gameObject;

        textPrompt = gameObject.transform.Find("testTalkSprite").gameObject;
        moveScript = player.GetComponent<newMove>();
        collPlayer = player.GetComponent<Collision>();
        playerRB = player.GetComponent<Rigidbody2D>();
        playerAnim = player.GetComponent<dogAnimator>();

        textPromptColor = textPrompt.GetComponent<SpriteRenderer>().color;
        textColorR = textPrompt.GetComponent<SpriteRenderer>().color.r;
        textColorG = textPrompt.GetComponent<SpriteRenderer>().color.g;
        textColorB = textPrompt.GetComponent<SpriteRenderer>().color.b;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(touchingPlayer)
        {
            if (!isTalking)
            {
                drawPrompt = true;
            }
            else
            {
                drawPrompt = false;
            }
            if(controls.Player.Movement.ReadValue<Vector2>().y > 0.9)
            {
                isTalking = true;
                moveScript.enabled = false;
                StartDialogue();
            }
        }
        else
        {
            drawPrompt = false;
        }
    }

    private void FixedUpdate()
    {
        if (drawPrompt)
        {
            DrawPrompt();
        }
        else
        {
            unDrawPrompt();
        }

        if(goToPoint)
        {
            if(moveRight)
            {
                playerRB.velocity = new Vector2(7, 0);
                if(player.transform.position.x >= pointToGo.x)
                {
                    playerRB.velocity = Vector2.zero;
                    goToPoint = false;
                    playerAnim.facingRight = false;
                }
            }
            else
            {
                playerRB.velocity = new Vector2(-7, 0);
                if (player.transform.position.x <= pointToGo.x)
                {
                    playerRB.velocity = Vector2.zero;
                    goToPoint = false;
                    playerAnim.facingRight = true;
                }
            }
        }
    }

    private void DrawPrompt()
    {
        //this handles transparency/fade in, but poorly lol
        //print(textPrompt.GetComponent<SpriteRenderer>().color);
        
        if (promptTransparency <= 1f)
        {
            promptTransparency += 0.01f * drawSpeed;
            textPrompt.GetComponent<SpriteRenderer>().color = new Color(textColorR, textColorG, textColorB, promptTransparency);
        }
    }

    private void unDrawPrompt()
    {
        //this handles transparency/fade in, but poorly lol
        //print(textPrompt.GetComponent<SpriteRenderer>().color);
        
        if (promptTransparency >= 0)
        {
            promptTransparency -= 0.01f * drawSpeed;
            textPrompt.GetComponent<SpriteRenderer>().color = new Color(textColorR, textColorG, textColorB, promptTransparency);
        }
    }

    private void DialogueInteract()
    {
        //Put code to progress dialogue here.
        //For now we just cancel dialogue.
        isTalking = false;
        moveScript.enabled = true;

    }

    private void StartDialogue()
    {
        //First, make player go to point.
        while (!collPlayer.onGround)
        {

        }

        //Go to the point we want to walk to by comparing positions.
        if (player.transform.position.x >= gameObject.transform.position.x)
        {
            pointToGo = (new Vector3(gameObject.transform.position.x + 2, player.transform.position.y, player.transform.position.z));
            moveRight = true;
            playerAnim.facingRight = true;
        }
        else
        {
            pointToGo = (new Vector3(gameObject.transform.position.x - 2, player.transform.position.y, player.transform.position.z));
            moveRight = false;
            playerAnim.facingRight = false;
        }
        goToPoint = true;

        //Next (tell other script to?) draw dialogue boxes and start conversation.

        //When dialogue ends, set isTalking to false, enable movescript.
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
