using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A useful utility for playing audio that solves the issues of having audiosources in your gameobjects:
/// + Removes clutter from the inspector of your gameobjects
/// + Ensures that there are enough audio sources to play audio clips uninterrupted
/// + Manages the parameters of your audio source components for you (pitch, volume, mixer, clip, etc)
/// 
/// Warning: this class is inteded for OneShot sound playing. If looping is inteded you'll need to use a different way.
/// </summary>
public class SFXManager : Singleton<SFXManager>
{

    [Tooltip("The default sound channel")]
    public AudioMixerGroup mixerGroup;



    [Header("Parameters")]
    [SerializeField, Tooltip("How many audio sources to generate")]
    protected int sourceAmount = 32;

    // Autogenerated
    protected AudioSource[] sources;

    // Start is called before the first frame update
    void Start()
    {
        // Create temporary object so we don't bloat our inspector
        GameObject temp = new GameObject("AudioSources Container");
        temp.transform.parent = transform;
        temp.transform.localPosition = Vector3.zero;

        // Create a bunch of audio sources
        sources = new AudioSource[sourceAmount];
        for (int i = 0; i < sourceAmount; i++)
        {
            AudioSource source = temp.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;

            sources[i] = source;
        }
    }

    /// <summary>
    /// Helper function that finds the next available audio source
    /// </summary>
    /// <returns></returns>
    protected AudioSource FindNextSource()
    {
        AudioSource source = sources[0];
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].isPlaying == false)
            {
                source = sources[i];
                break;
            }
        }
        return source;
    }

    /// <summary>
    /// Helper function that plays a specific clip on an available audiosource.
    /// </summary>
    /// <param name="clip">AudioClip to play</param>
    /// <param name="position">Where the sound is located</param>
    /// <param name="volume">How loud the sound is</param>
    /// <param name="pitch">The sound pitch</param>
    /// <param name="spatialBlend">2d vs 3d sound</param>
    protected void Play(AudioClip clip, Vector3 position, float volume, float pitch, float spatialBlend )
    {
        // Find first available audiosource
        AudioSource source = FindNextSource();
        source.transform.position = position;

        // Fill in audiosource parameters
        source.spatialBlend = spatialBlend;
        source.volume = volume;
        source.clip = clip;
        source.pitch = pitch;
        source.outputAudioMixerGroup = mixerGroup;

        // Play sound
        source.Play();
    }

    /// <summary>
    /// Helper function for playing a random sound effect.
    /// </summary>
    /// <param name="sfxClip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="spatialBlend"></param>
    protected void Play(SFXClip sfxClip, Vector3 position, float volume, float spatialBlend)
    {
        // Reuse core helper function
        var clip = sfxClip.NextClip();
        var pitch = sfxClip.NextPitch();
        Play(clip, position, volume, pitch, spatialBlend);
    }

    /// <summary>
    /// Publicly accessible function. Plays a sound clip.
    /// </summary>
    /// <param name="sfxClip"></param>
    public static void Play(SFXClip sfxClip, float volume = 1.0f) => instance.Play(sfxClip, Vector3.zero, volume, 0.0f);

    /// <summary>
    /// Publicly accessible functions that plays 3d audio.
    /// </summary>
    /// <param name="sfxClip"></param>
    /// <param name="position"></param>
    public static void Play3D(SFXClip sfxClip, Vector3 position, float volume = 1.0f) => instance.Play(sfxClip, position, volume, 1.0f);


    /// <summary>
    /// Publicly accessible function. Plays a sound clip.
    /// </summary>
    /// <param name="sfxClip"></param>
    public static void Play(AudioClip clip, float volume = 1.0f, float pitch = 1.0f) => instance.Play(clip, Vector3.zero, volume,pitch, 0.0f);

    /// <summary>
    /// Publicly accessible functions that plays 3d audio.
    /// </summary>
    /// <param name="sfxClip"></param>
    /// <param name="position"></param>
    public static void Play3D(AudioClip clip, Vector3 position, float volume = 1.0f, float pitch = 1.0f) => instance.Play(clip, position, volume,pitch, 1.0f);



}
