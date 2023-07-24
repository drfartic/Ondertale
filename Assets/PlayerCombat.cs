using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;
    private PlayerHealth playerHealth;

    public float speed;
    public Vector2 areaSize;
    public int boxPpu;
    public bool canMove = false;

    [HideInInspector] public bool doTickDamage = false;

    [HideInInspector] public Vector2 pos;
    [HideInInspector] public float linewidth;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        linewidth = 1f / boxPpu;
        playerHealth = GetComponent<PlayerHealth>();
        StartCoroutine(DoTick());
    }

    private IEnumerator DoTick()
    {
        if (doTickDamage && canMove)
        {
            playerHealth.OnDamageReceive(1.5f);
            Camera.main.DOShakePosition(0.075f, 0.035f, 10, 90, false);
        }
        
        yield return new WaitForSeconds(0.075f);
        StartCoroutine(DoTick());

    }

    void Update()
    {
         if (canMove) Move();
    }

    void Move()
    {
        Vector2 input;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1);

        pos.x += input.x * Time.deltaTime * speed;
        pos.y += input.y * Time.deltaTime * speed;

        pos.x = Mathf.Clamp(pos.x, -areaSize.x / 2f + transform.localScale.x/4f + linewidth, areaSize.x/2 - transform.localScale.x / 4f - linewidth);
        pos.y = Mathf.Clamp(pos.y, -areaSize.y/2f + transform.localScale.y / 4f + linewidth, areaSize.y/2f - transform.localScale.y / 4f - linewidth);

        transform.localPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        doTickDamage = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        doTickDamage = false;
    }
}
