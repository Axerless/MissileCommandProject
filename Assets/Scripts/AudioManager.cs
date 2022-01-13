using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if(instance == null){

                GameObject gameObject = new GameObject("AudioManager");
                gameObject.AddComponent<AudioManager>();
            }
            return instance;
        }
    }
    
    public Sound[] sounds;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(instance == null){

            instance = this;
        } 
        else {
            
            Destroy(gameObject);
            return;
        }
        

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) return;
        s.source.Play();
    }

    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null) return;
        s.source.Stop();
    }
}
