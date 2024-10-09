using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
  private GameEngine engine;
  private int enemyId;
  private int startingHealth;
  private int health;

  // Properties are given numerical identifiers, enemy compares id to their weakness list
  private int[] propertyIds;
  private bool[] validProperty;
  private int currentProperty = 0;
  
  private void Awake()
  {
    health = startingHealth;
    engine = GameObject.FindObjectOfType<GameEngine>();
  }

  private void Init(int enemyId, int startingHealth, int[] propertyIds, bool[] validProperty)
  {
    this.enemyId = enemyId;
    this.startingHealth = startingHealth;
    this.propertyIds = propertyIds;
    this.validProperty = validProperty;
  }


  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  private bool EvaluateInput(bool valid)
  {
    bool correct = valid == validProperty[currentProperty];
    currentProperty++;
    if (correct && valid)
      DealDamage();
    else if (!correct)
      engine.DamagePlayer();
    
    //TODO handle playing animations etc

    return correct;
  }

  private void DealDamage()
  {
    health--;
    if (health == 0)
    {
      Die();
    }
  }

  private void Appear()
  {
    //TODO
  }

  private void Die()
  {
    //TODO
  }
}
