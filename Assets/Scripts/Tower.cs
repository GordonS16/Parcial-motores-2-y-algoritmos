using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [SerializeField] private string projectileType;
    [SerializeField] private float projectileSpeed;

    private SpriteRenderer mySpriteRenderer;

    private Monster target;


    //private Animator myAnimator;


    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]private int damage;

    [SerializeField] private float attackCooldown;

    public float ProjectileSpeed
    {
        get { return projectileSpeed;}
    }

    public Monster Target
    {
        get {return target;}
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    void Awake()
    {
        //myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Attack();
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();

        }
        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                Shoot();
                //myAnimator.SetTrigger("Attack");
                canAttack = false;
            }
        }
        else if (monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }

        if (target != null && !target.Alive)
        {
            target = null;
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        projectile.transform.position = transform.position;

        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            target = null;
        }
    }
}
