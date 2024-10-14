using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float GetRandomFloatBetween(float from, float to, float step = 1) {
        System.Random r = new();
        double rand = r.NextDouble();
        rand = (to - from) * rand;
        rand += from;
        return (float)rand;
    }
}
