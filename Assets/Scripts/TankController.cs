using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    private GameManager gm;
    private PlayerInput playerInputInstance;
    private InputAction move;
    private InputAction fire;
    private InputAction super;
    private bool moving;
    private bool firing;
    [SerializeField] private int _moveSpeed;
    private Rigidbody2D rb2D;

    private float bulletCooldown;
    [SerializeField] private float _maxBulletCooldown;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Transform _explosionSpawn;
    [SerializeField] private GameObject _bigBulletPrefab;

    private bool midSuper;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb2D = GetComponent<Rigidbody2D>();
        playerInputInstance = GetComponent<PlayerInput>();
        playerInputInstance.currentActionMap.Enable();
        move = playerInputInstance.currentActionMap.FindAction("Move");
        move.started += Move_started;
        move.canceled += Move_canceled;
        fire = playerInputInstance.currentActionMap.FindAction("Fire");
        fire.started += Fire_started;
        fire.canceled += Fire_canceled;
        super = playerInputInstance.currentActionMap.FindAction("Super");
        super.started += Super_started;
    }

    private void Super_started(InputAction.CallbackContext context)
    {
        SetSuper(true);
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
        if (midSuper == true)
        {
            return;
        }

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

    public int GetLives()
    {
        return gm.GetLives();
    }

    public void LoseLife()
    {
        gm.LoseLife();
    }

    public void SetSuper(bool set)
    {
        if (set)
        {
            midSuper = true;
            moving = false;
            rb2D.velocity = Vector2.zero;
            firing = false;
            bulletCooldown = _maxBulletCooldown;
            StartCoroutine(gm.Super());
        }
        else
        {
            midSuper = false;
        }
    }

    public void FireBigBullet()
    {
        Instantiate(_bigBulletPrefab, new Vector2(-6.5f, 0), Quaternion.identity);
    }

    private void OnDestroy()
    {
        move.started -= Move_started;
        move.canceled -= Move_canceled;
        fire.started -= Fire_started;
        fire.canceled -= Fire_canceled;
        super.started -= Super_started;
    }
}
