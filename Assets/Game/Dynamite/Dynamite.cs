using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Dynamite : FateMonoBehaviour, IPooledObject
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float distanceCheckInterval = 0.5f;
    [SerializeField] float maxDistancaToExplode = 0.1f;
    [SerializeField] float delay = 1.5f;
    [SerializeField] float releaseDelayAfterExp = 1.5f;
    [SerializeField] ParticleSystem effect;
    [SerializeField] Transform mesh;
    [SerializeField] Transform fuse;
    [SerializeField] float effectSizeMultiplier = 2;
    [SerializeField] Vector3 baseFusePos;
    [SerializeField] Vector3 targetFusePos;
    [SerializeField] float baseForce;
    [SerializeField] Vector2 randomForceRange;
    [SerializeField] SoundEntity bombSound;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameEvent onStoneBombed;

    bool exploded = false;
    Vector3 lastPos = Vector3.zero;
    float range;
    float power;
    float maxSquaredDistanceToExplode;

    private void Start()
    {
        maxSquaredDistanceToExplode = maxDistancaToExplode * maxDistancaToExplode;
        InvokeRepeating(nameof(CheckStop), 0, distanceCheckInterval);
    }

    public void Fall(float range, float power)
    {
        effect.transform.localScale = Vector3.one * range * effectSizeMultiplier;

        mesh.DOScale(1, 0.2f).SetEase(Ease.OutSine);

        this.range = range;
        this.power = power;
    }

    private void CheckStop()
    {
        if (!exploded)
        {
            if ((transform.position - lastPos).sqrMagnitude < maxSquaredDistanceToExplode) Explode();
            else lastPos = transform.position;
        }
    }

    private void Explode()
    {
        exploded = true;
        fuse.DOLocalMove(targetFusePos, delay).OnComplete(() =>
        {
            GameManager.Instance.PlayHaptic();
            GameManager.Instance.PlaySoundOneShot(bombSound);
            effect.Play();
            mesh.gameObject.SetActive(false);

            OptimizationManager.Instance.UnlockArea(transform.position, range + 1, false);

            Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);

            List<Ore> ores = new();
            for (int i = 0; i < colliders.Length; i++)
            {
                Ore ore = colliders[i].GetComponent<Ore>();
                if (ore != null && !ore.IsDestroyed && !ores.Contains(ore) && ore.Power() <= power)
                {
                    ores.Add(ore);
                    if (ore.Type == -1 && !ore.IsDug()) onStoneBombed.Raise();
                    if (ore.isActiveAndEnabled && !ore.IsDug()) ore.GetDug();
                    ore.Rigidbody.AddForce((baseForce + UnityEngine.Random.Range(randomForceRange.x, randomForceRange.y)) * Vector3.up, ForceMode.VelocityChange);
                }
            }
            DOVirtual.DelayedCall(releaseDelayAfterExp, () => { Release(); });
        });
    }


    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        mesh.gameObject.SetActive(true);
        mesh.transform.DOKill();
        mesh.localScale = Vector3.zero;
        fuse.localPosition = baseFusePos;
        rb.isKinematic = false;
        lastPos = Vector3.zero;
        exploded = false;
    }

    public void OnRelease()
    {
        rb.isKinematic = true;
    }

}
