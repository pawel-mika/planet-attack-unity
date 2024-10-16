using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlanet : MonoBehaviour
{
    public float rotationPerSec = 0.05f;
    private float rotationSpeed = 1f * 360f;

    public Transform planetTransform;
    // Start is called before the first frame update

    void Start()
    {
        planetTransform = this.transform.Find("Main Planet");
        PGSolidPlanet pgSolidPlanet = planetTransform.GetComponent<PGSolidPlanet>();
        pgSolidPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
        pgSolidPlanet.RandomizePlanet(true);

        this.SetRotationPerSec(0.1f);
        Debug.Log(string.Format("Rotation speed: {0}", rotationSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        if(planetTransform) {
            planetTransform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
        }
    }

    void SetRotationPerSec(float rps) {
        this.rotationPerSec = rps;
        this.rotationSpeed = this.rotationPerSec * 360f;
    }
}
