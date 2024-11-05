using System;
using System.Collections.Generic;
using PlanetAttack.Enums;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace PlanetAttack.ThePlanet
{
    public class MainPlanet : MonoBehaviour
    {
        private readonly GameController GameController = GameManager.GameController;
        
        private float rotationPerSec = 0.05f;
        public float RotationPerSec
        {
            get { return rotationPerSec; }
            set
            {
                rotationPerSec = value;
                rotationSpeed = rotationPerSec * 360f;
            }
        }
        private float rotationSpeed = 1f * 360f;

        private float ships = 0;
        public float Ships
        {
            get { return ships; }
            set
            {
                ships = value;
                ShipsLabel.LabelText = String.Format("{0}", (int)ships);
            }
        }

        private float minerals = 0;
        public float Minerals
        {
            get { return minerals; }
            set
            {
                minerals = value;
                MineralsLabel.LabelText = String.Format("{0}", (int)minerals);
            }
        }

        private float food = 0;
        public float Food
        {
            get { return food; }
            set
            {
                food = value;
                FoodLabel.LabelText = String.Format("{0}", (int)food);
            }
        }

        public GameObject Planet;

        public GameObject PlayerPlanetHalo;
        public GameObject EnemyPlanetHalo;
        public GameObject TargetPlanetMarker;

        public TheLabel ShipsLabel;
        public TheLabel MineralsLabel;
        public TheLabel FoodLabel;

        private float ShipsBonus = 0;
        private float MineralsBonus = 0;
        private float FoodBonus = 0;

        private float ShipCostMinerals = 0;
        private float ShipCostFood = 0;

        private float nextActionTime = 0.0f;
        public float period = 1f;

        public EPlanetState PlanetState = EPlanetState.NONE;
        private EPlanetState PreviousState = EPlanetState.NONE;
        public EPlayerType PlanetOwner = EPlayerType.NONE;
        private EPlayerType PreviousOwner = EPlayerType.NONE;

        public Vector3 DrawTarget;

        // Start is called before the first frame update
        void Start()
        {
            PGSolidPlanet pgSolidPlanet = Planet.GetComponent<PGSolidPlanet>();
            pgSolidPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            pgSolidPlanet.RandomizePlanet(true);
            RotationPerSec = 0.1f;

            InitEmptyPlanetState();
        }

        // Update is called once per frame
        void Update()
        {
            Planet.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
            // CheckClicked();

            if (PlanetState == EPlanetState.SELECTED)
            {
                BlinkPlayerHalo();  // separate later perhaps to 2 cases of ownerrs?
                BlinkEnemyHalo();
            }

            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                if (GameController.GameState == EGameState.IN_GAME)
                {
                    // recalc every 1s or later get game level settings into account? (faster for higher level?) dunno yet
                    HarvestFood();
                    MineMinerals();
                    ManufactureShips();
                }
            }

            DrawLineToTarget();
        }

        private void OnDrawGizmos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.magenta);
        }

        private void BlinkPlayerHalo()
        {
            BlinkTheHalo(PlayerPlanetHalo);
        }

        private void BlinkEnemyHalo()
        {
            BlinkTheHalo(EnemyPlanetHalo);
        }

        private void BlinkTheHalo(GameObject go)
        {
            Material mp = go.GetComponent<Renderer>().material;
            float cp = 1f + (float)(Math.Sin(Utils.Sawtooth(Time.time, 1, 0f, 1f)) * 6f);
            mp.SetFloat("_Falloff", cp);

            Transform tPlayer = go.transform;
            float scale = 1f + (float)Math.Sin(Utils.Sawtooth(Time.time, 1, 0.25f, 1.25f));
            tPlayer.localScale = new Vector3(scale, scale, scale);
        }

        private void ResetHalo(GameObject go)
        {
            Material mp = go.GetComponent<Renderer>().material;
            mp.SetFloat("_Falloff", 1);

            Transform tPlayer = go.transform;
            tPlayer.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        }

        private void HarvestFood()
        {
            Food += 1 * FoodBonus;
        }

        private void MineMinerals()
        {
            Minerals += 1 * MineralsBonus;
        }

        private void ManufactureShips()
        {
            if (Minerals > ShipCostMinerals && Food > ShipCostFood)
            {
                Ships += (int)(1 * ShipsBonus);
                Minerals -= ShipCostMinerals;
                Food -= ShipCostFood;
            }
        }

        public void InitEmptyPlanetState()
        {
            DrawTarget = transform.position;
            PlanetOwner = EPlayerType.NONE;
            PlanetState = EPlanetState.NONE;
            PreviousOwner = EPlayerType.NONE;
            PreviousState = EPlanetState.NONE;

            PlayerPlanetHalo.SetActive(false);
            EnemyPlanetHalo.SetActive(false);
            TargetPlanetMarker.SetActive(false);

            Ships = Random.Range(0, 128);
            Minerals = Random.Range(0, 512);
            Food = Random.Range(0, 512);

            ShipsBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating
            MineralsBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating
            FoodBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating

            ShipCostMinerals = Random.Range(10f, 15f);
            ShipCostFood = Random.Range(10f, 15f);
        }

        public void RevertPreviousState() {
            SetPlanetState(PreviousState);
        }

        public void SetPlanetState(EPlanetState state)
        {
            PreviousState = PlanetState;
            PlanetState = state;
            RecalcStateAndOwnerChanges();
            Debug.Log(String.Format("Set '{0}' state to: {1}", name, Enum.GetName(typeof(EPlanetState), state)));
        }

        public void SetPlanetOwner(EPlayerType type)
        {
            PlanetOwner = type;
            RecalcStateAndOwnerChanges();
            Debug.Log(String.Format("Set '{0}' state to: {1}", name, Enum.GetName(typeof(EPlayerType), type)));
        }

        private void RecalcStateAndOwnerChanges()
        {
            switch (PlanetState)
            {
                case EPlanetState.OWNED:
                    ResetHalo(PlayerPlanetHalo);
                    ResetHalo(EnemyPlanetHalo);
                    switch (PlanetOwner)
                    {
                        case EPlayerType.PLAYER:
                            PlayerPlanetHalo.SetActive(true);
                            EnemyPlanetHalo.SetActive(false);
                            break;
                        case EPlayerType.ENEMY:
                        case EPlayerType.AI:
                            PlayerPlanetHalo.SetActive(false);
                            EnemyPlanetHalo.SetActive(true);
                            break;
                        default:
                            PlayerPlanetHalo.SetActive(true);
                            EnemyPlanetHalo.SetActive(false);
                            break;
                    }
                    break;
                case EPlanetState.POTENTIAL_TARGET:
                    TargetPlanetMarker.SetActive(true);
                    break;
                default:
                    TargetPlanetMarker.SetActive(false);
                    break;
            }
        }

        private void DrawLineToTarget()
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            if (DrawTarget.normalized != transform.position.normalized)
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, DrawTarget);
                // Debug.Log(String.Format("Draw from {0} to {1} for {2}", transform.position, DrawTarget, name));
            } else {
                lr.SetPositions(new List<Vector3>() { transform.position, transform.position }.ToArray());
            }
        }
    }
}
