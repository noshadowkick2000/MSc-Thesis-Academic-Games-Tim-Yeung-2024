using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public float speed = 1;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI name;

    [SerializeField] private Transform centralObject;
    [SerializeField] private GameObject[] trialObjects;
    [SerializeField] private GameObject[] discoverables;

    [SerializeField] private Material skyboxNormal;
    [SerializeField] private Material skyboxEvil;
    
    private void Start()
    {
        // InvokeRepeating(nameof(SlowUpdate), 0, 2);
        name.text = LocalizationTextLoader.GetLocaleEntry(27);
        textPanel.SetActive(false);
    }

    private void SlowUpdate()
    {
        Time.timeScale = speed;
    }

    public void TriggerAnimation(int id)
    {
        animator.SetTrigger("Trigger");
        animator.SetInteger("ID", id);
    }

    public void SetText(int id)
    {
        text.SetText(LocalizationTextLoader.GetLocaleEntry(id));
        textPanel.SetActive(true);
    }

    public void ClearText()
    {
        text.SetText("");
        textPanel.SetActive(false);
    }

    private bool skyboxSwitched = false;
    public void SwitchSkybox()
    {
        RenderSettings.skybox = skyboxSwitched ? skyboxNormal : skyboxEvil;
        skyboxSwitched = !skyboxSwitched;
    }

    [SerializeField] private AudioSource audioSource;
    public void SetAmbienceAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayOneShotAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, 3f);
    }

    public void LoadLevel()
    {
        if (LevelHandler.CurrentLevel == 0)
            SceneManager.LoadScene(2);
        else
            SceneManager.LoadScene(0);
    }

    public float offset = 1.2f;
    private void Update()
    {
        for (int i = 0; i < discoverables.Length; i++)
        {
            float t = Time.time + (i*(2*Mathf.PI/discoverables.Length));
            Vector3 circlePosition = new Vector3(offset * Mathf.Cos(t) + centralObject.transform.position.x, centralObject.transform.position.y, offset * Mathf.Sin(t) + centralObject.transform.position.z);
            // trialObjects[i].transform.position = circlePosition;
            discoverables[i].transform.position = circlePosition;
        }
        
        if (TInput.GetButtonDown(TInput.ButtonNames.DOWN))
            SceneManager.LoadScene(2);
    }
}
