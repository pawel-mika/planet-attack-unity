using System;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace PlanetAttack.ThePlanet
{
    public class MainPlanet : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler
    {
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

        // Start is called before the first frame update
        void Start()
        {
            PGSolidPlanet pgSolidPlanet = Planet.GetComponent<PGSolidPlanet>();
            pgSolidPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            pgSolidPlanet.RandomizePlanet(true);
            this.RotationPerSec = 0.1f;

            InitializePlanetState();
        }

        // Update is called once per frame
        void Update()
        {
            Planet.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
            CheckClicked();

            BlinkPlayerHalo();

            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                if (GameController.GameState == GameController.EGameState.IN_GAME)
                {
                    // recalc every 1s or later get game level settings into account? (faster for higher level?) dunno yet
                    HarvestFood();
                    MineMinerals();
                    ManufactureShips();
                }
            }
        }

        private void CheckClicked()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, 100f))
                {
                    if (raycastHit.transform != null)
                    {
                        if (raycastHit.transform.gameObject.name == name)
                        {
                            PlayerPlanetHalo.SetActive(!(PlayerPlanetHalo.activeSelf));
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.magenta);
        }

        private void BlinkPlayerHalo()
        {
            Material mp = PlayerPlanetHalo.GetComponent<Renderer>().material;
            float cp = 1f + (float)(Math.Sin(Utils.Sawtooth(Time.time, 1, 0f, 1f)) * 6f);
            mp.SetFloat("_Falloff", cp);

            Transform tPlayer = PlayerPlanetHalo.transform;
            float scale = 1f + (float)Math.Sin(Utils.Sawtooth(Time.time, 1, 0.25f, 1.25f));
            tPlayer.localScale = new Vector3(scale, scale, scale);
        }

        Vector3 dragStartPoint;

        public void OnBeginDrag()
        {
            dragStartPoint = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Vector3 delta = eventData.pointerPressRaycast.worldPosition - eventData.pointerCurrentRaycast.worldPosition;
            // Vector3 delta = eventData.pointerCurrentRaycast.worldPosition;
            // Vector3 delta = Camera.main.WorldToViewportPoint(eventData.pointerPressRaycast.worldPosition) - Camera.main.WorldToViewportPoint(eventData.pointerCurrentRaycast.worldPosition);

            // Vector3 delta = dragStartPoint - Camera.main.WorldToViewportPoint(eventData.pointerCurrentRaycast.worldPosition);
            // delta.z = 0;
            // Debug.Log(delta);
            // transform.position += delta;

            Ray R = Camera.main.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
            Vector3 PO = transform.position; // Take current position of this draggable object as Plane's Origin
            Vector3 PN = -Camera.main.transform.forward; // Take current negative camera's forward as Plane's Normal
            float t = Vector3.Dot(PO - R.origin, PN) / Vector3.Dot(R.direction, PN); // plane vs. line intersection in algebric form. It find t as distance from the camera of the new point in the ray's direction.
            Vector3 P = R.origin + R.direction * t; // Find the new point.

            transform.position = P;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
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

        public void InitializePlanetState()
        {
            PlayerPlanetHalo.SetActive(false);

            Ships = Random.Range(0, 128);
            Minerals = Random.Range(0, 512);
            Food = Random.Range(0, 512);

            ShipsBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating
            MineralsBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating
            FoodBonus = Random.Range(1f, 3f); // get gameLevel into accout while calculating

            ShipCostMinerals = Random.Range(10f, 15f);
            ShipCostFood = Random.Range(10f, 15f);
        }
    }
}
