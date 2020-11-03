using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Camera mainCamera;
    public List<Color> colors;
    public Board board;
    public float rotationDuration = 0.5f;
    
    void Start()
    {
        instance = this;
        mainCamera = Camera.main;
    }

}
