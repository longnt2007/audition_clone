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
    AudioSource audioSource;
    public Text txtLevel;
    public Text txtLevelNum;
    public GameObject objLevel;
    public GameObject objLevelNum;
    public GameObject objLevelFinish;

    public GameObject objWait;
    public GameObject objHit;

    public Image backgroundWait;
    public Image backgroundHit;
    public Image backgroundHitPoint;
    float blinkTime;
    float blinkTimeHolder;
    bool isAlphaMax;



    private void Awake()
    {
        //instance = this;
        _startTime = Time.time;
        isSliding = false;
        _rhythmSlider = GetComponent<Slider>();
        levelCount = 0;
        levelHolder = 6;
        txtLevelNum.text = levelHolder.ToString();
        _rhythmSlider.enabled = false;
        isAlphaMax = true;
    }

    void GetBpM()
    {
        switch (audioSource.clip.name)
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
        blinkTime = Time.time;
        blinkTimeHolder = _rhythmSlider.maxValue / 4.0f;
        _waitTimer = _rhythmSlider.maxValue * 3.0f;// + (_rhythmSlider.maxValue / 4.0f) * 3.0f;
    }
    void Start()
    {
        audioSource = MusicPlayerController.instance.GetAudioSource();
        GetBpM();
    }
    void UpdateRthymSlider()
    {

        float u = Time.time - blinkTime;
        if (u >= blinkTimeHolder)
        {
            blinkTime = Time.time;
            float alpha;
            float a = 1.0f;
            float b = 0.5f;
            if (isAlphaMax)
            {
                alpha = Mathf.Lerp(b, a, _rhythmSlider.maxValue);
            }
            else
            {
                alpha = Mathf.Lerp(a, b, _rhythmSlider.maxValue);
            }
            isAlphaMax = !isAlphaMax;
            backgroundWait.color = new Color(backgroundWait.color.r, backgroundWait.color.g, backgroundWait.color.b, alpha);
            backgroundHit.color = new Color(backgroundWait.color.r, backgroundWait.color.g, backgroundWait.color.b, alpha);
            backgroundHitPoint.color = new Color(backgroundWait.color.r, backgroundWait.color.g, backgroundWait.color.b, alpha);
        }

        if ((_waitTimer > Time.time - _startTime && !isSliding)) //|| !audioSource.isPlaying)
        {
            return;
        }
        else if (!isSliding)
        {
            _rhythmSlider.enabled = true;
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

        if (levelCount >= 8)
        {
            levelCount = 0;
            levelHolder++;
            txtLevelNum.text = levelHolder.ToString();
        }
        _rhythmSlider.value = t;

    }
    void UpdateLevel()
    {
        if (levelCount == 6 || levelCount == 7)
        {
            objLevelFinish.SetActive(true);
            objLevel.SetActive(false);
            objLevelNum.SetActive(false);

            objWait.SetActive(false);
            objHit.SetActive(true);
        }
        else
        {
            objLevelFinish.SetActive(false);
            objLevel.SetActive(true);
            objLevelNum.SetActive(true);

            objWait.SetActive(true);
            objHit.SetActive(false);
        }
    }

    void Update()
    {
        UpdateRthymSlider();
        UpdateLevel();
    }
}
