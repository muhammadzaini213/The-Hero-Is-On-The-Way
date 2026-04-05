using UnityEngine;
using UnityEngine.SceneManagement;

public class AboutMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private AudioClip buttonClickSound;
    public void BackToMainMenu()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}