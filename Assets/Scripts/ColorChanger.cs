using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
   private void SetColor(Transform transform, Color color)
   {
       transform.GetComponent<Renderer>().material.color = color;
   }

   public void UpdateColor(string[] values)
   {
        values[0] = "red";
        values[1] = "sphere";
        
       var colorString = values[0];
       var shapeString = values[1];
       var transcription = values[2];



        if (ColorUtility.TryParseHtmlString(colorString, out var color))
       {

            if (!string.IsNullOrEmpty(shapeString))
           {
               var shape = transform.Find(shapeString);
               if (shape) SetColor(shape, color);
               
               //debugging shape selector
               //print("shape was found" + shape);

           }
           else
           {
               for (int i = 0; i < transform.childCount; i++)
               {
                   SetColor(transform.GetChild(i), color);
               }
           }
       }
       //debugging transcription
       //print(transcription);
   }
}