using KineticFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyGapEditor : MonoBehaviour
{
    [SerializeField]
    private GameObject PointsVisual2d, PointsVisual3d;


    [SerializeField]
    private float HideTime = 1.5f;

    private FFTService SpectrumBar;

    //public FrequencyGap activeGap =  null;
    [SerializeField]
    private GameObject GapVisualPrefab;

    private Coroutine hideCoroutine;

    private List<GameObject> gaps = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //KineticFieldController.Instance.ActiveGap.AddListener(ActiveGapChanged);

        /*
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        foreach (SpriteRenderer sr in PointsVisual2d.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        foreach (MeshRenderer mr in PointsVisual3d.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }*/
    }

    /*
    private void GroupChanged(FrequencyGap.GapGroup group)
    {
        foreach (Transform t in GapVisualPrefab.transform.parent)
        {
            t.gameObject.SetActive(false);
        }
        GapVisualPrefab.transform.parent.GetChild(1+(int)group).gameObject.SetActive(true);
    }
*/
    private void Update()
    {
        if (!PresetsLerper.Instance.Lerping.Value)
        {
            if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0 && Input.mouseScrollDelta.magnitude == 0 && !Input.anyKey)
            {
                if (hideCoroutine == null)
                {
                    // hideCoroutine = StartCoroutine(Hide(HideTime)); //hide ui
                }
            }
            else
            {
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                    hideCoroutine = null;
                    foreach (Transform t in transform)
                    {
                        t.gameObject.SetActive(true);
                    }
                    foreach (MeshRenderer mr in PointsVisual3d.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.enabled = true;
                    }
                    foreach (SpriteRenderer sr in PointsVisual2d.GetComponentsInChildren<SpriteRenderer>())
                    {
                        sr.enabled = true;
                    }
                    // FindObjectOfType<PointInspector>().TryShow();
                    // FindObjectOfType<MainPointInspector>().TryShow();
                }

            }
        }
    }

    private IEnumerator Hide(float hideTime)
    {
        yield return new WaitForSeconds(hideTime);
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        foreach (SpriteRenderer sr in PointsVisual2d.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        foreach (MeshRenderer mr in PointsVisual3d.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }

        FindObjectOfType<PointInspector>().Hide();
        FindObjectOfType<MainPointInspector>().Hide();
        //KineticFieldController.Instance.ActivePoint.SetState(null);
    }

    private void Multiplyerchanged(string s)
    {
        //activeGap.Multiplyer.SetState(float.Parse(s));
    }

    private void SizeSliderValueChanged(float v)
    {
        //activeGap.GapSize.SetState(v);
    }

    public void AddGap(FrequencyGap fg)
    {
        GameObject newGapSlider = Instantiate(GapVisualPrefab);
        newGapSlider.transform.SetParent(GapVisualPrefab.transform.parent);
        //newGapSlider.GetComponent<FrequencyGapSlider>().Init(fg);
        newGapSlider.SetActive(true);
        newGapSlider.GetComponent<RectTransform>().offsetMax = GapVisualPrefab.GetComponent<RectTransform>().offsetMax;
        newGapSlider.GetComponent<RectTransform>().offsetMin = GapVisualPrefab.GetComponent<RectTransform>().offsetMin;
        gaps.Add(newGapSlider);
    }

    private void SliderValueChanged(float v)
    {
        //activeGap.Position.SetState(v);
    }

    private void ActiveGapChanged(FrequencyGap gap)
    {
        /*
        if  (activeGap!=null)
        {
            //activeGap.Position.RemoveListener(GapChanged);
            //activeGap.GapSize.RemoveListener(GapChanged);
        }

        activeGap = gap;

        if (activeGap!= null)
        {
            //activeGap.Position.AddListener(GapChanged);
            //activeGap.GapSize.AddListener(GapChanged);
        }
        else
        {
            ColoriseSpectrum(0,0, Color.gray);
        }
        */
    }

    public void Init(KineticSession preset)
    {
        foreach (GameObject gap in gaps)
        {
            Destroy(gap);
        }

        foreach (FrequencyGap gap in preset.Gaps)
        {
            AddGap(gap);
        }
    }

    private void GapChanged(float v)
    {
        //ColoriseSpectrum(activeGap.Start, activeGap.End, activeGap.color);
    }

    private void ColoriseSpectrum(float start, float end, Color color)
    {
        int i = 0;


        List<MeshRenderer> renderers = new List<MeshRenderer>();// SpectrumBar.GetComponentsInChildren<MeshRenderer>().ToList();
        int startPos = Mathf.RoundToInt(renderers.Count * start);
        int endPos = Mathf.RoundToInt(renderers.Count * end);

        foreach (MeshRenderer mr in renderers)
        {
            if (i>startPos && i<endPos)
            {
                mr.material.SetColor("_UnlitColor", color);
            }
            else
            {
                mr.material.SetColor("_UnlitColor", Color.gray);
            }
            i++;
        }
    }

}
