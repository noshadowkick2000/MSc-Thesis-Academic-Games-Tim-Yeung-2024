using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private Transform mind;
  [SerializeField] private Material playerMat;
  [SerializeField] private Texture2D normalFace;
  [SerializeField] private Texture2D thinkingFace;
  [SerializeField] private GameObject mindUI;
  [SerializeField] private GameObject thoughtUI;

  private Quaternion baseRotation;
  private Quaternion thinkingRoation;

  private void Awake()
  {
    thoughtUI.SetActive(false);
    mindUI.SetActive(false);
    baseRotation = transform.rotation;
    thinkingRoation = Quaternion.Euler(0, 180, 0);
  }

  public void Idle()
  {
    transform.rotation = baseRotation;
    playerMat.mainTexture = normalFace;
  }

  public Transform StartMind()
  {
    mindUI.SetActive(true);
    transform.rotation = thinkingRoation;
    playerMat.mainTexture = thinkingFace;
    return mind;
  }

  public void StartThought() { thoughtUI.SetActive(true); }
  public void EndThought() { thoughtUI.SetActive(false); }
}
