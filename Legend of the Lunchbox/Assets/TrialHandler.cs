using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
  [SerializeField] private string pathToTrials;
  [SerializeField] private int levelId = 0;

  private void Awake()
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(@pathToTrials);

    XmlNode level = doc.DocumentElement.ChildNodes[levelId];

    foreach (XmlNode encounter in level.ChildNodes)
    {
      
      //print(encounter.Attributes["enemy"].Value);
      foreach (XmlNode node in encounter.ChildNodes)
      {
        //print(node.InnerText);
        
      }
    }
  }
}
}
