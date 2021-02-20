using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    //Start() variables
    private Rigidbody2D rb;   // creating object for our character.
    private Animator anim;    //method
    private Collider2D coll;
    public int cherries = 0;

    //FSM
    private enum State {idle, running, jumping, falling}   //using enum to store states of our entity
    private State state = State.idle;  //default state
    
    //inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {

        Movement();

        AnimationState();
        anim.SetInteger("state", (int)state);   //sets animation based on Enumerator state
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")   // if our entity collides with a cherry, ie, if it hits a trigger
        {
            Destroy(collision.gameObject);     //destroy the cherry(Destroy is an in built function in unity)
            cherries += 1;                    //incrementing cherries
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        //moving left
        if(hDirection < 0)    // if we press 'A'
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);   //velocity of entity in (horizontal direction, vertical direction)
            transform.localScale = new Vector2(-1, 1);
        }

        //moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1,1);
        }

        //jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    }

    private void AnimationState()
    {
        if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }

        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if(Mathf.Abs(rb.velocity.x) > 2f)   //Mathf.Epsilon is used for stopping,ie, 0 velocity
        {
            //Moving
            state = State.running;
        }

        else
        {
            state = State.idle;    
        }
    }
}
