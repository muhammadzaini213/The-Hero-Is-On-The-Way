using UnityEngine;

public class InjuredKnightHealth : Health
{
    [SerializeField] Animator _animator;
    [SerializeField] InjuredKnightAttack attack;
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip deathSfx;
    public bool isHitAnimation { get; private set; }
    public bool isDeath { get; private set; }
    protected override void Start()
    {
        base.Start();
        attack = GetComponent<InjuredKnightAttack>();
        isHitAnimation = false;
        OnDeath += HandlePlayerDeath;
        OnHealthChanged += HandleHealthChanged;
    }

    public override void TakeDamage(int damage)
    {
        if (isDeath) return;

        if (isHitAnimation) return;
        base.TakeDamage(damage);
        attack.OnAttackEnds();
        _animator.SetTrigger("hit");
        isHitAnimation = true;
        SfxPlayer.Instance.PlayPlayerSfx(hitSfx);
    }

    public void OnHitEnds()
    {
        isHitAnimation = false;
    }

    private void HandlePlayerDeath()
    {
        _animator.SetBool("isDeath", true);
        isDeath = true;
        SfxPlayer.Instance.PlayPlayerSfx(deathSfx);
        GameManager.Instance.OnPlayerDeath();
    }

    private void HandleHealthChanged(int current, int max)
    {
        HUD.Instance.SetPlayerHpBar(current, max);
    }
}