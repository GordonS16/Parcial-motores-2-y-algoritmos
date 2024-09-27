using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private int currency;
    private int wave = 0;
    private int lives;
    private bool gameOver = false;
    [SerializeField] private Text currencyTxt;
    [SerializeField] private Text waveTxt;
    [SerializeField] private Text livesTxt;
    [SerializeField] private GameObject waveBtn;
    [SerializeField] private GameObject gameOverMenu;
    private Tower selectedTower;
    private List<Monster> activeMonsters = new List<Monster>();
    public ObjectPool Pool { get; set; }
    public bool WaveActive
    {
        get
        {
            return activeMonsters.Count > 0;
        }
    }
    public TowerBtn ClickedBtn { get; set; }
    public int Currency
    {
        get
        {
            return currency;
        }
        set
        {
            this.currency = value;
            this.currencyTxt.text = value.ToString() + " <color=green>$</color>";
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }
        set
        {
            this.lives = value;

            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();


        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    public void Start()
    {
        Lives = 10;
        Currency = 100;
    }

    public void Update()
    {
        HandleEscape();
    }

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            this.ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
        }
    }


    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;
            Hover.Instance.Deactivate();
        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
 
            selectedTower.Select();
        }


        selectedTower = tower;

        selectedTower.Select();
    }
    public void DeselectTower()
    {

        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        selectedTower = null;
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hover.Instance.Deactivate();
        }
    }

    public void StartWave()
    {
        wave++;
        waveTxt.text = string.Format("Wave: <color=red>{0}</color>", wave);
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < wave; i++)
        {
            int monterIndex = Random.Range(0, 4);

            string type = string.Empty;

            switch (monterIndex)
            {
                case 0:
                    type = "BlueMonster";
                    break;
                case 1:
                    type = "RedMonster";
                    break;
                case 2:
                    type = "GreenMonster";
                    break;
                case 3:
                    type = "PurpleMonster";
                    break;
            }
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            monster.Spawn();

            activeMonsters.Add(monster);

            yield return new WaitForSeconds(2.5f);
        }
    }

    public void RemoveMonster(Monster monster)
    {

        activeMonsters.Remove(monster);

        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
