using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { STORM, FIRE, FROST, POISON, NONE }

public abstract class Tower : MonoBehaviour
{
    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    private Animator myAnimator;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float debuffDuration;

    [SerializeField]
    private float proc;

    private SpriteRenderer mySpriteRenderer;

    private Monster target;

    public int Level { get; protected set; }

    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    public TowerUpgrade[] Upgrades { get; protected set; }

    public Element ElementType { get; protected set; }

    public int Price { get; set; }

    public float ProjectileSpeed => projectileSpeed;

    public Monster Target => target;

    public int Damage => damage;

    public float DebuffDuration
    {
        get => debuffDuration;
        set => debuffDuration = value;
    }

    public float Proc
    {
        get => proc;
        set => proc = value;
    }

    public TowerUpgrade NextUpgrade
    {
        get
        {
            if (Upgrades.Length > Level - 1)
            {
                return Upgrades[Level - 1];
            }
            return null;
        }
    }

    void Awake()
    {
        myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Level = 1;
    }

    void Update()
    {
        Attack();
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
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

        if (target == null && monsters.Count > 0 && monsters.Peek().IsActive)
        {
            target = monsters.Dequeue();
        }

        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                Shoot();
                myAnimator.SetTrigger("Attack");
                canAttack = false;
            }
        }

        if (target != null && (!target.Alive || !target.IsActive))
        {
            target = null;
        }
    }

    public virtual string GetStats()
    {
        if (NextUpgrade != null)
        {
            return string.Format("\nLevel: {0} \nDamage: {1} <color=#00ff00ff> +{4}</color>\nProc: {2}% <color=#00ff00ff>+{5}%</color>\nDebuff: {3}sec <color=#00ff00ff>+{6}</color>", Level, damage, proc, DebuffDuration, NextUpgrade.Damage, NextUpgrade.ProcChance, NextUpgrade.DebuffDuration);
        }

        return string.Format("\nLevel: {0} \nDamage: {1} \nProc: {2}% \nDebuff: {3}sec", Level, damage, proc, DebuffDuration);
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public virtual void Upgrade()
    {
        GameManager.Instance.Currency -= NextUpgrade.Price;
        Price += NextUpgrade.Price;
        damage += NextUpgrade.Damage;
        proc += NextUpgrade.ProcChance;
        DebuffDuration += NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public abstract Debuff GetDebuff();

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            target = null;
        }
    }
}
