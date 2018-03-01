using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeasureInformation {

    public enum MeasureType
    {
        Global = 0,
        Partial = 1,
        Local = 2
    }

    private static MeasureType _measureType;

    public static MeasureType measureType
    {
        get { return _measureType; }
        set { _measureType = value; }
    }
}
