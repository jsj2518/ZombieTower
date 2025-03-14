using System;
using UnityEngine;
using UnityEngine.UI;

public class ZombieMeleeState : MonoBehaviour, IHealth
{
    #region Property
    private Animator anim;

    [Header("State")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform raycastFront;

    [Header("Stat")]
    [SerializeField] private ZombieMeleeStatSO stat;
    [SerializeField] private LayerMask attackTargetLayer;
    #endregion

    private static bool hashLoad = false;
    private static int animIsIdle;
    private static int animIsAttacking;
    private static int animIsDead;

    public event Action<GameObject> OnDead;

    private int currenHp;
    private GameObject attackTarget;



    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (hashLoad == false)
        {
            hashLoad = true;

            animIsIdle = Animator.StringToHash("IsIdle");
            animIsAttacking = Animator.StringToHash("IsAttacking");
            animIsDead = Animator.StringToHash("IsDead");
        }
    }

    public void Initialize()
    {
        currenHp = stat.hp;
        hpSlider.gameObject.SetActive(false);

        anim.SetBool(animIsIdle, true);
        anim.SetBool(animIsAttacking, false);
        anim.SetBool(animIsDead, false);
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastFront.position, Vector2.left, stat.rayCastFrontDistance, attackTargetLayer);
        if (hit && hit.collider.gameObject.CompareTag("PlayerMeleeBox"))
        {
            attackTarget = hit.collider.gameObject;
            anim.SetBool(animIsAttacking, true);
        }
        else
        {
            attackTarget = null;
            anim.SetBool(animIsAttacking, false);
        }
    }

    private void InflictDamage()
    {
        if (attackTarget != null)
        {
            // todo : Damage to player box
        }
    }

    public void TakeDamage(int damage)
    {
        GameObject damageIndicator = GameManager.Instance.poolManager.GetObject(Constants.DamageIndicator);
        if (damageIndicator != null && damageIndicator.TryGetComponent<FloatingMessage>(out FloatingMessage indicator))
        {
            damageIndicator.transform.position = hpSlider.transform.position;
            indicator.Initialize(damage.ToString());
        }

        currenHp -= damage;
        if (currenHp <= 0)
        {
            Dead();
        }
        else
        {
            hpSlider.gameObject.SetActive(true);
            hpSlider.value = (float)currenHp / stat.hp;
        }
    }

    public int GetHP()
    {
        return currenHp;
    }

    private void Dead()
    {
        anim.SetBool(animIsIdle, false);
        anim.SetBool(animIsDead, true);

        gameObject.SetActive(false);

        OnDead?.Invoke(gameObject);
    }
}
