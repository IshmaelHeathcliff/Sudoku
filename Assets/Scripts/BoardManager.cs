using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    public int sqrtSize = 3;

    int Size => sqrtSize * sqrtSize;

    public float interval = 0.1f;

    public GameObject cellPrefab;
    public GameObject boardHolder;

    Cell[,] _board;

    public Problems problems;
    SudokuGenerator _sudokuGenerator;
    Sudoku _sudoku;

    [Button]
    public void InitBoard()
    {
        ClearBoard();
        
        if (_board == null)
        {
            _board = new Cell[Size,Size];

            for (var i = 0; i < Size; i++)
            {

                for (var j = 0; j < Size; j++)
                {
                    var cellGameObject = Instantiate(cellPrefab, boardHolder.transform);
                    var cellTransform = cellGameObject.GetComponent<RectTransform>();
                    var sizeDelta = cellTransform.sizeDelta;
                    var cellPosition = new Vector2(sizeDelta.x * (1 + interval) * (i - (float) Size / 2),
                        sizeDelta.y * (1 + interval) * (-j + (float) Size / 2));
                    cellTransform.localPosition = cellPosition;
                    var cell = cellGameObject.GetComponent<Cell>();
                    cell.Init();
                    _board[i, j] = cell;
                }
            }
        }

        if (problems != null && problems.problems.Count != 0 && _sudoku == null)
        {
            int index = Random.Range(0, problems.problems.Count);
            _sudoku = new Sudoku(sqrtSize, problems.problems[index].Numbers);
            _sudoku.Calculate();
        }
        
        
        LockConfirmed();
        ShowNumbers(false);
    }

    [Button]
    void GetAnswer()
    {
        ShowNumbers(true);
    }

    [Button]
    void SaveSudoku()
    {
        problems.problems.Add(_sudoku.Save("ScriptableObjects/Problems/"));
    }
    
    [Button]
    public void GenerateProblem(int count)
    {
        _sudokuGenerator ??= new SudokuGenerator(sqrtSize);
        while(true)
        {
            _sudoku = _sudokuGenerator.GenerateSudoku(count);
            if (_sudoku.Calculate()) break;
        }
        
        InitBoard();
    }

    [Button]
    void ResetBoard()
    {
        _board = null;
        ClearBoard();
    }
    
    void ClearBoard()
    {
        if(_board == null)
        {
            int childCount = boardHolder.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                DestroyImmediate(boardHolder.transform.GetChild(0).gameObject);
            }
        }
        else
        {
            foreach (var cell in _board)
            {
                cell.ClearNumber();
            }
        }
    }

    [Button]
    public void CheckAnswer()
    {
        if (CheckNumbers())
        {
            Debug.Log("Congratulations!");
        }
        else
        {
            Debug.Log("Try again");
        }
    }
    
    bool CheckNumbers()
    {
        for (int i = 0; i < Size; i++)
        {
            var row = new List<int>();
            var column = new List<int>();
            var region = new List<int>();
            for (var j = 0; j < Size; j++)
            {
                int regionRow = i / sqrtSize * sqrtSize + j / sqrtSize;
                int regionColumn = i % sqrtSize * sqrtSize + j % sqrtSize;

                int rowNumber = _board[i, j].number;
                int columnNumber = _board[j, i].number;
                int regionNumber = _board[regionRow, regionColumn].number;

                if (rowNumber != 0 && !row.Contains(rowNumber) &&
                    columnNumber != 0 && !column.Contains(columnNumber) &&
                    regionNumber != 0 && !region.Contains(regionNumber))
                {
                    row.Add(rowNumber);
                    column.Add(columnNumber);
                    region.Add(regionNumber);
                }
                else
                {
                    return false;
                } 
            }
        }

        return true;
    }
    
    void LockConfirmed()
    {
        foreach (var cell in _board)
        {
            cell.Unlock();
        }
        
        int count = _sudoku.Confirmed.Count;
        for (var i = 0; i < count; i++)
        {
            int[] point = _sudoku.Confirmed[i];
            _board[point[0], point[1]].Lock();
        }
    }
    

    void ShowNumbers(bool showAnswer)
    {
        if (_sudoku == null) return;
        int count = _sudoku.Confirmed.Count;
        for (var i = 0; i < count; i++)
        {
            int[] point = _sudoku.Confirmed[i];
            _board[point[0], point[1]].SetNumber(point[2]);
        }

        if (!showAnswer) return;
        count = _sudoku.Unconfirmed.Length;
        for (var i = 0; i < count; i++)
        {
            int[] point = _sudoku.Unconfirmed[i];
            _board[point[0], point[1]].SetNumber(point[2]);
        }
    }

    public void SetCellColor(Color normal, Color selected, Color locked)
    {
        cellPrefab.GetComponent<Cell>().SetColor(normal, selected, locked);
        ResetBoard();
        InitBoard();
    }
    
    void Start()
    {
        InitBoard();
    }
}
