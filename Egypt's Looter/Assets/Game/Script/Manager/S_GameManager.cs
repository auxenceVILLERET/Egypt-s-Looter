using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_GameManager : MonoBehaviour
{

    [BoxGroup("Panels")]
    [SerializeField] private GameObject _startPanel;
    [BoxGroup("Panels")]
    [SerializeField] private GameObject _winPanel;

    [BoxGroup("Panels")]
    [SerializeField] private GameObject _losePanel;

    [BoxGroup("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [BoxGroup("Panels")]
    [SerializeField] private GameObject _settingsPanel;

    [BoxGroup("Channel")]
    [SerializeField] private SO_PauseChannel _pauseChannel;
    [BoxGroup("Channel")]
    [SerializeField] private  SO_GameManagerChannel _gameManagerChanel;

    [BoxGroup("References")]
    [SerializeField] private Slider _healthSlider;
    [BoxGroup("References")]
    [SerializeField] private S_GeneratorSRP _generatorSRP;
    [BoxGroup("References")]
    [SerializeField] private S_CursorManager _cursorManager;
    [BoxGroup("References")]
    [SerializeField] private S_Quest _quest;
    [BoxGroup("References")]
    [SerializeField] private S_Timer _timer;

    private GameObject _settingsReturnPanel;


    public bool IsGameOver { get; private set; }
    public bool IsPause { get; private set; }
    public bool IsChestOpen { get; set; }

    private void Start()
    {
        ShowStartMenu();
    }

    private void OnEnable()
    {
        _pauseChannel.OnPauseRequested += ShowPause;
        _gameManagerChanel.OnDiedRequested += ShowLose;
        _gameManagerChanel.OnWinRequested += TryWin;
        _gameManagerChanel.OnChestOpened += SetChestOpen;
    }

    private void OnDisable()
    {
        _pauseChannel.OnPauseRequested -= ShowPause;
        _gameManagerChanel.OnDiedRequested -= ShowLose;
        _gameManagerChanel.OnWinRequested -= TryWin;
        _gameManagerChanel.OnChestOpened -= SetChestOpen;
    }

    #region Menu Canva
    public void ShowStartMenu()
    {
        Time.timeScale = 0f;

        if (_startPanel != null) _startPanel.SetActive(true);
        if (_winPanel != null) _winPanel.SetActive(false);
        if (_losePanel != null) _losePanel.SetActive(false);
        if (_pausePanel != null) _pausePanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
        if (_healthSlider != null) _healthSlider.enabled = false;

        _timer.UITimer.enabled = false;

        _quest.SetFirstQuestActive(false);
        _quest.SetActiveBackground(false);
        _quest.SetSecondQuestActive(false);

        _cursorManager.LockCursor(false);
        _cursorManager.HideCursor(false);
    }

    public void StartGame()
    {
        IsGameOver = false;
        IsPause = false;
        IsChestOpen = false;
        Time.timeScale = 1f;
        if (_startPanel != null) _startPanel.SetActive(false);
        if (_healthSlider != null) _healthSlider.enabled = true;

        _cursorManager.LockCursor(true);
        _cursorManager.HideCursor(true);

        _quest.SetFirstQuestActive(true);
        _quest.SetActiveBackground(true);
        _quest.SetSecondQuestActive(false);

        _timer.ResetTimer();

        if (_generatorSRP != null)
            _generatorSRP.StartGame();
    }

    private void TryWin()
    {
        if (!IsChestOpen)
            return;

        ShowWin();
    }

    public void ShowWin()
    {
        EndGame(_winPanel);
    }

    public void ShowLose()
    {
        EndGame(_losePanel);
    }

    public void ShowPause()
    {
        if (IsGameOver)
            return;

        PauseGame(_pausePanel);
    }

    private void PauseGame(GameObject panel)
    {
        if (IsPause)
            return;
        IsPause = true;

        _quest.SetFirstQuestActive(false);
        _quest.SetActiveBackground(false);
        _quest.SetSecondQuestActive(false);

        panel.SetActive(true);
        if (_healthSlider != null) _healthSlider.enabled = false;
        Time.timeScale = 0f;
        _cursorManager.LockCursor(false);
        _cursorManager.HideCursor(false);
    }

    private void EndGame(GameObject panel)
    {
        if (IsGameOver) return;
        IsGameOver = true;

        panel.SetActive(true);
        if (_healthSlider != null) _healthSlider.enabled = false;
        Time.timeScale = 0f;

        _cursorManager.LockCursor(false);
        _cursorManager.HideCursor(false);

        _quest.SetFirstQuestActive(false);
        _quest.SetActiveBackground(false);
        _quest.SetSecondQuestActive(false);

        _timer.UITimer.enabled = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue(GameObject panel)
    {
        Time.timeScale = 1f;
        IsPause = false;
        panel.SetActive(false);
        if (_healthSlider != null) _healthSlider.enabled = true;

        _cursorManager.LockCursor(true);
        _cursorManager.HideCursor(true);

        if (IsChestOpen)
        {
            _quest.SetFirstQuestActive(false);
            _quest.SetActiveBackground(true);
            _quest.SetSecondQuestActive(true);
        }
        else
        {
            _quest.SetFirstQuestActive(true);
            _quest.SetActiveBackground(true);
            _quest.SetSecondQuestActive(false);
        }

    }

    public void ShowSettings(GameObject fromPanel)
    {
        _settingsReturnPanel = fromPanel;

        if (fromPanel != null) fromPanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (_settingsPanel != null) _settingsPanel.SetActive(false);

        if (_settingsReturnPanel != null)
        {
            _settingsReturnPanel.SetActive(true);
            _settingsReturnPanel = null;
        }
    }
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    private void SetChestOpen()
    {
        IsChestOpen = true;
        _quest.SetFirstQuestActive(false);
        _quest.SetSecondQuestActive(true);
        _quest.SetActiveBackground(true);

        _timer.UITimer.enabled = true;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}