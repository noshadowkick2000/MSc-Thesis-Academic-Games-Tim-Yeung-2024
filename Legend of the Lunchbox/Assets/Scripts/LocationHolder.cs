using UnityEngine;
using UnityEngine.Serialization;

public class LocationHolder : MonoBehaviour
{
  [Header("Camera positions")]
  [SerializeField] private Transform baseCameraLocation;
  public static Transform BaseCameraLocation;
  [SerializeField] private Transform mindCameraLocation;
  public static Transform MindCameraLocation;
  [SerializeField] private Transform enemyCameraLocation;
  public static Transform EnemyCameraLocation;
  [SerializeField] private Transform discoverableCameraLocation;
  public static Transform DiscoverableCameraLocation;
  
  [FormerlySerializedAs("enemyLocation")]
  [Header("Enemy Positions")]
  [SerializeField] private Transform discoverableLocation;
  public static Transform DiscoverableLocation;

  [Header("Property Positions")]
  [SerializeField] private Transform propertyLocation;
  public static Transform PropertyLocation;
  
  // [Header("Player positions")]
  // [SerializeField] private Transform basePlayerLocation;
  // public static Transform BasePlayerLocation;
  // [SerializeField] private Transform thinkingPlayerLocation;
  // public static Transform ThinkingPlayerLocation;

  private void Awake()
  {
    BaseCameraLocation = baseCameraLocation;
    MindCameraLocation = mindCameraLocation;
    EnemyCameraLocation = enemyCameraLocation;
    DiscoverableCameraLocation = discoverableCameraLocation;
    DiscoverableLocation = discoverableLocation;
    PropertyLocation = propertyLocation;
  }
}
