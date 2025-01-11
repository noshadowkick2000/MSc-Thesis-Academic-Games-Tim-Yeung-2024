using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterData
{
  public enum ObjectType
  {
    BREAK,
    IMAGE,
    WORD
  }

  public enum PropertyType
  {
    ACTION,
    SOUND,
    WORD
  }

  public class PropertyTrial
  {
    public int PropertyId;
    public PropertyType PropertyType;
    public bool ValidProperty;
  }
  
  public float EncounterBlockDelay;
  public ObjectType EncounterObjectType;
  public int StimulusObjectId;
  public int Health;
  public List<PropertyTrial> PropertyTrials = new List<PropertyTrial>();
  
  
  private int currentProperty;

  public int GetCurrentPropertyId() { return PropertyTrials[currentProperty].PropertyId; }
  
  public PropertyType GetCurrentPropertyType() { return PropertyTrials[currentProperty].PropertyType; }

  public bool EncounterOver => currentProperty >= PropertyTrials.Count;


  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  public bool EvaluateInput(bool used)
  {
    bool correct = used == PropertyTrials[currentProperty].ValidProperty;
    currentProperty++;

    return correct;
  }

  public void SkipProperty()
  {
    currentProperty++;
  }

  public void DealDamage()
  {
    Health--;
  }

  public bool EndEncounter()
  {
    bool won = Health <= 0;
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
