using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : FateMonoBehaviour
{
    [SerializeField] float radius = 1f;
    [SerializeField] GameObject gemMesh = null;
    [SerializeField] Transform gemPartsParent = null;
    [SerializeField] LayerMask layerMask;
    [SerializeField] SoundEntity sound;

    List<Ore> gemParticles = new();
    List<Ore> dirts = new();
    bool destructed = false;

    private void Awake()
    {
        for (int i = 0; i < gemPartsParent.childCount; i++)
        {
            Ore ore = gemPartsParent.GetChild(i).GetComponent<Ore>();
            gemParticles.Add(ore);
            ore.SetOnGetDug(Destruct);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Ore ore = colliders[i].GetComponent<Ore>();
            if (ore != null && !dirts.Contains(ore) && !gemParticles.Contains(ore))
                dirts.Add(ore);
        }
    }

    private void Start()
    {
        if (destructed) gemMesh.SetActive(false);
    }

    public void Destruct()
    {
        if (destructed) return;
        GameManager.Instance.PlaySoundOneShot(sound);
        //AdManager.Instance.ShowInterstitial();
        destructed = true;

        gemMesh.SetActive(false);

        for (int i = 0; i < gemParticles.Count; i++)
            if (!gemParticles[i].IsDug()) gemParticles[i].GetDug();

        for (int i = 0; i < dirts.Count; i++)
            if (dirts[i] != null && !dirts[i].IsDug()) dirts[i].DestroyOre();

    }
}
