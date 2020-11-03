using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int Coloum = 8;
    public int Row = 9 ;
    

    public Cell CellPrefab;

    public Cell[,] Cells;

    private List<Cell> SelectedCells = new List<Cell>();
    private Cell[,] VisitedCells;

    private List<Color> AllColors;
    
    private void Start()
    {
        Cells = new Cell[Coloum,Row];
        VisitedCells = new Cell[Coloum, Row];
        CreateCells();
        UpdateCells();
    }

    public void CreateCells()
    {
        for (var i = 0; i < Coloum; i++)
        {
            for (var j = 0; j < Row; j++)
            {
                var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
                Cells[i, j] = cell;
            }
        }
    }

    public void UpdateCells()
    {
        AllColors = GameManager.instance.Colors;
        for (var i = 0; i < Coloum; i++)
        {
            for (var j = 0; j < Row; j++)
            {
                Cells[i, j].SetCellPosColor(i, j);
                List<Color> colorList = ColorsCanBe(Cells[i, j]);
                Cells[i, j].SetColor(colorList[Random.Range(0,colorList.Count)]);
                
            }
        }
    }
    
    public Cell GetNeighbour(Cell cell, Direction direction)
    {
        var x = cell.X;
        var y = cell.Y;
        switch (direction)
        {
            case Direction.Up:
                y += 1;
                break;
            case Direction.UpRight:
                if (x % 2 != 0)
                {
                    x += 1;
                }
                else{ 
                    y += 1;
                    x += 1;
                }
                break;
            case Direction.DownRight:
                if (x % 2 != 0)
                {
                    x += 1;
                    y -= 1;
                }
                else{ 
                    x += 1;
                }
                break;
            case Direction.Down:
                y -= 1;
                break;
            case Direction.DownLeft:
                if (x % 2 != 0)
                {
                    x -= 1;
                    y -= 1;
                }
                else{ 
                    x -= 1;
                }
                break;
            case Direction.UpLeft:
                if (x % 2 != 0)
                {
                    x -= 1;
                }
                else{ 
                    x -= 1;
                    y += 1;
                }
                break;
        }

        if (x >= Coloum || x < 0 || y >= Row || y < 0) return null;

        return Cells[x, y];
    }

    public void FindMatches()
    {
        
    }

    public void CellPres(Cell cell , Vector2 pointPressed)
    {
        ResetSelectedCells();
        SelectedCells = new List<Cell> {cell};
        for (var i = 0; i < 2; i++)
        {
            var minDistance = float.MaxValue;
            var closeCell = cell;
            var neighbour = false;
            for (var j = 0; j < cell.Neighbours.Count; j++)
            {
                var cellDistance = Vector2.Distance(pointPressed, cell.Neighbours[j].transform.position);
                foreach (var selectedCell in SelectedCells)
                {
                    neighbour = selectedCell.IsNeighbour(cell.Neighbours[j]);
                }
                if (cellDistance < minDistance && !SelectedCells.Contains(cell.Neighbours[j]) && neighbour) 
                {
                    minDistance = cellDistance;
                    closeCell = cell.Neighbours[j];
                }
            }

            if (minDistance < float.MaxValue)
                SelectedCells.Add(closeCell);
        }
        HighlightSelectedCells();
    }

    public void HighlightSelectedCells()
    {
        for (int i = 0; i < SelectedCells.Count; i++)
        {
            SelectedCells[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public void ResetSelectedCells()
    {
        for (int i = 0; i < SelectedCells.Count; i++)
        {
            Cell cell = SelectedCells[i];
            cell.ResetColor();
        }
    }

    public List<Color> ColorsCanBe(Cell cell)
    {
        List<Cell> neighbours = cell.Neighbours;
        AllColors = GameManager.instance.Colors;
        List<Color> colorsCanBeUse = new List<Color>(AllColors);
        for (int i = 0; i < AllColors.Count; i++)
        {
            int counter = 0;
            for (int j = 0; j <  neighbours.Count; j++)
            {
                if (AllColors[i] == neighbours[j].Color)
                {
                    counter++;
                }
                if (counter > 1)
                {
                    colorsCanBeUse.Remove(colorsCanBeUse.Find(x=> x == neighbours[j].Color));
                }
            }
        }

        return colorsCanBeUse;
    }
    
  
}
