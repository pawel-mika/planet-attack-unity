using System;
using System.Collections.Generic;
using System.Security.Policy;
using PlanetAttack.Enums;
using UnityEngine;

namespace PlanetAttack.ThePlanet
{
    public class PlanetVisuals : MonoBehaviour
    {
        public GameObject Planet;
        public GameObject PlayerPlanetHalo;
        public GameObject EnemyPlanetHalo;
        public GameObject TargetPlanetMarker;
        public GameObject TransferPlanetMarker;
        public GameObject Explosion;

        private LineRenderer _lineRenderer;
        private PlanetSettings _settings;
        private float _rotationSpeed;

        public void Initialize(PlanetSettings settings)
        {
            _settings = settings;
            _lineRenderer = GetComponent<LineRenderer>();
            _rotationSpeed = settings.RotationPerSec * 360f;
            
            // Initial visuals setup
            ResetHalo(PlayerPlanetHalo);
            ResetHalo(EnemyPlanetHalo);
            TargetPlanetMarker.SetActive(false);
            TransferPlanetMarker.SetActive(false);
        }

        public void Rotate()
        {
            Planet.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }

        public void UpdateLine(Vector3 target)
        {
            if (target != transform.position)
            {
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, target);
                float offset = Time.time * _settings.TextureScrollSpeed;
                _lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0f));
            }
            else
            {
                _lineRenderer.SetPositions(new[] { transform.position, transform.position });
            }
        }

        public void UpdateStateVisuals(EPlanetState state, EPlayerType owner)
        {
            Debug.Log(string.Format("UpdateStateVisuals: {0}, state: {1}, owner: {2}", Planet.name, state, owner));
            TargetPlanetMarker.SetActive(false);
            TransferPlanetMarker.SetActive(false);
            ResetHalo(PlayerPlanetHalo);
            ResetHalo(EnemyPlanetHalo);

            switch (state)
            {
                case EPlanetState.OWNED:
                    if (owner == EPlayerType.PLAYER)
                        PlayerPlanetHalo.SetActive(true);
                    else if (owner == EPlayerType.ENEMY || owner == EPlayerType.AI)
                        EnemyPlanetHalo.SetActive(true);
                    break;
                case EPlanetState.POTENTIAL_TRANSFER:
                    TransferPlanetMarker.SetActive(true);
                    break;
                case EPlanetState.POTENTIAL_TARGET:
                    TargetPlanetMarker.SetActive(true);
                    break;
            }
        }

        public void Blink(EPlanetState state)
        {
            if (state == EPlanetState.SELECTED)
            {
                BlinkTheHalo(PlayerPlanetHalo);
                BlinkTheHalo(EnemyPlanetHalo);
            }
        }

        private void BlinkTheHalo(GameObject go)
        {
            if (!go.activeInHierarchy) return;
            
            Material mp = go.GetComponent<Renderer>().material;
            float cp = 1f + (float)(Math.Sin(Utils.Sawtooth(Time.time, _settings.BlinkSpeed, 0f, 1f)) * _settings.BlinkIntensity);
            mp.SetFloat("_Falloff", cp);

            float scale = 1f + (float)Math.Sin(Utils.Sawtooth(Time.time, _settings.BlinkSpeed, 0.25f, 1.25f));
            go.transform.localScale = new Vector3(scale, scale, scale);
        }

        private void ResetHalo(GameObject go)
        {
            Material mp = go.GetComponent<Renderer>().material;
            mp.SetFloat("_Falloff", 1);
            go.transform.localScale = new Vector3(_settings.DefaultHaloScale, _settings.DefaultHaloScale, _settings.DefaultHaloScale);
        }

        public void PlayExplosion()
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }

        public string DebugString()
        {
            return string.Format("PlanetVisuals: PPH={0}, EPH={1}, TaPM={2}, TrPM={3}",
             PlayerPlanetHalo.activeInHierarchy, EnemyPlanetHalo.activeInHierarchy, TargetPlanetMarker.activeInHierarchy, TransferPlanetMarker.activeInHierarchy);
        }
    }
}