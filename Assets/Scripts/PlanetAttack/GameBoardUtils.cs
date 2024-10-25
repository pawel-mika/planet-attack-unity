
using UnityEngine;
using PlanetAttack.ThePlanet;
using UnityEditor;

namespace PlanetAttack
{

    public class GameBoardUtils {

        public static void GeneratePlanets(int planetsCount = 12)
        {
            for (int i = 0; i < planetsCount; i++)
            {
                MainPlanet p = PlanetUtils.GeneratePlanet();
                p.gameObject.name += " " + i;
                ArrangePlanetInSpace(p);
            }
        }

        public static void CleanupPlanetsStats() {}

        public static void RandomizePlanetsInSpace() {
            foreach(MainPlanet planet in PlanetUtils.GetAllThePlanets()) {
                // ArrangePlanetInSpace(planet);
                ArrangePlanetInCameraViewport(planet);
                PlanetUtils.RandomizePlanetMaterials(planet);
            }
        }

        private static void ArrangePlanetInSpace(MainPlanet planet) {
            int antiInfLoopCounter = 10;
            Vector3 bounds = GetPlanetBounds();
            // float margin = 2;
            // float x = (Camera.main.sensorSize.x / 2) - margin;
            // float y = (Camera.main.sensorSize.y / 2) - margin;
            float x = bounds.x / 2;
            float y = bounds.y / 2;
            do {
                planet.transform.position = Utils.GetRandomVectorInRange(-y, y, -x, x);
                var scale = Utils.GetRandomFloat(1.5f, 3);
                planet.transform.localScale = new Vector3(scale, scale, scale);
                antiInfLoopCounter--;
            } while (PlanetUtils.CheckCollisionWithOtherPlanets(planet.gameObject) | antiInfLoopCounter > 0);
        }

        private static Vector3 GetPlanetBounds() {
            float aspect = (float)Screen.width / (float)Screen.height;
            GameObject sceneCamObj = GameObject.Find("SceneCamera");
            Camera camera = sceneCamObj.GetComponent<Camera>();
            
            Vector3 ur = camera.ViewportToWorldPoint(new Vector3(1.92f, 1.08f, 48));
            Debug.Log(ur);

            return ur;
        }

        private static void ArrangePlanetInCameraViewport(MainPlanet planet) {
            int antiInfinityLoopCounter = 8;
            GameObject sceneCamObj = GameObject.Find("PlanetsBoardCamera");
            Camera camera = sceneCamObj.GetComponent<Camera>();
            do {
                Vector3 screenPosition = camera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0,Screen.height), Camera.main.farClipPlane/8));
                planet.transform.position = screenPosition;
                var scale = Random.Range(1.25f, 3);
                planet.transform.localScale = new Vector3(scale, scale, scale);
                antiInfinityLoopCounter--;
            } while (PlanetUtils.CheckCollisionWithOtherPlanets(planet.gameObject) | antiInfinityLoopCounter > 0);
        }
    }
}