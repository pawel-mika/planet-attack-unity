using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlanetAttack.ThePlanet
{
    public class MainPlanet : MonoBehaviour
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
    }
}
