using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class OpenXRControllerEvent : MonoBehaviour{
    public InputActionProperty action;
    public UnityEvent inputEvent;

    void OnEnable(){
        action.action.Enable();
        action.action.performed += (e) => { inputEvent?.Invoke(); };
    }

    void OnDisable(){
        action.action.performed -= (e) => { inputEvent?.Invoke(); };
    }
}
