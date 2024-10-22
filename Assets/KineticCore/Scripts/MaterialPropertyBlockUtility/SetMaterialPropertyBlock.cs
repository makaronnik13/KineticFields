using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetMaterialPropertyBlock : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] bool ExecuteInEditor = false;
#endif

    [SerializeField] Renderer m_renderer = null;
    [SerializeField] PropertyScriptableObject m_overrideSettings = null;
    [SerializeReference] List<IProperty> m_properties = new List<IProperty>();

    private MaterialPropertyBlock m_cachedPropBlock;


    public PropertyScriptableObject OverrideSettings {
        get {
            return m_overrideSettings;
        }
        set {
            m_overrideSettings = value;
        }
    }

    public List<IProperty> CurrentSettings => m_overrideSettings ? m_overrideSettings.m_properties : m_properties;

#if UNITY_EDITOR
    public void OnValidate() {
        GetRequired();
        if(ExecuteInEditor) {
            ApplyCurrentSetting();
        }
    }
#endif

    void Start() {
        GetRequired();
        ApplyCurrentSetting();
    }

    private void Update()
    {
        ApplyCurrentSetting();
    }

    void GetRequired()
    {
        if (!m_renderer) m_renderer = GetComponent<Renderer>();
        if (m_cachedPropBlock == null) m_cachedPropBlock = new MaterialPropertyBlock();
    }

    public void ApplyCurrentSetting()
    {
        List<IProperty> properties = CurrentSettings;

        if (m_renderer == null || properties.Count == 0) return;

        m_renderer.GetPropertyBlock(m_cachedPropBlock);
        foreach (var propValue in properties)
        {
            propValue.Set(m_cachedPropBlock);
        }
        m_renderer.SetPropertyBlock(m_cachedPropBlock);
    }

    public void ClearBlockData() {
        Renderer renderer = m_renderer;

        if(renderer == null) { return; }
        var prop = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(prop);
        prop.Clear();
        renderer.SetPropertyBlock(prop);
    }
}
