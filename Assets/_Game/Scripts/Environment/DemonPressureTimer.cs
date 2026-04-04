using UnityEngine;

public class DemonPressureTimer : MonoBehaviour
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
        HUD.Instance.SetDemonPressureBar(timer, arrivalTime);
        if (timer <= 0f)
        {
            OnDemonPressureFull();
            enabled = false;
        }
    }

    private void OnDemonPressureFull()
    {
        GameManager.Instance.OnDemonPressureFull();
    }
}