using System.Collections.Generic;
using UnityEngine;
using PlanetAttack.ThePlanet;
using System.Linq;
using PlanetAttack.Enums;

namespace PlanetAttack
{
    public class PlanetUtils
    {
        private static PlanetsPool GetPlanetsPool()
        {
            GameObject mainMenu = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where((o) => o.name == "MainMenu").ToArray()[0];
            PlanetsPool op = mainMenu.GetComponent<PlanetsPool>();
            return op;
        }

        public static MainPlanet GeneratePlanet()
        {
            MainPlanet mp = GetPlanetsPool().GetPooledObject();
            mp.transform.localScale = Vector3.zero;
            mp.gameObject.SetActive(true);

            mp.InitEmptyPlanetState();
            RandomizePlanetMaterials(mp);

            return mp;
        }

        public static void RandomizePlanetMaterials(MainPlanet mainPlanet)
        {
            PGSolidPlanet planet = mainPlanet.Planet.GetComponent<PGSolidPlanet>();
            RandomizePlanetMaterials(planet);
        }

        public static void RandomizePlanetMaterials(PGSolidPlanet planet)
        {
            planet.planetMaterial.SetColor("_AtmosphereColor", new Color(Random.Range(0.7f, 1f), Random.Range(0.7f, 1f), Random.Range(0.5f, 1f), Random.Range(0f, 0.2f)));
            planet.planetMaterial.SetFloat("_SeaLevel", Random.Range(0f, 1f));
            planet.planetMaterial.SetColor("_SeaColor", new Color(Random.Range(0f, 0.1f), Random.Range(0.25f, 0.5f), Random.Range(0.5f, 0.8f)));
            planet.planetMaterial.SetColor("_LandColor", new Color(Random.Range(0f, 0.25f), Random.Range(0.2f, 0.5f), Random.Range(0.0f, 0.2f)));
            planet.planetMaterial.SetFloat("_MountainLevel", Random.Range(0f, 1f));
        }

        public static void ReleasePlanetsToPool()
        {
            var planets = GetAllThePlanets();

            foreach (MainPlanet mp in planets)
            {
                mp.RunWithDelay(() =>
                {
                    mp.transform.PopOut(mp, duration: 0.25f, onComplete: () =>
                    {
                        GetPlanetsPool().ReleaseObjectToPool(mp);
                    });
                }, baseDelay: 0.25f, variance: 0.25f);
            }
        }

        public static bool CheckCollisionWithOtherPlanets(GameObject obj, float marginMultiplier = 1.5f)
        {
            SphereCollider objCollider = obj.GetComponent<SphereCollider>();
            if (objCollider == null) return false;

            Vector3 center = obj.transform.position;
            // Calculate the radius considering the scale and margin
            float radius = objCollider.radius * obj.transform.lossyScale.x * marginMultiplier;

            foreach (MainPlanet other in GetAllThePlanets())
            {
                if (other.gameObject == obj)
                    continue;

                SphereCollider otherCollider = other.GetComponent<SphereCollider>();
                if (otherCollider == null) continue;

                // Radius of the second planet (also with a margin, so they don't enter their "safe zones")
                float otherRadius = otherCollider.radius * other.transform.lossyScale.x * marginMultiplier;

                float minDistance = radius + otherRadius;

                // Optimization: compare squared distances instead of Vector3.Distance
                Vector3 diff = center - other.transform.position;
                float sqrDistance = diff.sqrMagnitude;
                float sqrMinDistance = minDistance * minDistance;

                if (sqrDistance < sqrMinDistance)
                {
                    // Debug.Log($"{obj.name} in the safe zone of {other.name}");
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<GameObject> GetAllPlanets(string name = "Solid Planet")
        {
            return GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where((o) => o.name.Contains(name));
        }

        public static IEnumerable<MainPlanet> GetAllThePlanets(string name = "ThePlanet")
        {
            return GameObject.FindObjectsByType<MainPlanet>(FindObjectsSortMode.None).Where((o) => o.name.Contains(name));
        }

        public static IEnumerable<MainPlanet> GetSelectedPlanets(EPlayerType owner)
        {
            return GetAllThePlanets().Where((planet) => planet.PlanetOwner == owner && planet.PlanetState == EPlanetState.SELECTED);
        }

        public static IEnumerable<MainPlanet> GetPotentialTargetMarkedPlanets()
        {
            return GetPlanetsInState(EPlanetState.POTENTIAL_TARGET);
        }

        public static IEnumerable<MainPlanet> GetPlanetsInState(EPlanetState state)
        {
            return GetAllThePlanets().Where((planet) => planet.PlanetState == state);
        }

        public static IEnumerable<MainPlanet> GetPlanetsByOwner(EPlayerType owner)
        {
            return GetAllThePlanets().Where((planet) => planet.PlanetOwner == owner);
        }
    }
}