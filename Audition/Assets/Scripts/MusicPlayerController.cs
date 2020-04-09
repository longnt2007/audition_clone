using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayerController : MonoBehaviour
{
    public static MusicPlayerController instance;
    public List<AudioClip> songList;
    AudioSource audioSource;
    float timerStart;
    Text timeSong;
    Text nameSong;
    Slider sliderTime;
    // Start is called before the first frame update
    void Awake()
    {
        timerStart = Time.time;
        instance = this;
        audioSource = GetComponent<AudioSource>();
        SetSongName();
        timeSong = transform.Find("txtTime").GetComponent<Text>();
        nameSong = transform.Find("txtSong").GetComponent<Text>();
        nameSong.text = audioSource.clip.name;
        sliderTime = transform.Find("sliderTime").GetComponent<Slider>();
        sliderTime.maxValue = audioSource.clip.length;
    }

    void SetSongName()
    {
        int song = Random.Range(0, songList.Count);
        audioSource.clip = songList[song];
        audioSource.Play();
    }

    void SetSongTime()
    {
        float t = audioSource.clip.length - (Time.time - timerStart);
        string min = ((int)t / 60).ToString("00");
        string sec = (t % 60).ToString("00");
        if (audioSource.isPlaying)
        {
            timeSong.text = min + " : " + sec;
            sliderTime.value = audioSource.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetSongTime();
    }
}
