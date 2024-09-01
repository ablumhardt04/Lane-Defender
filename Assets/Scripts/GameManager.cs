using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private TankController tc;
    private GameObject canvas;
    private PlayerInput playerInputInstance;
    private InputAction restart;

    private int lives = 3;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private RectTransform _powerBar;
    [SerializeField] private GameObject _pressEnterText;
    [SerializeField] private Transform _powerBarPosition;
    private Slider powerSlider;
    private bool midSuper;
    private int superTotal;

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
    [SerializeField] private Collider2D _leftWall;

    [SerializeField] private GameObject _gameEndParent;
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private TMP_Text _endScoreText;
    [SerializeField] private GameObject _endGroup2;
    [SerializeField] private TMP_Text _endHighScoreText;
    [SerializeField] private GameObject _newHighScore;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        playerInputInstance = GetComponent<PlayerInput>();
        playerInputInstance.currentActionMap.Enable();
        restart = playerInputInstance.currentActionMap.FindAction("Restart");
        restart.started += Restart_started;

        tc = GameObject.Find("Tank").GetComponent<TankController>();
        powerSlider = _powerBar.GetComponent<Slider>();
        _enemyStartTime = Time.time;
        StartCoroutine(EnemySpawner());
    }

    private void Restart_started(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        _powerBar.anchoredPosition = Camera.main.WorldToScreenPoint(_powerBarPosition.position);
    }

    private IEnumerator EnterTextFlash()
    {
        while (powerSlider.value == 1)
        {
            _pressEnterText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _pressEnterText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator GameOver()
    {
        _livesText.gameObject.SetActive(false);
        _powerBar.gameObject.SetActive(false);
        for (int i = 0; i < _enemyParent.childCount; i++)
        {
            _enemyParent.GetChild(i).GetComponent<Bullet>().SetDirection(1);
            _enemyParent.GetChild(i).GetComponent<SpriteRenderer>().flipX = true;
        }
        yield break;
    }

    private IEnumerator EnemySpawner()
    {
        // initial wave
        _enemyIntensity = 0;
        SpawnEnemy(1, false);
        yield return new WaitForSeconds(0.5f);
        SpawnEnemy(5, false);

        while (lives > 0)
        {
            yield return new WaitForSeconds(LengthBetweenSpawns());
            if (midSuper)
            {
                // Waits until end of super move, then another second
                while (midSuper)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(1);
            }

            // The enemy intensity value is (seconds since start*) / 5
            // *excluding time the super attack is taking place in
            _enemyIntensity = (Time.time - _enemyStartTime - (superTotal * 5)) / 5;
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
            length = 1.5f;
        }
        if (_enemyIntensity >= 18) // 90 seconds have passed
        {
            length = 1f;
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
                    goodLane = false;
                }
            }
        }
        while (goodLane == false);
        lastLanesChosen[2] = lastLanesChosen[1];
        lastLanesChosen[1] = lastLanesChosen[0];
        lastLanesChosen[0] = lane;

        // Spawns enemy
        GameObject foe;
        if (health == 5)
        {
            foe = Instantiate(_snailPrefab, new Vector2(10, snailPositions[lane]), Quaternion.identity, _enemyParent);
        }
        else if (health == 3)
        {
            foe = Instantiate(_snakePrefab, new Vector2(10.25f, snakePositions[lane]), Quaternion.identity, _enemyParent);
        }
        else
        {
            foe = Instantiate(_slimePrefab, new Vector2(9.75f, slimePositions[lane]), Quaternion.identity, _enemyParent);
        }
        foe.GetComponent<Bullet>().MaxSpeedUp(EnemySpeedUp(), true);
        foe.GetComponent<Bullet>().SetDirection(lives < 1 ? 1 : -1); // ensures all foes move right after game over

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
        if (_enemyIntensity <= 18) // < 60 seconds have passed
        {
            int[] options = { 5, 3, 3, 3, 1 };
            return options[UnityEngine.Random.Range(0, options.Length)];
        }
        if (_enemyIntensity >= 18) // > 90 seconds have passed
        {
            return 3;
        }
        return 1;
    }

    private float EnemySpeedUp()
    {
        if (_enemyIntensity <= 4) // < 20 seconds have passed
        {
            return 0;
        }
        if (_enemyIntensity <= 8) // < 40 seconds have passed
        {
            return 0.5f;
        }
        if (_enemyIntensity <= 12) // < 60 seconds have passed
        {
            return 1f;
        }
        if (_enemyIntensity >= 12) // > 60 seconds have passed
        {
            return 1.25f;
        }
        return 0;
    }

    public IEnumerator Super()
    {
        powerSlider.value = 0;
        float t = 0;
        Vector2 startPos = tc.transform.position;
        Vector2 endPos = new Vector2(-15, 0);
        bool bulletFired = false;
        midSuper = true;
        tc.GetComponent<Collider2D>().enabled = false;
        _leftWall.isTrigger = false;
        canvas.SetActive(false);
        while (true)
        {
            // One second: moving tank and camera into a cinematic position, slowing down enemies
            if (t <= 2)
            {
                tc.transform.position = Vector2.Lerp(startPos, endPos, t);
                Camera.main.transform.position = Vector2.Lerp(Vector2.zero, endPos, t);
                Camera.main.orthographicSize = Mathf.Lerp(5, 1, t / 2);
                for (int i = 0; i < _enemyParent.childCount; i++)
                {
                    _enemyParent.GetChild(i).GetComponent<Bullet>().PercentageSlowDown(1 - t);
                }
            }
            // One second: tank tilts back and starts to vibrate and turns red
            if ((t > 1) && (t <= 2))
            {
                tc.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 5, t - 1));
                float vibrateValue = (t - 1) / 10;
                tc.transform.position = endPos + new Vector2(UnityEngine.Random.Range(-vibrateValue, vibrateValue),
                    UnityEngine.Random.Range(-vibrateValue, vibrateValue));
                tc.GetComponent<SpriteRenderer>().color = new Color(1, Mathf.Lerp(1, 2f / 5, t - 1), 
                    Mathf.Lerp(1, 2f / 5, t - 1), 1);
            }
            // One second: bullet fires, camera moves back to normal
            if (t > 2)
            {
                if (!bulletFired)
                {
                    bulletFired = true;
                    tc.FireBigBullet();
                    tc.transform.eulerAngles = new Vector3(0, 0, 10);
                    tc.transform.position = new Vector2(-20, 0);
                    Camera.main.transform.position = tc.transform.position;
                }
                Camera.main.transform.position = new Vector2(Mathf.Lerp(-20, 0, (t - 2) * 2), 0);
                Camera.main.orthographicSize = Mathf.Lerp(1.75f, 5, (t - 2) * 2);
            }
            // Half second: moves tank back into play
            if (t > 3.5f)
            {
                tc.GetComponent<SpriteRenderer>().color = Color.white;
                tc.transform.position = Vector2.Lerp(new Vector2(-10.5f, 0), new Vector2(-7.65f, 0), (t - 3.5f) * 2);
                tc.transform.eulerAngles = Vector3.zero;
            }
            // Fixes camera before next frame / before coroutine exits
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                Camera.main.transform.position.y, -10);
            // Resets things before coroutine ends
            if (t > 4)
            {
                _leftWall.isTrigger = true;
                tc.GetComponent<Collider2D>().enabled = true;
                midSuper = false;
                tc.SetSuper(false);
                superTotal++;
                powerSlider.value = 0;
                canvas.SetActive(true);
                _pressEnterText.SetActive(false);
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    public int GetLives()
    {
        return lives;
    }

    public void LoseLife()
    {
        lives--;
        if (lives == 0)
        {
            StartCoroutine(GameOver());
        }
        else
        {
            _livesText.text = "LIVES: " + lives;
            IncreasePowerSlider(0.25f);
        }
    }

    public void IncreasePowerSlider(float amount)
    {
        if (powerSlider.value == 1) { return; }
        powerSlider.value = Mathf.Min(1, powerSlider.value + amount);
        if (powerSlider.value == 1)
        {
            tc.SuperIsReady();
            StartCoroutine(EnterTextFlash());
        }
    }

    public void FoeDied(int foeType, float foeX, bool killedByBigBullet)
    {
        // Allocates power meter for the kill
        if ((powerSlider.value != 1) && (!killedByBigBullet))
        {
            float superEnergy = foeType * 0.02f; // defaults to 2, 5, or 10% of the bar
            superEnergy *= 1 + Mathf.Max(0, foeX) / 18; // up to 1.5x multiplier if it's killed quickly
            superEnergy *= 1 + Mathf.Min(1, _enemyIntensity / 12) / 2; // up to 1.5x time multiplier
            IncreasePowerSlider(superEnergy);
        }
        
    }

    public TankController GetTankController()
    {
        return tc;
    }

    private void OnDestroy()
    {
        restart.started -= Restart_started;
    }
}
