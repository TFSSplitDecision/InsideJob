using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    #region Components
    private CharacterController m_charController;
    private ShootManager m_shootManager;
    #endregion

    public enum SpecialType { Dash = 0, Blink = 1 }
    [SerializeField] private SpecialType m_SpecialType;
    public SpecialType specialType => m_SpecialType;

    private Vector3 m_lookTarget;
    public float moveSpeed = 1f;
    public bool isMoving => m_charController.velocity.magnitude > 0.0f;

    [SerializeField] private Dash dash;
    [SerializeField] private Blink blink;
    private Vector3 m_lastMoveDir;

    public AudioClip dashSound; // Dash sound effect
    private AudioSource audioSource; 

    private void Start()
    {
        // Get or add AudioSource component to the PlayerController GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        //Movement
        m_charController = GetComponent<CharacterController>();
        m_shootManager = GetComponent<ShootManager>();
        // Initialize dash ability
        dash.Init(gameObject);

        // Call the OnDashActivated event and play the dash sound effect
        dash.OnDashActivated += PlayDashSound;

        blink.Init(gameObject);
    }

    private void PlayDashSound()
    {
        if (dashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }

    private void Update()
    {
        // Player Look Mechanics
        m_lookTarget = InputUtils.groundPoint;
        m_lookTarget.y = transform.position.y;
        if (Vector3.Distance(m_lookTarget, transform.position) > 0.1f)
            transform.LookAt(m_lookTarget, Vector3.up);

        // Movement mechanics
        if (!dash.isDashing)
            MovePlayer();
        
        // Special Movement mechanics
        bool buttonPress = InputUtils.dashed;
        switch (specialType)
        {
            case SpecialType.Dash:

                if (buttonPress)
                    dash.Begin(m_lastMoveDir);
                // Update dash logic
                dash.Update();
                break;

            case SpecialType.Blink:

                if (dash.isDashing)
                    dash.Cancel();
                if (buttonPress)
                    blink.Begin(); //Debug.Log("dash is in blink");
                // Update blink logic
                blink.Update(m_lookTarget + Vector3.up);
                break;
        }

        // Shooting Mechanics
        if (InputUtils.primaryFiring) m_shootManager.PrimaryFire();
        if (InputUtils.secondaryFiring) m_shootManager.SecondaryFire();

    }

    private void MovePlayer()
    {
        Vector3 movement = InputUtils.move3d * moveSpeed;
        m_charController.SimpleMove(movement);

        if (movement.magnitude > float.Epsilon)
            m_lastMoveDir = movement;
    }
    
    #if UNITY_EDITOR
    // Test code. Delete on production.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(m_lookTarget, Vector3.one * 0.1f);
        if( specialType == SpecialType.Blink )
        {
            Gizmos.color = blink.checkHit ? Color.red : Color.green;
            Gizmos.DrawCube(blink.checkPoint, Vector3.one * 0.3f);
        }
    }
#endif
}

