using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using UnityEngine;

public class Cell : MonoBehaviour
{
   public int X;
   public int Y;
   public Color Color ;

   public int neighbourCOunt = 0;
   
   public List<GameObject> sides;

   public List<Cell> Neighbours;

   public Cell FirstCellBelow;

   private float height = 0.9f; //0.45f * 2f;

   public bool positionSetted = false;
   
   public Cell FirstCellUp;
   //private float width = 0.5f;//0.25f * 2f;

   public void SetCellPosColor(int x, int y)
   {
      positionSetted = true;
      X = x;
      Y = y;
      float offset = 0;
      if (x % 2 != 0)
      {
         offset = height / 2;
      }

      transform.localPosition = new Vector3(x * 0.75f, y * height - offset);
      string cellName = x + ":" + y;
      gameObject.name = "Cell " + cellName;
      UpdateNeighbours(GameManager.instance.board);
   }

   public void SetGridPos(int x, int y)
   {
      X = x;
      Y = y;
      string cellName = X + ":" + Y;
      gameObject.name = "Cell " + cellName;
   }
   

   public void SetColor(Color color)
   {
      Color = color;
      GetComponent<SpriteRenderer>().color = color;
   }

   public void UpdateAllNeighbours()
   {
      foreach (Cell cell in Neighbours)
      {
         cell.UpdateNeighbours(GameManager.instance.board);
      }
   }

   public void UpdateNeighbours(Board board)
   {
      Neighbours = new List<Cell>();
      var up = board.GetNeighbour(this, Direction.Up);
      var upRight = board.GetNeighbour(this, Direction.UpRight);
      var downRight = board.GetNeighbour(this, Direction.DownRight);
      var down = board.GetNeighbour(this, Direction.Down);
      var downLeft = board.GetNeighbour(this, Direction.DownLeft);
      var upLeft = board.GetNeighbour(this, Direction.UpLeft);

			
      if(up!=null) Neighbours.Add(up);
      if(upRight!=null) Neighbours.Add(upRight);
      if(downRight!=null) Neighbours.Add(downRight);
      if(down!=null) Neighbours.Add(down);
      if(downLeft!=null) Neighbours.Add(downLeft);
      if(upLeft!=null) Neighbours.Add(upLeft);

      if (down != null) FirstCellBelow = down;
      if (up != null) FirstCellUp = up;

      neighbourCOunt = Neighbours.Count;
   }

   public void ShineSide(Direction direction)
   {
      
   }

   public bool IsNeighbour(Cell cell)
   {
      return Neighbours.Contains(cell);
   }
   
   public void ResetColor()
   {
      GetComponent<SpriteRenderer>().color = Color;
   }

   public bool IsEqual(Cell cell)
   {
      return X == cell.X && Y == cell.Y && Color == cell.Color;
   }
   
}
