using UnityEngine;
using UnityEngine.VFX;
using XNode;

namespace ComponentsNodes
{
    public class VFXNode : ComponentNode<VisualEffect>
    {
        public override object GetValue(NodePort port)
        {
            return base.GetValue(port) as VisualEffect;
        }
    }
}