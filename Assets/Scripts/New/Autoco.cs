namespace KineticFields
{
    public class Autoco
    {
        private int del_length;
        private float decay;
        private float[] delays;
        private float[] outputs;
        private int indx;

        private float[] bpms;
        private float[] rweight;
        private float wmidbpm = 120f;
        private float woctavewidth;

        public Autoco(int len, float alpha, float framePeriod, float bandwidth)
        {
            woctavewidth = bandwidth;
            decay = alpha;
            del_length = len;
            delays = new float[del_length];
            outputs = new float[del_length];
            indx = 0;

            // calculate a log-lag gaussian weighting function, to prefer tempi around 120 bpm
            bpms = new float[del_length];
            rweight = new float[del_length];
            for (int i = 0; i < del_length; ++i)
            {
                bpms[i] = 60.0f / (framePeriod * ((float)i + 1));
                //Debug.Log(bpms[i]);
                // weighting is Gaussian on log-BPM axis, centered at wmidbpm, SD = woctavewidth octaves
                rweight[i] = (float)System.Math.Exp(-0.5f * System.Math.Pow(System.Math.Log(bpms[i] / wmidbpm) / System.Math.Log(2.0f) / woctavewidth, 2.0f));
            }
        }

        public void newVal(float val)
        {

            delays[indx] = val;

            // update running autocorrelator values
            for (int i = 0; i < del_length; ++i)
            {
                int delix = (indx - i + del_length) % del_length;
                outputs[i] += (1 - decay) * (delays[indx] * delays[delix] - outputs[i]);
            }

            if (++indx == del_length)
                indx = 0;
        }

        // read back the current autocorrelator value at a particular lag
        public float autoco(int del)
        {
            float blah = rweight[del] * outputs[del];
            return blah;
        }

        public float avgBpm()
        {
            float sum = 0;
            for (int i = 0; i < bpms.Length; ++i)
            {
                sum += bpms[i];
            }
            return sum / del_length;
        }
    }
}