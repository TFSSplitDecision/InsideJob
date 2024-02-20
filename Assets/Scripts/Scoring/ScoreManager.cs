using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Singleton pattern for score manager

    public static ScoreManager instance;

    private int score;
    public int baseEnemyKillScore = 50;
    public int earlyWaveBonus = 50;
    public int remainingHealthBonus = 100;
    
    // Combo system parameters
    public int multiplier;
    public float comboDuration = 2.0f;
    public int comboThresholdX = 5; // Threshold for 2x multiplier
    public int comboThresholdY = 10; // Threshold for 4x multiplier
    public int comboThresholdZ = 20; // Threshold for 8x multiplier
    private Coroutine comboTimerCoroutine; // Reference to the active combo timer coroutine
    public int currentComboCount = 0;

    public HealthManager healthManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
    }

    // Public function to return the current score when called
    public int GetScore()
    {
        return score;
    }

    // TODO: Add a function to increase score that don't use the multiplier for events like early wave spawn bonus
    public int EarlyWaveSpawnBonus()
    {
        score += earlyWaveBonus;
        return score;
    }

    // Public function that gives bonus score at the end of level based on player's current health
    public int ScoreHealthBonus()
    {
        float currentHealth = healthManager.GetHealth();
        score += Mathf.RoundToInt(currentHealth);
        return score;
    }

    // OR ALTERNATIVELY: Add a generic score increase function that will be called manually

    
    // Combo system

    // The RegisterKill function should be called everytime an enemy is killed. We could also update this function to
    // subscribe to an event listener instead if the enemy kills work on an event system
    public void RegisterKill()
    {
        currentComboCount++;
        if (comboTimerCoroutine != null)
        {
            StopCoroutine(comboTimerCoroutine);
        }
        comboTimerCoroutine = StartCoroutine(ComboTimer());
        UpdateMultiplier();

        score += baseEnemyKillScore * multiplier;
       
        // Test items, feel free to comment them out or delete
        Debug.Log("Score: " + score);
        Debug.Log("Multiplier: " + multiplier);
        Debug.Log("Combo Count: " + currentComboCount);
    }

    // This is the coroutine for the combo timer
    private IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(comboDuration);
        ResetCombo();
    }

    // This function will reset the combo to 0
    private void ResetCombo()
    {
        currentComboCount = 0;
        multiplier = 1;
        comboTimerCoroutine = null;
    }

    // This function is called every time a kill happens within a combo, it will check the current combo count and update
    // the multiplier accordingly
    private void UpdateMultiplier()
    {
        if (currentComboCount >= comboThresholdZ)
        {
            multiplier = 8;
        }
        else if (currentComboCount >= comboThresholdY)
        {
            multiplier = 4;
        }
        else if (currentComboCount >= comboThresholdX)
        {
            multiplier = 2;
        }
        else
        {
            multiplier = 1;
        }
    }
}

