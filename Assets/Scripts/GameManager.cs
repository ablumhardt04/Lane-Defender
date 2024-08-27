using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    [SerializeField] private float _enemyIntensity;
    [SerializeField] private GameObject _snailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemySpawner());
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
}
