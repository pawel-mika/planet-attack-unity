using System.Collections.Generic;
using UnityEngine;
using PlanetAttack.ThePlanet;
using System.Linq;

namespace PlanetAttack
{
    public class PlanetUtils
    {
        // deprecated
        public static Transform GeneratePGPlanet()
        {
            Transform newPlanet = UnityEngine.Object.Instantiate(Resources.Load<Transform>("Solid Planet"));
            newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
            PGSolidPlanet planet = newPlanet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            planet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
            // RandomizePlanetMaterials(newPlanet);
            return newPlanet;
        }

        // use this or just GeneratePlane?
        public static MainPlanet GeneratePooledPlanet() {
            GameObject goNewPlanet = ObjectPool.SharedInstance.GetPooledObject();
            goNewPlanet.SetActive(true);
            MainPlanet newPlanet = goNewPlanet.GetComponent<MainPlanet>();
            newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
            PGSolidPlanet planet = newPlanet.Planet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            planet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
            RandomizePlanetMaterials(newPlanet);
            return newPlanet;
        }

        public static MainPlanet GeneratePlanet() {
            MainPlanet newPlanet = UnityEngine.Object.Instantiate(Resources.Load<MainPlanet>("ThePlanet"));
            newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
            PGSolidPlanet planet = newPlanet.Planet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            planet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
            RandomizePlanetMaterials(newPlanet);
            return newPlanet;
        }

        public static void RandomizePlanetMaterials(MainPlanet mainPlanet) {
            PGSolidPlanet planet = mainPlanet.Planet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial.SetColor("_AtmosphereColor", new Color(Random.Range(0.7f, 1f), Random.Range(0.7f, 1f), Random.Range(0.5f, 1f), Random.Range(0f, 0.2f)));
            planet.planetMaterial.SetFloat("_SeaLevel", Random.Range(0f, 1f));
            planet.planetMaterial.SetColor("_SeaColor", new Color(Random.Range(0f, 0.1f), Random.Range(0.25f, 0.5f), Random.Range(0.5f, 0.8f)));
            planet.planetMaterial.SetColor("_LandColor", new Color(Random.Range(0f, 0.25f), Random.Range(0.2f, 0.5f), Random.Range(0.0f, 0.2f)));
            planet.planetMaterial.SetFloat("_MountainLevel", Random.Range(0f, 1f));
        }

        public static void RemoveAllThePlanets()
        {
            foreach(MainPlanet gameObject in GetAllThePlanets()) {
                GameObject.DestroyImmediate(gameObject);
            }
        }

        public static bool CheckCollisionWithOtherPlanets(GameObject obj)
        {
            SphereCollider objCollider = obj.GetComponent<SphereCollider>();
            // trick to align center of collider to center of planet
            objCollider.enabled = false;
            objCollider.enabled = true;

            Collider[] touching = Physics.OverlapSphere(objCollider.bounds.center, objCollider.radius);
            foreach (Collider touch in touching)
            {
                //avoid detecting itself, detect only other planets (same gameobject type)
                if (touch.gameObject != obj && touch.gameObject.GetType().Equals(obj.GetType()))
                {
                    // Debug.Log(string.Format("{0} touching {1}", obj.name, touch.gameObject.name));
                    return (true);
                }
            }
            return (false);
        }

        public static IEnumerable<GameObject> GetAllPlanets(string name = "Solid Planet") {
            return GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where((o) => o.name.Contains(name));
        }

        public static IEnumerable<MainPlanet> GetAllThePlanets(string name = "ThePlanet") {
            return GameObject.FindObjectsByType<MainPlanet>(FindObjectsSortMode.None).Where((o) => o.name.Contains(name));
        }

    }
}