using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    [Header("Bar")]

    [SerializeField] private Slider playerHpBar;
    [SerializeField] private Slider gateHpBar;
    [SerializeField] private Slider heroArrivalBar;
    [SerializeField] private Slider demonPressureBar;

    void Awake()
    {
        Instance = this;
    }

    // ======================== SLIDER SETTERS ========================
    public void SetPlayerHpBar(int current, int max)
    {
        if (playerHpBar == null)
        {
            Debug.LogError("Player HP Bar not assigned in HUD.");
            return;
        }

        playerHpBar.maxValue = max;
        playerHpBar.value = current;
    }

    public void SetGateHpBar(int current, int max)
    {
        if (gateHpBar == null)
        {
            Debug.LogError("Gate HP Bar not assigned in HUD.");
            return;
        }

        gateHpBar.maxValue = max;
        gateHpBar.value = current;
    }

    public void SetHeroArrivalBar(float current, float max)
    {
        if (heroArrivalBar == null)
        {
            Debug.LogError("Hero Arrival Bar not assigned in HUD.");
            return;
        }

        heroArrivalBar.maxValue = max;
        heroArrivalBar.value = current;
    }

    public void SetDemonPressureBar(float current, float max)
    {
        if (demonPressureBar == null)
        {
            Debug.LogError("Demon Pressure Bar not assigned in HUD.");
            return;
        }

        demonPressureBar.maxValue = max;
        demonPressureBar.value = current;
    }

    // ======================== SUPPORT FUNCTIONS ========================
    public void OnHealthSupportClicked()
    {
        Debug.Log("Health support button clicked!");
    }


    // ======================== PAUSE ========================
    public void OnPauseClicked()
    {
        Debug.Log("Pause button clicked!");
    }
}