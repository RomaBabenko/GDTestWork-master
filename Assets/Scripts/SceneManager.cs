using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemy> Enemies;
    [SerializeField] private GameObject _lose;
    [SerializeField] private GameObject _win;
    [SerializeField] private LevelConfig _config;
    [SerializeField] private Text _waveText;
    private int _currWave = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
    }

    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        Enemies.Remove(enemy);
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    public void GameOver()
    {
        _lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (_currWave >= _config.Waves.Length)
        {
            _win.SetActive(true);
            return;
        }

        var wave = _config.Waves[_currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            GameObject enemyInstance = Instantiate(character, pos, Quaternion.identity);
            AddEnemy(enemyInstance.GetComponent<Enemy>());
        }

        Update_waveText();
        _currWave++;
    }

    private void Update_waveText()
    {
        _waveText.text = $"Wave {_currWave + 1} / {_config.Waves.Length}";
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


}
