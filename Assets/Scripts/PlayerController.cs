using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }    
    public float speed;
    public float groundDist;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Sprite rightSprite; // Sprite for moving right
    public Sprite leftSprite;  // Sprite for moving left
    public Sprite downSprite; // Sprite for moving down
    public Sprite upSprite;  // Sprite for moving up
    private Animator animator;

    public bool IsFacingLeft;
    public bool IsFacingUp;
    public bool CanMove;
    public Vector3 lastMoveDirection;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }
        if (CanMove)
        {

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector3 moveDir = new Vector3(x, 0, y);
            rb.linearVelocity = moveDir * speed;
            // Update sprite based on movement
            if (moveDir != Vector3.zero)
            {
                lastMoveDirection = moveDir;
                animator.SetBool("isWalking", true);

                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
                {
                    // Horizontal movement
                    //sr.sprite = moveDir.x > 0 ? rightSprite : leftSprite;
                    animator.SetFloat("LastX", moveDir.x);
                }
                else
                {
                    // Vertical movement
                    //sr.sprite = moveDir.z > 0 ? upSprite : downSprite;
                    animator.SetFloat("LastY", moveDir.z);

                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0,0,0);
        }
    }

}
