using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Problems", fileName = "Problems")]
public class Problems : SerializedScriptableObject
{
    public List<Problem> problems;
    string problemPath = "Assets/ScriptableObjects/Problems/";

    [Button]
    public void Clear()
    {
        foreach (var problem in problems)
        {
            AssetDatabase.DeleteAsset(problemPath + problem.name + ".asset");
        }
        problems = new List<Problem>();
    }
}