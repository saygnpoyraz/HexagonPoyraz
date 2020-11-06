using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int coloum = 8;
    public int row = 9;
    public Cell cellPrefab;
    public Camera mainCamera;
    public List<Color> colors;
    public Transform particleParent;
    public Board board;
    public float rotationDuration = 0.5f;
    public int score = 0;
    public int moveCount = 0;
    public bool gameOver;

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
    
    IEnumerator RestartScene()
    {
        DestroyImmediate(particleParent.gameObject);
        particleParent = new GameObject("Particle Parent").transform;
        board.GameOver();
        yield return new WaitForSeconds(1f);
        board.InitializeBoard();
    }
}
