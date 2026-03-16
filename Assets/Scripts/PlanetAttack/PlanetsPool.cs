using System.Collections;
using System.Collections.Generic;
using PlanetAttack.ThePlanet;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                tmp = GeneratePlanet();
                tmp.name = tmp.name.Replace("(Clone)", "");
                tmp.name += " " + i;
                pooledPlanets.Add(tmp);
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
            MainPlanet newPlanet = Instantiate(objectToPool);

            Scene inGameScene = SceneManager.GetSceneByName("InGame");
            if (inGameScene.isLoaded)
            {
                SceneManager.MoveGameObjectToScene(newPlanet.gameObject, inGameScene);
            }

            PGSolidPlanet planet = newPlanet.Planet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));

            planet.RandomizePlanet(true);
            PlanetUtils.RandomizePlanetMaterials(newPlanet);

            newPlanet.gameObject.SetActive(true);
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