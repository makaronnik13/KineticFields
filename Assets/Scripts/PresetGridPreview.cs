using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetGridPreview : MonoBehaviour
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    private GameObject RectPrefab;

    [SerializeField]
    private RectTransform Hub;

    private KineticSession session;

    private void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
        PresetsGrid.Instance.GridShowing.AddListener(SetState);
    }


    private void SessionChanged(KineticSession session)
    {
        if (this.session!=null)
        {
            session.SelectedPresetPos.RemoveListener(PositionChanged);
        }
        this.session = session;
        if (this.session!=null)
        {
            session.SelectedPresetPos.AddListener(PositionChanged);
        }

    }

    private void PositionChanged(SerializedVector2Int pos)
    {
        foreach (PresetSquare rect in Hub.GetComponentsInChildren<PresetSquare>())
        {
            int y = rect.transform.GetSiblingIndex() / KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(0);
            int x = rect.transform.GetSiblingIndex() % KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(0);
            rect.SetSelected(pos.x == y && pos.y == x);
        }
    }

    public void Toggle()
    {

        SetState(!View.activeInHierarchy);
    }

    public void SetState(bool v)
    {
        View.SetActive(v);
        if (View.activeInHierarchy)
        {
            for (int i = 0; i < KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(0); i++)
            {
                for (int j = 0; j < KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(1); j++)
                {
                    CreatePresetRect(KineticFieldController.Instance.Session.Value.GetPresetByPosition(new Vector2Int(j, i)));
                }
            }
        }
        else
        {
            foreach (Transform t in Hub)
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void CreatePresetRect(KineticPreset kineticPreset)
    {
        GameObject newRect = Instantiate(RectPrefab);
        newRect.transform.SetParent(Hub);
        newRect.transform.localPosition = Vector3.zero;
        newRect.transform.localScale = Vector3.one;
        newRect.GetComponent<PresetSquare>().Init(kineticPreset);

        newRect.GetComponent<Image>().raycastTarget = true;

        newRect.AddComponent<Button>().onClick.AddListener(() =>
        {
            int y = newRect.transform.GetSiblingIndex() / KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(0);
            int x = newRect.transform.GetSiblingIndex() % KineticFieldController.Instance.Session.Value.PresetsGrid.GetLength(0);

            if (KineticFieldController.Instance.Session.Value.PresetsGrid[x,y]!=string.Empty)
            {
                KineticFieldController.Instance.LoadPreset(new SerializedVector2Int(y, x));
            }
        });

        newRect.GetComponent<Button>().targetGraphic = newRect.transform.GetChild(0).GetComponent<Image>();
    }
}
