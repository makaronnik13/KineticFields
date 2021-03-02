using System;
using com.armatur.common.flags;
using UnityEngine;
using UnityEngine.UI;

public class PresetsGrid : Singleton<PresetsGrid>
{
    [SerializeField]
    private GameObject DragView;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private Toggle ShowGridToggle;

    [SerializeField]
    private RectTransform Grid, PreviewsContent;

    [SerializeField]
    private Button AddPresetBtn, DuplicateBtn, DeleteBtn;

    [SerializeField]
    private GameObject PresetPanelPrefab, PresetGridPrefab;

    public GenericFlag<KineticPreset> DraggingPreset = new GenericFlag<KineticPreset>("DraggingPreset", null);
    public GenericFlag<KineticPreset> SelectedPreset = new GenericFlag<KineticPreset>("SelectedPreset", null);
    public GenericFlag<bool> GridShowing = new GenericFlag<bool>("ShowGrid", false);

    private void Start()
    {
        DraggingPreset.AddListener(DraggingPresetChanged);
        SelectedPreset.AddListener(SelectedPresetChanged);
        AddPresetBtn.onClick.AddListener(CreatePreset);
        ShowGridToggle.onValueChanged.AddListener(ShowPreviewStateChanged);
        DeleteBtn.onClick.AddListener(DeleteSelected);
        DuplicateBtn.onClick.AddListener(DuplicateSelected);
    }

    private void DuplicateSelected()
    {
        KineticPreset duplPreset = SelectedPreset.Value.Clone() as KineticPreset;
        KineticFieldController.Instance.Session.Value.Presets.Add(duplPreset);
        CreatePresetPanel(duplPreset);
    }

    private void DraggingPresetChanged(KineticPreset preset)
    {
        DragView.SetActive(preset!=null);
        DragView.GetComponent<PresetSquare>().Init(preset);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && SelectedPreset.Value!=null)
        {
            DeleteSelected();
        }

        DragView.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            DraggingPreset.SetState(null);
        }
    }

    private void DeleteSelected()
    {
        KineticPreset deletedPreset = SelectedPreset.Value;

     
        KineticFieldController.Instance.Session.Value.Presets.Remove(deletedPreset);

        Toggle();
        Toggle();
        SelectedPreset.SetState(null);
        UpdatePreview();
    }

    public void DropPreset(PresetSquare presetSquare)
    {
        if (DraggingPreset.Value==null)
        {
            return;
        }

      
        presetSquare.Init(DraggingPreset.Value);

        UpdatePreview();
    }

    private void SelectedPresetChanged(KineticPreset preset)
    {
        foreach (PresetPreviewPanel panel in PreviewsContent.GetComponentsInChildren<PresetPreviewPanel>())
        {
            panel.SetSelected(panel.Preset == preset);
        }

        DeleteBtn.gameObject.SetActive(preset!=null);
        DuplicateBtn.gameObject.SetActive(preset!=null);

        foreach (PresetSquare ps in Grid.GetComponentsInChildren<PresetSquare>())
        {
            ps.SetSelected(ps.Preset == preset && preset!=null);
        }
    }

    private void ShowPreviewStateChanged(bool v)
    {
        GridShowing.SetState(v);
    }

    private void CreatePreset()
    {
        KineticPreset newPreset = new KineticPreset("Preset_"+ KineticFieldController.Instance.Session.Value.Presets.Count);
        KineticFieldController.Instance.Session.Value.Presets.Add(newPreset);
        CreatePresetPanel(newPreset);
    }

    public void Toggle()
    {
        View.SetActive(!View.activeInHierarchy);
        if (View.activeInHierarchy)
        {
            foreach (KineticPreset preset in KineticFieldController.Instance.Session.Value.Presets)
            {
                CreatePresetPanel(preset);
            }

        }
        else
        {
            foreach (Transform t in Grid)
            {
                Destroy(t.gameObject);
            }
            foreach (Transform t in PreviewsContent)
            {
                if (t.gameObject!=AddPresetBtn.gameObject)
                {
                    Destroy(t.gameObject);
                }
            }
        }
    }

    private void CreatePresetRect(KineticPreset kineticPreset)
    {
        GameObject newRect = Instantiate(PresetGridPrefab);
        newRect.transform.SetParent(Grid);
        newRect.transform.localPosition = Vector3.zero;
        newRect.transform.localScale = Vector3.one;
        newRect.GetComponent<PresetSquare>().Init(kineticPreset);

        newRect.GetComponent<Image>().raycastTarget = true;

        newRect.AddComponent<Button>().onClick.AddListener(()=>
        {
            newRect.GetComponent<PresetSquare>().Init(null);
            UpdatePreview();
        });
    }

    private void CreatePresetPanel(KineticPreset preset)
    {
        GameObject newPanel = Instantiate(PresetPanelPrefab);
        newPanel.transform.SetParent(PreviewsContent);
        newPanel.transform.localPosition = Vector3.zero;
        newPanel.transform.localScale = Vector3.one;
        newPanel.GetComponent<PresetPreviewPanel>().Init(preset);

        KineticPreset pr = preset;

        newPanel.AddComponent<Button>().onClick.AddListener(()=>
        {
            SelectedPreset.SetState(pr);
        });

    }

    void UpdatePreview()
    {
        if (GridShowing.Value)
        {
            GridShowing.SetState(false);
            GridShowing.SetState(true);
        }
    }
}
