using KineticFields;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;


[CreateAssetMenu(menuName = "KineticGraph/New Kinetic Graph")]
public class KineticGraph : NodeGraph
{
    public List<T> GetNodes<T>() where T : Node
    {
        return nodes.OfType<T>().ToList();
    }
}