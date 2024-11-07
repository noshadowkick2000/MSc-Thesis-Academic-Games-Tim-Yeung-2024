using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectAnimator : MonoBehaviour
{
   private void Awake()
   {
      SubscribeToEvents();
   }

   private void OnDestroy()
   {
      UnsubscribeFromEvents();
   }

   private void SubscribeToEvents()
   {
      TrialHandler.OnObjectSpawnedEvent += ObjectSpawned;
      GameEngine.LostEncounterStartedEvent += LostEncounter;
      GameEngine.WonEncounterStartedEvent += WonEncounter;
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
      GameEngine.LostEncounterStartedEvent -= LostEncounter;
      GameEngine.WonEncounterStartedEvent -= WonEncounter;
   }

   private Rigidbody objectRb;
   private GameObject face;
   protected virtual void ObjectSpawned(Transform objectTransform)
   {
      objectRb = objectTransform.GetComponent<Rigidbody>();
      face = objectTransform.GetComponentInChildren<Animation>().gameObject;
      
      face.SetActive(false);
   }

   protected virtual void WonEncounter()
   {
      StartCoroutine(DelayedAnimation());
   }
   
   protected virtual void LostEncounter()
   {
      StartCoroutine(DelayedPhysics());
   }

   private IEnumerator DelayedPhysics()
   {
      Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      
      yield return new WaitForSecondsRealtime(1f);
      
      objectRb.isKinematic = false;
      
      objectRb.AddTorque(randomVector, ForceMode.VelocityChange);
   }

   private IEnumerator DelayedAnimation()
   {
      yield return new WaitForSecondsRealtime(1f);
      
      face.SetActive(true);
   }
}
