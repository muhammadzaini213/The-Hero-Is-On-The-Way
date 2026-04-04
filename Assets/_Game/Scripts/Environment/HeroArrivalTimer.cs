using UnityEngine;

public class HeroArrivalTimer : MonoBehaviour
{
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