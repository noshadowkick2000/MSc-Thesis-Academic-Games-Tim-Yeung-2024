using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISound : MonoBehaviour
{

    [SerializeField] private AudioClip selectAudio;
    [SerializeField] private AudioClip proceedAudio;

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SelectAudio()
    {
        audioSource.PlayOneShot(selectAudio);
    }
    
    public void ProceedAudio()
    {
        audioSource.PlayOneShot(proceedAudio);
    }
}
