using UnityEngine;
using XNode;

namespace ComponentsNodes
{
    public class RendererNode : ComponentNode<Renderer>
    {
        public override object GetValue(NodePort port)
        {
            return base.GetValue(port) as Renderer;
        }
    }
}