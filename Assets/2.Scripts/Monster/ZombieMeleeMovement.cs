using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ZombieMeleeMovement : MonoBehaviour
{
    #region Property
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Movement")]
    [SerializeField] private Transform raycastFront;
    [SerializeField] private Transform raycastBack;
    [SerializeField] private Transform raycastUp;

    [Header("Stat")]
    [SerializeField] private ZombieMeleeStatSO stat;

    private LayerMask layerZombieMelee;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Temp
        Initialize(Enums.Lane.Lane1);
    }

    public void Initialize(Enums.Lane lane)
    {
        gameObject.layer = (int)lane;
        layerZombieMelee = 1 << (int)lane;
    }

    void Update()
    {
        bool goForward = true;
        bool goBackward = false;
        bool goUpward = false;

        #region Raycast
        //If Another Zombie up there, Push back
        RaycastHit2D hit = Physics2D.Raycast(raycastUp.position, Vector2.up, stat.rayCastUpDistance, layerZombieMelee);
        if (hit == true)
        {
            Debug.DrawRay(raycastUp.position, Vector2.up * stat.rayCastUpDistance, Color.cyan);
            goForward = false;
            goBackward = true;
        }
        else
        {
            Debug.DrawRay(raycastUp.position, Vector2.up * stat.rayCastUpDistance, Color.red);
            // Object detected in front
            hit = Physics2D.Raycast(raycastFront.position, Vector2.left, stat.rayCastFrontDistance);
            if (hit == true)
            {
                Debug.DrawRay(raycastFront.position, Vector2.left * stat.rayCastFrontDistance, Color.cyan);
                switch (hit.collider.gameObject.tag)
                {
                    case "PlayerMeleeBox":
                        break;

                    case "ZombieMelee":
                        //If No Zombie in the back, Climb the zombie in the front
                        hit = Physics2D.Raycast(raycastBack.position, Vector2.right, stat.rayCastBackDistance);
                        if (hit == true)
                        {
                            Debug.DrawRay(raycastBack.position, Vector2.right * stat.rayCastBackDistance, Color.cyan);
                            goForward = false;
                        }
                        else
                        {
                            Debug.DrawRay(raycastBack.position, Vector2.right * stat.rayCastBackDistance, Color.red);
                            goUpward = true;
                        }
                        break;
                }
            }
            else
            {
                Debug.DrawRay(raycastFront.position, Vector2.left * stat.rayCastFrontDistance, Color.red);
            }
        }
        #endregion

        float speedForward = -rb.velocity.x;
        float speedUpward = rb.velocity.y;

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
}
