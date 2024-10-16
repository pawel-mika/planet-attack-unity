using UnityEngine;

namespace PlanetAttack
{
    public class Utils
    {
        public static float GetRandomFloatBetween(float from, float to, float step = 1)
        {
            System.Random r = new();
            double rand = r.NextDouble();
            rand = (to - from) * rand;
            rand += from;
            return (float)rand;
        }

        public static Vector3 GetRandomVectorInRange(float fromX, float toX, float fromY, float toY)
        {
            return new Vector3(GetRandomFloatBetween(fromX, toX), GetRandomFloatBetween(fromY, toY));
        }
    }

}
