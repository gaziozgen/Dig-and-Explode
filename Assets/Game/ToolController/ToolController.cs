using FateGames.Core;
using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class ToolController : FateMonoBehaviour
{
    [SerializeField] float baseSpeed;
    [SerializeField] float baseDepth = -10;
    [SerializeField] float depthIncreasePerLevel = -5;
    [SerializeField] float ropeTensionRange = 5;
    [SerializeField] float baseRopeTop = 14;
    [SerializeField] float ropeTopMaxTensionRange = 5;
    [SerializeField] IntVariable ropeLengthLevel;
    [SerializeField] Gradient ropeColorRange;
    [SerializeField] MeshRenderer ropeMesh;
    [SerializeField] LayerMask dugLayerMask;
    [SerializeField] float minimumReducedSpeedRatio = 0.2f;
    [SerializeField] float overlapRadius = 1;
    [SerializeField] int maxCollider = 40;
    [SerializeField] float recoilMultiplier = 0.1f;
    [SerializeField] GameObject maxLength;
    [SerializeField] Rigidbody rb;
    [SerializeField] ToolSwitcher toolSwitcher;
    [SerializeField] GameObject rope;
    [SerializeField] Transform ropeTop;
    [SerializeField] Swerve swerve;
    /*[Header("Borders")]
    [SerializeField] float right;
    [SerializeField] float left;
    [SerializeField] float top;
    [SerializeField] float bottom;*/

    public float SpeedMultiplier => 1 - Mathf.Clamp(numOverlapColliders / (float)maxCollider, 0, 1) * toolSwitcher.CurrentTool.SlowdownMultiplier() * (1 - minimumReducedSpeedRatio);
    float currentMaxDepth => baseDepth + depthIncreasePerLevel * ropeLengthLevel.Value;


    bool isWorking = false;
    Collider[] overlapColliders;
    int numOverlapColliders = 0;
    float posX = 0;
    float posY = 1;
    Material ropeMat;

    private void Awake()
    {
        overlapColliders = new Collider[maxCollider];
        rope.SetActive(true);
        ropeMat = ropeMesh.material;
    }

    private void Start()
    {
        LoadPos();
        InvokeRepeating(nameof(SavePos), 1, 1);
    }

    private void FixedUpdate()
    {
        //Debug.Log(SpeedMultiplier);
        numOverlapColliders = Physics.OverlapSphereNonAlloc(transform.position, overlapRadius, overlapColliders, dugLayerMask, QueryTriggerInteraction.Ignore);
        if (isWorking) toolSwitcher.CurrentTool.OnWork();
        else toolSwitcher.CurrentTool.OnNotWork();

        rb.velocity = Vector3.zero;
    }

    public void Move(Swerve swerve)
    {
        float speed = Time.deltaTime * baseSpeed * swerve.Rate * SpeedMultiplier;
        Vector3 pos = Vector3.MoveTowards(transform.localPosition, transform.localPosition + (Vector3)swerve.Direction * speed, speed);

        if (pos.y < currentMaxDepth) pos.y = currentMaxDepth;

        transform.localPosition = pos;
        UpdateRopeTop();

        /*pos.x = Mathf.Clamp(pos.x, -left, right);
        pos.y = Mathf.Clamp(pos.y, -bottom, top);*/
        //rb.AddForce((Vector3)swerve.Direction * speed * rb.mass, ForceMode.Force);
    }

    public void Recoil(Vector3 direction)
    {
        rb.AddForce(direction * rb.mass * recoilMultiplier, ForceMode.Impulse);
    }

    public void SetWorking(bool working)
    {
        isWorking = working;
    }

    public Rigidbody GetRigidbody() { return rb; }

    private void UpdateRopeTop()
    {
        if (transform.localPosition.y < currentMaxDepth + ropeTensionRange)
        {
            float value = (currentMaxDepth + ropeTensionRange - transform.localPosition.y) / ropeTensionRange;
            ropeTop.localPosition = (baseRopeTop + ropeTopMaxTensionRange * value) * Vector3.up;
            ropeMat.color = ropeColorRange.Evaluate(value);

            if (value > 0.9f)
            {
                if (!maxLength.activeSelf) maxLength.SetActive(true);
            }
            else if (maxLength.activeSelf) maxLength.SetActive(false);
        }
        else ropeMat.color = ropeColorRange.Evaluate(0);
    }

    private void LoadPos()
    {
        transform.localPosition = new Vector3(posX, posY, 0);
        rope.transform.localPosition = new Vector3(posX, posY - 1, 0.3f);
        UpdateRopeTop();
    }

    private void SavePos()
    {
        posX = transform.localPosition.x;
        posY = transform.localPosition.y;
    }

    private void OnDisable()
    {
        SavePos();
    }
}
