using UnityEngine;

public class GateHealth : Health
{
    protected override void Start()
    {
        base.Start();
        OnDeath += HandleGateDestroyed;
        OnHealthChanged += HandleHealthChanged;
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