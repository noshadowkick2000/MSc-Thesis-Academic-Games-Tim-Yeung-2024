using System.Collections;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectAnimator : ObjectMover
{
   [FormerlySerializedAs("discoverablePrefab")] [SerializeField] private GameObject discoverableEndPrefab;
   private SpriteRenderer objectRenderer;
   private TextMeshPro textMesh;
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
      GameEngine.SettingUpMindStartedEvent += SettingUpMind;
      GameEngine.ShowingObjectInMindStartedEvent += ShowingObjectInMind;
      GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
      GameEngine.MovingToObjectStartedEvent += MovingToObject;
      GameEngine.AnswerCorrectStartedEvent += CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent += WrongAnswer;
      GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
      GameEngine.LostEncounterStartedEvent += LostEncounter;
      GameEngine.WonEncounterStartedEvent += WonEncounter;
   }

   private void UnsubscribeFromEvents()
   {
      TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
      GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
      GameEngine.ShowingObjectInMindStartedEvent -= ShowingObjectInMind;
      GameEngine.MovingToPropertyStartedEvent -= MovingToProperty;
      GameEngine.MovingToObjectStartedEvent -= MovingToObject;
      GameEngine.AnswerCorrectStartedEvent -= CorrectAnswer;
      GameEngine.AnswerWrongStartedEvent -= WrongAnswer;
      GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
      GameEngine.LostEncounterStartedEvent -= LostEncounter;
      GameEngine.WonEncounterStartedEvent -= WonEncounter;
   }

   // private Rigidbody objectRb;
   // private GameObject face;

   private bool isWord = false;
   
   private void ObjectSpawned(Transform objectTransform)
   {
      // objectRb = objectTransform.GetComponentInChildren<Rigidbody>();
      // face = objectTransform.GetComponentInChildren<Animation>().gameObject;

      if (objectTransform.GetComponentInChildren<TrialObject>() == null)
      {
         isWord = true;
         textMesh = objectTransform.GetComponentInChildren<TextMeshPro>();
      }
      else
      {
         objectRenderer = objectTransform.GetComponentInChildren<TrialObject>().MainSpriteRenderer;
         objectMat = objectRenderer.material;
      }
      
      // face.SetActive(false);
      mainObject = objectTransform;
      mainObject.gameObject.SetActive(false);
      
      // ImmediateToObject(LocationHolder.EnemyLocation.position + Vector3.down, mainObject.transform.rotation);
      // SmoothToObject(LocationHolder.EnemyLocation, GameEngine.StaticTimeVariables.EncounterStartDuration, true);
      // StartCoroutine(GrowObject());
   }
   
   private void SettingUpMind()
   {
      if (isWord) return;
      
      objectMat.SetFloat("_t", 1);
   }
   
   private void ShowingObjectInMind()
   {
      mainObject.gameObject.SetActive(true);
      StartCoroutine(Fade(true, GameEngine.StaticTimeVariables.EnemyMindShowTime / 10));
   }

   private void MovingToProperty(EncounterData.PropertyType propertyType)
   {
      StartCoroutine(Fade(false, GameEngine.StaticTimeVariables.ExplanationPromptDuration / 6));
   }

   private IEnumerator Fade(bool fadingIn, float duration)
   {
      if (isWord)
         textMesh.color = fadingIn ? Color.clear : Color.white;
      else
         objectRenderer.color = fadingIn ? Color.clear : Color.white;

      float startTime = Time.time;
      float x = 0;
      Color tempColor = Color.white;

      while (x < 1)
      {
         tempColor.a = fadingIn ? UtilsT.EasedT(x) : UtilsT.EasedT(1-x);
         
         if (isWord)
            textMesh.color = tempColor;
         else
            objectRenderer.color = tempColor;
         
         x = (Time.time - startTime) / duration;
         yield return null;
      }
      
      if (isWord)
         textMesh.color = fadingIn ? Color.white : Color.clear;
      else
         objectRenderer.color = fadingIn ? Color.white : Color.clear;
   }

   private void MovingToObject()
   {
      // StartCoroutine(Fade(true, GameEngine.StaticTimeVariables.ExplanationPromptDuration / 2));
   }

   private void CorrectAnswer()
   {
      // StartCoroutine(Nod(GameEngine.StaticTimeVariables.TrialFeedbackDuration, .1f, 2, true));
   }

   private void WrongAnswer()
   {
      // StartCoroutine(Nod(GameEngine.StaticTimeVariables.TrialFeedbackDuration, .1f, 2, false));
   }

   private void EvaluatingEncounter()
   {
      ShowingObjectInMind();
   }

   private void WonEncounter()
   {
      StartCoroutine(WinAnimation());
   }
   
   private void LostEncounter()
   {
      StartCoroutine(LostAnimation());
   }

   private IEnumerator LostAnimation()
   {
      // Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      
      yield return new WaitForSeconds(GameEngine.StaticTimeVariables.EncounterEvaluationDuration / 4);

      Vector3 pos = mainObject.position;
      mainObject.gameObject.SetActive(false);
      mainObject = Instantiate(discoverableEndPrefab, pos, Quaternion.identity).transform;

      SpriteRenderer[] spriteRenderers = mainObject.GetComponentsInChildren<SpriteRenderer>();
      float startTime = Time.time;
      float duration = (GameEngine.StaticTimeVariables.EncounterEvaluationDuration / 4) * 3;
      float x = 0f;
      
      Color tempColor = Color.white;

      while (x < 1)
      {
         x = (Time.time - startTime) / duration;

         tempColor.a = 1f - x;
         
         foreach (SpriteRenderer renderer in spriteRenderers)
         {
            renderer.color = tempColor;
         }

         yield return null;
      }
      
      Destroy(mainObject.gameObject);
      mainObject = null;
   }

   private IEnumerator WinAnimation()
   {
      float startTime = Time.time;
      float t = 0f;
      float duration = GameEngine.StaticTimeVariables.EncounterEvaluationDuration * .3f;

      if (isWord)
      {
         yield return new WaitForSeconds(duration);
      }
      else
      {
         while (t < 1)
         {
            objectMat.SetFloat("_t", 1-t);
         
            t = (Time.time - startTime) / duration;
            yield return null;
         }  
         
         objectMat.SetFloat("_t", 0);
      }
      
      // face.gameObject.SetActive(true);

      Vector3 startPosition = mainObject.transform.position;
      float speed = 0f;
      float acceleration = .2f;
      float h = 0;
      startTime = Time.time;
      t = 0f;
      duration = GameEngine.StaticTimeVariables.EncounterEvaluationDuration * .7f;
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
         t = (Time.time - startTime) / duration;
         yield return null;
      }
   }
}
