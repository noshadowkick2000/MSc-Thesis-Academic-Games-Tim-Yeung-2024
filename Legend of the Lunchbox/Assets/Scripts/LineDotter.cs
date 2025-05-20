// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Assets;
// using UnityEngine;
//
// public class LineDotter : MonoBehaviour
// {
//     [SerializeField] private LineRenderer lineRenderer;
//     private Material mat;
//
//     private float offset;
//     
//     // Start is called before the first frame update
//     void Awake()
//     {
//         lineRenderer.SetPositions(new Vector3[] { LocationHolder.PropertyLocation.position, LocationHolder.PropertyLocation.position });
//         mat = lineRenderer.GetComponent<LineRenderer>().material;
//         offset = Vector3.Distance(LocationHolder.PropertyLocation.position, LocationHolder.MindCameraLocation.position) + 1f;
//         
//         SubscribeToEvents();
//     }
//
//     private void OnDestroy()
//     {
//         UnsubscribeFromEvents();
//     }
//
//     private void SubscribeToEvents()
//     {
//         GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
//         GameEngine.ShowingObjectInMindStartedEvent += ShowingEnemeyInMind;
//     }
//
//     private void UnsubscribeFromEvents()
//     {
//         GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
//         GameEngine.ShowingObjectInMindStartedEvent += ShowingEnemeyInMind;
//     }
//
//     private bool followingCam = false;
//     private void MovingToProperty(EncounterData.PropertyType propertyType)
//     {
//         followingCam = true;
//         // StartCoroutine(AnimateLine(PropertyCameraController.PropertyCamTransform.));
//     }
//
//     private void ShowingEnemeyInMind()
//     {
//         followingCam = false;
//         // StartCoroutine(AnimateLine(LocationHolder.PropertyLocation.position));
//     }
//
//     // private IEnumerator AnimateLine(Vector3 endPosition)
//     // {
//     //     float startTime = Time.time;
//     //     float x = 0;
//     //     
//     //     Vector3 startPos = lineRenderer.GetPosition(0);
//     //     
//     //     while (x < 1)
//     //     {
//     //         x = (Time.time - startTime) / GameEngine.StaticTimeVariables.ExplanationPromptDuration;
//     //         Vector3 pos = Vector3.Lerp(startPos, endPosition, MathT.EasedT(x));
//     //         lineRenderer.SetPosition(1, pos);
//     //         
//     //         yield return null;
//     //     }
//     //     
//     //     lineRenderer.SetPosition(1, endPosition);
//     // }
//
//     // Update is called once per frame
//     void Update()
//     {
//         // mat.mainTextureOffset = new Vector2(Time.time, 0);
//
//         if (!followingCam) return;
//         
//         lineRenderer.SetPosition(1, offset * PropertyCameraController.PropertyCamTransform.forward + PropertyCameraController.PropertyCamTransform.position);
//     }
// }
