using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private Material playerMat;
  [SerializeField] private Texture2D normalFace;
  [SerializeField] private Texture2D thinkingFace;
  [SerializeField] private GameObject mindUI;
  [SerializeField] private GameObject thoughtUI;
  [SerializeField] private Animator playerAnimator;

  private void Awake()
  {
    thoughtUI.SetActive(false);
    mindUI.SetActive(false);
  }

  public void Idle(bool encounterOver)
  {
    playerAnimator.SetTrigger("idle");
    mindUI.SetActive(false);
    if (encounterOver) { StartCoroutine(AnimateCanvas(true)); }
    thoughtUI?.SetActive(false);
    transform.rotation = LocationHolder.BasePlayerLocation.rotation;
    transform.position = LocationHolder.BasePlayerLocation.position;
    playerMat.mainTexture = normalFace;
  }

  public void StartMind()
  {
    playerAnimator.SetTrigger("think");
    mindUI.SetActive(true);
    StartCoroutine(AnimateCanvas(false));
    transform.rotation = LocationHolder.ThinkingPlayerLocation.rotation;
    transform.position = LocationHolder.ThinkingPlayerLocation.position;
    playerMat.mainTexture = thinkingFace;
  }

  private IEnumerator AnimateCanvas(bool inverse) 
  {
    RectTransform mt = mindUI.GetComponent<RectTransform>();
    mt.localScale = inverse ? Vector3.one : Vector3.zero;

    float duration = 0.5f; //TODO make this dependent on start time GameEngine

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      if (inverse) { x = 1 - x; }
      mt.localScale = new Vector3 (x, x, 1);
      yield return null;
    }

    mt.localScale = inverse ? Vector3.zero : Vector3.one;
  }

  public void StartThought() { thoughtUI.SetActive(true); }
  public void EndThought() { thoughtUI.SetActive(false); }
}
