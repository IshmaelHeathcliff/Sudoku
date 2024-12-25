using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Sudoku
{
    readonly int _sqrtSize;
    int Size => _sqrtSize * _sqrtSize;
    DancingLinks _dancingLinks;
    readonly int[,] _numbers;
    public int[][] Unconfirmed;
    public List<int[]> Confirmed;

    
    void AddRow(IReadOnlyList<int> point, int row)
    {
        int i = point[0];
        int j = point[1];
        int n = point[2] - 1;
        _dancingLinks.AddNode(row,i + Size * j);
        _dancingLinks.AddNode(row, i + Size * n + Size * Size);
        _dancingLinks.AddNode(row, j + Size * n + Size * Size * 2);
        _dancingLinks.AddNode(row, i / _sqrtSize + j / _sqrtSize * _sqrtSize + Size * n + Size * Size * 3);
    }
    
    void InitDancingLinks()
    {
        Confirmed = new List<int[]>();
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                if (_numbers[i, j] != 0)
                {
                    Confirmed.Add(new []{i, j, _numbers[i, j]});
                }
            }
        }
        
        int count = Confirmed.Count;
        _dancingLinks = new DancingLinks(count + (Size * Size - count) * Size, 4*Size*Size);

        for (var i = 0; i < count; i++)
        {
            int[] point = Confirmed[i];
            AddRow(point, i);
        }

        Unconfirmed = new int[Size * Size - count][];

        int k = count;
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                if (_numbers[i, j] != 0) continue;
                
                Unconfirmed[(k - count)/Size] = new[]{i, j, 0};
                for (var n = 0; n < Size; n++)
                {
                    AddRow(new [] {i, j, n + 1}, k);
                    k++;
                }
            }
        }
    }

    public bool Calculate()
    {
        if (!_dancingLinks.Dancing()) return false;

        int count = Confirmed.Count;
        foreach (int k in _dancingLinks.Result.Select(node => node.Row))
        {
            if(k < count) continue;
            Unconfirmed[(k - count)/Size][2] = (k - count) % Size + 1;
        }

        return true;
    }

    public Sudoku(int sqrtSize, int[,] numbers) 
    {
        _sqrtSize = sqrtSize;
        _numbers = numbers;
        
        InitDancingLinks();
    }

    public Problem Save(string path, string name = "Problem")
    {
        var problem = ScriptableObject.CreateInstance<Problem>();
        problem.InitNumbers(_sqrtSize);
        problem.Numbers = _numbers;
        
        var count = 1;
        var fileName = $"{name} {count}.asset";
        var fi = new FileInfo(Application.dataPath + "/" + path + fileName);
        while (fi.Exists)
        {
            count++;
            fileName = $"{name} {count}.asset";
            fi = new FileInfo(Application.dataPath + "/" + path + fileName);
        }
        
        AssetDatabase.CreateAsset(problem, "Assets/" + path + fileName);
        return problem;
    }
}