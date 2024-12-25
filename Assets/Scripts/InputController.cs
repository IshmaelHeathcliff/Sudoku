using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputController : Singleton<InputController>
{
    public BoardInput BoardInput;
    [HideInInspector]public Cell currentCell;

    public void SelectCell(Cell cell)
    {
        if(currentCell != null)
            currentCell.Unselected();
        currentCell = cell;
        cell.Selected();
    }

    void SetNumber(int n)
    {
        if (currentCell.isLocked) return;
        currentCell.SetNumber(n);
    }

    void Register()
    {
        Debug.Log("Register Input");
        BoardInput = new BoardInput();
        BoardInput.Board.SetNumber1.performed += _ => { SetNumber(1); };
        BoardInput.Board.SetNumber2.performed += _ => { SetNumber(2); };
        BoardInput.Board.SetNumber3.performed += _ => { SetNumber(3); };
        BoardInput.Board.SetNumber4.performed += _ => { SetNumber(4); };
        BoardInput.Board.SetNumber5.performed += _ => { SetNumber(5); };
        BoardInput.Board.SetNumber6.performed += _ => { SetNumber(6); };
        BoardInput.Board.SetNumber7.performed += _ => { SetNumber(7); };
        BoardInput.Board.SetNumber8.performed += _ => { SetNumber(8); };
        BoardInput.Board.SetNumber9.performed += _ => { SetNumber(9); };
        BoardInput.Board.Clear.performed += _ => { currentCell.ClearNumber(); };
    }

    void Start()
    {
        Register();
        BoardInput.Enable();
    }
}