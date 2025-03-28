using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;

    private void Awake()
    {
        //Grab references for rigidbody and animaor from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput*speed, body.linearVelocity.y);

        //Flip player when moving left/right
        if(horizontalInput>.01f)
            transform.localScale = Vector2.one;
        else if(horizontalInput <-.01f)
            transform.localScale = new Vector2(-1, 1);

        if(Input.GetKey(KeyCode.Space) && isGrounded())
            Jump();

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        print(onWall());

        //Wall jump logic
        if(wallJumpCooldown > .2f)
        {
            body.linearVelocity = new Vector2(horizontalInput*speed, body.linearVelocity.y);

            if(onWall() && isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = 7;

            if(Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    private void Jump()
    {
        if(isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
       else if(onWall() && isGrounded())
       {
            wallJumpCooldown = 0;
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
       }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
