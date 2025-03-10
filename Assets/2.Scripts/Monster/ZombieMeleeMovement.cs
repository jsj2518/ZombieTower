using UnityEngine;

public class ZombieMeleeMovement : MonoBehaviour
{
    private const float basePosX = 6f;
    private const float basePosY = -3.6f;
    private const float stepPosY = 0.4f;

    #region Property
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Movement")]
    [SerializeField] private Transform raycastFront;
    [SerializeField] private Transform raycastBack;
    [SerializeField] private Transform raycastUpFront;
    [SerializeField] private Transform raycastUpBack;

    [Header("Stat")]
    [SerializeField] private ZombieMeleeStatSO stat;

    private LayerMask layerZombieMelee;
    #endregion

    private 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Initialize(Enums.Lane lane)
    {
        gameObject.layer = (int)lane;
        layerZombieMelee = 1 << (int)lane;

        float posY = (lane - Enums.Lane.Lane1) * stepPosY + basePosY;
        transform.position = new Vector2 (basePosX, posY);

        gameObject.SetActive(true);
    }

    void Update()
    {
        bool goForward = true;
        bool goBackward = false;
        bool goUpward = false;
        float speedForward = -rb.velocity.x;
        float speedUpward = rb.velocity.y;

        DoRaycast(out bool frontDetected, out bool backDetected, out bool upFrontDetected, out bool upBackDetected);
        if (upFrontDetected)
        {
            goForward = false;
            goBackward = true;
        }
        else
        {
            if (backDetected || upBackDetected)
            {
                //goForward = false;
            }
            else if (frontDetected)
            {
                goUpward = true;
            }
        }

        if (goBackward)
        {
            speedForward = -stat.pushbackSpeed;
        }
        else if (goForward)
        {
            speedForward += stat.runAcceleration * Time.deltaTime;
            if (speedForward > stat.runSpeed) speedForward = stat.runSpeed;
        }

        if (goUpward)
        {
            speedUpward = stat.climbSpeed;
        }
        else
        {
            speedUpward += Physics2D.gravity.y * rb.gravityScale * Time.deltaTime;
        }

        rb.velocity = new Vector2(-speedForward, speedUpward);
    }

    private void DoRaycast(out bool frontDetected, out bool backDetected, out bool upFrontDetected, out bool upBackDetected)
    {
        frontDetected = Physics2D.Raycast(raycastFront.position, Vector2.left, stat.rayCastFrontDistance, layerZombieMelee);
        Debug.DrawRay(raycastFront.position, Vector2.left * stat.rayCastFrontDistance, frontDetected ? Color.cyan : Color.red);

        backDetected = Physics2D.Raycast(raycastBack.position, Vector2.right, stat.rayCastBackDistance, layerZombieMelee);
        Debug.DrawRay(raycastBack.position, Vector2.right * stat.rayCastBackDistance, backDetected ? Color.cyan : Color.red);

        upFrontDetected = Physics2D.Raycast(raycastUpFront.position, Vector2.up, stat.rayCastUpDistance, layerZombieMelee);
        Debug.DrawRay(raycastUpFront.position, Vector2.up * stat.rayCastUpDistance, upFrontDetected ? Color.cyan : Color.red);

        upBackDetected = Physics2D.Raycast(raycastUpBack.position, Vector2.up, stat.rayCastUpDistance, layerZombieMelee);
        Debug.DrawRay(raycastUpBack.position, Vector2.up * stat.rayCastUpDistance, upBackDetected ? Color.cyan : Color.red);
    }
}
