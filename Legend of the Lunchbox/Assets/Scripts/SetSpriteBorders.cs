// using UnityEditor;
// using UnityEngine;
//
// public class SetSpriteBorders : EditorWindow
// {
//     public Vector4 border = new Vector4(10, 10, 10, 10); // Set default border values
//     private Object[] selectedSprites;
//
//     [MenuItem("Tools/Set Sprite Borders")]
//     public static void ShowWindow()
//     {
//         GetWindow<SetSpriteBorders>("Set Sprite Borders");
//     }
//
//     private void OnGUI()
//     {
//         EditorGUILayout.LabelField("Set Borders for Selected Sprites", EditorStyles.boldLabel);
//         border = EditorGUILayout.Vector4Field("Border (Left, Bottom, Right, Top)", border);
//
//         if (GUILayout.Button("Apply Borders"))
//         {
//             SetBorders();
//         }
//     }
//
//     private void SetBorders()
//     {
//         selectedSprites = Selection.objects;
//
//         foreach (Object obj in selectedSprites)
//         {
//             string path = AssetDatabase.GetAssetPath(obj);
//             TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
//
//             if (importer != null && importer.spriteImportMode == SpriteImportMode.Single)
//             {
//                 importer.spriteBorder = border;
//                 AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
//             }
//         }
//
//         Debug.Log("Borders applied to selected sprites.");
//     }
// }