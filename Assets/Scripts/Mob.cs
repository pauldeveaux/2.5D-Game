using System;
using System.Collections;
using System.Collections.Generic;
using MagicPigGames;
using UnityEngine;
using UnityEngine.UIElements;


public class Mob : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private HorizontalProgressBar HealthBar;
    private float m_maxHealth;
    private GameObject playerGO;
    private HeroKnight player;
    private Rigidbody rb;
    [SerializeField] private int damage = 25;
    private float timeSinceJump = 0f;
    [SerializeField] private float jumpCooldown = 1;
    [SerializeField] private float jumpSpeed = 1;
    private AudioSource audio;
    public float time;

    void Awake()
    {
        m_maxHealth = health;
        playerGO = GameObject.Find("HeroKnight");
        player = playerGO.GetComponent<HeroKnight>();
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }
    

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {

        if (timeSinceJump >= jumpCooldown)
        {
            Vector3 direction = (playerGO.transform.position - transform.position).normalized;
        
            
            rb.AddForce(direction * jumpSpeed + new Vector3(0,2,0), ForceMode.Impulse);
            timeSinceJump = 0;
        }
        else
        {
            timeSinceJump += Time.deltaTime;
        }
    }

    void OnTakeDamage(int damage)
    {
        health -= damage;
        HealthBar.SetProgress(Math.Max(health / m_maxHealth,0));
        if(health <= 0)
            OnDeath();
    }
    
    
    void OnDeath()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        LayerMask layer = other.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Attack"))
        {
            OnTakeDamage(player.m_attack);
        }
        
    }

    
    private void OnCollisionStay(Collision other)
    {
        LayerMask layer = other.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Player"))
        {
            player.TakeDamage(damage);
        }
    }
}
