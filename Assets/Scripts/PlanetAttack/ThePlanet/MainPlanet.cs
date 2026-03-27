using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack.Enums;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace PlanetAttack.ThePlanet
{
    [RequireComponent(typeof(PlanetProduction), typeof(PlanetVisuals))]
    public class MainPlanet : MonoBehaviour
    {
        [Header("Settings")]
        public PlanetSettings Settings;

        [Header("UI References")]
        public TheLabel ShipsLabel;
        public TheLabel MineralsLabel;
        public TheLabel FoodLabel;
        public TheLabel DebugLabel;

        private PlanetProduction _production;
        private PlanetVisuals _visuals;
        public EPlanetState PlanetState { get; private set; } = EPlanetState.NONE;
        public EPlayerType PlanetOwner { get; private set; } = EPlayerType.NONE;
        public Vector3 DrawTarget;

        private const float Period = 1f;
        void Awake()
        {
            _production = GetComponent<PlanetProduction>();
            _visuals = GetComponent<PlanetVisuals>();

            _production.OnShipsChanged += (v) => ShipsLabel.LabelText = ((int)v).ToString();
            _production.OnMineralsChanged += (v) => MineralsLabel.LabelText = ((int)v).ToString();
            _production.OnFoodChanged += (v) => FoodLabel.LabelText = ((int)v).ToString();
        }

        void Start()
        {
            PGSolidPlanet pg = GetComponentInChildren<PGSolidPlanet>();
            pg.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            pg.RandomizePlanet(true);

            _visuals.Initialize(Settings);
            InitEmptyPlanetState();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                SetDebugMode(Debug.isDebugBuild && !DebugLabel.gameObject.activeInHierarchy);
            }

            _visuals.Rotate();
            if (PlanetState == EPlanetState.SELECTED)
            {
                _visuals.Blink(PlanetState);
            }

            if (Time.time > _nextActionTime)
            {
                _nextActionTime += Period;
                if (GameManager.GameController.GameState == EGameState.IN_GAME)
                {
                    _production.ProduceResources();
                }
            }

            _visuals.UpdateLine(DrawTarget);

            if (DebugLabel.gameObject.activeInHierarchy)
            {
                DebugLabel.LabelText = string.Format("{0} owner:{1}, state: {2}, {3}", name, PlanetOwner, PlanetState, _visuals.DebugString());
            }
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

        // Add these proxy properties so the rest of the code doesn't break
        public float Ships { get => _production.Ships; set => _production.Ships = value; }
        public float Minerals { get => _production.Minerals; set => _production.Minerals = value; }
        public float Food { get => _production.Food; set => _production.Food = value; }
        public float ShipsBonus => _production.ShipsBonus;
        public float MineralsBonus => _production.MineralsBonus;
        public float FoodBonus => _production.FoodBonus;
        public float ShipCostMinerals => _production.ShipCostMinerals;
        public float ShipCostFood => _production.ShipCostFood;

        private float _nextActionTime = 0.0f;

        public void InitEmptyPlanetState()
        {
            DrawTarget = transform.position;
            PlanetOwner = EPlayerType.NONE;
            PlanetState = EPlanetState.NONE;

            _production.Initialize(Settings, GameManager.GameController.AILevel.MaxFreePlanetShips);
            _visuals.UpdateStateVisuals(PlanetState, PlanetOwner);
        }

        public void SetPlanetState(EPlanetState state)
        {
            PlanetState = state;
            _visuals.UpdateStateVisuals(PlanetState, PlanetOwner);
        }

        public void SetPlanetOwner(EPlayerType type)
        {
            if (PlanetOwner != type)
            {
                _visuals.PlayExplosion(); // Trigger explosion on owner change
            }
            PlanetOwner = type;
            _visuals.UpdateStateVisuals(PlanetState, PlanetOwner);
        }

        public void RevertPreviousState()
        {
            SetPlanetState(EPlanetState.NONE);
        }

        public void SetDebugMode(bool isDebugMode)
        {
            if (DebugLabel)
            {
                DebugLabel.gameObject.SetActive(isDebugMode);
            }
        }

        public void PlayExplosion()
        {
            _visuals.PlayExplosion();
        }
    }
}

