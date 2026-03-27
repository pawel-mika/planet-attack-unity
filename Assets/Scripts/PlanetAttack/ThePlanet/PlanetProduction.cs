using System;
using PlanetAttack.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlanetAttack.ThePlanet
{
    public class PlanetProduction : MonoBehaviour
    {
        private float _ships = 0;
        public float Ships
        {
            get => _ships;
            set
            {
                _ships = value;
                OnShipsChanged?.Invoke(_ships);
            }
        }

        private float _minerals = 0;
        public float Minerals
        {
            get => _minerals;
            set
            {
                _minerals = value;
                OnMineralsChanged?.Invoke(_minerals);
            }
        }

        private float _food = 0;
        public float Food
        {
            get => _food;
            set
            {
                _food = value;
                OnFoodChanged?.Invoke(_food);
            }
        }

        public event Action<float> OnShipsChanged;
        public event Action<float> OnMineralsChanged;
        public event Action<float> OnFoodChanged;

        public float ShipsBonus { get; private set; }
        public float MineralsBonus { get; private set; }
        public float FoodBonus { get; private set; }
        public float ShipCostMinerals { get; private set; }
        public float ShipCostFood { get; private set; }

        public void Initialize(PlanetSettings settings, float maxFreeShips)
        {
            Ships = Random.Range(0, maxFreeShips);
            Minerals = Random.Range(0, 1024);
            Food = Random.Range(0, 1024);

            ShipsBonus = Random.Range(settings.MinShipsBonus, settings.MaxShipsBonus);
            MineralsBonus = Random.Range(settings.MinMineralsBonus, settings.MaxMineralsBonus);
            FoodBonus = Random.Range(settings.MinFoodBonus, settings.MaxFoodBonus);

            ShipCostMinerals = Random.Range(settings.MinShipCostMinerals, settings.MaxShipCostMinerals);
            ShipCostFood = Random.Range(settings.MinShipCostFood, settings.MaxShipCostFood);
        }

        public void ProduceResources()
        {
            Food += 1 * FoodBonus;
            Minerals += 1 * MineralsBonus;

            if (Minerals >= ShipCostMinerals && Food >= ShipCostFood)
            {
                Ships += (1 * ShipsBonus);
                Minerals -= ShipCostMinerals;
                Food -= ShipCostFood;
            }
        }
    }
}