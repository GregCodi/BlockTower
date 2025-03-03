using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using BlockTower.Config;

[CustomEditor(typeof(ScriptableObjectConfigProvider))]
public class GameConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ScriptableObjectConfigProvider configProvider = (ScriptableObjectConfigProvider)target;
        
        EditorGUILayout.LabelField("Game Configuration", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Available Cubes", EditorStyles.boldLabel);
        
        int cubesToRemove = -1;
        
        for (int i = 0; i < configProvider.config.availableCubes.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"Cube {i + 1}", EditorStyles.boldLabel);
            
            configProvider.config.availableCubes[i].cubeId = EditorGUILayout.TextField("Cube ID", configProvider.config.availableCubes[i].cubeId);
            configProvider.config.availableCubes[i].cubeSprite = (Sprite)EditorGUILayout.ObjectField("Cube Sprite", configProvider.config.availableCubes[i].cubeSprite, typeof(Sprite), false);
            configProvider.config.availableCubes[i].cubeColor = EditorGUILayout.ColorField("Cube Color", configProvider.config.availableCubes[i].cubeColor);
            
            if (GUILayout.Button("Remove Cube"))
            {
                cubesToRemove = i;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        if (cubesToRemove >= 0)
        {
            configProvider.config.availableCubes.RemoveAt(cubesToRemove);
        }
        
        if (GUILayout.Button("Add New Cube"))
        {
            CubeConfig newCube = new CubeConfig();
            newCube.cubeId = "cube_" + configProvider.config.availableCubes.Count;
            newCube.cubeColor = Color.white;
            configProvider.config.availableCubes.Add(newCube);
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif 