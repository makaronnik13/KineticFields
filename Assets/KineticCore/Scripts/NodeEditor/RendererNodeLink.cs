using KineticNodes;
using UnityEngine;

    [ExecuteInEditMode]
    public class RendererNodeLink : MonoBehaviour {
        public string id;

        private void OnEnable() {
            if (!string.IsNullOrEmpty(id)) {
                SceneGraphRegistry.RegisterRenderer(id, GetComponent<Renderer>());
            }
        }

        private void OnDisable() {
            SceneGraphRegistry.ClearRegistry();
        }
    }

