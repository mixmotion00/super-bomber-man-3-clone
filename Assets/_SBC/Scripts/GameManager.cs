using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private Transform _spawnPoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //var player = Instantiate(_characterPrefab);
            //player.transform.position = _spawnPoint.position;

            SceneManager.LoadScene(0);
        }
    }
}
