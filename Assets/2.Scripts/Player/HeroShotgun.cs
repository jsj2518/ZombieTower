using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroShotgun : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform shootingPivot;
    [SerializeField] private float defaultAngle = -33f;

    [Header("Stat")]
    [SerializeField] private float firstShotDelay;
    [SerializeField] private float coolDown;
    [SerializeField] private int palletNum;
    [SerializeField] private float speedMin;
    [SerializeField] private float speedMax;
    [SerializeField] private float spreadAngle;

    private float coolDownRemain;
    private float shootingAngle;
    private GameObject shootingTarget;
    private IHealth targetHealth;
    private Transform targetTransform;

    private void Start()
    {
        coolDownRemain = firstShotDelay;
    }

    private void Update()
    {
        FindTarget();
        ShootTarget();
    }

    private void FindTarget()
    {
        if (shootingTarget != null && shootingTarget.activeInHierarchy
            && targetHealth.GetHP() > 0 && targetTransform != null)
        {
            return;
        }
        // Find new target
        else
        {
            bool isFind = false;
            foreach (GameObject target in GameManager.Instance.zombiesMelee)
            {
                if (target.TryGetComponent<IHealth>(out IHealth health))
                {
                    if (health.GetHP() <= 0) continue;

                    if (target.TryGetComponent<ITargetable>(out ITargetable targetable))
                    {
                        targetHealth = health;
                        targetTransform = targetable.GetTarget();

                        isFind = true;
                        break;
                    }
                }
            }

            if (isFind == false)
            {
                targetTransform = null;
            }
        }
    }

    private void ShootTarget()
    {
        // aim
        if (targetTransform != null)
        {
            Vector2 direction = targetTransform.position - shotgun.position;
            shootingAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            shootingAngle = 0;
        }
        shotgun.rotation = Quaternion.Euler(0, 0, defaultAngle + shootingAngle);

        coolDownRemain -= Time.deltaTime;

        if (coolDownRemain < 0 && targetTransform != null)
        {
            FireShotgun();
            coolDownRemain = coolDown;
        }
    }

    private void FireShotgun()
    {
        float angleMin = shootingAngle - spreadAngle / 2;
        float angleMax = shootingAngle + spreadAngle / 2;

        for (int i = 0; i < palletNum; i++)
        {
            float speed = Random.Range(speedMin, speedMax);
            float Angle = Random.Range(angleMin, angleMax);
            GameObject pallet = GameManager.Instance.poolManager.GetObject(Constants.ShotgunPallet);
            pallet.transform.position = shootingPivot.transform.position;
            float radian = Angle * Mathf.Deg2Rad;
            Vector2 velocity = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * speed;

            pallet.GetComponent<PlayerProjectile>().Initialize();
            pallet.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }
}
