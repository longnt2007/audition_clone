using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelRthymController : MonoBehaviour
{
    public float _bpm;
    public float _bps;
    private float _startTime, _waitTimer;
    Slider _rhythmSlider;
    bool isSliding;
    int levelCount;
    int levelHolder;
    public Text txtLevel;
    public Text txtLevelNum;
    public GameObject txtLevelFinish;

    private void Awake()
    {
        //instance = this;
        _startTime = Time.time;
        isSliding = false;
        _rhythmSlider = GetComponent<Slider>();
        levelCount = 0;
        levelHolder = 6;
        txtLevelNum.text = levelHolder.ToString();
    }

    void GetBpM()
    {
        switch (MusicPlayerController.instance.GetAudioSource().clip.name)
        {
            case "Dance Monkey - Tones And I [64kbps]":
                Debug.Log("Dance Monkey - Tones And I [64kbps]");
                _bpm = 98.0f;
                break;
            case "Mike Posner - I Took A Pill In Ibiza [128kbps]":
                Debug.Log("Mike Posner - I Took A Pill In Ibiza [128kbps]");
                _bpm = 102.0f;
                break;
            default:
                Debug.Log("Ooh La La - Kim Geon Mo [320kbps]");
                _bpm = 94.0f;
                break;
        }
        _bps = _bpm / 60.0f;
        _rhythmSlider.maxValue = 4.0f / _bps;

        _waitTimer = _rhythmSlider.maxValue * 3.0f + _rhythmSlider.maxValue / 4.0f;
    }
    void Start()
    {
        GetBpM();
    }
    void UpdateRthymSlider()
    {
        if (_waitTimer > Time.time - _startTime && !isSliding)
        {
            return;
        }
        else if (!isSliding)
        {
            isSliding = true;
            _startTime = Time.time;
        }
        float t = Time.time - _startTime;
        if (t >= _rhythmSlider.maxValue)
        {
            t = 0;
            _startTime = Time.time;
            levelCount++;
        }
        _rhythmSlider.value = t;
    }
    void UpdateLevel()
    {
        if (levelCount >= 8)
        {
            levelCount = 0;
            levelHolder++;
            txtLevelNum.text = levelHolder.ToString();
        }
    }

    void Update()
    {
        UpdateRthymSlider();
        UpdateLevel();
    }
}
