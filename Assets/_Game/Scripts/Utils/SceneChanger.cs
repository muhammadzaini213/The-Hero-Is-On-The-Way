using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float typingSpeed    = 0.04f;
    [SerializeField] private float holdAfterLine  = 1.5f;
    [SerializeField] private float holdBeforeLoad = 2.0f;
    [SerializeField] private float fadeDuration   = 0.5f; 

    [Header("UI References")]
    [SerializeField] private CanvasGroup     canvasGroup;
    [SerializeField] private TextMeshProUGUI label;

    private bool _isBusy;

    void Awake()
    {
        // Singleton pattern agar tidak hancur saat pindah scene
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Reset UI State
        canvasGroup.alpha          = 0f;
        canvasGroup.blocksRaycasts = false;
        label.text                 = "";
    }

    // ------------------------------------------------------------------
    // Public API

    public void ChangeScene(string sceneName, string[] messages = null, bool fadeIn = true, bool fadeOut = true)
    {
        if (_isBusy) return;
        StartCoroutine(ChangeSceneRoutine(sceneName, messages, fadeIn, fadeOut));
    }

    public void ChangeScene(string sceneName, string message, bool fadeIn = true, bool fadeOut = true)
    {
        ChangeScene(sceneName, new[] { message }, fadeIn, fadeOut);
    }

    public void ChangeSceneInstant(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ------------------------------------------------------------------
    // Core Logic

    private IEnumerator ChangeSceneRoutine(string sceneName, string[] messages, bool fadeIn, bool fadeOut)
    {
        _isBusy = true;
        canvasGroup.blocksRaycasts = true;
        label.text = "";

        // STEP 1: FADE IN (Menutup layar dengan warna hitam)
        if (fadeIn && fadeDuration > 0f)
        {
            yield return StartCoroutine(Fade(0f, 1f));
        }
        else
        {
            canvasGroup.alpha = 1f;
        }

        // STEP 2: TYPING MESSAGES (Jika ada pesan)
        if (messages != null && messages.Length > 0)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                yield return StartCoroutine(TypeText(messages[i]));

                if (i == messages.Length - 1)
                {
                    // Tunggu sebentar setelah kalimat terakhir selesai diketik
                    yield return new WaitForSeconds(holdBeforeLoad);
                }
                else
                {
                    // Jeda antar kalimat
                    yield return new WaitForSeconds(holdAfterLine);
                    label.text = "";
                }
            }
        }

        // STEP 3: LOAD SCENE ASYNC (Pindah scene saat layar masih hitam)
        // Kita pakai Async agar Coroutine tetap jalan sampai scene baru 'ready'
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Tunggu sampai scene benar-benar termuat
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // STEP 4: FADE OUT (Membuka layar hitam di scene yang baru)
        if (fadeOut && fadeDuration > 0f)
        {
            label.text = ""; // Hapus teks sisa agar tidak muncul di scene baru
            yield return StartCoroutine(Fade(1f, 0f));
        }
        else
        {
            canvasGroup.alpha = 0f;
        }

        canvasGroup.blocksRaycasts = false;
        _isBusy = false;
    }

    private IEnumerator TypeText(string message)
    {
        label.text = "";
        foreach (char c in message)
        {
            label.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Pakai unscaled agar tidak terpengaruh Time.timeScale = 0
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}