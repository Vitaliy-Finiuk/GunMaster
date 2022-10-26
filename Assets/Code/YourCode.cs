using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YourCode : MonoBehaviour
{
   public Image old;
   public Sprite[] im;
   public GameObject[] im2;
   public Dropdown myDD;
   public Text myDDText;
   private void Start()
   {
      throw new NotImplementedException();
   }

   public void Drop(Dropdown myDD)
   {
     
      
      old.sprite = im[myDD.value];
      myDDText.text = myDD.options [myDD.value].text;
   }
}