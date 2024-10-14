using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEditor.SearchService;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameBoard : MonoBehaviour
{
    float rotatePlanetsInterval = 0.05f;
    float nextTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlanets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTime) {
            RotatePlanets();
            nextTime += rotatePlanetsInterval;
        }
    }

    private void RotatePlanets()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>().Where((o) => o.name.Contains("Planet"));
        foreach(GameObject o in objects) {
            o.transform.Rotate(Vector3.up, 5);
        }
    }

    Transform GeneratePlanet()
    {
        // Transform newPlanet = Instantiate(Resources.Load<Transform>("Solid Planet"));
        // ProceduralPlanetMaker ppm = gameObject.AddComponent<ProceduralPlanetMaker>();
        // ppm.planetPrefab = newPlanet;
        // ppm.planetShader = Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet");
        //gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_SeaLevel", Utils.GetRandomFloatBetween(0, 1));
        // return newPlanet;

        Transform newPlanet = Instantiate(Resources.Load<Transform>("Solid Planet"));
        newPlanet.gameObject.name = newPlanet.gameObject.name.Replace("(Clone)", "");
        newPlanet.GetComponent<PGSolidPlanet>().planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
        newPlanet.GetComponent<PGSolidPlanet>().RandomizePlanet(true);

        return newPlanet;
    }

    void GeneratePlanets()
    {
        for (int i = 0; i < 12; i++)
        {
            Transform p = GeneratePlanet();
            p.gameObject.name += " " + i;
            do
            {
                float margin = 2;
                float x = (Camera.main.sensorSize.x / 2) - margin;
                float y = (Camera.main.sensorSize.y / 2) - margin;
                p.SetPositionAndRotation(GetRandomVectorInRange(-y, y, -x, x), Quaternion.Euler(0, Utils.GetRandomFloatBetween(0, 360), 0));
                var scale = Utils.GetRandomFloatBetween(1.5f, 3);
                p.localScale = new Vector3(scale, scale, scale);
            } while (CheckCollisionWithOtherPlanets(p.gameObject));
        }
    }

    bool CheckCollisionWithOtherPlanets(GameObject obj)
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
                print(string.Format("{0} touching {1}", obj.name, touch.gameObject.name));
                return (true);
            }
        }
        return (false);
    }

    Vector3 GetRandomVectorInRange(float fromX, float toX, float fromY, float toY, float step = 2)
    {
        return new Vector3(Utils.GetRandomFloatBetween(fromX, toX), Utils.GetRandomFloatBetween(fromY, toY));
    }

}
