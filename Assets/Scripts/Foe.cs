using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foe : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator animator;
    private Bullet moveScript;
    [SerializeField] private int _health;
    [SerializeField] private Sprite _walk1;
    [SerializeField] private Sprite _walk2;
    [SerializeField] private Sprite _hit;
    [SerializeField] private Sprite _dead;
    [SerializeField] private int _spriteState;
    private bool hit;
    private bool dead;
    private Color initialColor;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        moveScript = GetComponent<Bullet>();
        initialColor = sr.color;
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
        if ((transform.childCount > 0) && (transform.GetChild(0).name == "Shadow"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (_health > 1)
            {
                animator.SetTrigger("hit");
                _health--;
            }
            else
            {
                animator.SetBool("dead", true);
            }
        }
    }
}
