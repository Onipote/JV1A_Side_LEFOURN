using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    Vector3 startingPosition;
    public float moveSpeed;
    public bool isJumping = false;
    public float jump;

    [Header("Contrôles Joueur")]
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode leftKey1 = KeyCode.LeftArrow;

    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode rightKey1 = KeyCode.RightArrow;

    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode jumpKeyController = KeyCode.Joystick1Button0;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private KeyCode eatKey = KeyCode.E;
    [SerializeField] private KeyCode powerupKey = KeyCode.X;
    //[SerializeField] private KeyCode attackKey = KeyCode.R;
    
    [Header("Colliders")]
    [SerializeField] private Rigidbody2D rgbd;
    [SerializeField] private BoxCollider2D boxCharacter;
    private BoxCollider2D snack1;
    private BoxCollider2D snack2;
    [SerializeField] private BoxCollider2D powerup;
    public PolygonCollider2D bewitchedCat1;
    public PolygonCollider2D bewitchedCat2;
    [SerializeField] private int enemyHealth;

    [Header("Layers")]
    private bool isTouchingLayers;
    private int groundMask;
    private int wallsMask;
    private int waterMask;
    private int sewersEntrancesMask;
    private int laddersMask;
    private int healMask;
    private int enemyMask;
    private int instantDeathMask;
    private int collectablesMask;
    private int powerupMask;


    [Header("Variables")]
    private bool isGrounded = true;
    private bool isTouchingWall;
    private bool isTouchingWater;
    private bool isTouched = false;
    private float timerIsTouched = 0f;
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private bool wallJumping;
    private float touchingLeftOrRight;
    private int freeCats = 0;
    //private int attackForce;
    
    [Header("HP System")]
    public float health;
    private float lerpTimer;
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public BoxCollider2D snack;
    public BoxCollider2D candies;

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
        enemyMask = LayerMask.GetMask("Enemy");
        instantDeathMask = LayerMask.GetMask("InstantDeath");
        collectablesMask = LayerMask.GetMask("Collectables");
        powerupMask = LayerMask.GetMask("PowerUp");

        health = maxHealth;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rgbd.velocity = new Vector2(horizontalInput * moveSpeed, rgbd.velocity.y);

        if (isGrounded == true)
        {
            //Retournement joueur selon le déplacement
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

            /*//Attaque
            if (boxCharacter.IsTouchingLayers(enemyMask) && Input.GetKey(attackKey))
            {
                MakeDamage();
            }*/
        }

        //Jauge Points de vie (part 1)
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        //Collectible chat
        if (boxCharacter.IsTouchingLayers(collectablesMask) && (Input.GetKey(interactKey)))
        {
            Destroy(bewitchedCat1.gameObject);
            freeCats += 1;
            Debug.Log("Compteur de chats :" + freeCats);
        }
        if (boxCharacter.IsTouchingLayers(collectablesMask) && (Input.GetKey(interactKey)))
        {
            Destroy(bewitchedCat2.gameObject);
            freeCats += 1;
            Debug.Log("Compteur de chats :" + freeCats);
        }

        //Ramasser un snack (death)
        if (boxCharacter.IsTouchingLayers(instantDeathMask) && (Input.GetKey(interactKey)))
        {
            Destroy(candies.gameObject);
        }

        //Ramasser un snack (heal)
        if (boxCharacter.IsTouchingLayers(healMask) && (Input.GetKey(interactKey)))
        {
            Destroy(snack.gameObject);
        }

        //Ramasser un snack (death)
        if (boxCharacter.IsTouchingLayers(instantDeathMask) && (Input.GetKey(interactKey)))
        {
            Destroy(candies.gameObject);
        }

        //Manger
        if (Input.GetKey(eatKey))
        {
            rgbd.transform.position = startingPosition;
        }

        //Ramasser le power-up
        if (boxCharacter.IsTouchingLayers(powerupMask) && (Input.GetKey(interactKey)))
        {
            Destroy(powerup.gameObject);
        }

        //Utiliser le power-up
        if (Input.GetKey(powerupKey))
        {
            
        }

        //Saut
        if (isJumping == true)
        {
            rgbd.AddForce(new Vector2(rgbd.velocity.x, jump));
            isJumping = false;
            gameObject.GetComponent<Animator>().Play("Running");
        }

        //Wall jump
        isTouchingLeft = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y),
        new Vector2(0.2f, 0.9f ), 0f, wallsMask);

        isTouchingRight = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y),
        new Vector2(0.2f, 0.9f), 0f, wallsMask);

        if (isTouchingLeft)
        {
            touchingLeftOrRight = 1;

        }
        else if (isTouchingRight)
        {
            touchingLeftOrRight = -1;
        }
        Debug.Log("isTouching ; " + isTouchingLeft +"; " + isTouchingRight);
        if (Input.GetKeyDown(jumpKey) && ((isTouchingRight) || (isTouchingLeft)))
        {
            wallJumping = true;
            Invoke("SetJumpingToFalse", 0.08f);
            Debug.Log("wallJump123");
        }

        if (wallJumping)
        {
            rgbd.AddForce(new Vector2(touchingLeftOrRight*100, jump));
            Debug.Log("wallJump");
            wallJumping = false;
        }

        //Altération déplacement (résistance de l'eau)
        if (boxCharacter.IsTouchingLayers(waterMask))
        {
            rgbd.mass = 2;
            Debug.Log("touche l'eau");
        }
        else
        {
            rgbd.mass = 1;
        }

        //Frame d'invulnérabilité
        if (timerIsTouched > 0)
        {
            timerIsTouched -= Time.deltaTime;
        }
        else
        {
            isTouched = false;
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

    /*public void MakeDamage()
    {
        randomHit = Random.Range(1,2);

        if (randomHit == 1)
        {
            attackForce = Random.Range(5,30);
            enemyHealth -= attackForce;
        }
        else
        {
            attackForce = Random.Range(30,50);
            enemyHealth -= attackForce;
        }
    }*/
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (boxCharacter.IsTouchingLayers(groundMask))
        {
            isGrounded = true;
            gameObject.GetComponent<Animator>().Play("Idle");
            Debug.Log("touche le sol");
        }

        if (boxCharacter.IsTouchingLayers(enemyMask) && !isTouched)
        {
            TakeDamage(Random.Range(10,25));
            Debug.Log("a pris des degats");
            isTouched = true;
            timerIsTouched = 2f;
        }

        if (boxCharacter.IsTouchingLayers(healMask))
        {
            RestoreHealth(Random.Range(5,20));
        }
    }
}
