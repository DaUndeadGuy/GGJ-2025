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

                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
                {
                    // Horizontal movement
                    sr.sprite = moveDir.x > 0 ? rightSprite : leftSprite;
                }
                else
                {
                    // Vertical movement
                    sr.sprite = moveDir.z > 0 ? upSprite : downSprite;
                }
            }
            else
            {
                // Optional: Handle idle state by keeping the last direction
                if (lastMoveDirection != Vector3.zero)
                {
                    if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.z))
                    {
                        sr.sprite = lastMoveDirection.x > 0 ? rightSprite : leftSprite;
                    }
                    else
                    {
                        sr.sprite = lastMoveDirection.z > 0 ? upSprite : downSprite;
                    }
                }
            }
        }
        else if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0,0,0);
        }
    }

}
