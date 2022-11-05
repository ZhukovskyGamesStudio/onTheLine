using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Clock : InteractableObject {
    public static Clock instance;
    public float SecondsInOneMinute = 1;
    public Transform HoursTransform, MinutesTransform;
    public int hours, minutes;
    public bool isGoing;
    private AudioSource _audioSource;
    public AudioClip click1, click2, ring;
    private bool _isDayEnded;
    private bool _firstRingRang;
    private float _curTime;
    private int _curTick;

    public UnityAction onStartDay, onEndDay;

    void Awake() {
        instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        StartClock(Settings.config.arriveHour);
    }

    public void StartClock(int startHours, int startMinutes) {
        isGoing = true;
        SetTime(startHours, startMinutes);
    }

    public void StartClock(float time) {
        isGoing = true;
        SetTime(Mathf.FloorToInt(time), Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 60));
    }

    public void RingClock() {
        _audioSource.PlayOneShot(ring);
    }

    void StopClock() {
        isGoing = false;
        _audioSource.PlayOneShot(ring);
    }

    public float GetTime() {
        float time = hours + (minutes * 1f / 60);
        return time;
    }

    public bool IsWorkTime() {
        return GetTime() > Settings.config.startDayHour && GetTime() < Settings.config.endDayHour;
    }

    void SetTime(int hours, int minutes) {
        this.hours = hours;
        this.minutes = minutes;
        UpdateArrows();
    }

    void UpdateArrows() {
        _curTick++;
        _audioSource.PlayOneShot((_curTick % 2 == 0) ? click1 : click2);
        HoursTransform.localRotation = Quaternion.Euler(0, 0, 30 * hours);
        MinutesTransform.localRotation = Quaternion.Euler(0, 0, 6 * minutes);
    }

    void StartDay() {
        _isDayEnded = false;
        onStartDay?.Invoke();
        _audioSource.PlayOneShot(ring);
    }

    void RingBell() {
        _audioSource.PlayOneShot(ring);
    }

    public void EndDay() {
        _isDayEnded = true;
        onEndDay?.Invoke();
        SaveManager.sv.currentDay++;
        SaveManager.sv.dayResult.isWorkedAllDay = true;
        SaveManager.Save();
        RingBell();
        Debug.Log("Рабочий день закончен. Обслужите последние звонки");
    }

    public void Leave() {
        SceneManager.LoadScene("Menu");
    }

    protected override void Update() {
        base.Update();
        if (!isGoing) {
            _curTime = 0;
            return;
        }

        _curTime += Time.deltaTime;
        if (_curTime >= SecondsInOneMinute) {
            _curTime = 0;
            minutes++;
            if (minutes > 59) {
                minutes = 0;
                hours++;
            }

            UpdateArrows();
        }

        if (hours + minutes / 60f > Settings.config.startDayHour && !_firstRingRang) {
            StartDay();
            _firstRingRang = true;
        }

        if (hours + minutes / 60f > Settings.config.leaveHour && !_isDayEnded) {
            EndDay();
            if (Settings.config.isInstaExitOnEndOfDay)
                Leave();
        }
    }
}