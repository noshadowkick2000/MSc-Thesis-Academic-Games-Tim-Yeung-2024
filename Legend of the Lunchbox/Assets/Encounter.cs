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
  public int[] GetPropertIds()
  {
    return propertyIds;
  }
  private bool[] validProperty;
  public bool[] GetValidProperty ()
  {
    return validProperty;
  }
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


  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  public bool EvaluateInput(bool valid)
  {
    bool correct = valid == validProperty[currentProperty];
    currentProperty++;
    if (correct && valid)
      DealDamage();
    //else if (!correct)
    //  engine.DamagePlayer();
    
    //TODO handle playing animations etc

    return correct;
  }

  public void DealDamage()
  {
    health--;
    if (health == 0)
    {
      Die();
    }
  }

  private void Die()
  {
    //TODO
  }
}
