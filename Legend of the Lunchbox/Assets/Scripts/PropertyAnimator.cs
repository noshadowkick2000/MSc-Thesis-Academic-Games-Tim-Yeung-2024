using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropertyAnimator : MonoBehaviour
{
   private void Awake()
   {
      SubscribeToEvents();
   }

   private void OnDestroy()
   {
      UnsubscribeFromEvents();
   }
   
   // private IEnumerator ActivateProperty(Transform property)
   // {
   //    float duration = .1f;
   //
   //    Vector3 startScale = property.localScale;
   //    Quaternion startRotation = property.rotation;
   //    Quaternion randomRotation = property.rotation * Quaternion.Euler(Random.Range(20f, 30f), 0, Random.Range(20f, 30f)); 
   //    property.localScale = Vector3.zero;
   //  
   //    float startTime = Time.realtimeSinceStartup;
   //    while (Time.realtimeSinceStartup < startTime + duration)
   //    {
   //       float x = (Time.realtimeSinceStartup - startTime) / duration;
   //       float y = MathT.EasedT(x);
   //       property.localScale = new Vector3(startScale.x * y, startScale.y * y, startScale.z * y);
   //       property.rotation = Quaternion.Lerp(randomRotation, startRotation, y);
   //       yield return null;
   //    }
   //  
   //    property.localScale = startScale;
   //    property.rotation = startRotation;
   // }
   
   // private IEnumerator DeSpawnProperty(Transform property)
   // {
   //    yield return new WaitForSecondsRealtime(GameEngine.PropertyMindTransitionTime);
   //  
   //    property.gameObject.SetActive(false);
   // }

   private void SubscribeToEvents()
   {
      TrialHandler.OnPropertySpawnedEvent += PropertySpawned;
      GameEngine.TrialInputRegisteredStartedEvent += TrialInputRegistered;
      GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
      GameEngine.TimedOutStartedEvent += TimedOut;
      GameEngine.AnswerCorrectStartedEvent += ClearProperty;
      GameEngine.AnswerWrongStartedEvent += ClearProperty;
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= PropertySpawned;
      GameEngine.TrialInputRegisteredStartedEvent -= TrialInputRegistered;
      GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
      GameEngine.TimedOutStartedEvent -= TimedOut;
      GameEngine.AnswerCorrectStartedEvent -= ClearProperty;
      GameEngine.AnswerWrongStartedEvent -= ClearProperty;
   }

   Transform currentProperty = null;
   protected virtual void PropertySpawned(Transform property)
   {
      property.gameObject.SetActive(true);
      float offset = Vector3.Distance(LocationHolder.PropertyLocation.position, LocationHolder.MindCameraLocation.position);
      property.position = (offset * PropertyCameraController.PropertyCamTransform.forward) + PropertyCameraController.PropertyCamTransform.position;
      // StartCoroutine(ActivateProperty(property));
      Vector3 originalScale = property.localScale;
      property.localScale = Vector3.zero;
      LeanTween.scale(property.gameObject, originalScale, .2f);
      currentProperty = property;
   }

   protected virtual void TrialInputRegistered(InputHandler.InputState input)
   {
      Vector3 currentPropertyScale = currentProperty.localScale;
      bool use = input == InputHandler.InputState.USING;
      LeanTween.scale(currentProperty.gameObject, use ? 1.1f * currentPropertyScale : .9f * currentPropertyScale, .1f);
      if (!use)
         LeanTween.alpha(currentProperty.gameObject, .2f, .1f);
   }

   protected virtual void EvaluatingInput(InputHandler.InputState input)
   {
      // StartCoroutine(DeSpawnProperty(currentProperty));
      LeanTween.scale(currentProperty.gameObject, Vector3.zero, GameEngine.PropertyMindTransitionTime);
   }

   protected virtual void TimedOut(InputHandler.InputState input)
   {
      // StartCoroutine(DeSpawnProperty(currentProperty));
      LeanTween.scale(currentProperty.gameObject, Vector3.zero, GameEngine.PropertyMindTransitionTime);
   }

   protected virtual void ClearProperty()
   {
      currentProperty.gameObject.SetActive(false);
      currentProperty = null;
   }
}
