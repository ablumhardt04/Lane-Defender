using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _speedMultiplier;
    [SerializeField] private float _maxSpeed;
    private float currentSpeed;
    private Rigidbody2D rb2D;
    [SerializeField] private int _movementDirection;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _bigExplosionPrefab;
    private bool movePaused;
    private float storedSpeed;
    [SerializeField] private bool big;
    private float percentSpeed = 1;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (currentSpeed == 0)
        {
            currentSpeed = _initialSpeed;
        }
        if (tag == "Bullet")
        {
            Vector2 pos = GameObject.Find("Tank").GetComponent<TankController>().GetExplosionPos();
            if (!big)
            {
                Instantiate(_explosionPrefab, pos, Quaternion.identity);
            }
            else
            {
                Instantiate(_bigExplosionPrefab, pos, Quaternion.identity);
            }
        }
    }

    void FixedUpdate()
    {
        if (!movePaused)
        {
            rb2D.velocity = new Vector2(currentSpeed * _movementDirection * percentSpeed, 0);
            if (currentSpeed < _maxSpeed)
            {
                currentSpeed *= _speedMultiplier;
                if (currentSpeed > _maxSpeed)
                {
                    currentSpeed = _maxSpeed;
                }
            }
        }
        
        if (big && (transform.position.x > 17))
        {
            Destroy(gameObject);
        }
        if ((transform.position.x > 30) && (tag == "Bullet"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Halts movement when foe hit
    /// </summary>
    public void PauseMove(bool state)
    {
        movePaused = state;
        if (state == true)
        {
            rb2D.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Increases max speed
    /// </summary>
    public void MaxSpeedUp(float amount, bool setImmediately)
    {
        _maxSpeed += amount;
        if (setImmediately)
        {
            currentSpeed = amount; 
        }
    }

    /// <summary>
    /// Used for super animation
    /// </summary>
    public void PercentageSlowDown(float slow)
    {
        percentSpeed = Mathf.Max(0, slow);
    }

    public void SetDirection(int d)
    {
        _movementDirection = d;
    }

    public bool IsBig()
    {
        return big;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (tag == "Bullet")
        {
            if (!big)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                Instantiate(_explosionPrefab, collision.transform.position, Quaternion.identity);
            }
        }
    }
}
