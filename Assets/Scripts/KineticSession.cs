
using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KineticSession
{
    public List<FrequencyGap> Gaps = new List<FrequencyGap>();

    public List<Oscilator> Oscilators = new List<Oscilator>();

    public string SessionName;

    public GenericFlag<KineticPreset> ActivePreset = new GenericFlag<KineticPreset>("ActivePreset", null);

    public string[,] PresetsGrid = new string[8, 8];

    public SpectrumShot[,] SpectrumShots = new SpectrumShot[8, 8];    

    public GenericFlag<SerializedVector2Int> SelectedPresetPos = new GenericFlag<SerializedVector2Int>("SelectedPresetId", new SerializedVector2Int(0,0));

    public List<KineticPreset> Presets = new List<KineticPreset>();

    public KineticPreset GetPresetById(string presetId)
    {
        return Presets.FirstOrDefault(p => p.Id == presetId);
    }

    public KineticPreset GetPresetByPosition(Vector2Int pos)
    {
        string id = PresetsGrid[pos.x, pos.y];

        if (id == null)
        {
            return null;
        }

        return GetPresetById(id);

    }

    [NonSerialized]
    public KineticPreset AveragePreset;

    public KineticSession()
    {
       
    }

    public void Init()
    {
        foreach (KineticPreset preset in Presets)
        {
            preset.Init();
        }

        AveragePreset = new KineticPreset("AveragePreset");
    }

    public KineticSession(string sessionName)
    {
        Debug.Log("Create session");
        SessionName = sessionName;
        Presets.Clear();
        Presets.Add(new KineticPreset("Preset_0"));

        PresetsGrid[0, 0] = Presets[0].Id;

        ActivePreset.SetState(Presets[0]);

        AddGap("Fire", 0.1f, 0.3f, Color.red, DefaultResources.GapSprites[1]);
        AddGap("Air", 0.3f, 0.3f, Color.cyan, DefaultResources.GapSprites[2]);
        AddGap("Earth", 0.6f, 0.3f, Color.green, DefaultResources.GapSprites[3]);
        AddGap("Water", 0.9f, 0.3f, Color.blue, DefaultResources.GapSprites[4]);

        AddOscilator(1, 0);
        AddOscilator(2, 0);
        AddOscilator(1, 1);
        AddOscilator(1, 1);
        AddOscilator(1, 2);
        AddOscilator(1, 3);
        AddOscilator(1, 4);
    }

    public void UpdateAveragePreset(Dictionary<KineticPreset, float> weihgts)
    {
        float weigthsSum = weihgts.Select(p=>p.Value).Sum();
        Dictionary<int, float> meshesWeigth = new Dictionary<int, float>();

        float lifetime = 0f;
        float cutPlane = 0f;
        float nearPlane = 0f;
        float particlesCount = 0f;

        foreach (KeyValuePair<KineticPreset, float> weigthpair in weihgts)
        {
            cutPlane += weigthpair.Value * weigthpair.Key.FarCutPlane.Value.Value;
            lifetime += weigthpair.Value * weigthpair.Key.Lifetime.Value.Value;
            nearPlane += weigthpair.Value * weigthpair.Key.NearCutPlane.Value.Value;
            particlesCount += weigthpair.Value * weigthpair.Key.ParticlesCount.Value.Value;

            if (!meshesWeigth.ContainsKey(weigthpair.Key.MeshId.Value))
            {
                meshesWeigth.Add(weigthpair.Key.MeshId.Value, 0);
            }
            meshesWeigth[weigthpair.Key.MeshId.Value] += weigthpair.Value;

        }

        cutPlane /= weigthsSum;
        nearPlane /= weigthsSum;
        lifetime /= weigthsSum;
        particlesCount /= weigthsSum;


        AveragePreset.FarCutPlane.SetValue(cutPlane);
        AveragePreset.NearCutPlane.SetValue(nearPlane);
        AveragePreset.Lifetime.SetValue(lifetime);
        AveragePreset.ParticlesCount.SetValue(particlesCount);     
        AveragePreset.MeshId.SetState(meshesWeigth.OrderByDescending(p => p.Value).FirstOrDefault().Key);


        for (int i = 0; i < 13; i++)
        {
            Dictionary<KineticPointInstance, float> pointsWeigths = new Dictionary<KineticPointInstance, float>();
            foreach (KeyValuePair<KineticPreset, float> pair in weihgts)
            {
                pointsWeigths.Add(pair.Key.Points[i], pair.Value);
            }

            InterpolatePoint(AveragePreset.Points[i], pointsWeigths);
        }


    }

    private void InterpolatePoint(KineticPointInstance averagePoint, Dictionary<KineticPointInstance, float> pointsWeigths)
    {
        List<KineticPointInstance> points = new List<KineticPointInstance>();
        List<int> values = new List<int>();

        foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
        {
            if (!pair.Key.Active.Value)
            {
                pointsWeigths[pair.Key] = 0;
            }
        }

      
        float weigthsSum = pointsWeigths.Select(p => p.Value).Sum();

        averagePoint.Active.SetState(true);

        AnimationCurve pointCurve = new AnimationCurve();

        for (int i = 0; i < 100; i++)
        {
            float averageValue = 0;
            foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
            {
                averageValue += pair.Value * pair.Key.Curve.Curve.Evaluate(i/100f);
            }
            averageValue /= weigthsSum;
            pointCurve.AddKey(i/100f, averageValue);
        }

        averagePoint.Curve = new CurveInstance(pointCurve);

        float deep = 0;
        float x = 0;
        float y = 0;
        float radius = 0;
        float volume = 0;

        Gradient gradient = pointsWeigths.FirstOrDefault().Key.Gradient.Gradient;
        float weigth = pointsWeigths.FirstOrDefault().Value;

        float mult = 1;

        foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
        {
            float w = weigth *= mult;

            if (w>pair.Value)
            {
                gradient = StaticTools.Lerp(gradient, pair.Key.Gradient.Gradient, pair.Value / w);
            }
            else
            {
                gradient = StaticTools.Lerp(pair.Key.Gradient.Gradient, gradient, w/ pair.Value);
            }

            weigth = (weigth + pair.Value)/2f;

            mult += weigth;
        }


        foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
        {
            deep += pair.Value * pair.Key.Deep.Value.Value;
            x += pair.Value * pair.Key.Position.x;
            y += pair.Value * pair.Key.Position.y;
            radius += pair.Value * pair.Key.Radius.Value.Value;
            volume += pair.Value * pair.Key.Volume.Value.Value;
        }

        deep /= weigthsSum;
        x /= weigthsSum;
        y /= weigthsSum;
        radius /= weigthsSum;
        volume /= weigthsSum;

        averagePoint.Deep.SetValue(deep);
        averagePoint.Position = new Vector3(x,y,deep);
        averagePoint.Radius.SetValue(radius);
        averagePoint.Volume.SetValue(volume);
        averagePoint.Gradient = new GradientInstance(gradient);
    }

    private void AddOscilator(float multiplyer, int repeatRate)
    {
        Oscilators.Add(new Oscilator(multiplyer, repeatRate));
    }

    public FrequencyGap AddGap(string name, float pos, float size, Color color, Sprite sprite)
    {
        FrequencyGap fg = new FrequencyGap(name, pos, size, color, sprite);
        Gaps.Add(fg);
        return fg;
    }

    public void LoadPreset(SerializedVector2Int pos)
    {
        Debug.Log(pos.x+"/"+pos.y);

        KineticPreset pr = GetPresetByPosition(new Vector2Int(pos.y, pos.x));
        if (pr == null)
        {
            Debug.Log("null preset loaded!");
           // return;
            ///pr = Presets.FirstOrDefault();
        }

        if (ActivePreset.Value != null)
        {
            ActivePreset.Value.NearCutPlane.Value.RemoveListener(NearCutPlaneChanged);
            ActivePreset.Value.FarCutPlane.Value.RemoveListener(FarCutPlaneChanged);
            ActivePreset.Value.Lifetime.Value.RemoveListener(LifetimeChanged);
            ActivePreset.Value.ParticlesCount.Value.RemoveListener(PCountChanged);
            ActivePreset.Value.MainPoint.Deep.Value.RemoveListener(GlobalSizeChanged);
            ActivePreset.Value.MainPoint.Radius.Value.RemoveListener(NoizeChanged);
            ActivePreset.Value.MeshId.RemoveListener(MeshChanged);
        }

        SelectedPresetPos.SetState(pos);


        ActivePreset.SetState(pr);


        ActivePreset.Value.Init();

        ActivePreset.Value.NearCutPlane.Value.AddListener(NearCutPlaneChanged);
        ActivePreset.Value.FarCutPlane.Value.AddListener(FarCutPlaneChanged);
        ActivePreset.Value.Lifetime.Value.AddListener(LifetimeChanged);
        ActivePreset.Value.ParticlesCount.Value.AddListener(PCountChanged);

        ActivePreset.Value.MainPoint.Deep.Value.AddListener(GlobalSizeChanged);
        ActivePreset.Value.MainPoint.Radius.Value.AddListener(NoizeChanged);
        ActivePreset.Value.MeshId.AddListener(MeshChanged);


        MainPointInspector.Instance.PresetChanged(ActivePreset.Value);
    }

    private void MeshChanged(int meshId)
    {
        KineticFieldController.Instance.Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[meshId]);
    }

    private void NoizeChanged(float val)
    {
        KineticFieldController.Instance.Visual.SetFloat("Noize", val);
    }

    private void GlobalSizeChanged(float val)
    {
        KineticFieldController.Instance.Visual.SetFloat("Size", (0.05f + val - 1f) / 8f);
    }

    private void PCountChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetInt("Rate", Mathf.RoundToInt(v));
    }

    private void LifetimeChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("Lifetime", v);
        //KineticFieldController.Instance.Visual.SetInt("Rate", DefaultResources.Settings.GetCount(v));
    }

    private void NearCutPlaneChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("FrontCutPlane", v);
    }

    private void FarCutPlaneChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("BackCutPlane", v);
    }

    public void SavePreset(int i)
    {
        Debug.Log("save preset " + i);
        Presets[i] = ActivePreset.Value.Clone() as KineticPreset;
    }


}