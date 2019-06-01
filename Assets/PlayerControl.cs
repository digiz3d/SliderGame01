using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float runningSpeed = 15f;
    [SerializeField]
    private float slidingSpeed = 35f;

    [SerializeField]
    private float jumpForce = 25f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform spriteTransform;

    private bool isGrounded = false;
    public bool isOverSlide = false;

    private Direction direction = Direction.none;
    private bool jump = false;
    private bool slide = false;
    private Rigidbody2D rb;
    private Collider2D col2d;

    public float currentSpeedX = 0f;
    public float targetSpeed = 0f;

    [SerializeField]
    private float slideFallFactor;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Right();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Left();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
    }

    public float GetCurrentSpeedX()
    {
        return currentSpeedX;
    }

    private void SetTargetSpeed(float speed)
    {
        switch (direction)
        {
            case Direction.right:
                targetSpeed = speed;
                break;


            case Direction.left:
                targetSpeed = -speed;

                break;

            default:
                targetSpeed = 0f;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics2D.IsTouchingLayers(col2d, groundLayer);

        // if the user wants to jump while touching the ground
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        #region isOverSlide Detection
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 5, groundLayer);

        if (hit.collider != null)
        {
            float angle = Vector2.SignedAngle(Vector2.up, hit.normal);
            float angleAbs = Mathf.Abs(angle);
            // if the slide is between those numbers...
            if (15f < angleAbs && angleAbs < 75f)
            {
                // and is in the right direction :
                if (direction == Direction.right && angle < 0f)
                {
                    isOverSlide = true;
                }
                else if (direction == Direction.left && angle > 0f)
                {
                    isOverSlide = true;
                }
                else
                {
                    isOverSlide = false;
                }
            }
            else
            {
                isOverSlide = false;
            }
        }
        else
        {
            isOverSlide = false;
        }
        if (!isOverSlide)
        {
            slide = false;
        }
        #endregion

        // if the user wants to slide and can slide
        if (direction == Direction.none)
        {
            targetSpeed = 0f;
        }
        else if (slide && isOverSlide)
        {
            if (hit.collider != null)
            {
                // lets make the guy fall faster
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - Time.deltaTime * slideFallFactor);
                SetTargetSpeed(slidingSpeed);

            }
            else
            {
                slide = false;
                SetTargetSpeed(runningSpeed);
            }

        }
        else
        {
            slide = false;
            SetTargetSpeed(runningSpeed);
        }

        currentSpeedX = Mathf.Lerp(currentSpeedX, targetSpeed, Time.fixedDeltaTime * 2f);
        rb.velocity = new Vector2(Mathf.Clamp(currentSpeedX, -slidingSpeed, slidingSpeed), rb.velocity.y);
        jump = false;
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void Right()
    {
        direction = Direction.right;
        spriteTransform.localRotation = Quaternion.Euler(spriteTransform.localRotation.eulerAngles.x, 0, spriteTransform.localRotation.eulerAngles.z);
    }

    public void Left()
    {
        direction = Direction.left;
        spriteTransform.localRotation = Quaternion.Euler(spriteTransform.localRotation.eulerAngles.x, 180, spriteTransform.localRotation.eulerAngles.z);
    }

    public void Jump()
    {
        jump = true;
        slide = false;
    }

    public void Slide()
    {
        slide = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 5);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 5, groundLayer);

        if (hit.collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.point, 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hit.point, hit.point + hit.normal);

            //Debug.Log("2-angle = " + Vector2.Angle(Vector2.up, hit.normal));
            //Debug.Log("2-angle = " + Vector2.SignedAngle(Vector2.up, hit.normal));
        }
    }
}

public enum Direction
{
    none,
    left,
    right
}