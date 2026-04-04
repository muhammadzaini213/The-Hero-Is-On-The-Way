using UnityEngine;

public class InjuredKnightAttack : MonoBehaviour
{
    private Animator _animator;
    public bool isAttacking { get; private set; }
    InjuredKnightHealth health;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        health = GetComponent<InjuredKnightHealth>();
        isAttacking = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking && !health.isDeath)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        int random = Random.Range(0, 3);
        if (random == 0)
        {
            isAttacking = true;
            _animator.SetTrigger("attack1");
        }
        else
        {
            isAttacking = true;
            _animator.SetTrigger("attack2");
        }
    }

    public void OnAttackEnds()
    {
        isAttacking = false;
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }
}
