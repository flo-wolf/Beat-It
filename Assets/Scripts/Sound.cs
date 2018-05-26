using UnityEngine.Audio;
using UnityEngine;

//USE: FindObjectOfType<AudioManager>().Play(" insert the name of the clip here ");
//to play the sound you want to be played.

//This will make that the class appears in our Inspector.
//In our Sound class, we define what values we want to be able to change that will affect our Audio Clip
[System.Serializable]
public class Sound
{
    //Name of our AudioClip, will also be used to play the AudioClip.
    public string name;

    public AudioClip clip;

    //[Range(xf, xf)] just gives us the option to control the given values over a slider in the inspector.
    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    //You need an Audio Source to play an Audioclip
    [HideInInspector]
    public AudioSource source;
}
