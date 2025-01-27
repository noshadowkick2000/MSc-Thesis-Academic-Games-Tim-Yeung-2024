using System.Collections.Generic;

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
    public string PropertyName;
    public PropertyType PropertyType;
    public float ITI;
    public bool ValidProperty;
  }
  
  public float EncounterBlockDelay;
  public ObjectType EncounterObjectType;
  public int StimulusObjectId;
  public string StimulusObjectName;
  public int Health;
  public List<PropertyTrial> PropertyTrials = new List<PropertyTrial>();
  
  
  private int currentProperty;

  public int GetCurrentPropertyId() { return PropertyTrials[currentProperty].PropertyId; }
  
  public string GetCurrentPropertyName() { return PropertyTrials[currentProperty].PropertyName; }
  
  public PropertyType GetCurrentPropertyType() { return PropertyTrials[currentProperty].PropertyType; }

  public float GetCurrentTrialDelay() { return PropertyTrials[currentProperty].ITI; }

  public int GetCurrentProperty => currentProperty;

  public bool EncounterOver = false;

  public void IncreaseCounter()
  {
    if (currentProperty != PropertyTrials.Count - 1)
      currentProperty++;
    else
      EncounterOver = true;
  }

  /// <summary>
  /// Evaluates input from player for current item
  /// </summary>
  /// <returns>(bool) true if input was correct</returns>
  public bool EvaluateInput(bool used)
  {
    bool correct = used == PropertyTrials[currentProperty].ValidProperty;
    // currentProperty++;

    return correct;
  }

  public void SkipProperty()
  {
    // currentProperty++;
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
