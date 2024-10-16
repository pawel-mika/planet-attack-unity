using UnityEngine;

namespace PlanetAttack
{
    public class Utils
    {
        public static float GetRandomFloat(float from, float to)
        {
            System.Random r = new();
            double rand = r.NextDouble();
            rand = (to - from) * rand;
            rand += from;
            return (float)rand;
        }

        public static int GetRandomInt(int from, int to) {
            return (int)GetRandomFloat(from, to);
        }

        public static Vector3 GetRandomVectorInRange(float fromX, float toX, float fromY, float toY)
        {
            return new Vector3(GetRandomFloat(fromX, toX), GetRandomFloat(fromY, toY));
        }
    }

}
