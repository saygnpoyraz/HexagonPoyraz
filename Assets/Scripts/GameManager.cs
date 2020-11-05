using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Camera mainCamera;
    public List<Color> colors;
    public Board board;
    public float rotationDuration = 0.5f;
    public int score = 0;
    public int moveCount = 0;

    void Start()
    {
        instance = this;
        mainCamera = Camera.main;
        UIManager.instance.UpdateUI();
    }

    public void IncreaseScore()
    {
        DOTween.To(()=> score, x=> score = x,score+5, 0.25f).OnUpdate((() =>
        {
            UIManager.instance.UpdateUI();
        }));
    }

    public void IncreaseMoveCount()
    {
        moveCount++;
        UIManager.instance.UpdateUI();
    }
    
}
