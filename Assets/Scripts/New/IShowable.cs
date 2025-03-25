using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShowable
{
    public void Show();
    public void Hide();
}

public interface IShowable<T>
{
    public void Show(T parameter);
    public void Hide();
}