using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonEnemy : Enemy
{

    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLenght = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground; 
    private Collider2D coll;


    private bool facingLeft = true;

    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //Transition from jump to fall;
        if (anim.GetBool("Jumping")){
            if(rb.velocity.y < .1)
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }
        //Transition from fall to idle;
        if(coll.IsTouchingLayers(ground) && anim.GetBool("Falling"))
        {
            anim.SetBool("Falling", false);
        }
    }

    private void Move()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                //make sure sprite is facing right location, and if it is not, then face right direction
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }
                //Test to see if i am on the ground, if so jump
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLenght, jumpHeight);
                    anim.SetBool("Jumping", true);
                    
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                //make sure sprite is facing right location, and if it is not, then face right direction
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }
                //Test to see if i am on the ground, if so jump
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLenght, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }
}
