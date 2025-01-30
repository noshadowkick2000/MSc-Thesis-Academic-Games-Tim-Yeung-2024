using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentSoundSwapper : MonoBehaviour
{
    [System.Serializable]
    private struct EnvironmentSound
    {
        public AudioClip[] clip;
        public AudioMixerGroup[] mixerGroup;
    }

    [SerializeField] private EnvironmentSound[] environmentSounds;
    [SerializeField] private AudioSource[] sources;

    private void Awake()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].clip = environmentSounds[i].clip[LevelHandler.CurrentLevel];
            sources[i].outputAudioMixerGroup = environmentSounds[i].mixerGroup[LevelHandler.CurrentLevel];
        }
    }
}
