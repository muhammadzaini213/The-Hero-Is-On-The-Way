using UnityEngine;

public class KatSing : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Invoke (nameof(change), 0.5f);
    }

    private void change()
    {
                SceneChanger.Instance.ChangeScene("FinalScene", new string[] { "I’m sorry, Theresa… I won’t be home tonight.", "H-how!? That should have obliterated the entire city…" },
    fadeIn: true, fadeOut: false);
    }
}