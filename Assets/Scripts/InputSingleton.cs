using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputSingleton : MonoBehaviour
{

    public PlayerInput playerInput;

    public static InputSingleton Singleton { get; private set; }

    public void SetSingleton()
    {
        playerInput = GetComponent<PlayerInput>();  
        Singleton = this;
    }

    private void OnEnable()
    {
        if (Singleton == null)
        {
            SetSingleton();
        }
    }
}
