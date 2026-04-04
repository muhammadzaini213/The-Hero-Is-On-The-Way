using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    [Header("Bar")]
    private Slider playerHpBar;
    private Slider gateHpBar;
    private Slider heroArrivalBar;

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