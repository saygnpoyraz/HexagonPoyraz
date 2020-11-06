using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int coloum = 8;
    public int row = 9;
    public int bombSpawnPoint = 1000;
    public Cell cellPrefab;
    public Camera mainCamera;
    public List<Color> colors;
    public Transform particleParent;
    public Board board;
    public float rotationDuration = 0.5f;
    
    private int score = 0;
    private int moveCount = 0;
    private bool gameOver;

    void Start()
    {
        gameOver = false;
        instance = this;
        mainCamera = Camera.main;
        board.InitializeBoard();
    }

    public void IncreaseScore()
    {
        score += 5;
        UIManager.instance.UpdateUI();

    }

    public void IncreaseMoveCount()
    {
        moveCount++;
        UIManager.instance.UpdateUI();
    }
    

    public void SetParticleToParent(Transform explodeParticleTransform)
    {
        foreach (Transform child in particleParent)
        {
            if (child.GetComponent<ParticleSystem>().isStopped)
            {
                Destroy(child.gameObject);
            }
        }
        explodeParticleTransform.SetParent(particleParent);
    }

    public void GameOver()
    {
        InputManager.instance.CloseInput();
        moveCount = 0;
        score = 0;
        UIManager.instance.UpdateUI();
        StartCoroutine(RestartScene());
    }

    public void RestartCor()
    {
        StartCoroutine(RestartScene());
    }
    
    IEnumerator RestartScene()
    {
        gameOver = false;
        DestroyImmediate(particleParent.gameObject);
        particleParent = new GameObject("Particle Parent").transform;
        board.GameOver();
        yield return new WaitForSeconds(1f);
        board.InitializeBoard();
    }

   
    public int GetScore()
    {
        return score;
    }
    public int GetMoveCount()
    {
        return moveCount;
    }
    
    public bool IsGameOver()
    {
        return gameOver;
    }
    public void SetGameOver(bool gameOver)
    {
        this.gameOver = gameOver;
    }
     
}
