using System.Collections.Generic;
using UnityEngine;

namespace  KineticNodes
{
    public class SceneGraphRegistry
    {
    
        private static Dictionary<string, Renderer> rendererMap = new Dictionary<string, Renderer>();

        public static void RegisterRenderer(string id, Renderer renderer) {
            if (!rendererMap.ContainsKey(id)) {
                rendererMap.Add(id, renderer);
            }
        }

        public static Renderer GetRendererByID(string id) {
            rendererMap.TryGetValue(id, out Renderer renderer);
            return renderer;
        }

        public static void ClearRegistry() {
            rendererMap.Clear();
        }
    }   
}
