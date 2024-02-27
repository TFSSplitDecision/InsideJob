using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseShot : IShot
{
    protected GameObject m_owner;
    protected BulletSpawner m_bulletSpawner;
    protected Transform m_transform;
    protected AudioSource m_audioSource;

    public event UnityAction<Vector3> OnShoot; // Event to indicate that a shot is fired

    public BaseShot(GameObject owner, BulletSpawner bulletSpawner)
    {
        m_owner = owner;
        m_transform = owner.transform;
        m_bulletSpawner = bulletSpawner;
        m_audioSource = owner.GetComponent<AudioSource>();
    }

    protected void InvokeOnShoot(Vector3 forward)
    {
        // Moved the code here so I can reuse it in SpreadShot -Christos
        OnShoot?.Invoke(forward);
    }

    public virtual void Shoot(ShotData shotData, GameObject bullet, float damage)
    {
        Vector3 forward = m_transform.forward;
        Debug.Log("Forward: " + forward);
        m_bulletSpawner.SpawnBullet(forward, bullet, damage);

        // Raise the OnShoot event with the direction of the shot
        InvokeOnShoot(forward);
    }

    public virtual void Stop()
    {
        // Does nothing here.
    }
}

