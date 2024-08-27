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
    [SerializeField] private int _moveSpeed;
    private Rigidbody2D rb2D;

    private float bulletCooldown;
    [SerializeField] private float _maxBulletCooldown;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;

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
    }

    private void Fire_started(InputAction.CallbackContext context)
    {
        if (bulletCooldown < 0)
        {
            bulletCooldown = _maxBulletCooldown;
            Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity);
        }
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
            rb2D.velocity = new Vector2(0, move.ReadValue<float>() * _moveSpeed * Time.deltaTime);
        }
    }
}
