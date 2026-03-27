using UnityEngine;

namespace PlanetAttack.ThePlanet
{
    [CreateAssetMenu(fileName = "PlanetSettings", menuName = "PlanetAttack/PlanetSettings")]
    public class PlanetSettings : ScriptableObject
    {
        [Header("Production Settings")]
        public float MinShipsBonus = 1f;
        public float MaxShipsBonus = 3f;
        public float MinMineralsBonus = 1f;
        public float MaxMineralsBonus = 3f;
        public float MinFoodBonus = 1f;
        public float MaxFoodBonus = 3f;

        public float MinShipCostMinerals = 10f;
        public float MaxShipCostMinerals = 15f;
        public float MinShipCostFood = 10f;
        public float MaxShipCostFood = 15f;

        [Header("Visual Settings")]
        public float TextureScrollSpeed = 0.25f;
        public float RotationPerSec = 0.1f;
        public float DefaultHaloScale = 1.25f;
        public float BlinkSpeed = 1f;
        public float BlinkIntensity = 6f;
    }
}