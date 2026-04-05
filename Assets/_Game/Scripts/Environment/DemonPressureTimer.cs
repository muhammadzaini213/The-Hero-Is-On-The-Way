using UnityEngine;

public class DemonPressureTimer : MonoBehaviour
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
        HUD.Instance.SetDemonPressureBar(timer, arrivalTime);
        if (timer <= 0f)
        {
            OnDemonPressureFull();
            enabled = false;
        }
    }

    public void StopTimer()
    {
        enabled = false;
    }
    
    private void OnDemonPressureFull()
    {
        GameManager.Instance.OnDemonPressureFull();
    }
}