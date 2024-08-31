using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foe : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator animator;
    private Bullet moveScript;
    [SerializeField] private Sprite _walk1;
    [SerializeField] private Sprite _walk2;
    [SerializeField] private Sprite _hit;
    [SerializeField] private Sprite _dead;
    [SerializeField] private int _spriteState;
    private bool hit;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        moveScript = GetComponent<Bullet>();
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
        if (!hit && (_spriteState == 2))
        {
            hit = true;
            moveScript.PauseMove(true);
        }
        else if (hit && (_spriteState != 2))
        {
            hit = false;
            moveScript.PauseMove(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            animator.SetTrigger("hit");
        }
    }
}
