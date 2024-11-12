using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlanetAttack
{
    public class Utils
    {
        static double Fract(double t)
        {
            return t - Math.Floor(t);
        }

        public static double Sawtooth(double t, double period, double min, double max)
        {
            return min + (max - min) * Fract(t / period);
        }
    }
}
