using System.Collections.Generic;
using Enums;
using UnityEngine;

public class Cell : MonoBehaviour
{

   public Sprite bombSprite;
   public TextMesh bombCounter;
   public int bombCount = 7;
   
   public int X;
   public int Y;
   public Color color ;

   public ParticleSystem explodeParticle;
   public GameObject highlighted;
   public bool bomb;
   
   public List<Cell> Neighbours;

   public Cell FirstCellBelow;

   private float height = 0.9f; //0.45f * 2f;

   
   public Cell FirstCellUp;
   //private float width = 0.5f;//0.25f * 2f;

   public void SetCellPosColor(int x, int y)
   {
      if (bomb)
      {
         GetComponent<SpriteRenderer>().sprite = bombSprite;
         transform.GetChild(0).gameObject.SetActive(true);
         bombCounter.text = bombCount + "";
      } X = x;
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

   public Vector2 GetPos()
   {
      float offset = 0;
      if (X % 2 != 0)
      {
         offset = height / 2;
      }

      return new Vector2(X * 0.75f, Y * height - offset);
   }
   
   public Vector3 SetCellPosColor(int x, int y,bool isFill)
   {
      if (bomb)
      {
         GetComponent<SpriteRenderer>().sprite = bombSprite;
         transform.GetChild(0).gameObject.SetActive(true);
         bombCounter.text = bombCount + "";
      }
      X = x;
      Y = y;
      float offset = 0;
      if (x % 2 != 0)
      {
         offset = height / 2;
      }
      string cellName = x + ":" + y;
      gameObject.name = "Cell " + cellName;
      UpdateNeighbours(GameManager.instance.board);
      return new Vector3(x * 0.75f, y * height - offset);
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
      this.color = color;
      ParticleSystem.MainModule settings = explodeParticle.main;
      settings.startColor = new ParticleSystem.MinMaxGradient(this.color);      
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

   }

   public void Shine()
   {
      Color highligtedColor = this.color;
      highligtedColor.a = 0.5f;
      GetComponent<SpriteRenderer>().color = highligtedColor;
      //highlighted.SetActive(true);
   }
   

   public bool IsNeighbour(Cell cell)
   {
      return Neighbours.Contains(cell);
   }
   
   public void UnShine()
   {
      GetComponent<SpriteRenderer>().color = this.color;
      //highlighted.SetActive(false);
   }

   public bool IsEqual(Cell cell)
   {
      return X == cell.X && Y == cell.Y && this.color == cell.color;
   }

   public void DecreaseCounter()
   {
      bombCount--;
      bombCounter.text = bombCount + "";
      if (bombCount == 0)
      {
         GameManager.instance.gameOver = true;
      }
   }

   public void PlayExplodeParticle()
   {
      if (explodeParticle == null)
         return;
      GameManager.instance.SetParticleToParent(explodeParticle.transform);
      explodeParticle.Play();
   }
}
