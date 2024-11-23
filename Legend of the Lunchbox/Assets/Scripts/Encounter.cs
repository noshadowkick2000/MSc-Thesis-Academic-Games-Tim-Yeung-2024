using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Encounter : MonoBehaviour
{
  private bool isObject;
  public bool IsObject => isObject;
  
  //private GameEngine engine;
  private int enemyId;
  public int GetEnemyId()
  {
    return enemyId;
  }

  private float waitTime;

  public float GetWaitTime()
  {
    return waitTime;
  }
  
  private int startingHealth;
  private int health;
  
  private int[] propertyIds;

  public int[] GetAllPropertyIds()
  {
    return propertyIds;
  }
  private bool[] validProperty;
  private int currentProperty = 0;
  
  private void Awake()
  {
    //engine = FindObjectOfType<GameEngine>();
  }
  
  public void Init(int startingHealth, float waitTime)
  {
    this.startingHealth = startingHealth;
    this.waitTime = waitTime;
    
    health = startingHealth;

    isObject = false;
  }

  public void Init(int enemyId, int startingHealth, int[] propertyIds, bool[] validProperty, float waitTime)
  {
    this.enemyId = enemyId;
    this.startingHealth = startingHealth;
    this.propertyIds = propertyIds;
    this.validProperty = validProperty;
    this.waitTime = waitTime;
    
    health = startingHealth;

    isObject = true;
  }

  public override string ToString()
  {
    string propertyValues = "";
    foreach (int property in propertyIds) 
      propertyValues += $"[{property}] ";
    string validValues = "";
    foreach (bool validProperty in validProperty)
      validValues += $"[{validProperty}] ";


    return $"------" +
      $"{Environment.NewLine}" +
      $"Enemy id: {enemyId} | Starting health: {startingHealth} | Property ids: {propertyValues} | Correct properties: {validValues} " +
      $"{Environment.NewLine}" +
      $"Current enemy health: {health} | Properties used: {currentProperty}" +
      $"{Environment.NewLine}" +
      $"------";
  }

  public string CurrentPropertyInfo()
  {
    return $"Current property: {propertyIds[currentProperty]} | {validProperty[currentProperty]}";
  }

  public int GetCurrentPropertyId() { return propertyIds[currentProperty]; }

  public bool EncounterOver => currentProperty >= propertyIds.Length;


  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  public bool EvaluateInput(bool used)
  {
    bool correct = used == validProperty[currentProperty];
    currentProperty++;

    return correct;
  }

  public void SkipProperty()
  {
    currentProperty++;
  }

  public void DealDamage()
  {
    health--;
  }

  public bool EndEncounter()
  {
    bool won = health <= 0;
    if (won)
      WonEncounter();
    else
      LostEncounter();
    return won;
  }

  private void WonEncounter()
  {

  }

  private void LostEncounter()
  {

  }
}
