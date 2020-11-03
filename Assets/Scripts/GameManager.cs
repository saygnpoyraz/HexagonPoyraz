using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Color> Colors;
    public Board Board;
    
    void Start()
    {
        instance = this;
    }
    
}
