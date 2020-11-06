﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enums;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    

    public Cell[,] cells;
    
    private List<Cell> selectedCells;

    private List<Color> allColors;

    private List<List<Cell>> threeMatches;

    private List<Cell> fallCells;
    private Cell[] rightOrderRotate;
    
    private int scoreMultiplier = 1;
    private Cell bombCell;
    private int coloum;
    private int row;
    private Cell cellPrefab;
    private int lastColIndex = 0;


    public void InitializeBoard()
    {
        selectedCells = new List<Cell>();
        fallCells = new List<Cell>();
        rightOrderRotate = new Cell[3];

        cellPrefab = GameManager.instance.cellPrefab;
        coloum = GameManager.instance.coloum;
        row = GameManager.instance.row;
        cells = new Cell[coloum,row];
        CreateCells();
        UpdateCells();
        InputManager.instance.OpenInput();
    }
    
    public void CreateCells()
    {
        for (int i = 0; i < coloum; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var cell = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, transform);
                cells[i, j] = cell;
            }
        }
    }

    public void UpdateCells()
    {
        allColors = GameManager.instance.colors;
        for (int i = 0; i < coloum; i++)
        {
            for (int j = 0; j < row; j++)
            {
                cells[i, j].SetCellPosColor(i, j);
                List<Color> colorList = ColorsCanBe(cells[i, j]);
                cells[i, j].SetColor(colorList[Random.Range(0, colorList.Count)]);

            }
        }
    }

    public void GameOver()
    {
        DOTween.PauseAll();
        //DOTween.KillAll(true);
        for (int i = 0; i < coloum; i++)
        {
            for (int j = 0; j < row; j++)
            {
                //cells[i,j].PlayExplodeParticle();
                DestroyImmediate(cells[i,j].gameObject);
            }
        }    }
    
    
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

        if (x >= coloum || x < 0 || y >= row || y < 0) return null;

        return cells[x, y];
    }

    public bool FindMatch(List<Cell> checkCells)
    {
        threeMatches = new List<List<Cell>>();
        foreach (Cell cell in checkCells)
        {
            List<Cell> threeMatch = new List<Cell>(3) {cell};
            for (int i = 0; i < cell.Neighbours.Count; i++)
            {
                bool neighbour = true;
                foreach (var threeCell in threeMatch)
                {
                    if (!threeCell.IsNeighbour(cell.Neighbours[i]))
                    {
                        neighbour = false;
                        break;
                    }
                }

                if (cell.color == cell.Neighbours[i].color && neighbour)
                {
                    threeMatch.Add(cell.Neighbours[i]);
                }

                if (threeMatch.Count >= 3)
                {
                    bool uniqe = true;
                    for (int j = 0; j < threeMatches.Count; j++)
                    {
                        if (CheckListHaveSameMember(threeMatch, threeMatches[j]))
                            uniqe = false;
                    }

                    if (uniqe)
                        threeMatches.Add(threeMatch);
                }
            }
        }

        if (threeMatches.Count != 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckListHaveSameMember(List<Cell> list1, List<Cell> list2)
    {
        foreach (Cell cell1 in list1)
        {
            foreach (Cell cell2 in list2)
            {
                if (cell1.IsEqual(cell2))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void CellPressed(Cell cell, Vector2 pointPressed)
    {
        ResetSelectedCells();
        selectedCells = new List<Cell> {cell};
        for (int i = 0; i < 2; i++)
        {
            var minDistance = float.MaxValue;
            var closeCell = cell;
            for (int j = 0; j < cell.Neighbours.Count; j++)
            {
                var neighbour = true;
                var cellDistance = Vector2.Distance(pointPressed, cell.Neighbours[j].transform.position);
                foreach (var selectedCell in selectedCells)
                {
                    if (!selectedCell.IsNeighbour(cell.Neighbours[j]))
                    {
                        neighbour = false;
                        break;
                    }
                }

                if (cellDistance < minDistance && !selectedCells.Contains(cell.Neighbours[j]) && neighbour)
                {
                    minDistance = cellDistance;
                    closeCell = cell.Neighbours[j];
                }
            }

            if (minDistance < float.MaxValue)
            {
                selectedCells.Add(closeCell);
            }
        }

        HighlightSelectedCells();
    }

    public void HighlightSelectedCells()
    {
        for (int i = 0; i < selectedCells.Count; i++)
        {
            selectedCells[i].Shine();
        }
    }

    public void ResetSelectedCells()
    {
        for (int i = 0; i < selectedCells.Count; i++)
        {
            selectedCells[i].UnShine();
        }
    }

    public List<Color> ColorsCanBe(Cell cell)
    {
        List<Cell> neighbours = cell.Neighbours;
        allColors = GameManager.instance.colors;
        List<Color> colorsCanBeUse = new List<Color>(allColors);
        for (int i = 0; i < allColors.Count; i++)
        {
            int counter = 0;
            for (int j = 0; j < neighbours.Count; j++)
            {
                if (allColors[i] == neighbours[j].color)
                {
                    counter++;
                }

                if (counter > 1)
                {
                    colorsCanBeUse.Remove(colorsCanBeUse.Find(x => x == neighbours[j].color));
                }
            }
        }

        return colorsCanBeUse;
    }

    public Vector2 CalculateMidPoint()
    {
        float x = 0;
        float y = 0;
        foreach (Cell cell in selectedCells)
        {
            x += cell.transform.localPosition.x;
            y += cell.transform.localPosition.y;
        }

        x /= 3;
        y /= 3;
        return new Vector2(x, y);
    }


    public void RotateSelectedCells(bool clockwise, int state)
    {
        if (state == 0)
        {
            InputManager.instance.OpenInput();
            return;
        }
        if (state == 3)
        {
            rightOrderRotate = new Cell[3];
            foreach (Cell cell in selectedCells)
            {
                if (cell.FirstCellBelow != null && selectedCells.Contains(cell.FirstCellBelow))
                {
                    rightOrderRotate[0] = cell;
                }

            }

            if (clockwise)
            {
                if (GetNeighbour(rightOrderRotate[0], Direction.DownRight)!=null && selectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
                {
                    rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.DownRight);
                    rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.Down);
                }
                else
                {
                    rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.Down);
                    rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.DownLeft);
                }  
            }
            else
            {
                if (GetNeighbour(rightOrderRotate[0], Direction.DownRight)!=null && selectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
                {
                    rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.Down);
                    rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.DownRight);
                }
                else
                {
                    rightOrderRotate[1] = GetNeighbour(rightOrderRotate[0], Direction.DownLeft);
                    rightOrderRotate[2] = GetNeighbour(rightOrderRotate[0], Direction.Down);
                }  
            }
            
           

            
        }
        Vector2[] cellPositions = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            cellPositions[i] = rightOrderRotate[i].transform.position;
        } 
        int tempX = rightOrderRotate[2].X;
        int tempY = rightOrderRotate[2].Y;

        rightOrderRotate[2].SetGridPos(rightOrderRotate[0].X, rightOrderRotate[0].Y);
        cells[rightOrderRotate[2].X, rightOrderRotate[2].Y] = rightOrderRotate[2];

        rightOrderRotate[0].SetGridPos(rightOrderRotate[1].X, rightOrderRotate[1].Y);
        cells[rightOrderRotate[0].X, rightOrderRotate[0].Y] = rightOrderRotate[0];

        rightOrderRotate[1].SetGridPos(tempX, tempY);
        cells[rightOrderRotate[1].X, rightOrderRotate[1].Y] = rightOrderRotate[1];

        cells[rightOrderRotate[2].X, rightOrderRotate[2].Y].UpdateNeighbours(this);
        cells[rightOrderRotate[1].X, rightOrderRotate[1].Y].UpdateNeighbours(this);
        cells[rightOrderRotate[0].X, rightOrderRotate[0].Y].UpdateNeighbours(this);

        cells[rightOrderRotate[2].X, rightOrderRotate[2].Y].UpdateAllNeighbours();
        cells[rightOrderRotate[1].X, rightOrderRotate[1].Y].UpdateAllNeighbours();
        cells[rightOrderRotate[0].X, rightOrderRotate[0].Y].UpdateAllNeighbours();


        float duration = GameManager.instance.rotationDuration;
        rightOrderRotate[0].transform.DOMove(cellPositions[1], duration);
        rightOrderRotate[1].transform.DOMove(cellPositions[2], duration);
        rightOrderRotate[2].transform.DOMove(cellPositions[0], duration).OnComplete(() =>
        {
            
            
            
            if (!FindMatch(rightOrderRotate.ToList()))
            {
                RotateSelectedCells(clockwise, state - 1);
            }
            else
            {
                GameManager.instance.IncreaseMoveCount();
                if (bombCell != null)
                    bombCell.DecreaseCounter();
                rightOrderRotate = new Cell[3];
                foreach (var threeMatch in threeMatches)
                {
                    foreach (var cell in threeMatch)
                    {
                        GameManager.instance.IncreaseScore();
                        cell.PlayExplodeParticle();
                        DestroyImmediate(cell.gameObject);
                    }
                }



                SlideDown();
                selectedCells = new List<Cell>();

                // create new cell


                //InputManager.instance.OpenInput();
            }
            
        });


        ResetSelectedCells();
    }


 


    public void SlideDown()
    {
        for (int i = 0; i < coloum; i++)
        {
            for (int j = 1; j < row; j++)
            {
                if (j == row-1 && cells[i, j] == null)
                {
                   Fill(i);
                }
                else if (cells[i, j] != null)
                { Fall(cells[i, j],false);
                }
            }
        }
    }


    public Tween Fall(Cell cell,bool canBeLast)
    {
        Tween lastTween = null;
        if (GetNeighbour(cell,Direction.Down) == null && cell.Y != 0 && !DOTween.IsTweening(cell.transform))
        {
            if (!fallCells.Contains(cell))
            {
                fallCells.Add(cell);
            }
            cells[cell.X, cell.Y] = null;
            cell.SetGridPos(cell.X,cell.Y-1);
            cells[cell.X, cell.Y] = cell;
            cell.UpdateNeighbours(this);
            cell.UpdateAllNeighbours();
            Fill(cell.X);
            cell.transform.DOLocalMove(new Vector2(cell.transform.localPosition.x, cell.transform.localPosition.y - 0.9f),
                0.5f).OnComplete((() =>
            {
                
                if (cell.FirstCellBelow == null)
                { 
                    lastTween= Fall(cell,false);
                }
                if (cell.FirstCellUp != null)
                {
                    lastTween= Fall(cell.FirstCellUp,false);
                }
                else
                {
                    lastTween= Fill(cell.X);
                }
                lastTween = Fall(cell,false);
                
                if (FindMatch(new List<Cell>(){cell}))
                {
                    foreach (var threeMatch in threeMatches)
                    {
                        foreach (var cell1 in threeMatch) {
                            GameManager.instance.IncreaseScore();
                            cell1.PlayExplodeParticle();
                            DestroyImmediate(cell1.gameObject);
                        } 
                    } 
                }
                SlideDown();
            }));
            //cell.transform.localPosition = new Vector2(cell.transform.localPosition.x,cell.transform.localPosition.y-0.9f);
        }else 
        if (cell.FirstCellUp == null)
        { 
            lastTween=  Fill(cell.X);
        }
        if (cell.FirstCellUp != null)
        {
            lastTween= Fall(cell.FirstCellUp,false);
        }
        else if (canBeLast && cell.X == lastColIndex)
        {
            if (GameManager.instance.gameOver)
            {
                GameManager.instance.GameOver();
            }else
                InputManager.instance.OpenInput();
        }

       
        return lastTween;
    }

    public Tween Fill(int col)
    {
        
        Tween tween = null;
        if (cells[col, row-1] == null)
        {
            lastColIndex = col;
            var newCell = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, transform);
            if (GameManager.instance.score >= (100 * scoreMultiplier) && bombCell ==null)
            {
                scoreMultiplier++;
                newCell.bomb = true;
                bombCell = newCell;
            }
            cells[col, row-1] = newCell; 
            Vector2 targetPos = newCell.SetCellPosColor(col, row-1,true);
            newCell.transform.localPosition = new Vector2(targetPos.x, targetPos.y + 10f);
            List<Color> colorList = ColorsCanBe(newCell);
            newCell.SetColor(colorList[Random.Range(0,colorList.Count)]);
            newCell.UpdateAllNeighbours();
            newCell.transform.DOLocalMove(targetPos, 1f).OnComplete(() =>
            {
                tween = Fall(newCell,true);
                if (tween != null)
                {
                    tween.OnComplete(() =>
                    {
                        Fill(col);
                    });
                }
            });
        }

       
        return tween;
    }

    public void ChangeCell(Cell cell,int y)
    {
        cell.SetGridPos(cell.X,cell.Y-y);
        cells[cell.X, cell.Y-y] = cell;
    }


}
