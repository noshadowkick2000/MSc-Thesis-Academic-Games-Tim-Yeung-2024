using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WalkSound : MonoBehaviour
{
    [System.Serializable]
    private struct EnvironmentAudio
    {
        public AudioClip[] walkImpacts;
    }
    
    [SerializeField] private EnvironmentAudio[] environmentAudios;
    [SerializeField] private AudioSource audioSource;

    private AudioClip[] walkImpacts;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(MainMenuHandler.SoundKey) == 1)
        {
            SubscribeToEvents();
            walkImpacts = environmentAudios[LevelHandler.CurrentLevel].walkImpacts;
        }
    }

    private void OnDestroy()
    {
        if (PlayerPrefs.GetInt(MainMenuHandler.SoundKey) == 1)
            UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        MainCameraController.BobBottomEvent += Step;
    }

    private void UnsubscribeToEvents()
    {
        MainCameraController.BobBottomEvent -= Step;
    }

    private int previous = 0;
    private void Step()
    {
        int next = previous;
        while (next == previous)
            next = Random.Range(0, walkImpacts.Length);
        audioSource.PlayOneShot(walkImpacts[next], .1f);
        previous = next;
    }
}
