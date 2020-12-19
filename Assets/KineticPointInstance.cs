using System;
using com.armatur.common.flags;
using UnityEngine;

[System.Serializable]
public class KineticPointInstance: ICloneable
{
    public int Id;
    public string Name;
    public GenericFlag<bool> Active = new GenericFlag<bool>("IsActive", false);
    public ModifyingParameter Speed;
    public ModifyingParameter Radius;
    public ModifyingParameter Volume;
    public ModifyingParameter Deep;

    [SerializeField]
    private float[] position = new float[3];

    public Vector3 Position
    {
        get
        {
            return new Vector3(position[0], position[1], Deep.Value.Value);
        }
        set
        {
            position[0] = value.x;
            position[1] = value.y;
        }
    }

    public Action<int> OnCurveChanged = (v) => { };
    public Action<int> OnGradientChanged = (v) => { };

    public int CurveId = 0;
    public AnimationCurve Curve
    {
        get
        {
            return DefaultResources.Settings.SizeCurves[CurveId];
        }
        set
        {
            CurveId = DefaultResources.Settings.SizeCurves.IndexOf(value);
            OnCurveChanged(CurveId);
        }
    }

    public int gradientId = 0;
    public Gradient Gradient
    {
        get
        {
            return DefaultResources.Settings.Gradients[gradientId];
        }
        set
        {
            gradientId = DefaultResources.Settings.Gradients.IndexOf(value);
            OnGradientChanged(gradientId);
        }
    }
    public bool ShowGradient = false;

    public KineticPointInstance()
    {
       
    }

    public KineticPointInstance(int id, string name)
    {
        Id = id;
        Name = name;
        Speed = new ModifyingParameter(0f, 0, 1);
        Radius = new ModifyingParameter(0.3f, 0, 5f);
        Deep = new ModifyingParameter(2f, 0.3f, 4f);
        Volume = new ModifyingParameter(1, 0, 1);
        if (id == 0)
        {
            Active.SetState(true);
            Radius.SetValue(1);
        }

        if (id == 1 || id == 6 || id == 10)
        {
            ShowGradient = true;
        }

        CurveId = 2;
        Deep.Value.AddListener(DeepChanged);
    }

    public void Init()
    {
        Speed.Init();
        Radius.Init();
        Volume.Init();
        Deep.Init();
        Deep.Value.AddListener(DeepChanged);
        OnGradientChanged = GradientChanged;
        GradientChanged(gradientId);
        OnCurveChanged = CurveChanged;
        CurveChanged(CurveId);
    }

    private void DeepChanged(float v)
    {
        Position = new Vector3(Position.x, Position.y, v-1);
    }

    private void GradientChanged(int g)
    {
            KineticFieldController.Instance.Visual.SetGradient("P" + Id + "Gradient".ToString(), DefaultResources.Settings.Gradients[g]);
    }

    private void CurveChanged(int v)
    {
            KineticFieldController.Instance.Visual.SetAnimationCurve("P" + Id + "Func", DefaultResources.Settings.SizeCurves[v]);
    }

    public object Clone()
    {
        Debug.Log("clone point");  
        KineticPointInstance point = new KineticPointInstance(Id, Name);
        point.Active.SetState(Active.Value);
        point.CurveId = CurveId;
        point.Deep = Deep.Clone() as ModifyingParameter;
        point.gradientId = gradientId;
        point.Radius = Radius.Clone() as ModifyingParameter;
        point.ShowGradient = ShowGradient;
        point.Speed = Speed.Clone() as ModifyingParameter;
        point.Volume = Volume.Clone() as ModifyingParameter;
        point.Position = Position;
        return point;
    }
}