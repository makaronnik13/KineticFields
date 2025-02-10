using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Zenject;

namespace XNode {
	/// <summary> Lets you instantiate a node graph in the scene. This allows you to reference in-scene objects. </summary>
	public class SceneGraph : MonoBehaviour
    {
		public NodeGraph graph;

        public void StartGraph()
        {
            foreach (Node node in graph.nodes)
            {
                node.StartNode();
            }
        }
    }

	/// <summary> Derive from this class to create a SceneGraph with a specific graph type. </summary>
	/// <example>
	/// <code>
	/// public class MySceneGraph : SceneGraph<MyGraph> {
	///
	/// }
	/// </code>
	/// </example>
	public class SceneGraph<T> : SceneGraph where T : NodeGraph {
		public new T graph { get { return base.graph as T; } set { base.graph = value; } }
	}


    }