using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlanetAttack
{
    public class Utils
    {
        public static Vector3 GetRandomVectorInRange(float fromX, float toX, float fromY, float toY)
        {
            return new Vector3(Random.Range(fromX, toX), Random.Range(fromY, toY));
        }

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
