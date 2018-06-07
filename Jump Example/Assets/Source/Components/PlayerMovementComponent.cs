using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour {
    [Header("Movement")]
    public float gravity;

    public float jumpSpeed;
    public float jumpTime;
    public float coyoteTime;
    public float moveSpeed;
    public float moveSpeedTime;
    private float moveAcceleration;

    private bool jumping;
    private float jumpStartTime;

    private bool coyoteTiming;
    private bool previousColliding;
    private float lastCollidedTime;

    private Vector3 velocity;

    [Header("Animation")]
    public Sprite idleSprite;
    public Sprite run1Sprite;
    public Sprite run2Sprite;
    public Sprite jumpSprite;
    public Sprite fallSprite;

    private SpriteRenderer spriteRenderer;

	void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	void Update(){
        HandleInput();
        UpdatePhysics();
        UpdateAnimation();
    }

    void HandleInput(){
        float now = Time.time;
        bool colliding = Colliding(transform.position);

        // Jumping
        if(Input.GetKeyDown(KeyCode.Space) && (colliding || (coyoteTiming && lastCollidedTime + coyoteTime >= now))){
            jumpStartTime = now;
            jumping = true;
            coyoteTiming = false;
        }

        if(jumping){
            velocity.y = jumpSpeed;

            if(Input.GetKeyUp(KeyCode.Space) || jumpStartTime + jumpTime <= now){
                jumping = false;
            }
        }

        // If we've walked off a cliff
        if(previousColliding && !colliding && !jumping && !coyoteTiming){
            coyoteTiming = true;
            lastCollidedTime = now;
        }

        // Movement
        if(Input.GetKey(KeyCode.RightArrow)){
            velocity.x = Mathf.SmoothDamp(velocity.x, moveSpeed, ref moveAcceleration, moveSpeedTime);
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            velocity.x = Mathf.SmoothDamp(velocity.x, -moveSpeed, ref moveAcceleration, moveSpeedTime);
        } else {
            velocity.x = Mathf.SmoothDamp(velocity.x, 0.0f, ref moveAcceleration, moveSpeedTime);
        }

        previousColliding = colliding;
    }

    void UpdatePhysics(){
        velocity += new Vector3(0.0f, -gravity, 0.0f);

        // The cheapest way I could think of to make a platform with a cliff without using any physics code
        Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        if(Colliding(newPosition)){
            velocity.y = 0.0f;
            newPosition.y = Mathf.Max(newPosition.y, 0.0f);
        }
        transform.position = newPosition;
	}

    bool Colliding(Vector3 position){
        return position.y <= 0.0f && position.x <= 0.0f;
    }

    void UpdateAnimation(){
        spriteRenderer.flipX = velocity.x < 0.0f;

        if(Mathf.Abs(velocity.x) > 0.05f){
            // run animation...
        }

        // stomp all the other sprites if we're jumping or falling
        if(velocity.y < 0.05f){
            spriteRenderer.sprite = fallSprite;
        } else if(velocity.y > 0.05f){
            spriteRenderer.sprite = jumpSprite;
        }
    }
}
