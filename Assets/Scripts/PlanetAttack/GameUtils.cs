using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlanetAttack;
using System.Linq;
using Unity.VisualScripting;

namespace PlanetAttack
{
    public class GameUtils
    {
        public static Transform GeneratePlanet()
        {
            Transform newPlanet = UnityEngine.Object.Instantiate(Resources.Load<Transform>("Solid Planet"));
            newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
            PGSolidPlanet planet = newPlanet.GetComponent<PGSolidPlanet>();
            planet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
            planet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
            RandomizePlanetMaterials(newPlanet);
            return newPlanet;
        }

        public static void RandomizePlanetMaterials(Transform planetTransform) {
            PGSolidPlanet planet = planetTransform.GetComponent<PGSolidPlanet>();
            planet.planetMaterial.SetColor("_AtmosphereColor", new Color(Utils.GetRandomFloatBetween(0.7f, 1f), Utils.GetRandomFloatBetween(0.7f, 1f), Utils.GetRandomFloatBetween(0.5f, 1f), Utils.GetRandomFloatBetween(0f, 0.2f)));
            planet.planetMaterial.SetFloat("_SeaLevel", Utils.GetRandomFloatBetween(0, 1));
            planet.planetMaterial.SetColor("_SeaColor", new Color(Utils.GetRandomFloatBetween(0, 0.1f), Utils.GetRandomFloatBetween(0.25f, 0.5f), Utils.GetRandomFloatBetween(0.5f, 0.8f)));
            planet.planetMaterial.SetColor("_LandColor", new Color(Utils.GetRandomFloatBetween(0, 0.25f), Utils.GetRandomFloatBetween(0.2f, 0.5f), Utils.GetRandomFloatBetween(0.0f, 0.2f)));
            planet.planetMaterial.SetFloat("_MountainLevel", Utils.GetRandomFloatBetween(0, 1));
        }

        public static void RemoveAllPlanets()
        {
            foreach(GameObject gameObject in GetAllPlanets()) {
                GameObject.DestroyImmediate(gameObject);
            }
        }

        public static bool CheckCollisionWithOtherPlanets(GameObject obj)
        {
            SphereCollider objCollider = obj.GetComponent<SphereCollider>();
            // trick to align center of collider to center of planet
            objCollider.enabled = false;
            objCollider.enabled = true;

            Collider[] touching = Physics.OverlapSphere(objCollider.bounds.center, objCollider.radius * 3);
            foreach (Collider touch in touching)
            {
                //avoid detecting itself, detect only other planets (same gameobject type)
                if (touch.gameObject != obj && touch.gameObject.GetType().Equals(obj.GetType()))
                {
                    Debug.Log(string.Format("{0} touching {1}", obj.name, touch.gameObject.name));
                    return (true);
                }
            }
            return (false);
        }

        public static IEnumerable<GameObject> GetAllPlanets() {
            return GameObject.FindObjectsOfType<GameObject>().Where((o) => o.name.Contains("Solid Planet"));
        }

    }
}