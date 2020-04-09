using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    private bool isMusicPlaying;
    void Awake()
    {
        if(instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        isMusicPlaying = false;
    }

    public void PlaySingle(AudioClip clip)
    {
    }

    public void PlayMusic()
    {
        GameObject music = GameObject.Find("Music");
        if(music != null && !isMusicPlaying)
        {
            Debug.Log("SoundManager::PlayMusic");
            music.GetComponent<AudioSource>().Play();
            isMusicPlaying = true;
        }
    }

    public void StopMusic()
    {
        GameObject music = GameObject.Find("Music");
        if(music != null && isMusicPlaying)
        {
            Debug.Log("SoundManager::StopMusic");
            music.GetComponent<AudioSource>().Stop();
            isMusicPlaying = false;
        }
    }
}
