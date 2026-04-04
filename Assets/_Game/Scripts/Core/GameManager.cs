using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Health playerHealth;
    [SerializeField] private GateHealth gateHealth;
    [SerializeField] private HeroArrivalTimer heroArrivalTimer;
    [SerializeField] private DemonPressureTimer demonPressureTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetHealth();
        SetGateHealth();
        SetHeroArrival();
        SetDemonPressure();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)){
            playerHealth.TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.N)){
            gateHealth.TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.M)){
            TutorialText.Instance.ShowTutorial("This is a tutorial text. It will fade in and out smoothly.");
        }

        if (Input.GetKeyDown(KeyCode.K)){
            TutorialText.Instance.HideTutorial();
        }
    }

    // ================== PUBLIC LISTENER =================

    public void OnPlayerDeath()
    {
        SceneChanger.Instance.ChangeScene("GameOver", new string[] { "You have been falled.", "The demons killed everyone before they could help." }, fadeIn: true, fadeOut: false);
    }
    public void OnGateDestroyed()
    {
        SceneChanger.Instance.ChangeScene("GameOver", new string[] { "The gate was destroyed!", "No one survived." }, fadeIn: true, fadeOut: false);
    }

    public void OnHeroArrived()
    {
        SceneChanger.Instance.ChangeScene("MainMenu", "Thank you, we’ll take it from here.", fadeIn: true, fadeOut: false);
    }

    public void OnDemonPressureFull()
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

    private void SetDemonPressure()
    {
        float arrivalTime = demonPressureTimer.arrivalTime;
        HUD.Instance.SetDemonPressureBar(arrivalTime, arrivalTime);
    }
}