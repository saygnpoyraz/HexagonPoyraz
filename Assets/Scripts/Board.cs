using System;
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
    public int Coloum = 8;
    public int Row = 9;


    public Cell CellPrefab;

    public Cell[,] Cells;

    public GameObject SelectedCellParent;

    private List<Cell> SelectedCells = new List<Cell>();

    private List<Color> AllColors;

    private List<List<Cell>> threeMatches;

    private List<Cell> fallCells = new List<Cell>();
    
    Cell[] rightOrderRotate = new Cell[3];


    private void Start()
    {
        Cells = new Cell[Coloum, Row];
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
    
    public bool CheckBoardIsFull()
    {
        bool full = true;
        for (int i = 0; i < Coloum; i++)
        {
            if (Cells[i, Row-1] == null)
            {
                full = false;
            }
        }

        return full;
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
                Cells[i, j].SetColor(colorList[Random.Range(0, colorList.Count)]);

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

                if (cell.Color == cell.Neighbours[i].Color && neighbour)
                {
                    threeMatch.Add(cell.Neighbours[i]);
                }

                if (threeMatch.Count == 3)
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


    // public bool FindMatches(Cell cell)
    // {
    //     List<Cell> MatchCells = new List<Cell>() {cell};
    //     for (int j = 0; j < cell.Neighbours.Count; j++)
    //     {
    //         var neighbour = true;
    //         foreach (var matchCell in MatchCells)
    //         {
    //             if (!matchCell.IsNeighbour(cell.Neighbours[j]))
    //             {
    //                 neighbour = false;
    //                 break;
    //             }
    //         }
    //         if (cell.Color == cell.Neighbours[j].Color &&  neighbour) 
    //         {
    //             MatchCells.Add(cell.Neighbours[j]);
    //         }
    //     }
    //     List<int> indexs = new List<int>();
    //     if (MatchCells.Count == 3)
    //     {
    //         foreach (Cell mCell in MatchCells)
    //         {
    //             if (!indexs.Contains(mCell.X))
    //             {
    //                 indexs.Add(mCell.X);
    //             }
    //             DestroyImmediate(mCell.gameObject);
    //         }
    //
    //         SelectedCells = new List<Cell>();
    //     }
    //
    //     foreach (int index in indexs)
    //     {
    //         MoveDownAllColoum(index);
    //     }
    //    
    //     return MatchCells.Count == 3;
    // }

    public void MoveDownAllColoum(int coloumIndex)
    {
        int counter = 0;
        for (int i = 0; i < Row; i++)
        {
            if (Cells[coloumIndex, i] == null)
            {
                counter++;
            }
            else if (counter != 0)
            {
                // for (int j = 0; j < counter; j++)
                // {
                //     Debug.Log("1");
                //     var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
                //     Cells[coloumIndex, Row-1] = cell;
                //     Debug.Log("2");
                //
                //     cell.SetCellPosColor(coloumIndex,Row-1 );
                //     Debug.Log("3");
                //
                //     //MoveDown( Cells[coloumIndex, Row-1],counter);
                // }

                //List<Color> colorList = ColorsCanBe(Cells[i, j]);
                //Cells[i, j].SetColor(colorList[Random.Range(0,colorList.Count)]);
                ChangeCell(Cells[coloumIndex, i], counter);
                MoveDown(Cells[coloumIndex, i], counter);
            }
            else
            {
                //Debug.Log(Cells[coloumIndex,i].name);

            }
        }
    }


    public void MoveDown(Cell cell, int numberOfRow)
    {
        if (cell == null)
            return;

        cell.transform.DOLocalMove(
            new Vector2(cell.transform.localPosition.x, cell.transform.localPosition.y - (0.9f * numberOfRow)),
            0.5f).OnComplete((() =>
        {
            InputManager.instance.OpenInput();
        }));

    }

    public void CellPressed(Cell cell, Vector2 pointPressed)
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
            for (int j = 0; j < neighbours.Count; j++)
            {
                if (AllColors[i] == neighbours[j].Color)
                {
                    counter++;
                }

                if (counter > 1)
                {
                    colorsCanBeUse.Remove(colorsCanBeUse.Find(x => x == neighbours[j].Color));
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
        return new Vector2(x, y);
    }

    public void AddSelectedCellToParent()
    {
        foreach (Cell cell in SelectedCells)
        {
            cell.transform.parent = SelectedCellParent.transform;
        }
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
            foreach (Cell cell in SelectedCells)
            {
                if (cell.FirstCellBelow != null && SelectedCells.Contains(cell.FirstCellBelow))
                {
                    rightOrderRotate[0] = cell;
                }

            }

            if (clockwise)
            {
                if (GetNeighbour(rightOrderRotate[0], Direction.DownRight)!=null && SelectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
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
                if (GetNeighbour(rightOrderRotate[0], Direction.DownRight)!=null && SelectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
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
            
            
            
            if (!FindMatch(rightOrderRotate.ToList()))
            {
                RotateSelectedCells(clockwise, state - 1);
            }
            else
            {
                GameManager.instance.IncreaseMoveCount();
                rightOrderRotate = new Cell[3];
                foreach (var threeMatch in threeMatches)
                {
                    foreach (var cell in threeMatch)
                    {
                        GameManager.instance.IncreaseScore();
                        DestroyImmediate(cell.gameObject);
                    }
                }



                SlideDown();
                SelectedCells = new List<Cell>();

                // create new cell


                //InputManager.instance.OpenInput();
            }
            
        });


        ResetSelectedCells();
    }

    // public void FallTheCols(List<int> cols)
    // {
    //     for (int i = 0; i < cols.Count; i++) 
    //     { 
    //         for (int j = 1; j < Row; j++) 
    //         { 
    //             if (Cells[cols[i],j] != null && Cells[cols[i],j].FirstCellBelow == null)
    //             {
    //                 FallV2(cols[i],j);
    //                 // Cell upCell = Cells[cols[i], j].FirstCellUp;
    //                 // Cell mainCell = Cells[cols[i], j];
    //                 // Cells[mainCell.X, mainCell.Y] = null;
    //                 // mainCell.SetGridPos(mainCell.X,mainCell.Y-1);
    //                 // Cells[mainCell.X, mainCell.Y] = mainCell;
    //                 // upCell.UpdateNeighbours(this);
    //                 // mainCell.UpdateNeighbours(this);
    //                 // mainCell.UpdateAllNeighbours();
    //                 // mainCell.transform.DOLocalMove(new Vector2(mainCell.transform.localPosition.x,
    //                 //     mainCell.transform.localPosition.y - 0.9f),0.5f).OnComplete((() =>
    //                 // {
    //                 //     
    //                 // }));
    //             }
    //         }
    //     }
    // }
    //
    // public void FallV2(int x, int y)
    // {
    //     Cell upCell = Cells[x, y].FirstCellUp;
    //     Cell mainCell = Cells[x, y];
    //     Cells[mainCell.X, mainCell.Y] = null;
    //     mainCell.SetGridPos(mainCell.X,mainCell.Y-1);
    //     Cells[mainCell.X, mainCell.Y] = mainCell;
    //     mainCell.UpdateAllNeighbours();
    //     mainCell.UpdateNeighbours(this);
    //     mainCell.transform.DOLocalMove(new Vector2(mainCell.transform.localPosition.x,
    //         mainCell.transform.localPosition.y - 0.9f),0.5f).OnComplete((() =>
    //     {
    //         if (mainCell.FirstCellBelow == null)
    //         {
    //             FallV2(mainCell.X,mainCell.Y);
    //         }
    //
    //         if (upCell.FirstCellBelow == null)
    //         {
    //             Debug.Log("VAR");
    //             FallV2(upCell.X,upCell.Y);
    //         }
    //     }));
    // }

    
    public void FillBlanks()
    {
        for (int i = 0; i < Coloum; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                if (Cells[i, j] == null)
                {
                    var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
                    Cells[i, j] = cell;  
                }
            }
        }
        
        for (int i = 0; i < Coloum; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                if (!Cells[i, j].positionSetted)
                {
                    Cells[i, j].SetCellPosColor(i, j);
                    List<Color> colorList = ColorsCanBe(Cells[i, j]);
                    Cells[i, j].SetColor(colorList[Random.Range(0,colorList.Count)]);
                    Cells[i, j].UpdateAllNeighbours();
                }
            }
        }
    }


    public void SlideDown()
    {
        Tween lastTween = null;
        for (int i = 0; i < Coloum; i++)
        {
            for (int j = 1; j < Row; j++)
            {
                if (j == Row-1 && Cells[i, j] == null)
                {
                    Tween tween = Fill(i);
                    if (tween != null)
                    {
                        lastTween = tween;
                    }
                }
                else if (Cells[i, j] != null)
                {
                    Tween tween = Fall(Cells[i, j]);
                    if (tween != null)
                    {
                        lastTween = tween;
                    }
                }
            }
        }
        InputManager.instance.OpenInput();
        lastTween.OnComplete((() =>
        {
            InputManager.instance.OpenInput();
            //EXPLODE MATHCES AFTER FALL !!!

            
            
            // if (FindMatch(fallCells))
            // {
            //     foreach (var threeMatch in threeMatches)
            //     {
            //         foreach (var cell in threeMatch) {
            //             DestroyImmediate(cell.gameObject);
            //         }
            //     }
            //     SlideDown();
            // }
            // fallCells = new List<Cell>();


        }));


    }
        // find matches after fall
        // for (int i = 0; i < Coloum; i++)
        // {
        //     for (int j = 1; j < Row; j++)
        //     {
        //         if (Cells[i , j] != null)
        //         {
        //             if (FindMatch(new List<Cell>(){Cells[i,j]}))
        //             {
        //                 foreach (var threeMatch in threeMatches)
        //                 {
        //                     foreach (var cell in threeMatch)
        //                     {
        //                         DestroyImmediate(cell.gameObject);
        //                     }
        //                 }
        //
        //                 SlideDown();
        //             }
        //         }
        //     }
        // }
    

    public Tween Fall(Cell cell)
    {
        Tween lastTween = null;
        if (GetNeighbour(cell,Direction.Down) == null && cell.Y != 0 && !DOTween.IsTweening(cell.transform))
        {
            if (!fallCells.Contains(cell))
            {
                fallCells.Add(cell);
            }
            Cells[cell.X, cell.Y] = null;
            cell.SetGridPos(cell.X,cell.Y-1);
            Cells[cell.X, cell.Y] = cell;
            cell.UpdateNeighbours(this);
            cell.UpdateAllNeighbours();
            Fill(cell.X);
            cell.transform.DOLocalMove(new Vector2(cell.transform.localPosition.x, cell.transform.localPosition.y - 0.9f),
                0.5f).OnComplete((() =>
            {
                
                if (cell.FirstCellBelow == null)
                { 
                    lastTween= Fall(cell);
                }
                //Fall(Cells[cell.X, Row - 1]);
                if (cell.FirstCellUp != null)
                {
                    lastTween= Fall(cell.FirstCellUp);
                }
                else
                {
                    lastTween= Fill(cell.X);
                }
                lastTween = Fall(cell);
                
                if (FindMatch(new List<Cell>(){cell}))
                {
                    foreach (var threeMatch in threeMatches)
                    {
                        foreach (var cell1 in threeMatch) {
                            GameManager.instance.IncreaseScore();
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
            lastTween= Fall(cell.FirstCellUp);
        }
        
        return lastTween;
    }

    public Tween Fill(int col)
    {
        Tween tween = null;
        if (Cells[col, Row-1] == null)
        {
            var newCell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
            Cells[col, Row-1] = newCell; 
            Vector2 targetPos = newCell.SetCellPosColor(col, Row-1,true);
            newCell.transform.localPosition = new Vector2(targetPos.x, targetPos.y + 10f);
            List<Color> colorList = ColorsCanBe(newCell);
            newCell.SetColor(colorList[Random.Range(0,colorList.Count)]);
            newCell.UpdateAllNeighbours();
            newCell.transform.DOLocalMove(targetPos, 1f).OnComplete(() =>
            {
                tween = Fall(newCell);
                if (tween != null)
                {
                    tween.OnComplete(() => { Fill(col); });
                }
            });
        }

       
        return tween;
    }

    public void ChangeCell(Cell cell,int y)
    {
        cell.SetGridPos(cell.X,cell.Y-y);
        Cells[cell.X, cell.Y-y] = cell;
    }
    

    // public void Fall()
    // {
    //     for (int i = 0; i < Coloum; i++)
    //     {
    //         for (int j = 0; j < Row; j++)
    //         {
    //             if (Cells[i,j].FirstCellBelow == null)
    //             {
    //                 MoveDown(Cells[i,j]);
    //             }
    //         }
    //     }
    // }
    
  
}
