using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetAttack.ThePlanet
{
    public class TheLabel : MonoBehaviour
    {
        // public icon

        public string LabelText = "543210";

        private TextMeshProUGUI textMeshPro;

        public GameObject Label;

        // Start is called before the first frame update
        void Start()
        {
            textMeshPro = this.Label.GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            textMeshPro.text = LabelText;
        }
    }
}