using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    [SerializeField] private AudioClip buttonClickSound;
    private void Start()
    {
        _musicSlider.value = MusicPlayer.Instance.GetMusicVolume();
        _sfxSlider.value = SfxPlayer.Instance.GetSfxVolume();
    }

    public void ChangeMusicVolume()
    {
        MusicPlayer.Instance.SetMusicVolume(_musicSlider.value);
    }

    public void ChangeSfxVolume()
    {
        SfxPlayer.Instance.SetSfxVolume(_sfxSlider.value);
    }

    public void BackToMainMenu()
    {
        SfxPlayer.Instance.PlayUISfx(buttonClickSound);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}