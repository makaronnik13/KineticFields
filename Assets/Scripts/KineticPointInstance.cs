using System;
using com.armatur.common.flags;
using UnityEngine;

[System.Serializable]
public class KineticPointInstance: ICloneable
{
    public int Id;
    public string Name;
    public GenericFlag<bool> Active = new GenericFlag<bool>("IsActive", false);
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

    [NonSerialized]
    private KineticFieldController kfController;

    private CurveInstance tempCurve;

    public CurveInstance TempCurve
    {
        get
        {
            if (tempCurve == null)
            {
                tempCurve = new CurveInstance(Curve.Curve);
            }
            return tempCurve;
        }
        set
        {
            tempCurve = value;
        }
    }


    public CurveInstance Curve
    {
        get
        {
            if (kfController == null)
            {
                kfController = KineticFieldController.Instance;
            }
            return kfController.Session.Value.Curves.GetCurve(CurveId);//DefaultResources.Settings.SizeCurves[CurveId];
        }
        set
        {
            CurveId = value.Id;
            OnCurveChanged(CurveId);
        }
    }

    public string gradientId = "";


    private GradientInstance tempGradient;

    public GradientInstance TempGradient
    {
        get
        {
            if (tempGradient == null)
            {
                tempGradient = new GradientInstance(Gradient.Gradient);
            }
            return tempGradient;
        }
        set
        {
            tempGradient = value;
        }
    }

    public GradientInstance Gradient
    {
        get
        {
            return KineticFieldController.Instance.Session.Value.Gradients.GetGradient(gradientId);//DefaultResources.Settings.Gradients[gradientId];
        }
        set
        {
            gradientId = value.Id;
            OnGradientChanged(gradientId);
        }
    }
    public bool ShowGradient
    {
        get
        {
            return Id == 0 || Id == 1 || Id == 6 || Id == 10;
        }
    }

    public KineticPointInstance()
    {
       
    }

    public KineticPointInstance(int id, string name, Vector3 pos)
    {
        Id = id;
        Name = name;
        Radius = new ModifyingParameter(0.3f, 0, 5f);
        Deep = new ModifyingParameter(2f, 0.3f, 4f);
        Volume = new ModifyingParameter(1, 0, 1);
        if (id == 0)
        {
            Active.SetState(true);
            Radius.SetValue(1);
        }


        Position = pos;

        Deep.Value.AddListener(DeepChanged);
    }

    public void Init()
    {
        Radius.Init();
        Volume.Init();
        Deep.Init();
        Deep.Value.AddListener(DeepChanged);
        OnGradientChanged = GradientChanged;
        GradientChanged(gradientId);
        OnCurveChanged = CurveChanged;
        CurveChanged(CurveId);
        if (Id == 0)
        {
            Active.SetState(true);
        }
    }

    private void DeepChanged(float v)
    {
        Position = new Vector3(Position.x, Position.y, v-1);
    }

    private void GradientChanged(string g)
    {
            //KineticFieldController.Instance.Visual.SetGradient("P" + Id + "Gradient".ToString(), SessionsManipulator.Instance.Gradients.GetGradient(g).Gradient);
    }

    private void CurveChanged(string v)
    {
            //KineticFieldController.Instance.Visual.SetAnimationCurve("P" + Id + "Func", SessionsManipulator.Instance.Curves.GetCurve(v).Curve);
    }

    public object Clone()
    {
        KineticPointInstance point = new KineticPointInstance(Id, Name, Position);
        point.Active.SetState(Active.Value);
        point.CurveId = CurveId;
        point.Deep = Deep.Clone() as ModifyingParameter;
        point.gradientId = gradientId;
        point.Radius = Radius.Clone() as ModifyingParameter;
        point.Volume = Volume.Clone() as ModifyingParameter;
        point.Position = Position;
        return point;
    }
}