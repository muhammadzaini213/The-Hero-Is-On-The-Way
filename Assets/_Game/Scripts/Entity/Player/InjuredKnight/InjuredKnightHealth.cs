using UnityEngine;

public class InjuredKnightHealth : Health
{
    [SerializeField] Animator _animator;
    protected override void Start()
    {
        base.Start();
        OnDeath += HandlePlayerDeath;
        OnHealthChanged += HandleHealthChanged;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _animator.SetTrigger("hit");
    }

    private void HandlePlayerDeath()
    {
        _animator.SetBool("isDeath", true);
        GameManager.Instance.OnPlayerDeath();
    }

    private void HandleHealthChanged(int current, int max)
    {
        HUD.Instance.SetPlayerHpBar(current, max);
    }
}