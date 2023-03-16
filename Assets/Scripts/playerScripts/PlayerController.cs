using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Max values")]
    public int MaxHp;
    public int MaxJumps;
    public float MoveSpeed;
    public float slowTime;
    public float maxCharge;


    [Header("Cur Values")]
    public int CurHp;
    public int CurJumps;
    public int Score;
    public float CurMoveInput;
    public bool isSlowed;
    public float curSpeed;
    public float whenSlowed;
    public float curCharge;
    public bool isCharging;

    [Header("Mods")]
    public float JumpForce;
    public float chargeRate;

    [Header("Audio")]
    public AudioClip[] playerfx;
    //jump 0
    //hit 1

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D Rig;
    [SerializeField]
    private Animator Anim;
    [SerializeField]
    private AudioSource Audio;
    [SerializeField]
    private Transform muzzle;
    public PlayerContainerUI uiContainer;
    public GameObject DeathEffectPrefab;

    [Header("Attacking")]
    [SerializeField]
    private PlayerController curAttacker;
    public float attackRate;
    public float lastAttackTime;
    public float attackVelocity;
    public float attackDmg;
    public GameObject[] attackPrefabs;


    private void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
        Audio = GetComponent<AudioSource>();
        gameManager = GameManager.FindObjectOfType<GameManager>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();

    }

    // Start is called before the first frame update
    void Start()
    {
        CurHp = MaxHp;
        CurJumps = MaxJumps;
        Score = 0;
        curSpeed = MoveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10 || CurHp <= 0)
        {
            Die();
        }
        if (isCharging)
        {
            curCharge += chargeRate;
            uiContainer.updateChargeBar(curCharge, maxCharge);
        }
        

        if (isSlowed)
        {
            if (Time.time - whenSlowed > slowTime)
            {
                isSlowed = false;
                curSpeed = MoveSpeed;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D x in collision.contacts)
        {
            if (x.collider.CompareTag("Ground") && x.point.y < transform.position.y)
            {
                Audio.PlayOneShot(playerfx[1]);
                CurJumps = MaxJumps;
            }
            if ((x.point.x > transform.position.x || x.point.x < transform.position.x) && (x.point.y < transform.position.y))
            {
                if (CurJumps < MaxJumps)
                {
                    CurJumps++;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void youSuck()
    {
        Destroy(uiContainer.gameObject);
        Destroy(gameObject);
    }

    public void setUiContainer(PlayerContainerUI pcu)
    {
        this.uiContainer = pcu;
    }

    private void Jump()
    {
        Rig.velocity = new Vector2(Rig.velocity.x, 0);
        // play sound
        Audio.PlayOneShot(playerfx[0]);
        //add foce up
        Rig.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void Move()
    {
        Rig.velocity = new Vector2(CurMoveInput * curSpeed, Rig.velocity.y);

        if(CurMoveInput != 0.0f)
        {
            transform.localScale = new Vector3(CurMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    public void Die()
    {
        Destroy(Instantiate(DeathEffectPrefab, transform.position, Quaternion.identity), 1f);
        //Audio.PlayOneShot(playerfx[2]);
        if (curAttacker)
        {
            curAttacker.AddScore();
        }
        else {
            Score--;
            if (Score < 0)
            {
                Score = 0;
            }
           }
        uiContainer.updateScoreText(Score);

        Respawn();
    }

    public void AddScore()
    {
        Score++;
        uiContainer.updateScoreText(Score);
    }

    public void TakeDamage(int ammount, PlayerController attacker)
    {
        CurHp -= ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(CurHp, MaxHp);
    }

    public void TakeDamage(float ammount, PlayerController attacker)
    {
        CurHp -= (int)ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(CurHp, MaxHp);
    }

    public void TakeIceDamage(float ammount, PlayerController attacker)
    {
        whenSlowed = Time.time;
        CurHp -= (int)ammount;
        curAttacker = attacker;
        isSlowed = true;
        curSpeed /= 2;
        uiContainer.updateHealthBar(CurHp, MaxHp);
    }

    private void Respawn()
    {
        CurHp = MaxHp;
        CurJumps = MaxJumps;
        curAttacker = null;
        transform.position = gameManager.SpawnPoints[Random.Range(0, gameManager.SpawnPoints.Length)].position;
        Rig.velocity = Vector2.zero;
        curSpeed = MoveSpeed;
        uiContainer.updateHealthBar(CurHp, MaxHp);
    }

    private void spawnStdFireball(float damage, float speed)
    {
        //play sound here
        GameObject fireball = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireball.GetComponent<ProjectileScript>().onSpawn(damage, speed, this, transform.localScale.x);
    }

    private void spawnIceAttack(float damage, float speed)
    {
        //play sound here
        GameObject iceball = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceball.GetComponent<ProjectileScript>().onSpawn(damage, speed, this, transform.localScale.x);
    }

    private void spawnChargedAttack(float damage, float speed)
    {
        //play sound here
        GameObject chrball = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chrball.GetComponent<ProjectileScript>().onSpawn(damage, speed, this, transform.localScale.x);
        curCharge = 0;
        uiContainer.updateChargeBar(curCharge, maxCharge);
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        float x = context.ReadValue<float>();
        if (x > 0)
        {
            CurMoveInput = 1;
        }
        else if (x < 0)
        {
            CurMoveInput = -1;
        }
        else
        {
            CurMoveInput = 0;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed jump button");
            if (CurJumps > 0)
            {
                CurJumps--;
                Jump();
            }
        }
    }

    public void OnBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }
    }

    public void OnStdAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawnStdFireball(attackDmg, attackVelocity);
        }
    }

    public void OnChrAttackInput(InputAction.CallbackContext context)
    {
        print("charging");

        curCharge += chargeRate;
        if (context.phase == InputActionPhase.Performed)
        {
            isCharging = true;
        }
        if (context.phase == InputActionPhase.Canceled && Time.time - lastAttackTime > attackRate * 2)
        {
            isCharging = false;
            print("pressed Chr Attack button");
            spawnChargedAttack(curCharge, attackVelocity);
            lastAttackTime = Time.time;
            curCharge = 0;
        }
    }

    public void OnFrzAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            print("ice attack");
            lastAttackTime = Time.time;
            spawnIceAttack(attackDmg, attackVelocity);
        }
    }

    public void OnTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt1 button");
        }
    }

    public void OnTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt2 button");
        }
    }

    public void OnTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt3 button");
        }
    }

    public void OnTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt4 button");
        }
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
            print("pressed pause button");
        }
    }
}
