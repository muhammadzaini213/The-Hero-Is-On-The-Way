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
        // Subscribe dulu sebelum base.Start()
        OnDeath += HandlePlayerDeath;
        OnHealthChanged += HandleHealthChanged;

        base.Start(); // baru ini — NotifyHealthChanged() dipanggil di sini

        attack = GetComponent<InjuredKnightAttack>();
        isHitAnimation = false;
        isDeath = false;
        HUD.Instance.SetPlayerHpBar(CurrentHealth, MaxHealth);

    }

    public override void TakeDamage(int damage)
    {
        if (isInvincible) return;
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