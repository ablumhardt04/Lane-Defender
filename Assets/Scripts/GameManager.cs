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

    private float _enemyStartTime;
    [SerializeField] private float _enemyIntensity;
    [SerializeField] private GameObject _snailPrefab;
    private int[] lastLanesChosen = { -1, -1, -1 };
    private float[] _snailPositions = { 4f, 2.1f, 0.15f, -1.8f, -3.75f };

    // Start is called before the first frame update
    void Start()
    {
        playerInputInstance = GetComponent<PlayerInput>();
        playerInputInstance.currentActionMap.Enable();
        restart = playerInputInstance.currentActionMap.FindAction("Restart");
        restart.started += Restart_started;

        _enemyStartTime = Time.time;
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
        // initial wave
        _enemyIntensity = 0;
        SpawnEnemy(1);
        yield return new WaitForSeconds(0.5f);
        SpawnEnemy(5);

        while (true)
        {
            yield return new WaitForSeconds(LengthBetweenSpawns());
            _enemyIntensity = (Time.time - _enemyStartTime) % 5;
            SpawnEnemy(SelectRandomEnemy());
        }
    }

    private float LengthBetweenSpawns()
    {
        float length = 3f;
        if (_enemyIntensity >= 2) // 10 seconds have passed
        {
            length = 2.5f;
        }
        if (_enemyIntensity >= 4) // 20 seconds have passed
        {
            length = 2f;
        }
        if (_enemyIntensity >= 12) // 60 seconds have passed
        {
            length = 0.5f;
        }
        length += UnityEngine.Random.Range(0, 1f) - 0.5f;
        return Mathf.Max(0.01f, length);
    }

    private void SpawnEnemy(int health)
    {
        // Chooses a random lane different from the last three lanes chosen
        int lane;
        bool goodLane = false;
        do
        {
            goodLane = true;
            lane = UnityEngine.Random.Range(0, 5);
            for (int i = 0; i < 3; i++)
            {
                if (lane == lastLanesChosen[i])
                {
                    goodLane = true;
                }
            }
        }
        while (goodLane == false);
        lastLanesChosen[2] = lastLanesChosen[1];
        lastLanesChosen[1] = lastLanesChosen[0];
        lastLanesChosen[0] = lane;

        // Spawns enemy
        if (health == 5)
        {
            Instantiate(_snailPrefab, new Vector2(10, _snailPositions[lane]), Quaternion.identity, transform);
        }
        else if (health == 3)
        {

        }
        else
        {

        }
    }

    private int SelectRandomEnemy()
    {
        // Chooses enemy's health at random, which determines which enemy it will be
        if (_enemyIntensity <= 4) // 20 seconds have passed
        {
            int[] options = { 5, 3, 3, 1, 1, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity <= 8) // 40 seconds have passed
        {
            int[] options = { 5, 3, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity <= 12) // 60 seconds have passed
        {
            return 5;
        }
        return 1;
    }

    private void OnDestroy()
    {
        restart.started -= Restart_started;
    }
}
