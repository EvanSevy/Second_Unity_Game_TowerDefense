using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private float navigationUpdate;  // I believe this is to make sure graphical updates don't clip and result in a missing of a direction change
    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private int rewardAmount;

    private int target = 0;
    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator anim;
    private float navigationTime = 0;  // I believe this is to make sure graphical updates don't clip and result in a missing of a direction change
    private bool isDead = false;

    public bool IsDead { get { return isDead; } }

    // Use this for initialization
    void Start()
    {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        GameManager.Instance.RegisterEnemy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (wayPoints != null && !isDead)
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate)
            {
                if (target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
                }
                navigationTime = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Checkpoint")
            target += 1;
        else if (other.tag == "Finish")
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.IsWaveOver();
        }
        else if (other.tag == "projectile")
        {
            Projectile newP = other.gameObject.GetComponent<Projectile>();
            EnemyHit(newP.AttackStrength);
            Destroy(other.gameObject);
        }
    }
    public void EnemyHit(int hitPoints)
    {
        if (healthPoints - hitPoints > 0)
        {
            healthPoints -= hitPoints;
            anim.Play("Hurt");
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
        }
        else
        {
            anim.SetTrigger("didDie");
            die();
        }
    }
    public void die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        GameManager.Instance.TotalKilled += 1;
        GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        GameManager.Instance.AddMoney(rewardAmount);
        GameManager.Instance.IsWaveOver();
    }
}
