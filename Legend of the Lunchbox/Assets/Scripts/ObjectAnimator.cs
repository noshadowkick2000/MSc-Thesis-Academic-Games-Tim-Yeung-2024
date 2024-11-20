using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectAnimator : ObjectMover
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
      objectRb = objectTransform.GetComponentInChildren<Rigidbody>();
      face = objectTransform.GetComponentInChildren<Animation>().gameObject;
      
      face.SetActive(false);
      mainObject = objectTransform;
      
      ImmediateToObject(LocationHolder.EnemyLocation.position + Vector3.down, mainObject.transform.rotation);
      SmoothToObject(LocationHolder.EnemyLocation, GameEngine.EncounterStartTime, true);
      StartCoroutine(GrowObject());
   }

   private IEnumerator GrowObject()
   {
      Vector3 startScale = mainObject.localScale;
      mainObject.localScale = Vector3.zero;
      Quaternion startRotation = quaternion.Euler(0, -90, 0);
      Quaternion targetRotation = Quaternion.Euler(0, 30, 0);
    
      float startTime = Time.realtimeSinceStartup;
      while (Time.realtimeSinceStartup < startTime + GameEngine.EncounterStartTime)
      {
         float x = (Time.realtimeSinceStartup - startTime) / GameEngine.EncounterStartTime;
         float y = MathT.EasedT(x);
         mainObject.localScale = new Vector3(startScale.x * y, startScale.y * y, startScale.z * y);
         mainObject.rotation = Quaternion.Lerp(startRotation, targetRotation, y);
         yield return null;
      }
    
      mainObject.localScale = startScale;
      mainObject.rotation = targetRotation;
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
