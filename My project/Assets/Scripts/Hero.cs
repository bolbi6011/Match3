using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hero : Entity
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int health;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource missAttack;
    [SerializeField] private AudioSource attackMob;
    public float rotationSpeed = 10f;
    private bool isGrounded = false;
    public LayerMask notPlayerMask; 

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;
    public Transform groundCheckerTransform; 


    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy; 

    private Rigidbody rb;
    private Animator anim;
    private SpriteRenderer sprite;
    
    public static Hero Instance { get; set; }
  

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        lives = 5;
        health = lives;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        Instance = this;
        isRecharged = true;
    }
    private void FixedUpdate()
    {
        CheckGround();
    }
    public override void GetDamage()
    {
        health -= 1;
        damageSound.Play();
        if (health == 0)
        {
            foreach (var h in hearts)
                h.sprite = deadHeart;
            Die();

        }
    }
    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 directionVector = new Vector3(h, 0, v);
        if (directionVector.magnitude > Mathf.Abs(0.1f))
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(directionVector), Time.deltaTime * rotationSpeed);
        anim.SetFloat("speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
        Vector3 moveDir = Vector3.ClampMagnitude(directionVector, 1) * speed;
        rb.velocity = new Vector3(moveDir.x,rb.velocity.y, moveDir.z);
        rb.angularVelocity = Vector3.zero;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Physics.Raycast(groundCheckerTransform.position, Vector3.down, 0.2f, notPlayerMask))
        {
            anim.SetBool("isInAir", false);
        }
        else
        {
            anim.SetBool("isInAir", true);
        }
       
        

            if (isGrounded && !isAttacking) State = States.idle;

        if (!isAttacking && Input.GetButton("Horizontal"))
            Run();
       // if (!isAttacking && isGrounded && Input.GetButtonDown("Jump"))
       //     Jump();
        if (Input.GetButtonDown("Fire1"))
            Attack();

        if (health > lives)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)

                hearts[i].sprite = aliveHeart;
            else
                hearts[i].sprite = deadHeart;

            if (i < lives)
                hearts[i].enabled = true;
           
        }

    }
    private void Run()
    {
       //
       //if (isGrounded) State = States.run;

        
    }
    void Jump()

    {
        RaycastHit hit;
        if(Physics.CheckSphere(groundCheckerTransform.position, 0.3f, notPlayerMask))
        {
            anim.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Did not find ground layer");
        }
        
        jumpSound.Play();
    }
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.6f);
        isGrounded = collider.Length > 1;
        if (!isGrounded) State = States.jump; 

    }
    private void Attack()
    {
        if (isGrounded && isRecharged)
        {
            State = States.attack;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }
    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);
        if (colliders.Length == 0)
            missAttack.Play();
        else
            attackMob.Play();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i]. GetComponent<Entity> (). GetDamage();
            StartCoroutine(EnemyOnAttack(colliders[i]));
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }
    private IEnumerator EnemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1f, 0.4375f, 0.43f);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1);
    }


}

public enum States 
{
    
    idle,
    run,
    jump,
    attack
}
