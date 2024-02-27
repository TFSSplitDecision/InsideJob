using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shoots multiple bullets in a spread angle
/// </summary>
public class SpreadShot : BaseShot
{
    public SpreadShot(GameObject owner, BulletSpawner bulletSpawner) : base(owner, bulletSpawner)
    {
    }

    public override void Shoot(ShotData shotData, GameObject bullet, float damage)
    {
        float spreadAngle = shotData.spreadAngle;
        int bulletCount = shotData.bulletCount;
        float bulletDamage = damage / (float)bulletCount;

        float stepAngle = spreadAngle / (bulletCount - 1);
        float fullAngle = -spreadAngle / 2f;

        Vector3 bulletDirection = m_transform.forward;

        // cannot invoke OnShoot directly for some reason
        // that's why I'm calling this function in BaseSHot
        InvokeOnShoot(m_transform.forward);

        bulletDirection = Quaternion.Euler(0f, -spreadAngle, 0f) * bulletDirection;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(0f, stepAngle, 0f);
            bulletDirection = bulletRotation * bulletDirection;
            m_bulletSpawner.SpawnBullet(bulletDirection, bullet, bulletDamage);
        }

        
    }
        
}
