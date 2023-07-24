using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance;

    [SerializeField] GameObject[] attacks;
    GameObject currentAttack;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        currentAttack = Instantiate(attacks[0], transform);
    }

    public void OnAttackEnd() => StartCoroutine(c_OnAttackEnd());
    private IEnumerator c_OnAttackEnd()
    {
        yield return new WaitForSeconds(1f);
        PlayerCombat.Instance.pos = new Vector2(0, 0);
        Destroy(currentAttack);
        currentAttack = Instantiate(attacks[0], transform);
    }
}
