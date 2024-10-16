using UnityEngine;

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

        // Start is called before the first frame update
        void Start()
        {
            PGSolidPlanet pgSolidPlanet = Planet.GetComponent<PGSolidPlanet>();
            pgSolidPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            pgSolidPlanet.RandomizePlanet(true);
            this.RotationPerSec = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            Planet.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
        }
    }
}
