using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private ProjectileStatSO stat;

    private bool collisionIgnore;
    private float groundDecisionPosY;
    private float lifetimeRemain;

    public void Initialize()
    {
        collisionIgnore = false;
        groundDecisionPosY = Random.Range(stat.groundDecisionPosYMin, stat.groundDecisionPosYMax);
        lifetimeRemain = stat.lifetime;
        if (TryGetComponent<TrailRenderer>(out TrailRenderer trailRenderer))
        {
            trailRenderer.Clear();
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        lifetimeRemain -= Time.deltaTime;

        if (lifetimeRemain < 0 || transform.position.y < groundDecisionPosY)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionIgnore) return;

        if (collision.gameObject.CompareTag("ZombieMelee"))
        {
            // Destroy condition
            if (stat.penetration == false)
            {
                collisionIgnore = true;
                DestroyProjectile();
            }

            IHealth zombieState = collision.GetComponent<IHealth>();
            zombieState?.TakeDamage(stat.damage);

        }
    }

    private void DestroyProjectile()
    {
        gameObject.SetActive(false);
        GameManager.Instance.poolManager.ReleaseObject(Constants.ShotgunPallet, gameObject);
    }
}
