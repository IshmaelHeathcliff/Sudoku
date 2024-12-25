using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ColorManager : Singleton<ColorManager>
{
    public ColorScheme scheme;

    public GameObject bg;
    public GameObject board;
    public GameObject regions;

    [Button]
    public void SetUpColor()
    {
        bg.GetComponent<Image>().color = scheme.bgColor;
        board.GetComponent<Image>().color = scheme.boardColor;
        foreach (var img in regions.GetComponentsInChildren<Image>())
        {
            img.color = scheme.regionColor;
        }
        
        SetCellColor();
    }

    void SetCellColor()
    {
        BoardManager.Instance.SetCellColor(scheme.cellColor, scheme.selectedCellColor, scheme.lockedCellColor);
    }
}