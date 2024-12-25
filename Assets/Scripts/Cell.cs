using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int number = 0;
    TextMeshProUGUI _tmp;
    Image _image;

    public Color normalColor;
    public Color selectedColor;
    public Color lockedColor;

    public bool isLocked;

    TextMeshProUGUI Tmp
    {
        get
        {
            if (_tmp == null)
            {
                _tmp = GetComponentInChildren<TextMeshProUGUI>();
            }

            return _tmp;
        }
    }

    Image Img
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }
    
    public void SetNumber(int n)
    {
        number = n;
        Tmp.text = $"{number}";
    }

    public void ClearNumber()
    {
        number = 0;
        Tmp.text = "";
    }

    public void Selected()
    {
        Img.color = selectedColor;
    }

    public void Unselected()
    {
        Img.color = normalColor;
    }

    public void Lock()
    {
        if (isLocked) return;
        isLocked = true;
        Img.color = lockedColor;
    }

    public void Unlock()
    {
        if (!isLocked) return;
        isLocked = false;
        Img.color = normalColor;
    }

    public void SetColor(Color normal, Color selected, Color locked)
    {
        normalColor = normal;
        selectedColor = selected;
        lockedColor = locked;

        Img.color = normalColor;
        if (isLocked) Img.color = lockedColor;
    }


    public void Init()
    {
        Img.color = normalColor;
        GetComponent<Button>().onClick.AddListener(delegate { InputController.Instance.SelectCell(this); });
    }
}