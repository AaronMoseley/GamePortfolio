using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    InputManager input;
    Rigidbody2D rb;

    CollManager collisions;

    [Header("Art")]
    public Sprite normalPlayer;
    public Sprite crouchedPlayer;
    public Sprite playerBlink;
    public Sprite crouchPlayerBlink;
    public GameObject playerGFX;
    [Space]

    [Header("Dimensions")]
    public float normalHeight;
    public float width;
    public float crouchedHeight;
    [Space]

    [Header("Speeds")]
    public float normalSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float inAirForce;
    public float maxSpeed;
    public float currSpeed;
    [Space]

    [Header("Jumping")]
    public float jumpForce;
    public float wallJumpSideVel;
    public float wallJumpWaitTime;
    [Space]

    [Header("Sliding")]
    public float slideForce;
    public float frictionForce;
    public float slideToCrouchVelPercent;
    [Space]

    [Header("Misc")]
    public float gravForce;

    public string state = "normal";

    public int groundLayer;
    int groundLayerMask;

    public bool touchingHook = false;

    bool waiting = false;
    bool forceCrouch = false;

    GameObject hook;
    Grappler grappler;

    Camera mainCam;

    bool equippedGrappler;
    string grapplerState;

    public float grapplerHookDefaultDist;
    public bool talking;

    Inventory inventory;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        collisions = gameObject.GetComponentInChildren<CollManager>();
        inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        currSpeed = normalSpeed;

        groundLayerMask = 1 << groundLayer;

        if(gameObject.GetComponentInChildren<Grappler>())
        {
            SetUpGrappler(true);
        } else
        {
            grapplerState = "stationary";
        }
    }

    //Physics updates 
    void FixedUpdate()
    {
        //Establishes Axial Inputs for the 0.2 second time span
        float x = input.Axis("Horizontal");
        float y = input.Axis("Vertical");

        int xRaw = input.AxisRaw("Horizontal");
        int yRaw = input.AxisRaw("Vertical");

        if (!inventory.invOpen)
        {
            if (!touchingHook && ((xRaw > 0 && !collisions.bools[2]) || (xRaw < 0 && !collisions.bools[1]) || xRaw == 0 || state == "crouching") && (state == "normal" || state == "sprinting" || state == "crouching") && ((!waiting || (xRaw > 0 && collisions.bools[1])) || (!waiting || (xRaw < 0 && collisions.bools[2]))))
            {
                //Sets velocity to the inputs times speed if the grappler isn't being used, the player isn't trying to walk into a wall, the player isn't moving on the x axis,
                //the player is walking, crouching, or sprinting, and the player isn't trying to go back to a wall they just jumped from.
                if (collisions.bools[0])
                {
                    rb.velocity = new Vector2(currSpeed * xRaw, rb.velocity.y);
                } else
                {
                    rb.AddForce(new Vector2(inAirForce * xRaw, 0));
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -currSpeed, currSpeed), rb.velocity.y);
                }
            }
            else if (touchingHook)
            {
                //If the player is grappling, apply a force to swing around.
                rb.AddForce(new Vector2(xRaw * 100, 0));
            }

            if (state == "sliding")
            {
                //If player is crouch sliding, apply a customizable friction force.
                rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * frictionForce, 0));
            }
        }

        //Prevent the player from going too fast while falling, sprinting, etc.
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));

        //Customizable gravity force
        rb.AddForce(new Vector2(0, -gravForce));
    }

    private void Update()
    {
        float mouseXPos = mainCam.ScreenToWorldPoint(Input.mousePosition).x;

        if(mouseXPos >= gameObject.transform.position.x)
        {
            playerGFX.transform.localScale = new Vector3(1, 1, 1);
        } else
        {
            playerGFX.transform.localScale = new Vector3(-1, 1, 1);
        }

        if(equippedGrappler)
        {
            grapplerState = grappler.state;
        } 

        if (!inventory.invOpen)
        {
            if(state == "sliding" && !collisions.bools[0])
            {
                state = "normal";
                Crouch();
            }

            if (((collisions.bools[1] || collisions.bools[2]) && !collisions.bools[0] && input.Button("WallGrab") && !waiting) && grapplerState == "stationary")
            {
                //If player is touching a wall, clicking the wallgrab button, didn't just wallgrab, and isn't grappling, start wallgrabbing.
                rb.velocity = new Vector2(0, 0);
                state = "grabbing";

                if (input.ButtonDown("Jump"))
                {
                    state = "normal";

                    if (collisions.bools[1] && !collisions.bools[0])
                    {
                        rb.velocity = new Vector2(wallJumpSideVel, jumpForce);
                    }
                    else if (collisions.bools[2])
                    {
                        rb.velocity = new Vector2(-wallJumpSideVel, jumpForce);
                    }

                    waiting = true;
                    StartCoroutine(WaitForGrab());
                }
            }
            else if (state == "grabbing")
            {
                state = "normal";
            }
            else if (input.ButtonDown("Jump") && collisions.bools[0] && (state == "normal" || state == "sprinting") && !touchingHook)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            if (grapplerState == "stationary")
            {
                if (input.ButtonDown("Sprint") && state == "normal")
                {
                    state = "sprinting";
                    currSpeed = sprintSpeed;
                }
                else if (input.ButtonDown("Sprint") && state == "sprinting")
                {
                    state = "normal";
                    currSpeed = normalSpeed;
                }
                else if (currSpeed != normalSpeed && state == "normal")
                {
                    currSpeed = normalSpeed;
                }

                if (input.Button("Crouch") && (state == "normal" || state == "sprinting") && collisions.bools[0])
                {
                    state = "crouching";
                    Crouch();
                }
                else if ((state == "crouching" || state == "sliding") && !input.Button("Crouch"))
                {
                    state = "normal";
                    Crouch();
                }

                if (!UnCrouchTest() && forceCrouch)
                {
                    forceCrouch = false;
                    state = "normal";
                    Crouch();
                }
            }

            if (state == "sliding" && Mathf.Abs(rb.velocity.x) < crouchSpeed)
            {
                state = "crouching";
                Crouch();
            }
        }

        if (state != "crouching" && state != "sliding" && playerGFX.GetComponent<SpriteRenderer>().sprite != normalPlayer && playerGFX.GetComponent<SpriteRenderer>().sprite != playerBlink)
        {
            playerGFX.GetComponent<SpriteRenderer>().sprite = normalPlayer;
        }
        else if (state == "crouching" && state == "sliding" && playerGFX.GetComponent<SpriteRenderer>().sprite != crouchedPlayer && playerGFX.GetComponent<SpriteRenderer>().sprite != crouchPlayerBlink)
        {
            playerGFX.GetComponent<SpriteRenderer>().sprite = crouchedPlayer;
            Debug.Log(state);
        }
    }

    public void Crouch()
    {
        if (state == "crouching")
        {
            playerGFX.GetComponent<SpriteRenderer>().sprite = crouchedPlayer;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, crouchedHeight);
            currSpeed = crouchSpeed;

            if (Mathf.Abs(rb.velocity.x) > 0.5f * normalSpeed && state != "sliding")
            {
                state = "sliding";
                rb.AddForce(new Vector2(Mathf.Sign(rb.velocity.x) * slideForce, 0));
            }
        } else
        {
            if(!UnCrouchTest())
            {
                state = "normal";
                playerGFX.GetComponent<SpriteRenderer>().sprite = normalPlayer;
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, normalHeight);
                currSpeed = normalSpeed;
            } else
            {
                state = "crouching";
                forceCrouch = true;
                //Crouch();
            }
        }
    }

    public void SetUpGrappler (bool equip)
    {
        equippedGrappler = equip;

        if(equip)
        {
            grappler = gameObject.GetComponentInChildren<Grappler>();
            hook = gameObject.GetComponentInChildren<Hook>().gameObject;
            grapplerHookDefaultDist = Vector2.Distance(grappler.transform.position, hook.transform.position);
        } else
        {
            grappler = null;
            hook = null;
            grapplerHookDefaultDist = 0;

            grapplerState = "stationary";
        }
    }

    bool UnCrouchTest()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, normalHeight / 2, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - (width / 2), gameObject.transform.position.y), gameObject.transform.up, normalHeight / 2, groundLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + (width / 2), gameObject.transform.position.y), gameObject.transform.up, normalHeight / 2, groundLayerMask);

        if(hit1 || hit2 || hit3)
        {
            return true;
        }

        return false;
    }

    IEnumerator WaitForGrab()
    {
        yield return new WaitForSecondsRealtime(wallJumpWaitTime);
        waiting = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == hook)
        {
            touchingHook = true;
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject == hook)
        {
            touchingHook = false;
        }
    }
}
