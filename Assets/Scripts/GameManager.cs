using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerInput playerInputInstance;
    private InputAction restart;

    [SerializeField] private float _enemyIntensity;
    [SerializeField] private GameObject _snailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerInputInstance = GetComponent<PlayerInput>();
        playerInputInstance.currentActionMap.Enable();
        restart = playerInputInstance.currentActionMap.FindAction("Restart");
        restart.started += Restart_started;

        StartCoroutine(EnemySpawner());
    }

    private void Restart_started(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator EnemySpawner()
    {
        while (true)
        {
            Instantiate(_snailPrefab);
            yield return new WaitForSeconds(3);
        }
    }

    private void OnDestroy()
    {
        restart.started -= Restart_started;
    }
}
