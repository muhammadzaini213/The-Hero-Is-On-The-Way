using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Start()
    {
        base.Start();
        OnDeath += HandlePlayerDeath;
        OnHealthChanged += HandleHealthChanged;
    }

    private void HandlePlayerDeath()
    {
        GameManager.Instance.OnPlayerDeath();
    }

    private void HandleHealthChanged(int current, int max)
    {
        HUD.Instance.SetPlayerHpBar(current, max);
    }
}