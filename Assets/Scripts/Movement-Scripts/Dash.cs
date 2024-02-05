using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Internal utility script for dashing.
/// Heavily based on Bailey's PlayerDash
/// </summary>
[System.Serializable]
public class Dash
{

    private GameObject m_owner;
    private Transform m_transform;
    private CharacterController m_charController;


    [Header("Parameters")]
    [SerializeField] private int m_maxCharges = 3;
    [SerializeField] private float m_dashSpeed = 12.0f;
    [SerializeField] private float m_dashDuration = 1.0f;
    [SerializeField] private float m_dashDistance = 12.0f;
    [SerializeField] private float m_recoveryDelay = 3.0f;

    [Header("Live Variables")]
    [SerializeField, ReadOnly] private bool m_dashing;
    [SerializeField, ReadOnly] private Vector3 m_startPos;
    [SerializeField, ReadOnly] private Vector3 m_direction;
    [SerializeField, ReadOnly] private int m_remainingCharges;
    [SerializeField, ReadOnly] private float m_dashTimer;
    [SerializeField, ReadOnly] private float m_recoveryTimer;

    public bool isDashing => m_dashing;

    /// <summary>
    /// Use this in place of a constructor! 
    /// Serialized classes in the inspector of a Monobehavior are constructed automatically by the engine.
    /// Thus, construction will reset the inspector values of Dash set during editing.
    /// </summary>
    /// <param name="owner"></param>
    public void Init( GameObject owner )
    {
        m_owner = owner;
        m_transform = owner.transform;
        m_charController = owner.GetComponent<CharacterController>();
        m_dashing = false;

        m_remainingCharges = m_maxCharges;
    }


    /// <summary>
    /// Begins the dash ability sequence.
    /// </summary>
    public void Begin()
    {
        // Guard conditions
        if (m_dashing) return;
        if (m_remainingCharges <= 0) return;

        m_dashing = true;
        m_startPos = m_transform.position;

        // Calculate dashing direction
        Vector3 myPos = m_transform.position;
        Vector3 targetPos = GetMouseWorldPosition();
        targetPos.y = myPos.y;
        m_direction = (targetPos - myPos).normalized;


        m_remainingCharges--;
        m_dashTimer = 0.0f;
    }

    /// <summary>
    /// TODO: Replace this with the implemented InputUtils, to avoid duplicate code.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        float t = 0.0f;
        ground.Raycast(ray, out t);

        return ray.GetPoint(t);
    }


    /// <summary>
    /// Recovery logic.
    /// Recovers 1 charge every 3 seconds.
    /// Adapted from the coroutine code.
    /// </summary>
    private void Recovery()
    {
        if (m_remainingCharges >= m_maxCharges) return;
        m_recoveryTimer += Time.deltaTime;
        if( m_recoveryTimer > m_recoveryDelay )
        {
            m_recoveryTimer = 0.0f;
            m_remainingCharges++;
        }
    }

    /// <summary>
    /// Dashing logic.
    /// Will move the character in the dashing direction for a maximum time or distance.
    /// Adapted from the coroutine code.
    /// </summary>
    private void Dashing()
    {
        Vector3 displacement = m_direction * m_dashSpeed * Time.deltaTime;
        m_charController.Move(displacement);

        // Maximum distance check
        Vector3 currentPos = m_transform.position;
        float distance = Vector3.Distance(m_startPos, currentPos);
        if (distance > m_dashDistance) m_dashing = false;

        // Maximum duration check
        m_dashTimer += Time.deltaTime;
        if( m_dashTimer > m_dashDuration ) m_dashing = false;
    }


    /// <summary>
    /// Updates the dash ability.
    /// Must be called externally by the owner
    /// </summary>
    public void Update()
    {
        if (m_dashing) Dashing();
        Recovery();
    }
}
