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
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
      GameEngine.LostEncounterStartedEvent -= LostEncounter;
   }

   private Rigidbody objectRb;
   protected virtual void ObjectSpawned(Transform objectTransform)
   {
      objectRb = objectTransform.GetComponent<Rigidbody>();
      print(objectRb);
   }
   
   protected virtual void LostEncounter()
   {
      print(objectRb + "start");
      StartCoroutine(DelayedPhysics());
   }

   private IEnumerator DelayedPhysics()
   {
      Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      
      yield return new WaitForSecondsRealtime(1f);
      
      objectRb.isKinematic = false;
      
      objectRb.AddTorque(randomVector, ForceMode.VelocityChange);
   }
}
