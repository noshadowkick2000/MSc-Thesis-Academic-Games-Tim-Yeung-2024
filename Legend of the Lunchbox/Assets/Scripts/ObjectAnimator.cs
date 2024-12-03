using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectAnimator : ObjectMover
{
   private Material objectMat;
   
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
      GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
      GameEngine.AnswerCorrectStartedEvent += CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent += WrongAnswer;
      GameEngine.LostEncounterStartedEvent += LostEncounter;
      GameEngine.WonEncounterStartedEvent += WonEncounter;
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
      GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
      GameEngine.AnswerCorrectStartedEvent -= CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent -= WrongAnswer;
      GameEngine.LostEncounterStartedEvent -= LostEncounter;
      GameEngine.WonEncounterStartedEvent -= WonEncounter;
   }

   private Rigidbody objectRb;
   private GameObject face;
   protected virtual void ObjectSpawned(Transform objectTransform)
   {
      objectRb = objectTransform.GetComponentInChildren<Rigidbody>();
      face = objectTransform.GetComponentInChildren<Animation>().gameObject;
      objectMat = objectTransform.GetComponentInChildren<Renderer>().material;
      
      face.SetActive(false);
      mainObject = objectTransform;
      mainObject.gameObject.SetActive(false);
      
      // ImmediateToObject(LocationHolder.EnemyLocation.position + Vector3.down, mainObject.transform.rotation);
      // SmoothToObject(LocationHolder.EnemyLocation, GameEngine.EncounterStartTime, true);
      // StartCoroutine(GrowObject());
   }

   protected virtual void ShowingEnemy()
   {
      objectMat.SetFloat("_t", 1);
      mainObject.gameObject.SetActive(true);
      StartCoroutine(GrowObject());
   }

   protected virtual void CorrectAnswer()
   {
      StartCoroutine(Wiggle(GameEngine.FeedbackTime, 30f, 8));
   }

   protected virtual void WrongAnswer()
   {
      StartCoroutine(Wiggle(GameEngine.FeedbackTime, 45f, 2));
   }

   protected virtual void WonEncounter()
   {
      StartCoroutine(WinAnimation());
   }
   
   protected virtual void LostEncounter()
   {
      StartCoroutine(LostAnimation());
   }

   private IEnumerator LostAnimation()
   {
      Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      
      yield return new WaitForSecondsRealtime(1f);
      
      objectRb.isKinematic = false;
      
      objectRb.AddTorque(randomVector, ForceMode.VelocityChange);
   }

   private IEnumerator WinAnimation()
   {
      float startTime = Time.realtimeSinceStartup;
      float t = 0f;
      float duration = GameEngine.EncounterStopTime * .3f;

      while (t < 1)
      {
         objectMat.SetFloat("_t", 1-t);
         
         t = (Time.realtimeSinceStartup - startTime) / duration;
         yield return null;
      }
      
      objectMat.SetFloat("_t", 0);
      
      face.gameObject.SetActive(true);

      Vector3 startPosition = mainObject.transform.position;
      float speed = 0f;
      float acceleration = .2f;
      float h = 0;
      startTime = Time.realtimeSinceStartup;
      t = 0f;
      duration = GameEngine.EncounterStopTime * .7f;
      int repetitions = 10;
      float maxSpeed = 2f;
      
      while (t < 1)
      {
         float x = Mathf.Sin(t * repetitions * Mathf.PI) * speed;
         float y = Mathf.Cos(t * repetitions * Mathf.PI) * speed;
         h = Mathf.Pow(1.65f * t - .5f, 2) - .25f;

         mainObject.position = startPosition + new Vector3(x, h, y);
         // mainObject.Rotate(Vector3.up, speed * 20f);

         if (speed < maxSpeed)
            speed += acceleration * Time.deltaTime;
         t = (Time.realtimeSinceStartup - startTime) / duration;
         yield return null;
      }
   }
}
