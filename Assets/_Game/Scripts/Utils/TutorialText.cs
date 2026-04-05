using UnityEngine;
using TMPro; // Pastikan kamu menggunakan TextMeshPro
using System.Collections;

public class TutorialText : MonoBehaviour
{
    public static TutorialText Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tutorialTextMesh;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inisialisasi: Sembunyikan teks saat mulai
        canvasGroup.alpha = 0;
    }

    public void ShowTutorial(string message)
    {
        tutorialTextMesh.text = message;
        StartFade(1f);
    }

    /// <summary>
    /// Menghilangkan teks tutorial dengan efek fade out.
    /// </summary>
    public void HideTutorial()
    {
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        // Hentikan coroutine yang sedang berjalan agar tidak bentrok
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}