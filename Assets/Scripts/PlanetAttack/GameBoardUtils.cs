
using UnityEngine;
using PlanetAttack.ThePlanet;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace PlanetAttack
{

    public class GameBoardUtils
    {

        public static void GeneratePlanets(int planetsCount = 12)
        {
            for (int i = 0; i < planetsCount; i++)
            {
                MainPlanet p = PlanetUtils.GeneratePlanet();
            }
        }

        public static void RandomizePlanetsInSpace()
        {
            foreach (MainPlanet planet in PlanetUtils.GetAllThePlanets())
            {
                ArrangePlanetInCameraViewport(planet);
                PlanetUtils.RandomizePlanetMaterials(planet);
            }
        }

        public static void RandomizeStartingPlanets()
        {
            List<MainPlanet> AllPlanets = PlanetUtils.GetAllThePlanets().ToList();
            int playerIdx = Random.Range(0, AllPlanets.Count());
            AllPlanets[playerIdx].Ships = 128;
            AllPlanets[playerIdx].SetPlanetOwner(Enums.EPlayerType.PLAYER);
            AllPlanets[playerIdx].SetPlanetState(Enums.EPlanetState.OWNED);
            AllPlanets.RemoveAt(playerIdx);
            int enemyIdx = Random.Range(0, AllPlanets.Count());
            AllPlanets[enemyIdx].Ships = 128;
            AllPlanets[enemyIdx].SetPlanetOwner(Enums.EPlayerType.ENEMY);
            AllPlanets[enemyIdx].SetPlanetState(Enums.EPlanetState.OWNED);
        }

        public static void InitializePlanetsState()
        {
            foreach (MainPlanet planet in PlanetUtils.GetAllThePlanets())
            {
                planet.InitEmptyPlanetState();
            }
        }

        private static void ArrangePlanetInCameraViewport(MainPlanet planet)
        {
            GameObject sceneCamObj = GameObject.Find("PlanetsBoardGuideCamera");
            Camera camera = sceneCamObj.GetComponent<Camera>();
            float z = camera.farClipPlane / 8;
            int attempts = 0;

            do
            {
                Vector3 screenPosition = camera.ScreenToWorldPoint(
                    new Vector3(
                        Random.Range(0f, Screen.width),
                        Random.Range(0f, Screen.height),
                        z
                    )
                );

                planet.transform.position = screenPosition;

                var scale = Random.Range(1.25f, 3f);
                planet.transform.localScale = new Vector3(scale, scale, scale);

                attempts++;
            } while (PlanetUtils.CheckCollisionWithOtherPlanets(planet.gameObject) && attempts < 64);

            if (attempts >= 64)
            {
                Debug.Log($"Giving up repositioning: {planet.name}");
            }

        }

        public static void CleanupBoard()
        {
            PlanetUtils.ReleasePlanetsToPool();
        }
    }
}