using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ColorScheme", fileName = "Color Scheme")]
public class ColorScheme : ScriptableObject
{
    public Color bgColor = Color.white; 
    public Color numberColor = Color.white;
    public Color boardColor = Color.white;
    public Color regionColor = Color.white;
    public Color cellColor = Color.white;
    public Color lockedCellColor = Color.white;
    public Color selectedCellColor = Color.white;
}