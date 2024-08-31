using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    private PlayerInput playerInputInstance;
    private InputAction move;
    private InputAction fire;
    private bool moving;
    private bool firing;
    [SerializeField] private int _moveSpeed;
    private Rigidbody2D rb2D;

    private float bulletCooldown;
    [SerializeField] private float _maxBulletCooldown;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Transform _explosionSpawn;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerInputInstance = GetComponent<PlayerInput>();
        playerInputInstance.currentActionMap.Enable();
        move = playerInputInstance.currentActionMap.FindAction("Move");
        move.started += Move_started;
        move.canceled += Move_canceled;
        fire = playerInputInstance.currentActionMap.FindAction("Fire");
        fire.started += Fire_started;
        fire.canceled += Fire_canceled;
    }

    private void Fire_canceled(InputAction.CallbackContext context)
    {
        firing = false;
    }

    private void Fire_started(InputAction.CallbackContext context)
    {
        firing = true;
        StartCoroutine(ContinuedFire());
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        moving = false;
        rb2D.velocity = Vector3.zero;
    }

    private void Move_started(InputAction.CallbackContext context)
    {
        moving = true;
    }

    void Update()
    {
        bulletCooldown -= Time.deltaTime;
        if (moving)
        {
            print(rb2D.velocity.y);
            rb2D.velocity = new Vector2(0, move.ReadValue<float>() * _moveSpeed);
        }
    }

    private IEnumerator ContinuedFire()
    {
        while (firing)
        {
            if (bulletCooldown < 0)
            {
                bulletCooldown = _maxBulletCooldown;
                Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity);
            }
            yield return null;
        }
    }

    public Vector2 GetExplosionPos()
    {
        return _explosionSpawn.position;
    }

    private void OnDestroy()
    {
        move.started -= Move_started;
        move.canceled -= Move_canceled;
        fire.started -= Fire_started;
        fire.canceled += Fire_canceled;
    }
}
