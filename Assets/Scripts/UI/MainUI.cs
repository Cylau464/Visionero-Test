using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button[] _restartButtons;
        [SerializeField] private Button[] _quitButtons;
        [Space]
        [SerializeField] private GameObject _winWindow;
        [SerializeField] private GameObject _loseWindow;
        [SerializeField] private GameObject _pauseWindow;

        [Inject] private GameProgressHandler _gameProgressHandler;

        private void OnEnable()
        {
            _gameProgressHandler.OnGameEnd += GameEnded;
        }

        private void OnDisable()
        {
            _gameProgressHandler.OnGameEnd -= GameEnded;
        }

        private void Start()
        {
            _resumeButton.onClick.AddListener(Resume);
            _pauseButton.onClick.AddListener(Pause);

            foreach (Button button in _quitButtons)
                button.onClick.AddListener(Quit);

            foreach (Button button in _restartButtons)
                button.onClick.AddListener(Restart);
        }

        private void GameEnded(bool win)
        {
            if (win == true)
                _winWindow.SetActive(true);
            else
                _loseWindow.SetActive(true);
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            _pauseWindow.SetActive(false);
        }

        private void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            _pauseWindow.SetActive(true);
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}