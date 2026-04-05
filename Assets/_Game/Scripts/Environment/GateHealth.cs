using UnityEngine;

public class GateHealth : Health
{
    protected override void Start()
    {
        OnDeath += HandleGateDestroyed;
        OnHealthChanged += HandleHealthChanged;
        base.Start();
        HUD.Instance.SetGateHpBar(CurrentHealth, MaxHealth);
    }

    private void HandleGateDestroyed()
    {
        GameManager.Instance.OnGateDestroyed();
    }

    private void HandleHealthChanged(int current, int max)
    {
        HUD.Instance.SetGateHpBar(current, max);
    }
}