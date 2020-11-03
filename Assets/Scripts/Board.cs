using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int Coloum = 8;
    public int Row = 9 ;
    

    public Cell CellPrefab;

    public Cell[,] Cells;

    public GameObject SelectedCellParent;

    private List<Cell> SelectedCells = new List<Cell>();

    private List<Color> AllColors;
    
    private void Start()
    {
        Cells = new Cell[Coloum,Row];
        CreateCells();
        UpdateCells();
    }

    public void CreateCells()
    {
        for (int i = 0; i < Coloum; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
                Cells[i, j] = cell;
            }
        }
    }

    public void UpdateCells()
    {
        AllColors = GameManager.instance.colors;
        for (int i = 0; i < Coloum; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                Cells[i, j].SetCellPosColor(i, j);
                List<Color> colorList = ColorsCanBe(Cells[i, j]);
                Cells[i, j].SetColor(colorList[Random.Range(0,colorList.Count)]);
                
            }
        }
    }
    
    public Cell GetNeighbour(Cell cell, Direction direction)
    {
        if (cell == null)
            return null;
        int x = cell.X;
        int y = cell.Y;
        switch (direction)
        {
            case Direction.Up:
                y += 1;
                break;
            case Direction.UpRight:
                if (x % 2 == 0)
                    y += 1;
                x += 1;
                break;
            case Direction.DownRight:
                if (x % 2 != 0)
                    y -= 1;
                x += 1;
                break;
            case Direction.Down:
                y -= 1;
                break;
            case Direction.DownLeft:
                if (x % 2 != 0)
                    y -= 1;
                x -= 1;
                break;
            case Direction.UpLeft:
                if (x % 2 == 0)
                    y += 1;
                x -= 1;
                break;
        }

        if (x >= Coloum || x < 0 || y >= Row || y < 0) return null;

        return Cells[x, y];
    }

    public bool FindMatches(Cell cell)
    {
        List<Cell> MatchCells = new List<Cell>() {cell};
        for (int j = 0; j < cell.Neighbours.Count; j++)
        {
            var neighbour = true;
            foreach (var matchCell in MatchCells)
            {
                if (!matchCell.IsNeighbour(cell.Neighbours[j]))
                {
                    neighbour = false;
                    break;
                }
            }
            if (cell.Color == cell.Neighbours[j].Color &&  neighbour) 
            {
                MatchCells.Add(cell.Neighbours[j]);
            }
        }

        if (MatchCells.Count == 3)
        {
            foreach (Cell mCell in MatchCells)
            {
                Destroy(mCell.gameObject);
            }
        }
       
        return MatchCells.Count == 3;
    }

    public void CellPressed(Cell cell , Vector2 pointPressed)
    {
        ResetSelectedCells();
        SelectedCells = new List<Cell> {cell};
        for (int i = 0; i < 2; i++)
        {
            var minDistance = float.MaxValue;
            var closeCell = cell;
            for (int j = 0; j < cell.Neighbours.Count; j++)
            {
                var neighbour = true;
                var cellDistance = Vector2.Distance(pointPressed, cell.Neighbours[j].transform.position);
                foreach (var selectedCell in SelectedCells)
                {
                    if (!selectedCell.IsNeighbour(cell.Neighbours[j]))
                    {
                        neighbour = false;
                        break;
                    }
                }
                if (cellDistance < minDistance && !SelectedCells.Contains(cell.Neighbours[j]) && neighbour) 
                {
                    minDistance = cellDistance;
                    closeCell = cell.Neighbours[j];
                }
            }

            if (minDistance < float.MaxValue)
            {
                SelectedCells.Add(closeCell);
            }
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
           SelectedCells[i].ResetColor();
        }
    }

    public List<Color> ColorsCanBe(Cell cell)
    {
        List<Cell> neighbours = cell.Neighbours;
        AllColors = GameManager.instance.colors;
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

    public Vector2 CalculateMidPoint()
    {
        float x = 0;
        float y = 0;
        foreach (Cell cell in SelectedCells)
        {
            x += cell.transform.localPosition.x;
            y += cell.transform.localPosition.y;
        }
        x /= 3;
        y /= 3;
        return new Vector2(x,y);
    }

    public void AddSelectedCellToParent()
    {
        foreach (Cell cell in SelectedCells)
        {
            cell.transform.parent = SelectedCellParent.transform;
        }
    }

    public void RotateSelectedCells(bool clockwise,int state)
    {
        if (state == 0)
        {
            InputManager.instance.OpenInput();
            return;
        }
        
        Cell[] rightOrderRotate = new Cell[3];
        foreach (Cell cell in SelectedCells)
        {
            if (cell.FirstCellBelow != null && SelectedCells.Contains(cell.FirstCellBelow))
                rightOrderRotate[0] = cell;
        }

        if (SelectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
        {
            rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.DownRight);
            rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.Down);
        }
        else
        {
            rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.Down);
            rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.DownLeft);
        }

        Vector2[] cellPositions = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            cellPositions[i] = rightOrderRotate[i].transform.position;
        }
        
        int tempX = rightOrderRotate[2].X;
        int tempY = rightOrderRotate[2].Y;

        rightOrderRotate[2].SetGridPos(rightOrderRotate[0].X, rightOrderRotate[0].Y);
        Cells[rightOrderRotate[2].X, rightOrderRotate[2].Y] = rightOrderRotate[2]; 
        
        rightOrderRotate[0].SetGridPos(rightOrderRotate[1].X, rightOrderRotate[1].Y);
        Cells[rightOrderRotate[0].X, rightOrderRotate[0].Y] = rightOrderRotate[0];
        
        rightOrderRotate[1].SetGridPos(tempX, tempY);
        Cells[rightOrderRotate[1].X, rightOrderRotate[1].Y] = rightOrderRotate[1];

        Cells[rightOrderRotate[2].X, rightOrderRotate[2].Y].UpdateNeighbours(this);
        Cells[rightOrderRotate[1].X, rightOrderRotate[1].Y].UpdateNeighbours(this);
        Cells[rightOrderRotate[0].X, rightOrderRotate[0].Y].UpdateNeighbours(this);
        
        Cells[rightOrderRotate[2].X, rightOrderRotate[2].Y].UpdateAllNeighbours();
        Cells[rightOrderRotate[1].X, rightOrderRotate[1].Y].UpdateAllNeighbours();
        Cells[rightOrderRotate[0].X, rightOrderRotate[0].Y].UpdateAllNeighbours();


        float duration = GameManager.instance.rotationDuration;
        rightOrderRotate[0].transform.DOMove(cellPositions[1], duration);
        rightOrderRotate[1].transform.DOMove(cellPositions[2], duration);
        rightOrderRotate[2].transform.DOMove(cellPositions[0], duration).OnComplete(() =>
        {
            foreach (Cell cell in rightOrderRotate)
            {
                if (FindMatches(cell))
                {
                    InputManager.instance.OpenInput();
                    return;
                }
            }
            RotateSelectedCells(true,state-1);
        });
        
    
        ResetSelectedCells();
    }
    
  
}
