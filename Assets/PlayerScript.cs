using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{ 
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    public Rigidbody2D myRigidBody;
    public float jumpStrength;
    public float speed;
    

    private float Move;
    private BoxCollider2D playerCollider;
    private BoxCollider2D wallCollider;

    //dashing variables
    public float dash;
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    //wallSliding variables
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    //wallJumping variables
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.1f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(8f,8f);

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        wallCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {


        
        if(isDashing){
            return;
        }

        //Move now equals -1, 0, or 1
        Move = Input.GetAxis("Horizontal");

        

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash){
            StartCoroutine(Dash());
        }

        Jump();
        WallJump();
        WallSlide();
        
    }

    void FixedUpdate()
    {
        if(isDashing){
            return;
        }

        if(!isWallJumping){
            //moves the player when uses a or d or arrow keys
            myRigidBody.velocity = new Vector2(speed * Move, myRigidBody.velocity.y);
        }
    }

    //Checks IsGrounded then moves the player up
    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded()){
            myRigidBody.velocity = new Vector2(0f, jumpStrength);
            
        }
    }
    //sends a box cast downward to see if the player is on the ground
    private bool IsGrounded(){
        float extraHeight = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);

        return raycastHit.collider != null;
    }

    //moves the player forward at an accelerated speed
    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;
        float originalGravity = myRigidBody.gravityScale;
        myRigidBody.gravityScale = 0f;
            myRigidBody.velocity = new Vector2(Move * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        myRigidBody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool IsWalled(){
        float extraHeight = .5f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(wallCollider.bounds.center, wallCollider.bounds.size, 0f, Vector2.right, extraHeight, wallLayer);

        return raycastHit.collider != null;
    }

    private void WallSlide(){
        if(IsWalled() && !IsGrounded() && Move != 0f){
            isWallSliding = true;
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, Mathf.Clamp(myRigidBody.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else{
            isWallSliding = false;
        }
    }

    private void WallJump(){
        if (isWallSliding){
            isWallJumping = false;
            wallJumpingDirection = -Move;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else{
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f){
            isWallJumping = true;
            myRigidBody.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

        
    }

    private void StopWallJumping(){
        Debug.Log("1234");
        isWallJumping = false;
    }
}
