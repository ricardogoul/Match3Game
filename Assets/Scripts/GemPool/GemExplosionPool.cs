using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemExplosionPool : MonoBehaviour
{
    [SerializeField]
    private Transform poolHolder;
    [SerializeField] 
    private GameObject explosionPrefab;

    private const int PoolSize = 10;
    private List<ParticleSystem> explosionPool;

    private void Awake()
    {
        explosionPool = new List<ParticleSystem>();
        CreateExplosionPool();
    }

    private void OnEnable()
    {
        ServiceLocator.Provide(this);
    }

    public ParticleSystem GetExplosion()
    {
        var explosionParticle = explosionPool.FirstOrDefault(item => !item.gameObject.activeInHierarchy);
        
        if (explosionParticle != null)
            return explosionParticle;
        
        InstantiateExplosion();
        return GetExplosion();
    }

    private void CreateExplosionPool()
    {
        for (var i = 0; i < PoolSize; i++)
        {
            InstantiateExplosion();
        }
    }

    private void InstantiateExplosion()
    {
        var explosionObject = Instantiate(explosionPrefab, poolHolder.position, Quaternion.identity, poolHolder);
        explosionObject.SetActive(false);
        var explosion = explosionObject.GetComponent<ParticleSystem>();
        explosionPool.Add(explosion);
    }
}