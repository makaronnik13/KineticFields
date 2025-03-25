using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using XNode;


namespace InputNode
{

    [NodeWidth(300)]
    [NodeTint(0.1f, 0.4f, 0.2f)]
    public class OscillatorNode : Node, IDisposable
    {
        public enum  BeatDivision
        {
            OneSixtyFourth,
            OneThirtySecond,
            OneSixteenth,
            OneEighth,
            OneFourth,
            OneHalf,
            One,
            Two,
            Four,
            Eight,
            Sixteen,
            ThirtyTwo,
            SixtyFour
        }

        [SerializeField] private AnimationCurve curve;
        public AnimationCurve Curve => curve;

        public enum PlayMode
        {
            None,
            Loop,
            Bounce
        }
        [SerializeField] public PlayMode mode = PlayMode.Loop;
        public bool random = false;


        [SerializeField] public BeatDivision beatDivision = BeatDivision.One;
        
        [SerializeField] public float randomInterval = 1f;
        [SerializeField] public int randomDistance = 0;

     
        private float time = 0;
        private bool direction = true;
        private float nextRandomTime = 0;
        
        [Output] public float outputValue;

        private IDisposable _disposables;
        private float _lastBpm;
        private float _normalizedTime = 0.5f;

        public float NormalizedTime => _normalizedTime;
        
        public override object GetValue(NodePort port)
        {
            return outputValue;
        }
        
        public void UpdateNode(float bpm)
        {
            
            outputValue = curve.Evaluate(time);
        }

        
        public void SetBpm(int bpm, bool restart = false)
        {
            Debug.Log("Set bpm "+bpm);
            if (restart)
            {
                nextRandomTime = 0;   
            }
            
            _disposables?.Dispose();
    _disposables = Observable.EveryUpdate().Subscribe(_ =>
    {
        // Calculate the time per beat based on the BPM and BeatDivision
        float secondsPerBeat = 60f / bpm;
        float normalizedSpeed = secondsPerBeat / GetNormalizedTimeForBeatDivision();

        // Adjust _normalizedTime based on the mode
        if (mode == PlayMode.Loop)
        {
            // In Loop mode, _normalizedTime increases steadily
            _normalizedTime += normalizedSpeed * Time.deltaTime;

            if (_normalizedTime >= 1f)
            {
                _normalizedTime = 0f; // Loop back to 0
            }
        }
        else if (mode == PlayMode.Bounce)
        {
            // In Bounce mode, _normalizedTime increases and decreases
            if (direction)
            {
                _normalizedTime += normalizedSpeed * Time.deltaTime;    
            }
            else
            {
                _normalizedTime -= normalizedSpeed * Time.deltaTime; 
            }

            if (_normalizedTime >= 1f)
            {
                direction = false;
            }
            else if (_normalizedTime <= 0f)
            {
                direction = true;
            }
        }

        // If random is enabled, adjust _normalizedTime based on the interval and distance
        if (random && (int)beatDivision>=(int)BeatDivision.One)
        {
            //Debug.Log(Time.time+"/"+nextRandomTime);
            if (Time.time >= nextRandomTime)
            {
                float randomSpeed = secondsPerBeat * randomInterval;
                
                int randomOffsetInBeats = UnityEngine.Random.Range(-randomDistance, randomDistance+1);

                _normalizedTime += (float)randomOffsetInBeats / (GetNormalizedTimeForBeatDivision() * 4f);

                _normalizedTime = Mathf.Repeat(_normalizedTime, 1f);

                
            //    Debug.Log(normalizedSpeed);
                
                // Устанавливаем следующее время для рандома на основе интервала
                nextRandomTime = Time.time + randomSpeed;
            }
        }

        // Update output value based on the curve and _normalizedTime
        outputValue = curve.Evaluate(_normalizedTime);
    });
}

// Helper method to get the normalized time speed based on BeatDivision
private float GetNormalizedTimeForBeatDivision()
{
    switch (beatDivision)
    {
        case BeatDivision.OneSixtyFourth: return 1f / 256f;
        case BeatDivision.OneThirtySecond: return 1f / 128f;
        case BeatDivision.OneSixteenth: return 1f / 64f;
        case BeatDivision.OneEighth: return 1f / 32f;
        case BeatDivision.OneFourth: return 1f / 16f;
        case BeatDivision.OneHalf: return 1f / 8f;
        case BeatDivision.One: return 1f/4f;
        case BeatDivision.Two: return 1f/2f;
        case BeatDivision.Four: return 1f;
        case BeatDivision.Eight: return 2f;
        case BeatDivision.Sixteen: return 4f;
        case BeatDivision.ThirtyTwo: return 8f;
        case BeatDivision.SixtyFour: return 16f;
        default: return 1f;
    }
}


// Helper method to get the total number of ticks for BeatDivision
private int GetBeatDivisionInTicks()
{
    switch (beatDivision)
    {
        case BeatDivision.OneSixtyFourth: return 64;
        case BeatDivision.OneThirtySecond: return 32;
        case BeatDivision.OneSixteenth: return 16;
        case BeatDivision.OneEighth: return 8;
        case BeatDivision.OneFourth: return 4;
        case BeatDivision.OneHalf: return 2;
        case BeatDivision.One: return 1;
        case BeatDivision.Two: return 1;
        case BeatDivision.Four: return 1;
        case BeatDivision.Eight: return 1;
        case BeatDivision.Sixteen: return 1;
        case BeatDivision.ThirtyTwo: return 1;
        case BeatDivision.SixtyFour: return 1;
        default: return 1;
    }
}

        public void Dispose()
        {
            _disposables?.Dispose();
        }
        
      
    }
}