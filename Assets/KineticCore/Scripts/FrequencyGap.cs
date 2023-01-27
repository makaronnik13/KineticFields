using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrequencyGap
{
    None = -1,
    SubBass = 0, //20 - 60hz
    Bass = 1, //60 - 250hz
    LowMidrange = 2, //250-500hz
    Midrange = 3, //500-2k hz
    UpperMidrange = 4, //2k - 4k hz
    Presence = 5 //4k - 15k hz
}
