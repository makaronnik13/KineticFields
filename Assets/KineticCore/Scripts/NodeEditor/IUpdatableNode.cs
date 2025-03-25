using UnityEngine;

public interface IUpdatableNode
{
    public void UpdateNode();
}

public interface IUpdatableNode<T>
{
    public void UpdateNode(T value);
}

