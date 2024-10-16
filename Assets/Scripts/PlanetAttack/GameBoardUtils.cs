
using UnityEngine;
using PlanetAttack.ThePlanet;

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
                ArrangePlanetInSpace(planet);
                PlanetUtils.RandomizePlanetMaterials(planet);
            }
        }

        private static void ArrangePlanetInSpace(MainPlanet planet) {
            do {
                float margin = 2;
                float x = (Camera.main.sensorSize.x / 2) - margin;
                float y = (Camera.main.sensorSize.y / 2) - margin;
                planet.transform.position = Utils.GetRandomVectorInRange(-y, y, -x, x);
                var scale = Utils.GetRandomFloat(1.5f, 3);
                planet.transform.localScale = new Vector3(scale, scale, scale);
            } while (PlanetUtils.CheckCollisionWithOtherPlanets(planet.gameObject));
        }
    }
}