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

    [SerializeField] private Transform _enemyParent;
    private float _enemyStartTime;
    [SerializeField] private float _enemyIntensity;
    [SerializeField] private GameObject _snailPrefab;
    [SerializeField] private GameObject _snakePrefab;
    [SerializeField] private GameObject _slimePrefab;
    private int[] lastLanesChosen = { -1, -1, -1 };
    private float[] snailPositions = { 4f, 2.1f, 0.15f, -1.8f, -3.75f };
    private float[] snakePositions = { 3.95f, 1.95f, 0.1f, -1.85f, -3.8f };
    private float[] slimePositions = { 4f, 2.05f, 0.1f, -1.85f, -3.8f };

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
        SpawnEnemy(1, false);
        yield return new WaitForSeconds(0.5f);
        SpawnEnemy(5, false);

        while (true)
        {
            yield return new WaitForSeconds(LengthBetweenSpawns());
            _enemyIntensity = (Time.time - _enemyStartTime) / 5;
            SpawnEnemy(SelectRandomEnemy(), true);
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

    private void SpawnEnemy(int health, bool couldDouble)
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
            Instantiate(_snailPrefab, new Vector2(10, snailPositions[lane]), Quaternion.identity, _enemyParent);
        }
        else if (health == 3)
        {
            Instantiate(_snakePrefab, new Vector2(10.25f, snakePositions[lane]), Quaternion.identity, _enemyParent);
        }
        else
        {
            Instantiate(_slimePrefab, new Vector2(9.75f, slimePositions[lane]), Quaternion.identity, _enemyParent);
        }

        if ((couldDouble) && (health == 1))
        {
            if ((_enemyIntensity <= 4) && (UnityEngine.Random.Range(0, 10) == 0)) // < 20 seconds have passed
            {
                SpawnEnemy(1, false); // 10% chance to spawn second slime in other lane
            }
            else if ((_enemyIntensity <= 8) && (UnityEngine.Random.Range(0, 5) == 0)) // < 40 seconds have passed
            {
                SpawnEnemy(1, false); // 20% chance to spawn second slime in other lane
            }
            else if (UnityEngine.Random.Range(0, 2) == 0) // > 40 seconds have passed
            {
                SpawnEnemy(1, false); // 50% chance to spawn second slime in other lane
            }
        }
    }

    private int SelectRandomEnemy()
    {
        // Chooses enemy's health at random, which determines which enemy it will be
        if (_enemyIntensity <= 4) // < 20 seconds have passed
        {
            int[] options = { 5, 3, 3, 1, 1, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity <= 8) // < 40 seconds have passed
        {
            int[] options = { 5, 3, 1, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity <= 12) // < 60 seconds have passed
        {
            int[] options = { 5, 3, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity <= 12) // > 60 seconds have passed
        {
            return 3;
        }
        return 1;
    }

    private void OnDestroy()
    {
        restart.started -= Restart_started;
    }
}
