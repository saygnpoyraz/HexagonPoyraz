using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public static UIManager instance;

   public TextMeshProUGUI scoreText;
   public TextMeshProUGUI moveCounterText;


   private void Start()
   {
      instance = this;
   }

   public void UpdateUI()
   {
      scoreText.text = GameManager.instance.score + "";
      moveCounterText.text = GameManager.instance.moveCount + "";
   }
}
