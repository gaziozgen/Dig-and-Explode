using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FateGames.Core
{
    public class Swerve : MonoBehaviour
    {
        public int Size { get; private set; } = Screen.height / 20;
        public Vector2 AnchorPosition { get; protected set; } = Vector2.zero;
        public Vector2 MousePosition { get; protected set; } = Vector2.zero;
        public Vector2 Difference { get => MousePosition - AnchorPosition; }
        public Vector2 Direction { get => Difference.normalized; }
        public float Distance { get => Difference.magnitude; }
        public float Rate { get => Distance / Size; }
        public float XRate { get => Difference.x / Size; }
        public float YRate { get => Difference.y / Size; }
        [SerializeField] public UnityEvent<Swerve> OnStart = new();
        [SerializeField] public UnityEvent<Swerve> OnSwerve = new();
        [SerializeField] public UnityEvent<Swerve> OnRelease = new();
        protected bool onUI = false;
        [SerializeField] protected bool worksOnUI = false;

        public void SetSize(int size)
        {
            Size = size;
        }
        private void OnDisable()
        {
            OnRelease.Invoke(this);
        }
        private void Update()
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case UnityEngine.TouchPhase.Began:
                            OnMouseButtonDown();
                            break;
                        case UnityEngine.TouchPhase.Moved:
                            OnMouseButton();
                            break;
                        case UnityEngine.TouchPhase.Stationary:
                            OnMouseButton();
                            break;
                        case UnityEngine.TouchPhase.Ended:
                            OnMouseButtonUp();
                            break;
                        case UnityEngine.TouchPhase.Canceled:
                            OnMouseButtonUp();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0)) OnMouseButtonDown();
                else if (Input.GetMouseButton(0)) OnMouseButton();
                else if (Input.GetMouseButtonUp(0)) OnMouseButtonUp();
            }
        }

        protected virtual void OnMouseButtonDown()
        {
            onUI = EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null;
            if (!worksOnUI && onUI) return;
            MousePosition = Input.mousePosition;
            AnchorPosition = MousePosition;
            OnStart.Invoke(this);
        }

        protected virtual void OnMouseButton()
        {
            if (!worksOnUI && onUI) return;
            Vector2 mousePosition = Input.mousePosition;
            Vector2 direction = (mousePosition - AnchorPosition).normalized;
            MousePosition = AnchorPosition + direction * Mathf.Clamp((mousePosition - AnchorPosition).magnitude, 0, Size);
            OnSwerve.Invoke(this);
        }

        protected virtual void OnMouseButtonUp()
        {
            if (!worksOnUI && onUI) return;
            OnRelease.Invoke(this);
        }

        public void RestartSwerve()
        {
            OnMouseButtonDown();
        }

    }
}

