using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] GameObject _target;
    [Header("Flash")]
    [SerializeField] Color _flashColor;
    private Color _originalColor;
    [Header("Timer")]
    [SerializeField] TMP_Text InGameTimeText;
    [SerializeField] float _timerTime;
    [SerializeField] float _flashTime;
    [SerializeField] float _flashAfterTime;
    private float _timer;
    private float _flashTimer;
    public UnityEvent onComplete = new();
    private bool runTimer = false;
    private bool runFlash = false;


    private void UpdateText()
    {
        float minutes = Mathf.FloorToInt(_timer / 60);
        float seconds = Mathf.FloorToInt(_timer % 60);
        minutes = minutes <= 0 ? 0 : minutes;
        seconds = seconds <= 0 ? 0 : seconds;
        // float playedMinutes = Mathf.FloorToInt(gameTimer / 60);
        // float playedSeconds = Mathf.FloorToInt(gameTimer % 60);

        InGameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        if (runTimer && _timer > 0)
        {
            UpdateText();
            _timer -= Time.deltaTime;
            if (_flashTimer > 0 && runFlash)
            {
                _flashTimer -= Time.deltaTime;
            }
            else if (_flashTimer <= 0)
            {
                _target.GetComponent<Image>().color = _target.GetComponent<Image>().color == _flashColor ? _originalColor : _flashColor;
                _flashTimer = _flashTime;
            }
        }
        else if (_timer < 0)
        {
            _timer = 0;
            onComplete.Invoke();
        }
        if (_timer > 0 && _timer <= _flashAfterTime)
        {
            runFlash = true;
        }
    }
    public void StartTimer()
    {
        _originalColor = _target.GetComponent<Image>().color;
        runTimer = true;
        runFlash = false;
        _timer = _timerTime;
        _flashTimer = _flashTime;
    }
    public void StopTimer()
    {
        runTimer = false;
        runFlash = false;

    }

}
