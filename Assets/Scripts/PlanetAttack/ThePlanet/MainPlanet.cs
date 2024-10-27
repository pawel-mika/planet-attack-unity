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

        private int ships = 0;
        public int Ships
        {
            get { return ships; }
            set { ships = value; }
        }

        private int minerals = 0;
        public int Minerals
        {
            get { return minerals; }
            set { minerals = value; }
        }

        private int food = 0;
        public int Food
        {
            get { return food; }
            set { food = value; }
        }


        public GameObject Planet;

        public GameObject PlayerPlanetHalo;
        public GameObject EnemyPlanetHalo;

        public TheLabel ShipsLabel;
        public TheLabel MineralsLabel;
        public TheLabel FoodLabel;

        // Start is called before the first frame update
        void Start()
        {
            PGSolidPlanet pgSolidPlanet = Planet.GetComponent<PGSolidPlanet>();
            pgSolidPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            pgSolidPlanet.RandomizePlanet(true);
            this.RotationPerSec = 0.1f;

            ShipsLabel.LabelText = "" + Random.Range(0, 128);
            MineralsLabel.LabelText = "" + Random.Range(0, 128);
            FoodLabel.LabelText = "" + Random.Range(0, 128);
        }

        // Update is called once per frame
        void Update()
        {
            Planet.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
            CheckClicked();

            BlinkPlayerHalo();
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

        private void BlinkPlayerHalo() {
            Material mp = PlayerPlanetHalo.GetComponent<Renderer>().material;
            float cp = 1f + (float)(Math.Sin(Utils.Sawtooth(Time.time, 1, 0f, 1f)) * 6f);
            mp.SetFloat("_Falloff", cp);

            Transform tPlayer = PlayerPlanetHalo.transform;
            float scale = 1f + (float)Math.Sin(Utils.Sawtooth(Time.time, 1, 0.25f, 1.25f));
            tPlayer.localScale = new Vector3(scale, scale, scale);
        }

        Vector3 dragStartPoint;

        public void OnBeginDrag() {
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
    }
}
