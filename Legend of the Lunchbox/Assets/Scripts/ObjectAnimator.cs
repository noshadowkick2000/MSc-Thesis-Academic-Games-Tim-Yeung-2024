using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectAnimator : ObjectMover
{
   private SpriteRenderer objectRenderer;
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
      GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
      GameEngine.MovingToEnemyStartedEvent += MovingToEnemy;
      GameEngine.AnswerCorrectStartedEvent += CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent += WrongAnswer;
      GameEngine.LostEncounterStartedEvent += LostEncounter;
      GameEngine.WonEncounterStartedEvent += WonEncounter;
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
      GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
      GameEngine.MovingToPropertyStartedEvent -= MovingToProperty;
      GameEngine.MovingToEnemyStartedEvent -= MovingToEnemy;
      GameEngine.AnswerCorrectStartedEvent -= CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent -= WrongAnswer;
      GameEngine.LostEncounterStartedEvent -= LostEncounter;
      GameEngine.WonEncounterStartedEvent -= WonEncounter;
   }

   // private Rigidbody objectRb;
   private GameObject face;
   protected virtual void ObjectSpawned(Transform objectTransform)
   {
      // objectRb = objectTransform.GetComponentInChildren<Rigidbody>();
      face = objectTransform.GetComponentInChildren<Animation>().gameObject;
      objectRenderer = objectTransform.GetComponentInChildren<TrialObject>().MainSprite;
      objectMat = objectRenderer.material;
      
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

   protected virtual void MovingToProperty(TrialHandler.PropertyType propertyType)
   {
      StartCoroutine(Fade(false, GameEngine.MindPropertyTransitionTime / 6));
   }

   private IEnumerator Fade(bool fadingIn, float duration)
   {
      objectRenderer.color = fadingIn ? Color.clear : Color.white;

      float startTime = Time.realtimeSinceStartup;
      float x = 0;
      Color tempColor = Color.white;

      while (x < 1)
      {
         tempColor.a = fadingIn ? MathT.EasedT(x) : MathT.EasedT(1-x);
         objectRenderer.color = tempColor;
         
         x = (Time.realtimeSinceStartup - startTime) / duration;
         yield return null;
      }
      
      objectRenderer.color = fadingIn ? Color.white : Color.clear;
   }

   protected virtual void MovingToEnemy()
   {
      StartCoroutine(Fade(true, GameEngine.MindPropertyTransitionTime / 2));
   }

   protected virtual void CorrectAnswer()
   {
      StartCoroutine(Nod(GameEngine.FeedbackTime, .1f, 2, true));
   }

   protected virtual void WrongAnswer()
   {
      StartCoroutine(Nod(GameEngine.FeedbackTime, .1f, 2, false));
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

      SmoothToObject(mainObject.position + Vector3.down, mainObject.rotation, .5f, true);
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
