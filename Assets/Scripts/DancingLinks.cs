using System;
using System.Collections.Generic;
using UnityEngine;

public class DancingLinks
{

    public int Rows;
    public int Columns;

    public readonly Stack<DancingNode> Result;

    readonly DancingNode _head;

    readonly DancingNode[] _columnHeads;
    readonly DancingNode[] _rowHeads;
    readonly Stack<Stack<DancingNode>> _restores = new();

    public DancingLinks(int rows, int columns)
    {
        _columnHeads = new DancingNode[columns];
        _rowHeads = new DancingNode[rows];

        Result = new Stack<DancingNode>();
        Columns = columns;
        Rows = rows;
        
        for (var i = 0; i < columns; i++)
        {
            var columnHead = new DancingNode
            {
                Row = rows,
                Column = i
            };
            _columnHeads[i] = columnHead;
        }

        for (var i = 0; i < columns; i++)
        {
            var columnHead = _columnHeads[i];

            columnHead.Right = _columnHeads[i + 1 >= columns? 0: i + 1];
            columnHead.Left = _columnHeads[i - 1 < 0 ? columns - 1 : i - 1];
            columnHead.Up = _columnHeads[i];
            columnHead.Down = _columnHeads[i];
        }
        
        for (var i = 0; i < rows; i++)
        {
            var rowHead = new DancingNode
            {
                Column = columns,
                Row = i
            };
            _rowHeads[i] = rowHead;
        }

        for (var i = 0; i < rows; i++)
        {
            var rowHead = _rowHeads[i];

            rowHead.Down = _rowHeads[i + 1 >= rows? 0: i + 1];
            rowHead.Up = _rowHeads[i - 1 < 0 ? rows - 1 : i - 1];
            rowHead.Left = _rowHeads[i];
            rowHead.Right = _rowHeads[i];
        }
        
        _head = new DancingNode
        {
            Right = _columnHeads[0],
            Left = _columnHeads[columns - 1],
            Down = _rowHeads[0],
            Up = _rowHeads[rows - 1],
            Row = 0,
            Column = 0
        };
        
        _columnHeads[0].Left = _head;
        _columnHeads[columns - 1].Right = _head;
        _rowHeads[0].Up = _head;
        _rowHeads[rows - 1].Down = _head;
    }

    public void AddNode(int row, int column)
    {
        var node = new DancingNode
        {
            Row = row,
            Column = column,
        };

        var upNode = _columnHeads[column].Down;
        while (upNode.Row < row)
        {
            upNode = upNode.Down;
        }

        if (upNode.Row == row)
        {
            return;
        }

        upNode = upNode.Up;
        node.Up = upNode;
        node.Down = upNode.Down;
        upNode.Down.Up = node;
        upNode.Down = node;

        var leftNode = _rowHeads[row].Right;
        while (leftNode.Column < column)
        {
            leftNode = leftNode.Right;
        }

        if (leftNode.Column == column)
        {
            return;
        }

        leftNode = leftNode.Left;
        node.Left = leftNode;
        node.Right = leftNode.Right;
        leftNode.Right.Left = node;
        leftNode.Right = node;
    }

    void Failed()
    {
        Debug.Log("Failed");
    }

    DancingNode FindMinColumn()
    {
        var minColumn = _head.Right;
        var node = minColumn;
        while (node != _head)
        {
            if (node.GetColumnCount() < minColumn.GetColumnCount())
            {
                minColumn = node;
            }

            node = node.Right;
        }

        return minColumn;

    }
    
    public bool Dancing()
    {
        while (true)
        {
            DancingNode currentRow;
        
            while(true)
            {
                var minColumn = FindMinColumn();

                if (minColumn.GetColumnCount() == 0)
                {
                    if(Restore())
                        continue;
                    Failed();
                    return false;
                }

                currentRow = _rowHeads[minColumn.Down.Row];
                break;

            }
            
            TestRow(currentRow);

            if (_head.Right == _head)
            {
                break;
            }
        }

        return true;
    }

    void TestRow(DancingNode rowHead)
    {
        var restore = new Stack<DancingNode>();
        
        Result.Push(rowHead);
        rowHead.RemoveRow();
        restore.Push(rowHead);
        

        var rowNode = rowHead.Right;
        while (rowNode != rowHead)
        {
            var columnHead = _columnHeads[rowNode.Column];
            columnHead.RemoveColumn();
            restore.Push(columnHead);

            var columnNode = columnHead.Down;

            while (columnNode != columnHead)
            {
                var rh = _rowHeads[columnNode.Row];
                rh.RemoveRow();
                restore.Push(rh);
                columnNode = columnNode.Down;
            }
            
            rowNode = rowNode.Right;
        }

        _restores.Push(restore);
    }

    bool Restore()
    {
        while (true)
        {
            if (!RestoreTest(out var currentRow))
            {
                return false;
            }


            if (!_restores.TryPeek(out var lastRestore))
            {
                return false;
            }

            lastRestore.Push(currentRow);
            currentRow.RemoveRow();

            if (currentRow.Down != _head) break;
        }

        return true;
    }

    bool RestoreTest(out DancingNode rowHead)
    {
        if(!_restores.TryPop(out var restore))
        {
            rowHead = null;
            return false;
        }

        var restoreNode = new DancingNode();
        while (restore.TryPop(out var node))
        {
            restoreNode = node;
            node.Restore();
        }

        rowHead = Result.Pop();

        return rowHead == restoreNode;
    }
}

public class DancingNode
{
    public DancingNode Left;
    public DancingNode Right;
    public DancingNode Up;
    public DancingNode Down;

    public int Row;
    public int Column;

    bool _isRowRemoved;
    bool _isColumnRemoved;

    void RemoveNodeRow()
    {
        Up.Down = Down;
        Down.Up = Up;
        _isRowRemoved = true;
    }
    
    public void RemoveRow()
    {
        if (_isRowRemoved) return;
        
        RemoveNodeRow();
        var node = Right;
        while (node != this)
        {
            node.RemoveNodeRow();
            node = node.Right;
        }
    }
    
    void RemoveNodeColumn()
    {
        Left.Right = Right;
        Right.Left = Left;
        _isColumnRemoved = true;
    }

    public void RemoveColumn()
    {
        if (_isColumnRemoved) return;
        
        RemoveNodeColumn();
        var node = Down;
        while (node != this)
        {
            node.RemoveNodeColumn();
            node = node.Down;
        }
    }


    void RestoreNodeRow()
    {
        Up.Down = this;
        Down.Up = this;
        _isRowRemoved = false;
    }
    
    void RestoreRow()
    {
        if (!_isRowRemoved) return;
        
        RestoreNodeRow();
        var node = Right;
        while (node != this)
        {
            node.RestoreNodeRow();
            node = node.Right;
        }
    }
    
    void RestoreNodeColumn()
    {
        Left.Right = this;
        Right.Left = this;
        _isColumnRemoved = false;
    }

    void RestoreColumn()
    {
        if (!_isColumnRemoved) return;

        RestoreNodeColumn();
        var node = Down;
        while (node != this)
        {
            node.RestoreNodeColumn();
            node = node.Down;
        }
    }

    public void Restore()
    {
        if (_isColumnRemoved)
        {
            RestoreColumn();
        }

        if (_isRowRemoved)
        {
            RestoreRow();
        }
    }

    public int GetColumnCount()
    {
        var count = 0;
        var node = Down;
        while (node != this)
        {
            count++;
            node = node.Down;
        }

        return count;
    }
}