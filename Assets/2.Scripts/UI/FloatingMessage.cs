using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingMessage : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private TMP_Text text;
    [SerializeField] private float InitialVelY = 7f;
    [SerializeField] private float InitialVelXRange = 3f;
    [SerializeField] private float lifetime = 0.8f;

    private float lifetimeRemain;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(string msg)
    {
        lifetimeRemain = lifetime;
        text.text = msg;
        gameObject.SetActive(true);
        rb.velocity = new Vector2(Random.Range(-InitialVelXRange, InitialVelXRange), InitialVelY);
    }

    private void Update()
    {
        lifetimeRemain -= Time.deltaTime;

        if (lifetimeRemain < 0)
        {
            DestroyText();
        }
        else
        {
            float elapsed = (lifetime - lifetimeRemain) / lifetime;
            if (elapsed < 0.2f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, elapsed * 5);
            }
            else if (elapsed > 0.7f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, (0.7f - elapsed) / 0.3f + 1f);
            }
        }
    }

    private void DestroyText()
    {
        gameObject.SetActive(false);
        GameManager.Instance.poolManager.ReleaseObject(Constants.DamageIndicator, gameObject);
    }
}
