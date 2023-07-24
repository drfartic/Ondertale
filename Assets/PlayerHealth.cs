using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public float currentHealth;

    [SerializeField] private Transform healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void OnDamageReceive(float dmg)
    {
        currentHealth -= dmg;

        float dmgPercent = currentHealth / maxHealth;
        healthBar.transform.localScale = new Vector3(dmgPercent, 1f, 1f);
    }
}
