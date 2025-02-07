using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

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

    [FormerlySerializedAs("environmentSource")] [SerializeField] private AudioSource ambientSource;

    private void Awake()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].clip = environmentSounds[i].clip[(int) TrialHandler.CurrentEnvironment];
            sources[i].outputAudioMixerGroup = environmentSounds[i].mixerGroup[(int) TrialHandler.CurrentEnvironment];
            if (sources[i].playOnAwake)
                sources[i].Play();
        }

        string envAmbientPath = Application.streamingAssetsPath + "/Ambient/" + TrialHandler.CurrentEnvironment.ToString() + ".wav";
        
        ambientSource.clip = WavUtility.ToAudioClip(envAmbientPath);
    }
}
