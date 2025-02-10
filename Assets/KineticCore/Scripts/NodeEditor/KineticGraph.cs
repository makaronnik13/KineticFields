using KineticFields;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;


[CreateAssetMenu(menuName = "KineticGraph/New Kinetic Graph")]
public class KineticGraph : NodeGraph
{
    public List<Node> GetNodes<T>()
    {
        return nodes.Where(n => n.GetType() == typeof(T)).ToList();
    }
}