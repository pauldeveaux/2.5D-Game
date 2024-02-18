using System;
using UnityEngine;
using System.Collections;
using MagicPigGames;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    public int m_attack { get; private set; } = 5;

    private Animator            m_animator;
    private Rigidbody           m_body;
    private GroundSensorManager   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool m_blocking = false;
    private bool m_dead;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    [SerializeField] private float invulnerabityCooldown = 1;
    private float m_timeSinceLastHit = 0;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    [SerializeField]
    private int health = 100;
    private int m_maxHealth;
    [SerializeField] private ProgressBar healthBar;
    private GameManager m_gameManager;


    
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body = GetComponent<Rigidbody>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorManager>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        m_maxHealth = health;
        m_timeSinceLastHit = Time.time - invulnerabityCooldown;
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update ()
    {
        
        if (m_dead)
            return;
        
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }


        if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            m_blocking = true;

            return;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_blocking = false;
        }

        if (m_blocking)
            return;

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling)
        {
            Vector3 movements = new Vector3(inputX * m_speed * transform.right.x, m_body.velocity.y, inputX * m_speed * transform.right.z);
            m_body.velocity = movements;
        }
            

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        /*if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }*/
            
        //Hurt
        /*else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");*/

        //Attack
        if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        /*if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body.velocity = new Vector3(m_facingDirection * m_rollForce, m_body.velocity.y, 0);
        }*/
            

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body.velocity = new Vector3(m_body.velocity.x, m_jumpForce, 0);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }


    public void SetRotation(Quaternion rotation)
    {
        this.transform.rotation = rotation;
    }

    public void TakeDamage(int damage)
    { 
        if(m_dead)
            return;

        if (m_blocking)
        {
            m_timeSinceLastHit = Math.Max((float)(Time.time-invulnerabityCooldown+0.4), m_timeSinceLastHit);
        }

        if (Time.time - m_timeSinceLastHit < invulnerabityCooldown)
            return;
        
        m_animator.SetTrigger("Hurt");
        m_timeSinceLastHit = Time.time;
        health -= damage;
        healthBar.SetProgress(Math.Max(0f, (health+0f) / m_maxHealth));
        if(health <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        m_dead = true;
        m_gameManager.OnGameOver();
    }
}
