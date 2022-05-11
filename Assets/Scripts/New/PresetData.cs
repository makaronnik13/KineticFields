using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KineticFields
{
    [Serializable]
    public class PresetData
    {
        public string PresetName;
        public List<ParameterData> Parameters = new List<ParameterData>();

        public PresetData(string presetName, List<ParameterData> parameters)
        {
            PresetName = presetName;
            Parameters = parameters;
        }
    }
}
