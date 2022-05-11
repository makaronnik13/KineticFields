using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using Assets.WasapiAudio.Scripts.Wasapi;
using CSCore.CoreAudioAPI;
using Assets.WasapiAudio.Scripts.Unity;
using Zenject;
using UniRx;

namespace KineticFields
{
    public class FFTService: IDisposable, ITickable
    {
        public ReactiveProperty<MMDevice> CaptureDevice { get; private set; } = new ReactiveProperty<MMDevice>(null);
        public ReactiveProperty<List<float>> OutputSpectrum { get; private set; } = new ReactiveProperty<List<float>>();
        public ReactiveCommand OnBeat { get; private set; } = new ReactiveCommand();
        public float AudioScale = 10f;

        public int SpectrumSize => spectrumSize;

        private float bandWidth => (2f / spectrumSize) * (samplingRate / 2f);
        private long currentTimeMillis => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        private WasapiAudio _wasapiAudio;
        private float[] _spectrumData;
        private SpectrumSmoother smoother;
        private AudioVisualizationProfile profile;

        private int samplingRate = 44100;
        private int nBand = 12;
        private long lastT, nowT, diff, entries, sum;
        private float gThresh = 0.1f;
        private int blipDelayLen = 16;
        private int[] blipDelay;
        private int sinceLast = 0;
        private float framePeriod;
        private int colmax = 120;
        private float[] spectrum;
        private float[] averages;
        private float[] acVals;
        private float[] onsets;
        private float[] scorefun;
        private float[] dobeat;
        private int now = 0;
        private float[] spec;
        private int maxlag = 100;
        private float decay = 0.997f;
        private Autoco auco;
        private float alph;

        private readonly int spectrumSize = 512;
        private readonly int minFrequency = 50;
        private readonly int maxFrequency = 20000;

        private CompositeDisposable disposables = new CompositeDisposable();

        [Inject]
        public void Construct(AudioVisualizationProfile profile)
        {
            this.profile = profile;
            init();

            smoother = new SpectrumSmoother(SpectrumSize, profile.AudioSmoothingIterations);
            CaptureDevice.Subscribe(device=> { AudioDeviceChanged(device);}).AddTo(disposables);
        }

        public void TapTempo()
        {
            nowT = currentTimeMillis;
            diff = nowT - lastT;
            lastT = nowT;
            sum = sum + diff;
            entries++;
            int average = (int)(sum / entries);
        }

        private void init()
        {
            blipDelay = new int[blipDelayLen];
            onsets = new float[colmax];
            scorefun = new float[colmax];
            dobeat = new float[colmax];
            spectrum = new float[spectrumSize];
            averages = new float[12];
            acVals = new float[maxlag];
            alph = 100 * gThresh;
            framePeriod = (float)spectrumSize / samplingRate;
            //initialize record of previous spectrum
            spec = new float[nBand];
            for (int i = 0; i < nBand; ++i)
                spec[i] = 100.0f;
            auco = new Autoco(maxlag, decay, framePeriod, bandWidth);
            lastT = currentTimeMillis;
        }

        public void Tick()
        {

            smoother.AdvanceFrame();

            OutputSpectrum.Value = GetSpectrumData().Select(s => s * AudioScale).ToList();

            if (OutputSpectrum.Value.Average()<0.1f)
            {
                AudioScale = 10f;
            }

            AudioScale += 0.25f*Time.deltaTime;

            if (OutputSpectrum.Value.Max() > 1f)
            {
                AudioScale -= Time.deltaTime;
            }
            if (OutputSpectrum.Value.Average()>0.3f)
            {
                AudioScale -= Time.deltaTime;
            }

            computeAverages(OutputSpectrum.Value);

            /* calculate the value of the onset function in this frame */
            float onset = 0;
            for (int i = 0; i < nBand; i++)
            {
                float specVal = (float)System.Math.Max(-100.0f, 20.0f * (float)System.Math.Log10(averages[i]) + 160); // dB value of this band
                specVal *= 0.025f;
                float dbInc = specVal - spec[i]; // dB increment since last frame
                spec[i] = specVal; // record this frome to use next time around
                onset += dbInc; // onset function is the sum of dB increments
            }

            onsets[now] = onset;

            /* update autocorrelator and find peak lag = current tempo */
            auco.newVal(onset);
            // record largest value in (weighted) autocorrelation as it will be the tempo
            float aMax = 0.0f;
            int tempopd = 0;
            //float[] acVals = new float[maxlag];
            for (int i = 0; i < maxlag; ++i)
            {
                float acVal = (float)System.Math.Sqrt(auco.autoco(i));
                if (acVal > aMax)
                {
                    aMax = acVal;
                    tempopd = i;
                }
                // store in array backwards, so it displays right-to-left, in line with traces
                acVals[maxlag - 1 - i] = acVal;
            }

            /* calculate DP-ish function to update the best-score function */
            float smax = -999999;
            int smaxix = 0;
            // weight can be varied dynamically with the mouse
            alph = 100 * gThresh;
            // consider all possible preceding beat times from 0.5 to 2.0 x current tempo period
            for (int i = tempopd / 2; i < System.Math.Min(colmax, 2 * tempopd); ++i)
            {
                // objective function - this beat's cost + score to last beat + transition penalty
                float score = onset + scorefun[(now - i + colmax) % colmax] - alph * (float)System.Math.Pow(System.Math.Log((float)i / (float)tempopd), 2);
                // keep track of the best-scoring predecesor
                if (score > smax)
                {
                    smax = score;
                    smaxix = i;
                }
            }

            scorefun[now] = smax;
            // keep the smallest value in the score fn window as zero, by subtracing the min val
            float smin = scorefun[0];
            for (int i = 0; i < colmax; ++i)
                if (scorefun[i] < smin)
                    smin = scorefun[i];
            for (int i = 0; i < colmax; ++i)
                scorefun[i] -= smin;

            /* find the largest value in the score fn window, to decide if we emit a blip */
            smax = scorefun[0];
            smaxix = 0;
            for (int i = 0; i < colmax; ++i)
            {
                if (scorefun[i] > smax)
                {
                    smax = scorefun[i];
                    smaxix = i;
                }
            }

            // dobeat array records where we actally place beats
            dobeat[now] = 0;  // default is no beat this frame
            ++sinceLast;
            // if current value is largest in the array, probably means we're on a beat
            if (smaxix == now)
            {
                //tapTempo();
                // make sure the most recent beat wasn't too recently
                if (sinceLast > tempopd / 4)
                {
                    OnBeat.Execute();
                    blipDelay[0] = 1;
                    // record that we did actually mark a beat this frame
                    dobeat[now] = 1;
                    // reset counter of frames since last beat
                    sinceLast = 0;
                }
            }

            /* update column index (for ring buffer) */
            if (++now == colmax)
                now = 0;
        }

        private void AudioDeviceChanged(MMDevice microphone)
        {
            _wasapiAudio?.StopListen();
            WasapiCaptureType wct = WasapiCaptureType.Loopback;

            if (microphone!=null)
            {
                wct = WasapiCaptureType.Microphone;
            }

            _wasapiAudio = new WasapiAudio(wct, SpectrumSize, ScalingStrategy.Sqrt, minFrequency, maxFrequency, new WasapiAudioFilter[0], spectrumData =>
            {
                _spectrumData = spectrumData;
            });

            _wasapiAudio.StartListen(microphone);
        }

        private float[] GetSpectrumData()
        {

            var scaledSpectrumData = new float[SpectrumSize];
            var scaledMinMaxSpectrumData = new float[SpectrumSize];

            // Apply AudioVisualizationProfile
            var scaledMax = 0.0f;
            var scaledAverage = 0.0f;
            var scaledTotal = 0.0f;
            var scaleStep = 1.0f / SpectrumSize;


            // 2: Scaled. Scales against animation curve
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = profile.ScaleCurve.Evaluate(scaleStep * i) * _spectrumData[i];
                scaledSpectrumData[i] = scaledValue;

                if (scaledSpectrumData[i] > scaledMax)
                {
                    scaledMax = scaledSpectrumData[i];
                }

                scaledTotal += scaledValue;
            }

            // 3: MinMax
            scaledAverage = scaledTotal / SpectrumSize;
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = scaledSpectrumData[i];
                var cutoff = scaledAverage * profile.MinMaxThreshold;

                if (scaledValue <= cutoff)
                {
                    scaledValue *= profile.MinScale;
                }
                else if (scaledValue >= cutoff)
                {
                    scaledValue *= profile.MaxScale;
                }

                scaledMinMaxSpectrumData[i] = scaledValue;
            }

            // 4: Smoothed
            return smoother.GetSpectrumData(scaledSpectrumData);
        }

        private void computeAverages(List<float> data)
        {
            for (int i = 0; i < 12; i++)
            {
                float avg = 0;
                int lowFreq;
                if (i == 0)
                    lowFreq = 0;
                else
                    lowFreq = (int)((samplingRate / 2) / (float)Math.Pow(2, 12 - i));
                int hiFreq = (int)((samplingRate / 2) / (float)Math.Pow(2, 11 - i));
                int lowBound = freqToIndex(lowFreq);
                int hiBound = freqToIndex(hiFreq);
                for (int j = lowBound; j <= hiBound; j++)
                {
                    avg += data[j];
                }
                avg /= (hiBound - lowBound + 1);
                averages[i] = avg;
            }
        }

        private int freqToIndex(int freq)
        {
            // special case: freq is lower than the bandwidth of spectrum[0]
            if (freq < bandWidth / 2)
                return 0;
            // special case: freq is within the bandwidth of spectrum[512]
            if (freq > samplingRate / 2 - bandWidth / 2)
                return (spectrumSize / 2);
            // all other cases
            float fraction = (float)freq / (float)samplingRate;
            int i = (int)Math.Round(spectrumSize * fraction);
            return i;
        }

        public void Dispose()
        {
            _wasapiAudio.StopListen();
            _wasapiAudio = null;
            disposables.Dispose();
        }
    }
}
