using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foe : MonoBehaviour
{
    private GameManager gm;
    private SpriteRenderer sr;
    private Animator animator;
    private Bullet moveScript;
    [SerializeField] private int _health;
    private int initialHealth;
    [SerializeField] private Sprite _walk1;
    [SerializeField] private Sprite _walk2;
    [SerializeField] private Sprite _hit;
    [SerializeField] private Sprite _dead;
    [SerializeField] private int _spriteState;
    [SerializeField] private GameObject _bigExplosionPrefab;
    private bool hit;
    private bool dead;
    private bool killedByBigBullet;
    private Color initialColor;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        moveScript = GetComponent<Bullet>();
        initialColor = sr.color;
        initialHealth = _health;
    }

    public void ChangeSprite(int state)
    {
        _spriteState = state;
        if (sr != null)
        {
            switch (_spriteState)
            {
                case 0:
                    sr.sprite = _walk1;
                    break;
                case 1:
                    sr.sprite = _walk2;
                    break;
                case 2:
                    sr.sprite = _hit;
                    break;
                case 3:
                    sr.sprite = _dead;
                    break;
            }
        }
    }

    private void Update()
    {
        // Knows it's been hit when it detects it has entered damage sprite
        if (!hit && (_spriteState == 2))
        {
            hit = true;
            moveScript.PauseMove(true);
            sr.color = Color.white;
        }
        else if (hit && !dead && (_spriteState != 2))
        {
            hit = false;
            moveScript.PauseMove(false);
            sr.color = initialColor;
        }

        if (!dead && (_spriteState == 3))
        {
            dead = true;
            Death();
        }

        if ((Mathf.Abs(transform.position.x) > 20) || (Mathf.Abs(transform.position.y) > 20))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets a variety of effects to indicate death
    /// Only applies to being shot, not death by collision with tank
    /// </summary>
    private void Death()
    {
        moveScript.enabled = false;
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 1;
        rb2D.constraints = RigidbodyConstraints2D.None;
        rb2D.freezeRotation = false;
        rb2D.velocity = new Vector2(20, 10);
        rb2D.angularVelocity = 360;
        GetComponent<Collider2D>().enabled = false;
        sr.sortingOrder = 20;
        if ((transform.childCount > 0) && (transform.GetChild(0).name == "Shadow"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<Bullet>().IsBig())
            {
                killedByBigBullet = true;
                _health = 0;
            }
            if (_health > 1)
            {
                gm.FoeHit(false, initialHealth, transform.position, false);
                animator.SetTrigger("hit");
                _health--;
            }
            else
            {
                gm.FoeHit(true, initialHealth, transform.position, killedByBigBullet);
                animator.SetBool("dead", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tank") // refers to tank and left wall
        {
            TankController tc = gm.GetTankController();
            gm.LoseLife();
            if (gm.GetLives() < 1)
            {
                Instantiate(_bigExplosionPrefab, tc.transform.position, Quaternion.identity);
                tc.gameObject.SetActive(false);
            }
            Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
