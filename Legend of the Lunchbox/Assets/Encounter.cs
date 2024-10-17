using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Encounter : MonoBehaviour
{
  //private GameEngine engine;
  private int enemyId;
  public int GetEnemyId()
  {
    return enemyId;
  }
  private int startingHealth;
  private int health;

  // Properties are given numerical identifiers, enemy compares id to their weakness list
  private int[] propertyIds;
  private bool[] validProperty;
  private int currentProperty = 0;
  
  private void Awake()
  {
    //engine = FindObjectOfType<GameEngine>();
  }

  public void Init(int enemyId, int startingHealth, int[] propertyIds, bool[] validProperty)
  {
    this.enemyId = enemyId;
    this.startingHealth = startingHealth;
    this.propertyIds = propertyIds;
    this.validProperty = validProperty;
    health = startingHealth;
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


  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  public bool EvaluateInput(bool valid)
  {
    bool correct = valid == validProperty[currentProperty];
    currentProperty++;

    return correct;
  }

  /// <summary>
  /// Deal damage to the enemy
  /// </summary>
  /// <returns>returns true if enemy's health is depleted</returns>
  public bool DealDamage()
  {
    health--;
    if (health == 0)
      return true;
    return false;
  }

  public void Die()
  {
    //TODO
  }
}
