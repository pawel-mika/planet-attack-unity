using PlanetAttack;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public PGSolidPlanet MenuPlanet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuPlanet.planetMaterial = new Material(Shader.Find("Zololgo/PlanetGen | Planet/Standard Solid Planet"));
        MenuPlanet.RandomizePlanet(true); // heavy op, let's do it just once here during generate time
        PlanetUtils.RandomizePlanetMaterials(MenuPlanet);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
