using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Problem", fileName = "Problem")]
public class Problem : SerializedScriptableObject
{
    [TableMatrix]
    public int[,] Numbers;
    
    [Button]
    public void InitNumbers(int sqrtSize)
    {
        Numbers = new int[sqrtSize * sqrtSize, sqrtSize * sqrtSize];
    }
}