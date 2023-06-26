using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Start() variables
    public Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    



    //FSM
    private enum State { idle, run, jump, fall, hurt, climb}
    private State state = State.idle;

    //Ladder variables
    public bool canClimb = false;
    public bool bottomLadder = false;
    public bool topLadder = false;
    public Ladder ladder;
    private float naturalGravity = 1f;
    [SerializeField] float climbSpeed = 4f;

    //Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource coinAS;
    [SerializeField] private AudioSource footStep;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        //PermanentUI.perm.healthAmount.text = PermanentUI.perm.health.ToString();
        naturalGravity = rb.gravityScale;

    }

    private void Update()
    {
        if (state == State.climb)
        {
            Climb();
        }
        if (state != State.hurt)
        {
            Movement();
        }
        
        AnimationState();
        anim.SetInteger("state", (int)state); //sets animation based on Enumerator state

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            coinAS.Play();
            Destroy(collision.gameObject);
            ++PermanentUI.perm.coins;
            PermanentUI.perm.coinCount.text = PermanentUI.perm.coins.ToString();

        }
        
        if(collision.tag == "PowerUp")
        {
            Destroy(collision.gameObject);
            jumpForce = 7f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (state == State.fall)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                HandleHealth(); //Deals with health, updating ui, and will reset level if health is <= 0
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to my right therefore i should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Enemy is to my left therefore i should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void HandleHealth()
    {
        PermanentUI.perm.health -= 1;
        PermanentUI.perm.healthAmount.text = PermanentUI.perm.health.ToString();
        if (PermanentUI.perm.health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            PermanentUI.perm.ResetHealth();
            PermanentUI.perm.ResetCoins();
        }
    }

    

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");
        if (canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            rb.drag = 5;
            state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(ladder.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;

        }
        //Moving Left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);

        }
        //Moving Right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }
        //Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void AnimationState()
    {
        if (state == State.climb)
        {

        }
        else if(state == State.jump)
        {
            if(rb.velocity.y < 0.1f)
            {
                state = State.fall;
            }
        }
        else if(state == State.fall)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
            
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            //Moving
            state = State.run;
        }
        else
        {
            state = State.idle;
        }
        
    }

    private void Jump()
    {
        state = State.jump;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void FootStep()
    {
        footStep.Play();
    }

    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(16);
        jumpForce = 5f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    private void Climb()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            rb.drag = 0;
            return;
            
        }
        float vDirection = Input.GetAxis("Vertical");
        //climbing up
        if (vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
        }
        //Climbing down
        else if(vDirection < -.1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
        }
        //still
        else
        {

        }
    }
}
