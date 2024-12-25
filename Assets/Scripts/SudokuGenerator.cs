using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SudokuGenerator
{
    const int GeneratingNumber = 11;

    int[,] _numbers;
    readonly int _sqrtSize;

    int Size => _sqrtSize * _sqrtSize;

    public Sudoku GenerateSudoku(int count)
    {
        Sudoku sudoku;
        while (true)
        {
            _numbers = new int[Size, Size];
            GenerateRandomNumbers();
            sudoku = new Sudoku(_sqrtSize, _numbers);

            if (sudoku.Calculate()) break;
        }

        if (count <= GeneratingNumber)
        {
            return sudoku;
        }

        while (true)
        {
            var unconfirmed = new List<int[]>();
            sudoku.Unconfirmed.ForEach(i => unconfirmed.Add(i));

            for (var i = 0; i < count - GeneratingNumber; i++)
            {
                int index = Random.Range(0, unconfirmed.Count);
                int[] point = unconfirmed[index];
                _numbers[point[0], point[1]] = point[2];
                unconfirmed.RemoveAt(index);
            }

            var newSudoku = new Sudoku(_sqrtSize, _numbers);
            if (newSudoku.Calculate()) return newSudoku;
        }

    }

    void GenerateRandomNumbers()
    {
        var positions = new List<int[]>();
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                positions.Add(new []{i, j});
            }
        }

        
        var targetPositions = new int[GeneratingNumber][];
        for (var i = 0; i < GeneratingNumber; i++)
        {
            int index = Random.Range(0, positions.Count);
            targetPositions[i] = positions[index];
            positions.RemoveAt(index);
        }

        while (true)
        {
            var pass = true;
            foreach (int[] pos in targetPositions)
            {
                var availableNumbers = new List<int>();

                for (int i = 1; i < 10; i++)
                {
                    availableNumbers.Add(i);
                }
                
                while (true)
                {
                    if (availableNumbers.Count == 0)
                    {
                        Debug.Log("GenerateRandomNumbers failed");
                        pass = false;
                        break;
                    }

                    int index = Random.Range(0, availableNumbers.Count);
                    int number = availableNumbers[index];
                    availableNumbers.RemoveAt(index);

                    pass = true;
                    for (var i = 0; i < Size; i++)
                    {
                        if (_numbers[pos[0], i] != number &&
                            _numbers[i, pos[1]] != number &&
                            _numbers[(pos[0] / _sqrtSize) * _sqrtSize + i / _sqrtSize,
                                (pos[1] / _sqrtSize) * _sqrtSize + i % _sqrtSize] != number) continue;
                        pass = false;
                        break;
                    }

                    if (!pass) continue;

                    _numbers[pos[0], pos[1]] = number;
                    break;
                }

                if (!pass) break;
            }

            if (pass) break;
        }
    }

    public SudokuGenerator(int sqrtSize)
    {
        _sqrtSize = sqrtSize;
    }
        
}