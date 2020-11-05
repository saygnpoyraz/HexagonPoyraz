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
        bool matchFound = false;
        if (state == 0)
        {
            rightOrderRotate = new Cell[3];
            InputManager.instance.OpenInput();
            return;
        }else if (state == 3)
        {
           
            foreach (Cell cell in SelectedCells)
            {
                if (cell.FirstCellBelow != null && SelectedCells.Contains(cell.FirstCellBelow))
                {
                    rightOrderRotate[0] = cell;
                }

            }

            if (clockwise)
            {
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
            }
            else
            {
                if (SelectedCells.Contains(GetNeighbour(rightOrderRotate[0], Direction.DownRight)))
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
                foreach (var threeMatch in threeMatches)
                {
                    foreach (var cell in threeMatch)
                    {
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

    public void RotateClockwise()
    {
        
    }

    public void RotateCounterClockWise()
    {
        
    }
    
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
                if (Cells[i, j] != null)
                {
                    Tween tween = Fall(Cells[i, j]);
                    if (tween != null)
                    {
                        lastTween = tween;
                    }
                }
            }
        }

        lastTween.OnComplete((() =>
        {
            InputManager.instance.OpenInput();
            
            // EXPLODE MATHCES AFTER FALL !!!

            // if ( FindMatch(fallCells))
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
        if (GetNeighbour(cell,Direction.Down) == null && cell.Y != 0)
        {
            if (!fallCells.Contains(cell))
            {
                fallCells.Add(cell);
                Debug.Log(cell.name);
            }
            Cells[cell.X, cell.Y] = null;
            cell.SetGridPos(cell.X,cell.Y-1);
            Cells[cell.X, cell.Y] = cell;
            cell.UpdateNeighbours(this);
            cell.UpdateAllNeighbours();
            return cell.transform.DOLocalMove(new Vector2(cell.transform.localPosition.x, cell.transform.localPosition.y - 0.9f),
                0.5f).OnComplete((() =>
            {

                //Fall(Cells[cell.X, Row - 1]);
                if (cell.FirstCellUp != null)
                {
                    Fall(cell);
                    Fall(cell.FirstCellUp);
                }
                else
                {
                    Debug.Log("VAR" + cell.X);
                    Fill(cell.X);
                }
            }));
            //cell.transform.localPosition = new Vector2(cell.transform.localPosition.x,cell.transform.localPosition.y-0.9f);
        }else 
        if (cell.FirstCellUp == null)
        { 
            Fill(cell.X);
        }

        return null;
    }

    public void Fill(int col)
    {
        if (Cells[col, 8] == null)
        {
            var newCell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, transform);
            Cells[col, Row-1] = newCell; 
            newCell.SetCellPosColor(col, Row-1);
            List<Color> colorList = ColorsCanBe(newCell);
            newCell.SetColor(colorList[Random.Range(0,colorList.Count)]);
            newCell.UpdateAllNeighbours();
            Tween fallTween = Fall(newCell);
            if (fallTween != null)
            {
                fallTween.OnComplete(() => { Fill(col); });
            }
              
        }
    }

    public void ChangeCell(Cell cell,int y)
    {
        cell.SetGridPos(cell.X,cell.Y-y);
        Cells[cell.X, cell.Y-y] = cell;
    }

    public void UpdateAllRow(int col)
    {
        for (int i = 0; i < Row; i++)
        {
            if (Cells[col,i] == null)
                continue;
            Cells[col,i].UpdateNeighbours(this);
            //Cells[col,i].UpdateAllNeighbours();
        }
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
