using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
//using com.facebook.witai;

public class WitActivation : MonoBehaviour
{
   [SerializeField] private Wit wit;

   private void OnValidate()
   {
       if (!wit) wit = GetComponent<Wit>();
   }

   void Update()
   {
       if (Input.GetKeyDown(KeyCode.Space))
       {
           wit.Activate();
       }
   }
}
