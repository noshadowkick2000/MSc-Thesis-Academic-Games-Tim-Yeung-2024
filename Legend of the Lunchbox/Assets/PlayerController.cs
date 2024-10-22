using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private Material playerMat;
  [SerializeField] private Texture2D normalFace;
  [SerializeField] private Texture2D thinkingFace;
  [SerializeField] private GameObject mindUI;
  [SerializeField] private GameObject thoughtUI;

  private void Awake()
  {
    thoughtUI.SetActive(false);
    mindUI.SetActive(false);
  }

  public void Idle()
  {
    mindUI.SetActive(false);
    thoughtUI?.SetActive(false);
    transform.rotation = LocationHolder.BasePlayerLocation.rotation;
    transform.position = LocationHolder.BasePlayerLocation.position;
    playerMat.mainTexture = normalFace;
  }

  public void StartMind()
  {
    mindUI.SetActive(true);
    transform.rotation = LocationHolder.ThinkingPlayerLocation.rotation;
    transform.position = LocationHolder.ThinkingPlayerLocation.position;
    playerMat.mainTexture = thinkingFace;
  }

  public void StartThought() { thoughtUI.SetActive(true); }
  public void EndThought() { thoughtUI.SetActive(false); }
}
