using UnityEngine;

public class HeroArrivalTimer : MonoBehaviour
{
    [Header("Timer Settings (Minute)")]
    public float arrivalTime = 10f;
    private float timer;

    void Start()
    {
        timer = arrivalTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        HUD.Instance.SetHeroArrivalBar(timer, arrivalTime);
        if (timer <= 0f)
        {
            OnHeroArrived();
            enabled = false;
        }
    }

    private void OnHeroArrived()
    {
        GameManager.Instance.OnHeroArrived();
    }
}