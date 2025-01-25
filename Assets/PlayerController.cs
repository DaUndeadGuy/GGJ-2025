using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject interactCollider;

    public float speed;
    public float groundDist;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Sprite rightSprite; // Sprite for moving right
    public Sprite leftSprite;  // Sprite for moving left
    public Sprite downSprite; // Sprite for moving right
    public Sprite upSprite;  // Sprite for moving left
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

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0, y);
        rb.linearVelocity = moveDir * speed;

        if (x != 0 && x < 0)
        {
            sr.sprite = leftSprite;
        }
        else if (x != 0 && x > 0)
        {
            sr.sprite = rightSprite;
        }

        else if (y != 0 && y < 0)
        {
            sr.sprite = downSprite;
        }
        else if (y != 0 && y > 0)
        {
            sr.sprite = upSprite;
        }
    }
}
