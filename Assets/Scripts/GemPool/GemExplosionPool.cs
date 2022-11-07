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
    private List<Particle> explosionPool;

    private void Awake()
    {
        explosionPool = new List<Particle>();
        CreateExplosionPool();
    }

    private void OnEnable()
    {
        ServiceLocator.Provide(this);
    }

    public Particle GetExplosion()
    {
        var explosionParticle = explosionPool.FirstOrDefault(item => !item.particleObject.activeInHierarchy);
        
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
        var explosion = new Particle();
        var explosionObject = Instantiate(explosionPrefab, poolHolder.position, Quaternion.identity, poolHolder);
        explosion.particleObject = explosionObject;
        explosion.particleObject.SetActive(false);
        explosion.particle = explosionObject.GetComponent<ParticleSystem>();
        explosionPool.Add(explosion);
    }
    
    public class Particle
    {
        public GameObject particleObject;
        public ParticleSystem particle;
    }
}