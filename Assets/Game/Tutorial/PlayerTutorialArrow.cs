using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialArrow : FateMonoBehaviour
{
    private static PlayerTutorialArrow instance = null;
    public static PlayerTutorialArrow Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerTutorialArrow>();
            return instance;
        }
    }

    [SerializeField] Transform arrowParent = null;
    [SerializeField] LayerMask objectLayerMask;
    [SerializeField] float detectionDistance = 10f;
    public bool ShowArrow = false;

    Camera mainCamera;

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
    }

    public void Show()
    {
        if (!ShowArrow) return;
        enabled = true;
        CheckTargetInCamera();
    }

    public void Hide()
    {
        enabled = false;
        arrowParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckTargetInCamera();
    }

    private void CheckTargetInCamera()
    {
        // Get the camera's view frustum planes
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Iterate through all objects in the scene
        foreach (Collider collider in FindObjectsOfType<Collider>())
        {
            // If the collider bounds intersect with any of the camera's frustum planes
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds))
            {
                // Check if the collider belongs to the object layer mask
                if (((1 << collider.gameObject.layer) & objectLayerMask) != 0)
                {
                    // An object has been detected
                    TargetInCamera();
                    return;
                }
            }
        }
        TargetOutOfCamera();
    }

    private void TargetInCamera()
    {
        if (arrowParent.gameObject.activeSelf == true) arrowParent.gameObject.SetActive(false);
    }

    private void TargetOutOfCamera()
    {
        if (arrowParent.gameObject.activeSelf == false) arrowParent.gameObject.SetActive(true);
        arrowParent.forward = TutorialManager.Instance.TargetPos - transform.parent.position;
    }
}
