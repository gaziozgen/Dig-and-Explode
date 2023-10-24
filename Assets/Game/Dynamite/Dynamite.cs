using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Dynamite : FateMonoBehaviour, IPooledObject
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float delay = 1.5f;
    [SerializeField] float releaseDelay = 1.5f;
    [SerializeField] ParticleSystem effect;
    [SerializeField] Transform mesh;
    [SerializeField] Transform fuse;
    [SerializeField] float effectSizeMultiplier = 2;
    [SerializeField] Vector3 baseFusePos;
    [SerializeField] Vector3 targetFusePos;
    [SerializeField] float baseForce;
    [SerializeField] Vector2 randomForceRange;


    public void Explode(float range, float power)
    {
        effect.transform.localScale = Vector3.one * range * effectSizeMultiplier;

        mesh.DOScale(1, 0.2f).SetEase(Ease.OutSine);
        fuse.DOLocalMove(targetFusePos, delay).OnComplete(() =>
        {
            effect.Play();
            mesh.gameObject.SetActive(false);

            Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);

            List<Ore> ores = new();
            for (int i = 0; i < colliders.Length; i++)
            {
                Ore ore = colliders[i].GetComponent<Ore>();
                if (ore != null && !ores.Contains(ore) && ore.Power() <= power)
                {
                    ores.Add(ore);
                    if (!ore.IsDug()) ore.GetDug();
                    ore.GetRigidbody().AddForce((baseForce + UnityEngine.Random.Range(randomForceRange.x, randomForceRange.y)) * Vector3.up, ForceMode.VelocityChange);
                }
            }
                
            DOVirtual.DelayedCall(releaseDelay, () => { Release(); });
        });


    }


    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        mesh.gameObject.SetActive(true);
        mesh.transform.DOKill();
        mesh.localScale = Vector3.zero;
        fuse.localPosition = baseFusePos;
    }

    public void OnRelease()
    {

    }

}
