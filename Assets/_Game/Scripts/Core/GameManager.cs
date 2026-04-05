using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Health playerHealth;
    [SerializeField] private GateHealth gateHealth;
    [SerializeField] private HeroArrivalTimer heroArrivalTimer;
    [SerializeField] private DemonPressureTimer demonPressureTimer;

    [Header("Heal Support")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float heroArrivalCost = 5f;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private Transform healEffectSpawn;  // drag child object player ke sini
    [SerializeField] private AudioClip healSfx;
    [SerializeField] private AudioClip heroArrivalSfx;
    [SerializeField] private GameObject heroArrivalVFX;
    [SerializeField] private GameObject demonKingObject;
    void Awake() => Instance = this;

    void Start()
    {
        SetHealth();
        SetGateHealth();
        SetHeroArrival();
        SetDemonPressure();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) UseHealSupport();

        if (Input.GetKeyDown(KeyCode.B)) playerHealth.TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.N)) gateHealth.TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.M)) TutorialText.Instance.ShowTutorial("This is a tutorial text. It will fade in and out smoothly.");
        if (Input.GetKeyDown(KeyCode.K)) TutorialText.Instance.HideTutorial();
    }

    // ================== HEAL SUPPORT ==================

    private void UseHealSupport()
    {
        if (healEffectPrefab != null)
        {
            // Pakai healEffectSpawn kalau ada, fallback ke posisi player
            Vector3 spawnPos = healEffectSpawn != null
                ? healEffectSpawn.position
                : playerHealth.transform.position;

            Instantiate(healEffectPrefab, spawnPos, Quaternion.identity);
        }

        if (healSfx != null)
            SfxPlayer.Instance.PlayPlayerSfx(healSfx);

        playerHealth.Heal(healAmount);
        heroArrivalTimer.DelayArrival(heroArrivalCost);
    }

    // ================== PUBLIC LISTENER ==================

    public void OnPlayerDeath()
    {
        SceneChanger.Instance.ChangeScene("GameOver",
            new string[] { "You have been falled.", "The demons killed everyone before they could help." },
            fadeIn: true, fadeOut: false);
    }

    public void OnGateDestroyed()
    {
        SceneChanger.Instance.ChangeScene("GameOver",
            new string[] { "The gate was destroyed!", "No one survived." },
            fadeIn: true, fadeOut: false);
    }

    public void OnHeroArrived()
    {
        heroArrivalVFX.SetActive(true);
        SfxPlayer.Instance.PlayEnvironmentSfx(heroArrivalSfx);
        SceneChanger.Instance.ChangeScene("MainMenu",
            "Thank you, we'll take it from here.",
            fadeIn: true, fadeOut: false);
    }

    public void OnDemonPressureFull()
    {
        demonKingObject.SetActive(true);

        if (heroArrivalTimer.timer < 60f)
        {
            heroArrivalTimer.StopTimer(); // stop timer supaya bar tidak aneh
            Invoke("SetDemonKingCutscene", 10f);
        } else
        {
            SceneChanger.Instance.ChangeScene("GameOver",
                new string[] { "The hero is too late!", "Demon King has arrived and destroyed everything." },
                fadeIn: true, fadeOut: false);
        }
    }

    private void SetDemonKingCutscene()
    {
        SceneChanger.Instance.ChangeScene("DemonKingCutscene", "Weak knight, you think you can stop me?",
            fadeIn: true, fadeOut: false);
    }

    // ================== DEFAULT SETTER ==================

    private void SetHealth()
    {
        HUD.Instance.SetPlayerHpBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void SetGateHealth()
    {
        HUD.Instance.SetGateHpBar(gateHealth.CurrentHealth, gateHealth.MaxHealth);
    }

    private void SetHeroArrival()
    {
        HUD.Instance.SetHeroArrivalBar(heroArrivalTimer.arrivalTime, heroArrivalTimer.arrivalTime);
    }

    private void SetDemonPressure()
    {
        HUD.Instance.SetDemonPressureBar(demonPressureTimer.arrivalTime, demonPressureTimer.arrivalTime);
    }
}