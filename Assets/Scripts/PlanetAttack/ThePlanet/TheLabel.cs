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

        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                if (textMeshPro != null)
                {
                    textMeshPro.text = _labelText;
                }
            }
        }
        private string _labelText = "0";

        private TextMeshProUGUI textMeshPro;

        public GameObject Label;

        void Awake()
        {
            textMeshPro = Label.GetComponent<TextMeshProUGUI>();
            textMeshPro.text = _labelText;
        }
    }
}