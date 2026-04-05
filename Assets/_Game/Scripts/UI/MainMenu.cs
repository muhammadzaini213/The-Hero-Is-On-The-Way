using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string aboutMenuSceneName = "AboutMenu";
    [SerializeField] private string settingsMenuSceneName = "SettingsMenu";

    [SerializeField] private AudioClip buttonClickSound;

    public void StartGame()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneChanger.Instance.ChangeScene(gameSceneName, new[] {"The hero is on the way." ,"Please, hold the line until they arrive."}, fadeIn: true, fadeOut: false);
    }

    public void ContinueGame()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(gameSceneName);
    }


    public void About()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(aboutMenuSceneName);
    }

    public void Settings()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(settingsMenuSceneName);
    }

    public void Exit()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        Application.Quit();
    }
}