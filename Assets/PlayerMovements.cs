using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    Vector3 startingPosition;
    public float moveSpeed;
    public bool isJumping = false;
    public float jumpForce;

     [Header("Contrôles Joueur")]
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode leftKey1 = KeyCode.LeftArrow;

    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode rightKey1 = KeyCode.RightArrow;

    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode jumpKeyController = KeyCode.Joystick1Button0;
    
    [Header("Colliders")]
    [SerializeField] private Rigidbody2D rgbd;
    [SerializeField] private BoxCollider2D boxCharacter;

    [Header("Layers")]
    private bool isTouchingLayers;
    private int groundMask;
    private int wallsMask;
    private int waterMask;
    private int sewersEntrancesMask;
    private int laddersMask;
    private int healMask;

    [Header("Variables")]
    private bool isGrounded = true;
    private bool isTouchingWall;
    private bool isTouchingWater;
    
     [Header("HP System")]
    public float health;
    private float lerpTimer;
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    void Start()
    {
        startingPosition = gameObject.transform.position;
        rgbd.gameObject.GetComponent<Rigidbody2D>();

        groundMask = LayerMask.GetMask("Ground");
        wallsMask = LayerMask.GetMask("Walls");
        waterMask = LayerMask.GetMask("Water");
        sewersEntrancesMask = LayerMask.GetMask("SewersEntrances");
        laddersMask = LayerMask.GetMask("Ladders");
        healMask = LayerMask.GetMask("Heal");

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rgbd.velocity = new Vector2(horizontalInput * moveSpeed, rgbd.velocity.y);

        if (isGrounded == true)
        {
            //Flip player when moving left or right
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            //Déplacements de base (droite, gauche, saut basique)
            if (Input.GetKey(leftKey) || Input.GetKey(leftKey1) || Input.GetAxis("Horizontal")<0)
            {
                rgbd.AddForce(Vector2.left * moveSpeed);
                gameObject.GetComponent<Animator>().Play("Running");
            }
            if (Input.GetKey(rightKey) || Input.GetKey(rightKey1) || Input.GetAxis("Horizontal")>0)
            {
                rgbd.AddForce(Vector2.right * moveSpeed);
                gameObject.GetComponent<Animator>().Play("Running");
            }
            if ((boxCharacter.IsTouchingLayers(groundMask) || (boxCharacter.IsTouchingLayers(waterMask))) && (Input.GetKeyDown(jumpKey) || Input.GetKeyDown(jumpKeyController)))
            {
                isJumping = true;
            }
        }
        //Jump
        if (isJumping == true)
        {
            rgbd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            gameObject.GetComponent<Animator>().Play("Running");
        }

        //Altération déplacement (résistance de l'eau)
            if (boxCharacter.IsTouchingLayers(waterMask))
        {
            rgbd.mass = 2;
            Debug.Log("touche l'eau");
        }

        //Jauge Points de vie (part 1)
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (boxCharacter.IsTouchingLayers(groundMask))
        {
            isGrounded = true;
            gameObject.GetComponent<Animator>().Play("Idle");
            Debug.Log("touche le sol");
        }
    }

    //Jauge Points de vie (part 2)
    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
