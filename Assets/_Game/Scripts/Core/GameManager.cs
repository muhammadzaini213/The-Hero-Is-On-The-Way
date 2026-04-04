using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GateHealth gateHealth;
    [SerializeField] private HeroArrivalTimer heroArrivalTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetHealth();
        SetGateHealth();
        SetHeroArrival();
    }

    // ================== PUBLIC LISTENER =================

    public void OnPlayerDeath()
    {
        
    }
    public void OnGateDestroyed()
    {
        
    }

    public void OnHeroArrived()
    {
        
    }

    // ================== DEFAULT SETTER ===============
    private void SetHealth()
    {
        int health = playerHealth.CurrentHealth;
        int maxHealth = playerHealth.MaxHealth;
    
        HUD.Instance.SetPlayerHpBar(health, maxHealth);
    }

    private void SetGateHealth()
    {
        int health = gateHealth.CurrentHealth;
        int maxHealth = gateHealth.MaxHealth;

        HUD.Instance.SetGateHpBar(health, maxHealth);
    }

    private void SetHeroArrival()
    {
        float arrivalTime = heroArrivalTimer.arrivalTime;
        HUD.Instance.SetHeroArrivalBar(arrivalTime, arrivalTime);
    }
}