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

    public Action<string> OnCurveChanged = (v) => { };
    public Action<string> OnGradientChanged = (v) => { };

    public string CurveId = "";
    public CurveInstance Curve
    {
        get
        {
            return SessionsManipulator.Instance.Curves.GetCurve(CurveId);//DefaultResources.Settings.SizeCurves[CurveId];
        }
        set
        {
            CurveId = value.Id;
            OnCurveChanged(CurveId);
        }
    }

    public string gradientId = "";
    public GradientInstance Gradient
    {
        get
        {
            return SessionsManipulator.Instance.Gradients.GetGradient(gradientId);//DefaultResources.Settings.Gradients[gradientId];
        }
        set
        {
            gradientId = value.Id;
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

    private void GradientChanged(string g)
    {
            KineticFieldController.Instance.Visual.SetGradient("P" + Id + "Gradient".ToString(), SessionsManipulator.Instance.Gradients.GetGradient(g).Gradient);
    }

    private void CurveChanged(string v)
    {
            KineticFieldController.Instance.Visual.SetAnimationCurve("P" + Id + "Func", SessionsManipulator.Instance.Curves.GetCurve(v).Curve);
    }

    public object Clone()
    {
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