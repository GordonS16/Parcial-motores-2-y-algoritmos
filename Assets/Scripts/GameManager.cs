using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{
    public event CurrencyChanged Changed;
    public TowerBtn ClickedBtn { get; set; }

    private int currency;
    private int wave = 0;
    private int lives;
    private bool gameOver = false;
    private int health = 15;

    [SerializeField]
    private Text livesTxt;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private Text sellText;

    [SerializeField]
    private Text statTxt;

    [SerializeField]
    private Text upgradePrice;

    [SerializeField]
    private GameObject inGameMenu;

    [SerializeField]
    private GameObject optionsMenu;

    private Tower selectedTower;
    private List<Monster> activeMonsters = new List<Monster>();

    public ObjectPool Pool { get; set; }

    public bool WaveActive => activeMonsters.Count > 0;

    public int Currency
    {
        get => currency;
        set
        {
            currency = value;
            currencyTxt.text = value.ToString() + " <color=lime>$</color>";
            OnCurrencyChanged();
        }
    }

    public int Lives
    {
        get => lives;
        set
        {
            lives = value;

            if (lives <= 0)
            {
                lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        Lives = 10;
        Currency = 50;
    }

    void Update()
    {
        HandleEscape();
    }

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            ClickedBtn = towerBtn;
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

    public void OnCurrencyChanged()
    {
        Changed?.Invoke();
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        selectedTower = tower;
        selectedTower.Select();
        sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";
        upgradePanel.SetActive(true);
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        upgradePanel.SetActive(false);
        selectedTower = null;
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedTower == null && !Hover.Instance.IsVisible)
            {
                ShowIngameMenu();
            }
            else if (Hover.Instance.IsVisible)
            {
                DropTower();
            }
            else if (selectedTower != null)
            {
                DeselectTower();
            }
        }
    }

    public void StartWave()
    {
        wave++;
        waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave);
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < wave; i++)
        {
            int monsterIndex = 0;
            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0: type = "BlueMonster"; break;
                case 1: type = "RedMonster"; break;
                case 2: type = "GreenMonster"; break;
                case 3: type = "PurpleMonster"; break;
            }

            Monster monster = Pool.GetObject(type).GetComponent<Monster>();
            monster.Spawn(100);

            if (wave % 3 == 0)
            {
                health += 5;
            }

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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += selectedTower.Price / 2;
            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;
            Destroy(selectedTower.transform.parent.gameObject);
            DeselectTower();
        }
    }

    public void ShowStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ShowSelectedTowerStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUpgradeTip();
    }

    public void SetTooltipText(string txt)
    {
        statTxt.text = txt;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower != null)
        {
            sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";
            SetTooltipText(selectedTower.GetStats());

            if (selectedTower.NextUpgrade != null)
            {
                upgradePrice.text = selectedTower.NextUpgrade.Price.ToString() + " $";
            }
            else
            {
                upgradePrice.text = string.Empty;
            }
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower != null)
        {
            if (selectedTower.Level <= selectedTower.Upgrades.Length && Currency >= selectedTower.NextUpgrade.Price)
            {
                selectedTower.Upgrade();
            }
        }
    }

    public void ShowIngameMenu()
    {
        if (optionsMenu.activeSelf)
        {
            ShowMain();
        }
        else
        {
            inGameMenu.SetActive(!inGameMenu.activeSelf);
            Time.timeScale = inGameMenu.activeSelf ? 0 : 1;
        }
    }

    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void ShowOptions()
    {
        inGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowMain()
    {
        inGameMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
