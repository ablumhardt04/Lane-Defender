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

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        currentSpeed = _initialSpeed;
    }

    void FixedUpdate()
    {
        rb2D.velocity = new Vector2(currentSpeed * _movementDirection, 0);
        if (currentSpeed < _maxSpeed)
        {
            currentSpeed *= _speedMultiplier;
            if (currentSpeed > _maxSpeed)
            {
                currentSpeed = _maxSpeed;
            }
        }

        if ((transform.position.x > 10) && (tag == "Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
