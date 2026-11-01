using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class S_Timer : MonoBehaviour
{
    [BoxGroup("Timer")]
    [SerializeField] private float _timer;
    [BoxGroup("Timer")]
    [SerializeField] private float _blinkingTime;
    [BoxGroup("References")]
    [SerializeField] private TextMeshProUGUI _timerUI;
    [BoxGroup("References")]
    [SerializeField] private S_GameManager _gameManager;

    private string _timerText;
    private float _blinkingInterval;

    private bool _endTimer;
    private bool _startBlinking;

    private float _restartTimer = 180f;

    public bool EndTimer
    {
        get { return _endTimer; }
        set { _endTimer = value; }
    }
    public bool StartBlinking
    {
        get { return _startBlinking; }
    }

    public TextMeshProUGUI UITimer {  get { return _timerUI; } }

    public float Timer { get { return _timer; } set { _timer = value; } }

    public float RestartTimer { get { return _restartTimer; } }

    private void Reset()
    {
        _timer = 180f;
    }
    private void Start()
    {
        _endTimer = false;
        _startBlinking = false;
        _blinkingInterval = 0.8f;
        _blinkingTime = 60f;
        UpdateTimerText();
    }

    void Update()
    {
        if (_endTimer)
            return;

        if (!_gameManager.IsChestOpen)
            return;

        if (_gameManager.IsGameOver)
        {
            StopAllCoroutines();
            enabled = false;
            return;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _endTimer = true;
            _gameManager.ShowLose();
            return;
        }

        UpdateTimerText();

        if (_timer < _blinkingTime && _startBlinking == false)
        {
            _startBlinking = true;
            _timerUI.color = Color.red;
            StartCoroutine(BlinkText());
        }
    }

    IEnumerator BlinkText()
    {

        while (!_endTimer)
        {
            _timerUI.enabled = !_timerUI.enabled;
            yield return new WaitForSeconds(_blinkingInterval);
        }

        _timerUI.enabled = true;
    }

    private void UpdateTimerText()
    {
        float minutes = Mathf.Floor(_timer / 60);
        float seconds = Mathf.RoundToInt(_timer % 60);

        if (seconds < 10)
            _timerText = minutes.ToString() + " : 0" + seconds.ToString();
        else
            _timerText = minutes.ToString() + " : " + seconds.ToString();

        _timerUI.text = _timerText;

    }

    public void SetTimerColor(Color color)
    {
        _timerUI.color = color;
    }

    public void ResetTimer()
    {
        StopAllCoroutines();

        _endTimer = false;
        _startBlinking = false;
        _timer = _restartTimer;

        enabled = true;

        SetTimerColor(Color.white);
        UpdateTimerText();
        _timerUI.enabled = false;
    }
}
