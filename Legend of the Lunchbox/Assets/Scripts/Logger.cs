using System;
using System.Globalization;
using System.IO;
using Assets;
using UnityEngine;
using CsvHelper;
using CsvHelper.Configuration;

public class Logger : MonoBehaviour
{
  public class LogEntry
  {
    public string DateTime { get; set; }
    public string Object { get; set; }
    public string Property { get; set; }
    public double InternalTime { get; set; }
    public string EventType { get; set; }
    public string Code { get; set; }
  }

  public class LogEntryMap : ClassMap<LogEntry>
  {
    public LogEntryMap()
    {
      Map(m => m.DateTime).Index(0).Name("DateTime");
      Map(m => m.Object).Index(1).Name("Object");
      Map(m => m.InternalTime).Index(2).Name("InternalTime");
      Map(m => m.EventType).Index(3).Name("EventType");
      Map(m => m.Code).Index(4).Name("Code");
    }
  }

  public enum Event
  {
    STIMULUS,
    PROMPT,
    FIXATE,
    INPUT,
    OTHER
  }

  public enum CodeTypes
  {
    OBJECT_IMAGE,
    OBJECT_WORD,
    OBJECT_STOP,
    PROPERTY_ACTION,
    PROPERTY_SOUND,
    PROPERTY_WORD,
    PROPERTY_STOP,
    INPUT_POSITIVE,
    INPUT_NEGATIVE,
    INPUT_TIMEOUT,
    FEEDBACK_POSITIVE,
    FEEDBACK_NEGATIVE,
    BREAK_START,
    BREAK_END,
    START_BLOCK,
    STOP_BLOCK,
    NONE
  }
  
  private static StreamWriter _output;
  private static CsvWriter _writer;
  private static string _folderName;

  // private static LoggerComponent _loggerComponent;

  // TODO IMPLEMENT FEATURE IN GAME SETUP TO SELECT FOLDER FOR THE RESULTS

  public static void StartLogger()
  {
    SubscribeToEvents();
    
    string path = Application.streamingAssetsPath + "/" + DateTime.Today.ToString("ddMMyyyy");
    Directory.CreateDirectory(path);
    string filename = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
    _output = new StreamWriter($"{path}\\{filename}.log");
    _writer = new CsvWriter(_output, CultureInfo.InvariantCulture);
    _writer.Context.RegisterClassMap<LogEntryMap>();
    _writer.WriteHeader<LogEntry>();
    _writer.NextRecord();
  }

  public static void Log(Event eventType, CodeTypes codeType)
  {
    LogEntry entry = new LogEntry();
    
    entry.DateTime = DateTime.Now.ToString("HH:mm:ss:FFF");
    entry.InternalTime = Time.realtimeSinceStartupAsDouble;
    entry.EventType = Enum.GetName(typeof(Event), eventType);
    entry.Code = Enum.GetName(typeof(CodeTypes), codeType);
    
    if (TrialHandler.currentEncounterData != null)
    {
      entry.Object = TrialHandler.currentEncounterData.StimulusObjectName;
      entry.Property = TrialHandler.currentEncounterData.GetCurrentPropertyName();
    }
    else
    {
      entry.Object = "None";
      entry.Property = "None";
    }

    _writer.WriteRecord(entry);
    _writer.NextRecord();
  }

  private static void Flush()
  {
    _writer.Flush();
  }

  public static void CloseLog()
  {
    _writer?.Dispose();
    _output?.Close();

    _writer = null;
    _output = null;
    
    UnSubscribeToEvents();
  }

  public void OnDestroy() // is also triggered when the duplicate is destroyed
  {
    CloseLog();
  }

  private static void SubscribeToEvents()
  {
    GameEngine.EndingBreakStartedEvent += Flush;
    GameEngine.EndingEncounterStartedEvent += Flush;

    GameEngine.StartingEncounterStartedEvent += StartBlock;
    GameEngine.StartingBreakStartedEvent += StartBreak;

    GameEngine.ShowingObjectInMindStartedEvent += ShowingObject;
    GameEngine.MovingToPropertyStartedEvent += DisappearObject;
    GameEngine.MovingToPropertyStartedEvent += ShowPrompt;
    GameEngine.ThinkingOfPropertyStartedEvent += ShowFixate;
    GameEngine.ShowingPropertyStartedEvent += ShowProperty;
    GameEngine.TimedOutStartedEvent += TimeOut;
    GameEngine.MovingToObjectStartedEvent += DisappearProperty;

    GameEngine.AnswerCorrectStartedEvent += FeedbackPositive;
    GameEngine.AnswerWrongStartedEvent += FeedbackNegative;
    
    GameEngine.EndingEncounterStartedEvent += StopBlock;
    GameEngine.EndingBreakStartedEvent += StopBreak;
  }

  private static void UnSubscribeToEvents()
  {
    GameEngine.EndingBreakStartedEvent -= Flush;
    GameEngine.EndingEncounterStartedEvent -= Flush;
    
    GameEngine.StartingEncounterStartedEvent -= StartBlock;
    GameEngine.StartingBreakStartedEvent -= StartBreak;

    GameEngine.ShowingObjectInMindStartedEvent -= ShowingObject;
    GameEngine.MovingToPropertyStartedEvent -= DisappearObject;
    GameEngine.MovingToPropertyStartedEvent -= ShowPrompt;
    GameEngine.ThinkingOfPropertyStartedEvent -= ShowFixate;
    GameEngine.ShowingPropertyStartedEvent -= ShowProperty;
    GameEngine.TimedOutStartedEvent -= TimeOut;
    GameEngine.MovingToObjectStartedEvent -= DisappearProperty;

    GameEngine.AnswerCorrectStartedEvent -= FeedbackPositive;
    GameEngine.AnswerWrongStartedEvent -= FeedbackNegative;

    GameEngine.EndingEncounterStartedEvent -= StopBlock;
    GameEngine.EndingBreakStartedEvent -= StopBreak;

  }

  private static void StartBlock()
  {
    Log(Event.OTHER, CodeTypes.START_BLOCK);
  }

  private static void StopBlock()
  {
    Log(Event.OTHER, CodeTypes.STOP_BLOCK);
  }

  private static void StartBreak()
  {
    Log(Event.OTHER, CodeTypes.BREAK_START);
  }

  private static void StopBreak()
  {
    Log(Event.OTHER, CodeTypes.BREAK_END);
  }

  private static void ShowingObject()
  {
    switch (TrialHandler.currentEncounterData.EncounterObjectType)
    {
      case EncounterData.ObjectType.BREAK:
        break;
      case EncounterData.ObjectType.IMAGE:
        Log(Event.STIMULUS, CodeTypes.OBJECT_IMAGE);
        break;
      case EncounterData.ObjectType.WORD:
        Log(Event.STIMULUS, CodeTypes.OBJECT_WORD);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private static void DisappearObject(EncounterData.PropertyType propertyType)
  {
    Log(Event.STIMULUS, CodeTypes.OBJECT_STOP);
  }

  private static void ShowPrompt(EncounterData.PropertyType propertyType)
  {
    Log(Event.PROMPT, CodeTypes.NONE);
  }

  private static void ShowFixate()
  {
    Log(Event.FIXATE, CodeTypes.NONE);
  }

  private static void ShowProperty(Action<InputHandler.InputState> callback)
  {
    switch (TrialHandler.currentEncounterData.GetCurrentPropertyType())
    {
      case EncounterData.PropertyType.ACTION:
        Log(Event.STIMULUS, CodeTypes.PROPERTY_ACTION);
        break;
      case EncounterData.PropertyType.SOUND:
        Log(Event.STIMULUS, CodeTypes.PROPERTY_SOUND);
        break;
      case EncounterData.PropertyType.WORD:
        Log(Event.STIMULUS, CodeTypes.PROPERTY_WORD);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private static void DisappearProperty()
  {
    Log(Event.STIMULUS, CodeTypes.PROPERTY_STOP);
  }

  private static void FeedbackPositive()
  {
    Log(Event.OTHER, CodeTypes.FEEDBACK_POSITIVE);
  }

  private static void FeedbackNegative()
  {
    Log(Event.OTHER, CodeTypes.FEEDBACK_NEGATIVE);
  }

  private static void TimeOut(InputHandler.InputState input)
  {
    Log(Event.INPUT, CodeTypes.INPUT_TIMEOUT);
  }
}
