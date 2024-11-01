using System.Collections;
using System.Collections.Generic;
using PlanetAttack.ThePlanet;
using UnityEngine;

namespace PlanetAttack
{

    public class PlanetsPool : MonoBehaviour
    {
        public static PlanetsPool SharedInstance;
        public List<MainPlanet> pooledPlanets;
        public MainPlanet objectToPool;
        public int amountToPool;

        void Awake()
        {
            SharedInstance = this;
        }

        void Start()
        {
            pooledPlanets = new List<MainPlanet>();
            MainPlanet tmp;
            for (int i = 0; i < amountToPool; i++)
            {
                // tmp = Instantiate(objectToPool);
                tmp = GeneratePlanet();
                tmp.name = tmp.name.Replace("(Clone)", "");
                tmp.name += " " + i;
                pooledPlanets.Add(tmp);
                // tmp.gameObject.SetActive(false);
            }
        }

        public MainPlanet GetPooledObject()
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (!pooledPlanets[i].gameObject.activeInHierarchy)
                {
                    return pooledPlanets[i];
                }
            }
            return null;
        }

        public void ReleaseObjectToPool(MainPlanet mp)
        {
            if (pooledPlanets.Contains(mp))
            {
                mp.gameObject.SetActive(false);
            }
            else
            {
                throw new System.Exception("Object not from this pool");
            }
        }

        private MainPlanet GeneratePlanet()
        {
            // MainPlanet newPlanet = Instantiate(Resources.Load<MainPlanet>("ThePlanet"));
            MainPlanet newPlanet = Instantiate(objectToPool);
            PGSolidPlanet planet = newPlanet.Planet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));

            planet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
            PlanetUtils.RandomizePlanetMaterials(newPlanet);

            newPlanet.gameObject.SetActive(true);

            // move to separate layer for preload
            newPlanet.gameObject.layer = LayerMask.NameToLayer("Planets");
            StartCoroutine(DeactivateNextFrame(newPlanet));

            return newPlanet;
        }

        // Deactivate after one frame
        private IEnumerator DeactivateNextFrame(MainPlanet planet)
        {
            yield return null; // wait one frame
            planet.gameObject.SetActive(false);
            planet.gameObject.layer = LayerMask.NameToLayer("Default"); // Move object back to default layer
        }
    }
}