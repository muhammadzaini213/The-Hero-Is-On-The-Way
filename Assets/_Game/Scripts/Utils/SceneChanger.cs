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
    [SerializeField] private float holdBeforeNext = 0.5f;
    [SerializeField] private float holdBeforeLoad = 2.0f;
    [SerializeField] private float fadeDuration   = 0.5f;  // dipakai untuk fade in & fade out

    [Header("UI References")]
    [SerializeField] private CanvasGroup     canvasGroup;
    [SerializeField] private TextMeshProUGUI label;

    private bool _isBusy;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        canvasGroup.alpha          = 0f;
        canvasGroup.blocksRaycasts = false;
        label.text                 = "";
    }

    // ------------------------------------------------------------------
    // Public API

    /// <summary>
    /// Ganti scene dengan array teks.
    /// fadeIn  : bg muncul dengan fade (default true)
    /// fadeOut : bg menghilang dengan fade sebelum load scene (default true)
    /// </summary>
    public void ChangeScene(string sceneName, string[] messages = null, bool fadeIn = true, bool fadeOut = true)
    {
        if (_isBusy) return;
        StartCoroutine(ChangeSceneRoutine(sceneName, messages, fadeIn, fadeOut));
    }

    /// <summary>Versi satu baris teks.</summary>
    public void ChangeScene(string sceneName, string message, bool fadeIn = true, bool fadeOut = true)
    {
        ChangeScene(sceneName, new[] { message }, fadeIn, fadeOut);
    }

    /// <summary>Langsung ganti scene tanpa efek apapun.</summary>
    public void ChangeSceneInstant(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ------------------------------------------------------------------
    // Internal

    private IEnumerator ChangeSceneRoutine(string sceneName, string[] messages, bool fadeIn, bool fadeOut)
    {
        _isBusy                    = true;
        canvasGroup.blocksRaycasts = true;
        label.text                 = "";

        // Fade in (bg muncul)
        if (fadeIn && fadeDuration > 0f)
        {
            canvasGroup.alpha = 0f;
            yield return StartCoroutine(Fade(0f, 1f));
        }
        else
        {
            canvasGroup.alpha = 1f;
        }

        // Ketik semua baris
        if (messages != null && messages.Length > 0)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                yield return StartCoroutine(TypeText(messages[i]));

                bool isLast = i == messages.Length - 1;

                if (isLast)
                {
                    yield return new WaitForSeconds(holdBeforeLoad);
                }
                else
                {
                    yield return new WaitForSeconds(holdAfterLine);
                    yield return new WaitForSeconds(holdBeforeNext);
                    label.text = "";
                }
            }
        }

        // Fade out (bg menghilang)
        if (fadeOut && fadeDuration > 0f)
            yield return StartCoroutine(Fade(1f, 0f));

        SceneManager.LoadScene(sceneName);

        canvasGroup.alpha          = 0f;
        canvasGroup.blocksRaycasts = false;
        _isBusy                    = false;
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
            elapsed           += Time.unscaledDeltaTime;
            canvasGroup.alpha  = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}