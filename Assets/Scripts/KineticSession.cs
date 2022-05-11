
using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KineticSession
{
    public CurvesStorage Curves;
    public GradientStorage Gradients;

    public List<FrequencyGap> Gaps = new List<FrequencyGap>();

    public List<Oscilator> Oscilators = new List<Oscilator>();

    public string SessionName;

    public GenericFlag<KineticPreset> ActivePreset = new GenericFlag<KineticPreset>("ActivePreset", null);

    public ModifyingParameter GeneralAnchor;
    public ModifyingParameter GeneralScale;

    public List<KineticPreset> Presets = new List<KineticPreset>();

    public KineticPreset GetPresetById(string presetId)
    {
        return Presets.FirstOrDefault(p => p.Id == presetId);
    }


    private KineticPreset averagePreset;

    public KineticPreset AveragePreset
    {
        get
        {
            if (averagePreset == null)
            {
                averagePreset = new KineticPreset("AveragePreset");
            }

            return averagePreset;
        }
        set
        {
            averagePreset = value;
        }
    }

    private KineticPreset mainPreset;
    public KineticPreset MainPreset
    {
        get
        {
            if (mainPreset == null)
            {
                mainPreset = new KineticPreset("EmptyPreset");
                mainPreset.Color = Color.white;
                mainPreset.Color2 = Color.white;
            }
            return mainPreset;
        }
    }

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
        SessionName = sessionName;
        Presets.Clear();
        KineticPreset preset = new KineticPreset("Preset_0");

        preset.Position = Vector2.up * 50f;

        Presets.Add(preset);


        ActivePreset.SetState(Presets[0]);

        AddGap("Fire", 0.1f, 0.3f, Color.red, DefaultResources.GapSprites[1]);
        AddGap("Air", 0.3f, 0.3f, Color.cyan, DefaultResources.GapSprites[2]);
        AddGap("Earth", 0.6f, 0.3f, Color.green, DefaultResources.GapSprites[3]);
        AddGap("Water", 0.9f, 0.3f, Color.blue, DefaultResources.GapSprites[4]);

        Gradients = new GradientStorage();
        foreach (Gradient gr in DefaultResources.Settings.Gradients)
        {
            Gradients.Gradients.Add(new GradientInstance(gr));
        }

        Curves = new CurvesStorage();

        Debug.Log(DefaultResources.Settings.SizeCurves.Count);

        foreach (AnimationCurve cu in DefaultResources.Settings.SizeCurves)
        {
            Curves.AddCurve(cu);
        }

        Debug.Log(Curves.Curves.Count);

        AddOscilator(3, 0, Curves.Curves[3].Id);
        AddOscilator(3, -1, Curves.Curves[13].Id);
        AddOscilator(3, -2, Curves.Curves[14].Id);
        AddOscilator(3, -2, Curves.Curves[22].Id);
        AddOscilator(3, -3, Curves.Curves[8].Id);
        AddOscilator(3, -3, Curves.Curves[11].Id);
        AddOscilator(3, -4, Curves.Curves[12].Id);

        GeneralAnchor = new ModifyingParameter(4f, 2f, 8f);
        GeneralScale = new ModifyingParameter(1f, 0.1f, 3f);
    }

    public void UpdateAveragePreset(Dictionary<KineticPreset, float> weihgts)
    {
  

        float weigthsSum = weihgts.Select(p => p.Value).Sum();

        if (weigthsSum == 0)
        {
            weigthsSum = 100000f;
        }

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

        //Debug.Log(cutPlane+"/"+nearPlane+"/"+lifetime+"/"+particlesCount+"/"+ meshesWeigth.OrderByDescending(p => p.Value).FirstOrDefault().Key);



        AveragePreset.FarCutPlane.Value.SetState(cutPlane);
        AveragePreset.NearCutPlane.Value.SetState(nearPlane);
        AveragePreset.Lifetime.Value.SetState(lifetime);
        AveragePreset.ParticlesCount.Value.SetState(particlesCount);

        if (meshesWeigth.Count>0)
        {
            AveragePreset.MeshId.SetState(meshesWeigth.OrderByDescending(p => p.Value).FirstOrDefault().Key);
        }
       


        for (int i = 0; i < 13; i++)
        {
            Dictionary<KineticPointInstance, float> pointsWeigths = new Dictionary<KineticPointInstance, float>();
            foreach (KeyValuePair<KineticPreset, float> pair in weihgts)
            {
                pointsWeigths.Add(pair.Key.Points.FirstOrDefault(p => p.Id == i), pair.Value);
            }

            InterpolatePoint(AveragePreset.Points.FirstOrDefault(p => p.Id == i), pointsWeigths);
        }

    }

    private void InterpolatePoint(KineticPointInstance averagePoint, Dictionary<KineticPointInstance, float> pointsWeigths)
    {
        if (pointsWeigths.Count == 0)
        {
            return;
        }

        List<KineticPointInstance> points = new List<KineticPointInstance>();
        List<int> values = new List<int>();


        for (int i = 0; i < pointsWeigths.Count; i++)
        {
            KeyValuePair<KineticPointInstance, float> pair = pointsWeigths.ToList<KeyValuePair<KineticPointInstance, float>>()[i];
            if (!pair.Key.Active.Value && pair.Key.Id != 0)
            {
                //pointsWeigths[pair.Key] = 0;
            } 
        }


        float weigthsSum = pointsWeigths.Select(p => p.Value).Sum();

        if (weigthsSum == 0)
        {
            weigthsSum = 10000f;
        }

        averagePoint.Active.SetState(true);

        AnimationCurve pointCurve = new AnimationCurve();

        for (int i = 0; i < 100; i++)
        {
            float averageValue = 0;
            foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
            {
                if (pair.Key.Active.Value)
                {
                    averageValue += pair.Value * pair.Key.Curve.Curve.Evaluate(i / 100f);
                }
            }
            averageValue /= weigthsSum;
            pointCurve.AddKey(i / 100f, averageValue);
        }

        if (averagePoint.TempCurve == null)
        {
            averagePoint.TempCurve = new CurveInstance(pointCurve);
        }
        else
        {
            averagePoint.TempCurve.Update(pointCurve);
        }

        float deep = 0;
        float x = 0;
        float y = 0;
        float radius = 0;
        float volume = 0;

        Gradient gradient = new Gradient();


        float weigth = pointsWeigths.FirstOrDefault().Value;

        // float mult = 1;

        if (averagePoint.ShowGradient)
        {
            List<GradientColorKey> keys = new List<GradientColorKey>();

            foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
            {

                foreach (GradientColorKey ck in pair.Key.Gradient.Gradient.colorKeys)
                {
                    List<GradientColorKey> colors = new List<GradientColorKey>();

                    foreach (KeyValuePair<KineticPointInstance, float> p in pointsWeigths)
                    {
                        if (p.Key.ShowGradient)
                        {
                            if (pair.Key.Active.Value)
                            {
                                colors.Add(new GradientColorKey(p.Key.Gradient.Gradient.Evaluate(ck.time), p.Value));
                            }
                        }
                    }

                    Color averageColor = InterpolateColor(colors);
                    keys.Add(new GradientColorKey(averageColor, ck.time));
                }
            }


            while (keys.Count > 8)
            {
                GradientColorKey k1 = keys[0];
                GradientColorKey k2 = keys[1];


                for (int i = 0; i < keys.Count; i++)
                {
                    for (int j = 0; j < keys.Count; j++)
                    {
                        if (i != j)
                        {
                            if (Mathf.Abs(k1.time - k2.time) > Mathf.Abs(keys[i].time - keys[j].time))
                            {
                                k1 = keys[i];
                                k2 = keys[j];
                            }
                        }
                    }
                }


                GradientColorKey average = new GradientColorKey(Color.Lerp(k1.color, k2.color, 0.5f), (k1.time + k2.time) / 2f);

                keys.Remove(k1);
                keys.Remove(k2);
                keys.Add(average);
            }
            

            gradient = pointsWeigths.OrderByDescending(v => v.Value).FirstOrDefault().Key.Gradient.Gradient;
            gradient.SetKeys(keys.ToArray().ToArray(), new GradientAlphaKey[2] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        }


        float summWithoutActive = pointsWeigths.Where(p=>p.Key.Active.Value).Select(p => p.Value).Sum();

        if (summWithoutActive == 0)
        {
            summWithoutActive = 10000f;
        }

        foreach (KeyValuePair<KineticPointInstance, float> pair in pointsWeigths)
        {
            if (pair.Key.Active.Value)
            {
                deep += pair.Value * pair.Key.Deep.Value.Value;
                x += pair.Value * pair.Key.Position.x;
                y += pair.Value * pair.Key.Position.y;
                radius += pair.Value * pair.Key.Radius.Value.Value;
                volume += pair.Value * pair.Key.Volume.Value.Value;
            }
        }


        deep /= summWithoutActive;



        x /= summWithoutActive;
        y /= summWithoutActive;


   

        radius /= summWithoutActive;


        volume /= summWithoutActive;


        averagePoint.Deep.Value.SetState(deep);
        averagePoint.Position = new Vector3(x, y, deep);


        averagePoint.Radius.Value.SetState(radius);


 
        averagePoint.Volume.Value.SetState(volume);

        if (averagePoint.ShowGradient)
        {
            averagePoint.TempGradient = new GradientInstance(gradient); // pointsWeigths.OrderByDescending(p => p.Value).FirstOrDefault().Key.Gradient;//
        }
       
    }

    private Color InterpolateColor(List<GradientColorKey> colors)
    {
        if (colors.Count == 1)
        {
            return colors[0].color;
        }

        float sum = colors.Select(p => p.time).Sum();


        if (sum == 0)
        {
            return Color.black;
        }
        float r = 0;
        foreach (GradientColorKey p in colors)
        {
            r += p.color.r * p.time;
        }
        r /= sum;

        float g = 0;
        foreach (GradientColorKey p in colors)
        {
            g += p.color.g * p.time;
        }
        g /= sum;

        float b = 0;
        foreach (GradientColorKey p in colors)
        {
            b += p.color.b * p.time;
        }
        b /= sum;

        return new Color(r,g,b);
    }

    private void AddOscilator(float multiplyer, int repeatRate, string curveId)
    {
        Oscilators.Add(new Oscilator("oscilator", null, Color.cyan, multiplyer, repeatRate, curveId));
    }

    public FrequencyGap AddGap(string name, float pos, float size, Color color, Sprite sprite)
    {
        FrequencyGap fg = new FrequencyGap(name, pos, size, color, sprite);
        Gaps.Add(fg);
        return fg;
    }

    public void LoadPreset(KineticPreset pr, Action onLoad)
    {
        if (pr == null)
        {
            Debug.Log("null preset loaded!");
            return;
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



        ActivePreset.SetState(pr);


        ActivePreset.Value.Init();

        ActivePreset.Value.NearCutPlane.Value.AddListener(NearCutPlaneChanged);
        ActivePreset.Value.FarCutPlane.Value.AddListener(FarCutPlaneChanged);
        ActivePreset.Value.Lifetime.Value.AddListener(LifetimeChanged);
        ActivePreset.Value.ParticlesCount.Value.AddListener(PCountChanged);

        ActivePreset.Value.MainPoint.Deep.Value.AddListener(GlobalSizeChanged);
        ActivePreset.Value.MainPoint.Radius.Value.AddListener(NoizeChanged);
        ActivePreset.Value.MeshId.AddListener(MeshChanged);


        onLoad.Invoke();
    }

    private void MeshChanged(int meshId)
    {
        //KineticFieldController.Instance.Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[meshId]);
    }

    private void NoizeChanged(float val)
    {
        //KineticFieldController.Instance.Visual.SetFloat("Noize", val);
    }

    private void GlobalSizeChanged(float val)
    {
        //KineticFieldController.Instance.Visual.SetFloat("Size", (0.05f + val - 1f) / 8f);
    }

    private void PCountChanged(float v)
    {
        //KineticFieldController.Instance.Visual.SetInt("Rate", Mathf.RoundToInt(v));
    }

    private void LifetimeChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("Lifetime", v);
        //KineticFieldController.Instance.Visual.SetInt("Rate", DefaultResources.Settings.GetCount(v));
    }

    private void NearCutPlaneChanged(float v)
    {
        //KineticFieldController.Instance.Visual.SetFloat("FrontCutPlane", v);
    }

    private void FarCutPlaneChanged(float v)
    {
        //KineticFieldController.Instance.Visual.SetFloat("BackCutPlane", v);
    }

    public void SavePreset(int i)
    {
        Debug.Log("save preset " + i);
        Presets[i] = ActivePreset.Value.Clone() as KineticPreset;
    }


}