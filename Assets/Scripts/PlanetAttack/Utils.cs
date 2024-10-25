using UnityEngine;

namespace PlanetAttack
{
    public class Utils
    {
        public static Vector3 GetRandomVectorInRange(float fromX, float toX, float fromY, float toY)
        {
            return new Vector3(Random.Range(fromX, toX), Random.Range(fromY, toY));
        }
    }

}
