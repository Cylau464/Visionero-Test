using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private const string GAME_SCENE_NAME = "Game";

    private void Start()
    {
        _playButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
