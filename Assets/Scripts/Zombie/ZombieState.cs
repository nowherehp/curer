using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombieState : MonoBehaviour
{
    // Health control
    public float health;
    public float Maxhealth;
    public GameObject healthbar;
    public Slider slider;

    public bool is_night = false;
    public bool sp = false;
    public bool ndmg = false;
    public bool boss = false;

    public Light flashlight;
    public float detectionDistance = 30f;

    // Color Controll
    public float timmer = 30f;
    private Renderer objectRenderer;
    public int zombie_state = 0;
    private Color[] colors = {new Color(1f, 0.5f, 0f), Color.red ,new Color(0.5f, 0f, 0.5f) };

    // Movement Control
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        // Color ini
        objectRenderer = GetComponent<Renderer>();
        if (boss)
        {
            objectRenderer.material.color = new Color(0f, 0.5f, 0f, 1f);
        }
        else
        {
            objectRenderer.material.color = colors[zombie_state];
        }
        

        // Health ini
        if(sp == false)
        {
            health = Maxhealth;
            
        }
        slider.value = calHealth();

       
        // State ini
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (SceneManager.GetActiveScene().name == "Hospital")
        {
            if (healthbar != null)
            {
                healthbar.SetActive(false);
            }
        }
        else
        {
            if (healthbar != null)
            {
                healthbar.SetActive(true);
            }
        }
    }

    private void colordeeper()
    {
        if (objectRenderer != null && zombie_state < 2)
        {
            zombie_state++;
            objectRenderer.material.color = colors[zombie_state];
            timmer = 30f;
        }
    }

    public void colorlighter()
    {
        if (objectRenderer != null && zombie_state > 0)
        {
            zombie_state--;
            objectRenderer.material.color = colors[zombie_state];
            health = Maxhealth;
        }
        else if(zombie_state == 0)
        {
            curedCount.Instance.count++;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (boss)
        {
            return;
        }
        if (ndmg && health <= 50)
        {
            return;

        }
        health -= damage;
    }

    private float calHealth()
    {
        return health / Maxhealth;
    }

    private void HealthCheck()
    {
        timmer -= Time.deltaTime;
        if (timmer <= 0 && !boss)
        {
            colordeeper();
        }

  
        if (health <= 0)
        {
            curedCount.Instance.killed++;
            Destroy(gameObject);
        }
        if (health > Maxhealth)
        {
            health = Maxhealth;
        }
        slider.value = calHealth();
    }

    void Update()
    {
        HealthCheck();
        if (DayNightCycle.Instance != null)
        {
            is_night = DayNightCycle.Instance.IsNight();
        }
        CheckFlashlight();
    }

    private bool Inlightrange(Light spotlight, GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - spotlight.transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > spotlight.range) return false;

        float angleToTarget = Vector3.Angle(spotlight.transform.forward, directionToTarget);
        if (angleToTarget > spotlight.spotAngle / 2) return false;

        return true;
    }

    private void CheckFlashlight()
    {
        if (SceneManager.GetActiveScene().name == "Hospital")
        {
            // Initially set health bar to false (hidden)
            healthbar.SetActive(false);

            // Check if flashlight is enabled
            if (flashlight != null && flashlight.enabled)
            {
                if(Inlightrange(flashlight , gameObject))
                { 
                    healthbar.SetActive(true);
                    gameObject.GetComponent<Zombie>().isIlluminated = true;
                    return;
                }
            }
            gameObject.GetComponent<Zombie>().isIlluminated = false;
            healthbar.SetActive(false);
        }
        else
        {
            // Always show health bar in other scenes
            if (healthbar != null)
            {
                healthbar.SetActive(true);
            }
        }
    }


}
