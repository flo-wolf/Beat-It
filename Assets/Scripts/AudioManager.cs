using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    //Here we create an array for instances of our Sound class
    public Sound[] sounds;

    public static AudioManager instance;

    // Use this for initialization before Start()
    void Awake()
    {
        //We dont want that there can be multiple AudioManagers, whenever a new scene starts, so we use a Singleton pattern,
        //to check if there is already an instance of our AudioManager, and if yes, we just destroy it.
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }


        //We dont want our AudioManager to be destroyed in case we switch scenes.
        DontDestroyOnLoad(gameObject);

        //Each instance of our Sound class will now get a AudioSource that is stored in the variable "source", we defined in our Sound class
        //We then set the diffrent variables of our Sound class equals to the variables the AudioClip brings with it.
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        RythmManager.onBPM.AddListener(RythmCall);
    }

    public void FixedRythmCall(float f)
    {
        Play("Bass");
    }

    public void RythmCall(RythmManager.BPM bpm)
    {
        if(bpm == RythmManager.instance.levelBPM)
            AudioManager.instance.Play("Bass");

        if (bpm == RythmManager.instance.playerBPM)
            AudioManager.instance.Play("Kick");

        if (bpm == RythmManager.BPMtoBPMh(RythmManager.instance.playerBPM))
            AudioManager.instance.Play("Snare");
    }

    //This enables us to Play an AudioClip just through his name.
    //When using System; we can use the Array.Find Method.
    //We first define the array we want to look through.
    //Then we define a variable that refers to the element in the Sound array, which in our case is an instance of the Sound class.
    //We then want to find that sound which name is equal to the name given as an argument in the Play() method.
    //sound here is just a variable and can be named what ever you want, but .name is important, because we refer to the name of the sound in our Instance.
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }
}
