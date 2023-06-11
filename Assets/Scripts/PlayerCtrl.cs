using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  make the player move left / right / jump / flips the player direction while moving
/// </summary>
public class PlayerCtrl : MonoBehaviour
{
    [Tooltip("This is an positive integer which speeds up the player movement")]
    public int speedBoost; //set this to 5
    public float jumpSpeed; //set this to 600
    public bool isGrounded;
    public Transform feet;
    public float feetRadius;
    public float boxWidth;
    public float boxHeight;
    public float delayForDoubleJump;
    public LayerMask whatIsGround;
    public Transform leftBulletSpawnPos, rightBulletSpawnPos;
    public GameObject leftBullet, rightBullet;
    public bool SFXOn;
    public bool canFire;
    public bool isJumping;
    public bool isStuck;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    bool canDoubleJump;
    public bool leftPressed, rightPressed;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    isGrounded = Physics2D.OverlapCircle(feet.position, feetRadius, whatIsGround);
       //isGrounded = Physics2D.OverlapBox(new Vector2(feet.position.x, feet.position.y), new Vector2(boxWidth, boxHeight) , 360.0f, whatIsGround);

        float playerSpeed = Input.GetAxisRaw("Horizontal"); // value : 1, -1 , 0
        playerSpeed *= speedBoost;
        if (playerSpeed != 0)
        {
            MoveHorizontal(playerSpeed);
        }
        else
        {
            StopMoving();
        }
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetInteger("State", 3);
            Jump();
        }
        if(Input.GetButtonDown("Fire1"))
        {
            FireBullets();
        }
        ShowFalling();
    }
    void OnDrawGizmos()
    {
       Gizmos.DrawWireSphere(feet.position, feetRadius);
        //Gizmos.DrawWireCube(feet.position, new Vector3(boxWidth, boxHeight, 0));
    }
    void MoveHorizontal(float playerSpeed)
    {
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
        if (playerSpeed < 0)
        {
            sr.flipX = true;
        }
        else if (playerSpeed > 0)
        {
            sr.flipX = false;
        }
        if (!isJumping)
        {
            anim.SetInteger("State", 1);
        }


    }

    // Update is called once per frame
    void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (!isJumping)
        {
            anim.SetInteger("State", 0);
        }
    }
    void ShowFalling()
    {
        if (rb.velocity.y < 0)
        {
            anim.SetInteger("State", 4);
        }
    }
    void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;
            rb.AddForce(new Vector2(0, jumpSpeed));
            anim.SetInteger("State", 3);

            Invoke("EnableDoubleJump",delayForDoubleJump);
        }

        if(canDoubleJump && !isGrounded) 
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, jumpSpeed));
            anim.SetInteger("State", 3);

            canDoubleJump = false;
        }
    }

    void EnableDoubleJump() {
        canDoubleJump = true;
    }

    void FireBullets() 
    {
        if(canFire) 
        {
            if(sr.flipX) 
        {
            Instantiate(leftBullet, leftBulletSpawnPos.position, Quaternion.identity);
        }
        if(!sr.flipX) 
        {
            Instantiate(rightBullet, rightBulletSpawnPos.position, Quaternion.identity);
        }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
	{
        switch (other.gameObject.tag)
        {
            case "Coin":
                if(SFXOn)
                    SFXCtrl.instance.ShowCoinSparkle(other.gameObject.transform.position);
                break;
            case "Water":
                // show the splash effect
                SFXCtrl.instance.ShowSplash(other.gameObject.transform.position);

                // inform the GameCtrl
                GameCtrl.instance.PlayerDrowned(gameObject);

                break;
            case "Powerup_Bullet":
                canFire = true;
                Vector3 powerupPos = other.gameObject.transform.position;
                Destroy(other.gameObject);
                if (SFXOn)
                    SFXCtrl.instance.ShowBulletSparkle(powerupPos);
                break;
            default:
                break;
        }		
	}
}
