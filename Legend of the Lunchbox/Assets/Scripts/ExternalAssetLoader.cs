using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ExternalAssetLoader : MonoBehaviour
{
    private class StimulusAsset
    {
        public List<Sprite> Sprites = new List<Sprite>();
        public AudioClip AudioClip;
    }
    
    private static Dictionary<int, StimulusAsset> stimulusDictionary = new Dictionary<int, StimulusAsset>();
    
    public enum AssetType
    {
        IMAGE, // Load in object or action that is static
        IMAGE_ANIMATED, // Load in object or action that is animated
        SOUND, // Load in sound 
        WORD // Load in text
    }
    
    [SerializeField] private GameObject[] assetTemplate;
    private static GameObject[] AssetTemplate;
    [SerializeField] private string assetSubFolder;


    private static bool Loaded = false;

    private void Start()
    {
        if (Loaded)
            Destroy(gameObject);
        else
        {
            LoadAssets();
            DontDestroyOnLoad(gameObject);

            AssetTemplate = assetTemplate;

            print(stimulusDictionary.Count);

            Loaded = true;
        }
    }

    private void LoadAssets()
    {
        string path = Application.streamingAssetsPath + "/" + assetSubFolder;

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException();

        string lastFileName = "";
        StimulusAsset stimulusAsset = new StimulusAsset();
        string[] currentFile = new string[2];
        
        string[] files = Directory.GetFiles(path);
        Array.Sort(files);

        // TEMP
        AudioClip lastClip = null;
        
        foreach (string fileName in files)
        {
            if (!Path.GetExtension(fileName).Equals(".png") && !Path.GetExtension(fileName).Equals(".wav")) 
                continue;
            
            currentFile = Path.GetFileNameWithoutExtension(fileName).Split('-');
            
            // print(currentFile[0]);
            
            if (lastFileName != currentFile[0]) // Start new StimulusAsset load
            {
                stimulusDictionary.Add(UtilsT.GetId(lastFileName), stimulusAsset);
                lastFileName = currentFile[0];
                stimulusAsset = new StimulusAsset();
            }
            
            if (Path.GetExtension(fileName).Equals(".png"))
            {
                Sprite sprite = LoadSprite(fileName);
                stimulusAsset.Sprites.Add(sprite);
            }
            else if (Path.GetExtension(fileName).Equals(".wav"))
            {
                AudioClip audioClip = LoadAudioClip(fileName);
                stimulusAsset.AudioClip = audioClip;
                
                //TEMP
                lastClip = audioClip;
            }
        }
        stimulusDictionary.Add(UtilsT.GetId(currentFile[0]), stimulusAsset);
    }

    private const int GoalTextureSize = 1024;
    private Sprite LoadSprite(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Create a new texture
        if (!texture.LoadImage(fileData)) return null; // Load image data into texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    private AudioClip LoadAudioClip(string filePath)
    {
        return WavUtility.ToAudioClip(filePath);
        // byte[] fileData = File.ReadAllBytes(filePath);
        //
        // // Parse WAV file header
        // int channels = BitConverter.ToInt16(fileData, 22);
        // int sampleRate = BitConverter.ToInt32(fileData, 24);
        // int dataLength = BitConverter.ToInt32(fileData, 40);
        // int dataOffset = 44;
        //
        // float[] samples = new float[dataLength / 2];
        // for (int i = 0; i < samples.Length; i++)
        // {
        //     short sample = BitConverter.ToInt16(fileData, dataOffset + i * 2);
        //     samples[i] = sample / 32768f; // Normalize to range -1 to 1
        // }
        //
        // AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(filePath), samples.Length, channels, sampleRate, false);
        // clip.SetData(samples, 0);
        // return clip;
    }

    // -----------------------------------------------
    
    public static GameObject GetAsset(int id)
    {
        StimulusAsset stimulusAsset = stimulusDictionary[id];
        AssetType assetType = GetAssetType(stimulusAsset);
        
        GameObject newAsset = Instantiate(AssetTemplate[(int)assetType]);
        
        switch (assetType)
        {
            case AssetType.IMAGE:
                newAsset.GetComponent<TrialObject>().SetSprite(stimulusAsset.Sprites[0]);
                break;
            case AssetType.IMAGE_ANIMATED:
                newAsset.GetComponent<TrialObject>().SetSprites(stimulusAsset.Sprites.ToArray());
                break;
            case AssetType.SOUND:
                newAsset.GetComponentInChildren<AudioSource>().clip = stimulusAsset.AudioClip;
                break;
            case AssetType.WORD:
                // Wrong usage
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
        }

        return newAsset;
    }

    public static GameObject GetTextAsset(string text)
    {
        GameObject textAsset = Instantiate(AssetTemplate[(int)AssetType.WORD]);
        
        textAsset.GetComponentInChildren<TextMeshPro>().text = text;

        return textAsset;
    }

    private static AssetType GetAssetType(StimulusAsset stimulusAsset)
    {
        if (stimulusAsset.Sprites.Count > 1)
            return AssetType.IMAGE_ANIMATED;
        if (stimulusAsset.Sprites.Count == 1)
            return AssetType.IMAGE;
        return AssetType.SOUND;
    }
}
