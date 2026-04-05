using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] Animator _animator;
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip deathSfx;
    protected override void Start()
    {
        base.Start();
        OnDeath += HandlePlayerDeath;
        OnHealthChanged += HandleHealthChanged;

    }

    public override void TakeDamage(int damage)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("hurt")) return; // prevent taking damage while already in hurt animation
        if (CurrentHealth <= 0)
        {
            return;
        }
        ; // prevent taking damage after death


        if (GetComponent<PlayerMove>().IsCountering()) return;

        base.TakeDamage(damage);
        _animator.SetTrigger("hurt");

    }


    private void HandlePlayerDeath()
    {
        _animator.SetBool("isDeath", true);

        if (deathSfx != null)

            SfxPlayer.Instance.PlayPlayerSfx(deathSfx);
        GameManager.Instance.OnPlayerDeath();
    }

    private void HandleHealthChanged(int current, int max)
    {
        HUD.Instance.SetPlayerHpBar(current, max);
    }
}