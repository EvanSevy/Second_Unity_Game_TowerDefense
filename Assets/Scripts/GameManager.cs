using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    private int totalWaves;
    [SerializeField]
    private Text totalMoneyLbl;
    [SerializeField]
    private Text currentWaveLbl;
    [SerializeField]
    private Text totalEscapedLbl;
    [SerializeField]
    private Text playBtnLbl;
    [SerializeField]
    private Button playBtn;

    private int waveNumber = 0;
    private int totalMoney = 50;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private int enemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;
    private AudioSource audioSource;

    //public static GameManager instance = null;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;
    //[SerializeField]
    //private int maxEnemiesOnScreen;
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;

    public List<Enemy> EnemyList = new List<Enemy>();

    //private int enemiesOnScreen = 0;
    const float spawnDelay = 0.5f;

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }
    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }
    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }

    public int TotalMoney
    {
        get { return totalMoney; }
        set
        {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }
    public AudioSource AudioSource
    {
        get { return audioSource; }
    }

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    } else if (instance != this) {
    //        Destroy(gameObject);
    //    }

    //    DontDestroyOnLoad(gameObject);
    //}

    //void spawnEnemy()
    //{
    //    if( (enemiesPerSpawn > 0) && (enemiesOnScreen < totalEnemies))
    //    {
    //        for(int i = 0; i < enemiesPerSpawn; i++)
    //        {
    //            if(enemiesOnScreen < maxEnemiesOnScreen)
    //            {
    //                GameObject newEnemy = Instantiate(enemies[0]) as GameObject;
    //                newEnemy.transform.position = spawnPoint.transform.position;
    //                //newEnemy.layer = 5;
    //                enemiesOnScreen++;
    //            }
    //        }
    //    } 
    //}

    IEnumerator spawn()
    {
        if ((enemiesPerSpawn > 0) && (EnemyList.Count < totalEnemies))
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (EnemyList.Count < totalEnemies)
                {
                    //Enemy newEnemy = Instantiate(enemies[Random.Range(0,enemiesToSpawn)]) as Enemy;
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy;

                    newEnemy.transform.position = spawnPoint.transform.position;
                    //newEnemy.layer = 5;
                    //enemiesOnScreen++;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    public void DestroyAllEnemies()  // wipes the board of all enemies
    {
        foreach (Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }
        EnemyList.Clear();
    }

    public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }
    public void SubtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void IsWaveOver()
    {
        totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
        if ((RoundEscaped + TotalKilled) == totalEnemies)
        {
            if(waveNumber <= enemies.Length)
            {
                enemiesToSpawn = waveNumber;
            }
            SetCurrentGameState();
            ShowMenu();
        }
    }
    public void SetCurrentGameState()
    {
        if (TotalEscaped >= 3)
        {
            currentState = gameStatus.gameover;
        } else if ( (waveNumber == 0) && (TotalKilled + RoundEscaped == 0) )
        {
            currentState = gameStatus.play;
        } else if (waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        } else
        {
            currentState = gameStatus.next;
        }
    }
    public void ShowMenu()
    {
        switch (currentState)
        {
            case gameStatus.gameover:
                playBtnLbl.text = "Play Again!";
                AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                break;
            case gameStatus.next:
                playBtnLbl.text = "Next Wave";
                break;
            case gameStatus.play:
                playBtnLbl.text = "Play";
                break;
            case gameStatus.win:
                playBtnLbl.text = "Play";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }
    public void PlayBtnPressed()
    {
        switch (currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;
                break;
            default:
                totalEnemies = 3;
                totalEscaped = 0;
                totalMoney = 50;
                enemiesToSpawn = 0;
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLbl.text = "Wave " + (waveNumber + 1);
        StartCoroutine(spawn());
        playBtn.gameObject.SetActive(false);
    }
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.towerBtnPressed = null;
        }
    }
    //public void removeEnemyFromScreen()
    //{
    //    if (enemiesOnScreen > 0)
    //        enemiesOnScreen -= 1;
    //    StartCoroutine(spawn());
    //}

    // Use this for initialization
    void Start()
    {
        playBtn.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        ShowMenu();
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
        //spawnEnemy();
    }
}
