using UnityEngine;
using Random = UnityEngine.Random;

public class WalkSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] walkImpacts;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(MainMenuHandler.SoundKey) == 1)
            SubscribeToEvents();
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
