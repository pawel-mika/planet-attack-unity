using UnityEngine;
using PlanetAttack.ThePlanet;
using PlanetAttack.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PlanetAttack
{

    public class InputManager : MonoBehaviour
    {
        [Header("Settings")]
        // public LayerMask planetLayer;
        public float dragDistanceThreshold = 0.1f;

        [Header("Internal State")]
        private MainPlanet _hoveredPlanet;
        private MainPlanet _dragStartPlanet;
        private Vector3 _mouseDownPoint = Vector3.negativeInfinity;

        // References to controllers
        private PlanetsController _planetsController => GameManager.PlanetsController;
        private ActionsController _actionsController => GameManager.ActionsController;

        void Update()
        {
            HandleMouseInput();
            HandleKeyboardInput();
        }

        private void HandleMouseInput()
        {
            _hoveredPlanet = GetPlanetUnderCursor();

            // 1. MOUSE DOWN
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDownPoint = GetMouseWorldPosition();
                if (_hoveredPlanet != null)
                {
                    _dragStartPlanet = _hoveredPlanet;
                    _actionsController.mouseDownPoint = _mouseDownPoint;
                    Debug.Log($"[Input] MouseDown on: {_hoveredPlanet.name}");
                }
            }

            // 2. MOUSE DRAG (While holding button)
            if (Input.GetMouseButton(0) && _dragStartPlanet != null)
            {
                float dist = Vector3.Distance(_mouseDownPoint, GetMouseWorldPosition());
                if (dist > dragDistanceThreshold)
                {
                    HandleGlobalDrag();
                }
            }

            // 3. MOUSE UP
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
        }

        private void HandleGlobalDrag()
        {
            _actionsController.isDragging = true;
            _actionsController.dragTargetPoint = GetMouseWorldPosition();
            _actionsController.dragStartPoint = _dragStartPlanet.transform.position;
            _planetsController.dragStartPlanet = _dragStartPlanet;

            // Logic for potential targets
            if (_hoveredPlanet != null && _hoveredPlanet != _dragStartPlanet)
            {
                if (_dragStartPlanet.PlanetOwner == EPlayerType.PLAYER)
                {
                    UpdateDragOverState(_hoveredPlanet);
                }
            }
            else
            {
                ClearDragOver();
            }

            // Update arrows for all selected planets
            UpdateAttackArrows();
        }

        private void HandleMouseUp()
        {
            // TestPlanetSelection();
            if (_hoveredPlanet == null)
            {
                DeselectAllPlanets();
            }
            else
            {
                ExecutePlanetAction(_hoveredPlanet);
            }

            ResetInputState();
        }

        private void ExecutePlanetAction(MainPlanet planet)
        {
            if (planet == _dragStartPlanet && planet.PlanetOwner == EPlayerType.PLAYER)
            {
                var newState = planet.PlanetState == EPlanetState.OWNED
                    ? EPlanetState.SELECTED
                    : EPlanetState.OWNED;

                planet.SetPlanetState(newState);
                return;
            }

            _actionsController.CreateActionFromCurrentState(GetSourcePlanets().ToList(), planet);
        }


        private void ResetInputState()
        {
            ClearDragOver();
            _planetsController.dragStartPlanet = null;
            _dragStartPlanet = null;
            _mouseDownPoint = Vector3.negativeInfinity;
            _actionsController.isDragging = false;
            _planetsController.ClearDrawTargetAttackArrows();
            _planetsController.ClearAllPotentnialTargets();
        }

        private void OnDrawGizmos()
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 50f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(GetMouseWorldPosition(), 0.3f);
        }

        // --- HELPER METHODS ---

        private void TestPlanetSelection()
        {
            MainPlanet mp = GetPlanetUnderCursor();
            if (mp != null)
            {
                Debug.Log(String.Format("TestPlanetSelection: {0}, isDragging: {1}, dragOverPlanet: {2}", mp.name, _actionsController.isDragging, _planetsController.dragOverPlanet));
                if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftControl))
                {
                    mp.SetPlanetOwner(EPlayerType.PLAYER);
                    mp.SetPlanetState(mp.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
                }
                else if (mp.PlanetOwner == EPlayerType.PLAYER)
                {
                    mp.SetPlanetState(mp.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
                }
            }
        }

        private MainPlanet GetPlanetUnderCursor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                return hit.transform.GetComponent<MainPlanet>();
            }
            return null;
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                return hit.point;
            }

            float distanceFromCamera = Mathf.Abs(Camera.main.transform.position.z);
            return ray.GetPoint(distanceFromCamera);
        }

        private void UpdateDragOverState(MainPlanet target)
        {
            if (_planetsController.dragOverPlanet != target)
            {
                _planetsController.dragOverPlanet?.RevertPreviousState();
                _planetsController.dragOverPlanet = target;

                // Set potential state visual
                EPlanetState newState = (target.PlanetOwner == EPlayerType.PLAYER) ? EPlanetState.POTENTIAL_TRANSFER : EPlanetState.POTENTIAL_TARGET;
                target.SetPlanetState(newState);
            }
        }

        private void ClearDragOver()
        {
            _planetsController.dragOverPlanet?.RevertPreviousState();
            _planetsController.dragOverPlanet = null;
        }

        private void DeselectAllPlanets()
        {
            foreach (MainPlanet p in PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER))
            {
                p.SetPlanetState(EPlanetState.OWNED);
            }
        }

        private void UpdateAttackArrows()
        {
            foreach (MainPlanet mp in GetSourcePlanets())
            {
                mp.DrawTarget = GetMouseWorldPosition();
            }
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                // Toggle debug globally
            }
        }

        private IEnumerable<MainPlanet> GetSourcePlanets()
        {
            var selected = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER).ToList();
            if (_dragStartPlanet != null && _dragStartPlanet.PlanetOwner == EPlayerType.PLAYER && !selected.Contains(_dragStartPlanet))
            {
                return new List<MainPlanet> { _dragStartPlanet };
            }
            return selected;
        }
    }
}