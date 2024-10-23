using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHolder : MonoBehaviour
{
  [Header("Camera positions")]
  [SerializeField] private Transform baseCameraLocation;
  public static Transform BaseCameraLocation;
  [SerializeField] private Transform mindCameraLocation;
  public static Transform MindCameraLocation;
  [SerializeField] private Transform enemyCameraLocation;
  public static Transform EnemyCameraLocation;

  [Header("Enemy Positions")]
  [SerializeField] private Transform enemyLocation;
  public static Transform EnemyLocation;

  [Header("Property Positions")]
  [SerializeField] private Transform propertyLocation;
  public static Transform PropertyLocation;
  
  [Header("Player positions")]
  [SerializeField] private Transform basePlayerLocation;
  public static Transform BasePlayerLocation;
  [SerializeField] private Transform thinkingPlayerLocation;
  public static Transform ThinkingPlayerLocation;

  private void Awake()
  {
    BaseCameraLocation = baseCameraLocation;
    MindCameraLocation = mindCameraLocation;
    EnemyCameraLocation = enemyCameraLocation;
    EnemyLocation = enemyLocation;
    PropertyLocation = propertyLocation;
    BasePlayerLocation = basePlayerLocation;
    ThinkingPlayerLocation = thinkingPlayerLocation;
  }
}
