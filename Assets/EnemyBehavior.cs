using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("Enemy")]
    private Rigidbody2D rb;
    [SerializeField] private GameObject LimiteGauche;
    [SerializeField] private GameObject LimiteDroite;
    private Transform currentPoint;
    public float speed;
    private int enemyMask;
    public int maxHealth = 100;
    public int health;

    [Header("Character's link")]
    [SerializeField] private BoxCollider2D boxCharacter;
    private int limitsMask;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = LimiteDroite.transform;

        enemyMask = LayerMask.GetMask("Enemy");
        limitsMask = LayerMask.GetMask("EnemysZone");
        
        health = maxHealth;
    }

    void Update()
    {
        //Basic Behavior
        Vector2 point = currentPoint.position - transform.position;
        if (currentPoint == LimiteDroite.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == LimiteDroite.transform)
        {
            currentPoint = LimiteGauche.transform;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == LimiteGauche.transform)
        {
            currentPoint = LimiteDroite.transform;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (boxCharacter.IsTouchingLayers(limitsMask))
        {
            gameObject.GetComponent<Animator>().Play("Attack");
        }
    }
}