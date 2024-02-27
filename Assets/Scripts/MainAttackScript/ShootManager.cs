using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootManager : MonoBehaviour
{
    [SerializeField] private Arm1 m_primary;
    [SerializeField] private Arm2 m_secondary;
    [SerializeField] private Transform m_shootPoint;

    private SingleShot singleShot;
    private SpreadShot spreadShot;
    private LaserShot laserShot;

    private BulletSpawner m_bulletSpawner;
    private InventoryManager m_inventory;

    private AudioSource m_audioSource; // audiosource variable

    // Live variables
    private float m_lastFire1;
    private float m_lastFire2;

    public void Equip(Weapon weapon)
    {
        if (weapon == null) return;
        if( weapon is Arm1 ) m_primary = weapon as Arm1;
        if (weapon is Arm2) m_secondary = weapon as Arm2;
    }

    public AudioClip gunSound; // so we can add the gun shot sound on the inspector

    void Start()
    {
        //need to null check A1 somehow
        m_inventory = FindObjectOfType<InventoryManager>();

        m_audioSource = GetComponent<AudioSource>(); // Get AudioSource from the GameObject


        m_bulletSpawner = new BulletSpawner(gameObject, m_shootPoint);
        singleShot = new SingleShot(gameObject, m_bulletSpawner);
        spreadShot = new SpreadShot(gameObject, m_bulletSpawner);
        laserShot = new LaserShot(gameObject, m_bulletSpawner);

        m_lastFire1 = 100.0f;
        m_lastFire2 = 100.0f;

        singleShot.OnShoot += PlayGunSound;
        spreadShot.OnShoot += PlayGunSound;
    }

    private void PlayGunSound(Vector3 direction)
    {
        if( m_audioSource == null )
            m_audioSource = GetComponent<AudioSource>();

        // Check if gunSound is assigned and play the sound
        if (gunSound != null && m_audioSource != null)
        {
            m_audioSource.PlayOneShot(gunSound);
        }
    }

    private void Fire( Weapon weapon, ref float lastFire )
    {
        ShotData shotData = weapon.shotData;
        GameObject bullet = weapon.bullet;

        // Get the modifiers
        float attackMult = 1.0f;
        float attackAdd = 0.0f;
        float speedMult = 1.0f;
        float speedAdd = 0.0f;
        if( m_inventory != null )
        {
            attackMult = m_inventory.getAttackDamageMultiply();
            attackAdd = m_inventory.getAttackDamageAdd();
            speedMult = m_inventory.getAttackSpeedMultiply();
            speedAdd = m_inventory.getAttackSpeedAdd();
        }

        // Fire Rate Check
        float fireRate = (shotData.fireRate / speedMult) - speedAdd;
        fireRate = Mathf.Clamp(fireRate, 0.05f, 3.0f);
        if (lastFire < fireRate)  return;
        
        // Modify Damage
        float damage = (weapon.flatDamage * attackMult) +attackAdd;

        // Select the right shot mechanics
        BaseShot shot = singleShot;
        float spreadAngle = shotData.spreadAngle;
        if (spreadAngle > 5.0f) shot = spreadShot;

        // Special case if laser type
        if (weapon is Laser) shot = laserShot;

        // Execute the shot
        shot.Shoot(shotData, bullet, damage);
        lastFire = 0.0f;
    }
    public void PrimaryFire() => Fire(m_primary, ref m_lastFire1);
    public void SecondaryFire() => Fire(m_secondary, ref m_lastFire2);

    void UpdateEquippedA1(Arm1 incoming)
    {
        m_primary = incoming;
    }

    void UpdateEquippedA2(Arm2 incoming)
    {
        m_secondary = incoming;
    }

    private void Update()
    {
        m_lastFire1 += Time.deltaTime;
        m_lastFire2 += Time.deltaTime;
    }

}
