using System;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Button", menuName = "InputSo/Button")]
public class ButtonSO : InputSO
{
    public InputAction Action;

    public Action OnPressed;
    
    public void Activate()
    {
        OnPressed?.Invoke();
    }
}