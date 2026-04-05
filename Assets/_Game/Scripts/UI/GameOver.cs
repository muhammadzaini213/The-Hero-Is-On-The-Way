using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private AudioClip buttonClickSound;
    public void Retry()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitToMenu()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}