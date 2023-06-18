using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Manages the important features like keeping the score, restarting levels,
/// saving/loading data, updating the HUD etc
/// </summary>
public class GameCtrl : MonoBehaviour
{
    public static GameCtrl instance;
    public float restartDelay;
    public GameData data;
    public UI ui;
    public GameObject bigCoin;
    public int coinValue;
    public int bigCoinValue;
    public int enemyValue;
    public float maxTime;

    public enum Item
    {
        Coin,
        BigCoin,
        Enemy
    }

    string dataFilePath;
    BinaryFormatter bf;
    float timeLeft;

	void Awake()
	{
        if (instance == null)
            instance = this;

        bf = new BinaryFormatter();
        dataFilePath = Application.persistentDataPath + "/game.dat";

        Debug.Log(dataFilePath);
	}

	void Start()
	{
		//timeLeft = maxTime;

        HandleFirstBoot();
        UpdateHearts();
        data.isFirstBoot = true;
	}

	void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
            ResetData();

         if (timeLeft > 0)
            UpdateTimer();

	}

    public void SaveData()
    {
        FileStream fs = new FileStream(dataFilePath, FileMode.Create);
        bf.Serialize(fs, data);
        fs.Close();
    }

    public void LoadData()
    {
        if(File.Exists(dataFilePath))
        {
            FileStream fs = new FileStream(dataFilePath, FileMode.Open);
            data = (GameData)bf.Deserialize(fs);
            ui.txtCoinCount.text = " x " + data.coinCount;
            ui.txtScore.text = "Score: " + data.score;
            fs.Close();
        }
    }

    void ResetData()
    {
        FileStream fs = new FileStream(dataFilePath, FileMode.Create);

        // reset all the data items
        data.coinCount = 0;
        ui.txtCoinCount.text = " x 0";
        data.score = 0;
        for (int keyNumber = 0; keyNumber <= 2; keyNumber++)
        {
            data.keyFound[keyNumber] = false;
        }
        data.lives = 3;
        UpdateHearts();
        ui.txtScore.text = "Score: " + data.score;
        bf.Serialize(fs, data);
        fs.Close();
        Debug.Log("Data Reset");
    }

	public void RefreshUI()
    {
        ui.txtCoinCount.text = " x " + data.coinCount;
        ui.txtScore.text = "Score: " + data.score;
    }
    void OnEnable()
    {
    // Debug.Log("Data Loaded");
    RefreshUI();
    }
    void OnDisable()
    {
        Debug.Log("OnDisable called");
        // DataCtrl.instance.SaveData (data);
        // Time.timeScale = 1;
    // Hide the AdMob banner
    // AdsCtrl.instance.HideBanner();
    }
    

    /// <summary>
    /// called when the player dies
    /// </summary>
    public void PlayerDied(GameObject player)
    {
        player.SetActive(false);
        CheckLives();
        // Invoke("RestartLevel", restartDelay);
    }

    public void PlayerDiedAnimation(GameObject player)
    {
        // throw the player back in the air
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(-150f, 400f));

        // rotate the player a bit
        player.transform.Rotate(new Vector3(0, 0, 45f));

        // disable the PlayerCtrl script
        player.GetComponent<PlayerCtrl>().enabled = false;

        // disable the colliders attached to the player so that the player can
        // fall through the ground
        foreach (Collider2D c2d in player.transform.GetComponents<Collider2D>())
        {
            c2d.enabled = false;
        }

        // disable the child gameobjects attached to the player cat
        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(false);
        }

        // disable the camera attached with the player cat
        Camera.main.GetComponent<CameraCtrl>().enabled = false;

        // set the velocity of the cat to zero
        rb.velocity = Vector2.zero;

        // restart level
        StartCoroutine("PauseBeforeReload", player);
    }

    public void PlayerStompsEnemy(GameObject enemy)
    {
        // change the enemy's tag
        enemy.tag = "Untagged";

        // destroy the enemy
        Destroy(enemy);

        // updat the score
        UpdateScore(Item.Enemy);
    }

    IEnumerator PauseBeforeReload(GameObject player)
    {
        yield return new WaitForSeconds(1.5f);  // causes a specified delay
        PlayerDied(player);
    }

    /// <summary>
    /// called when the player falls in water
    /// </summary>
    public void PlayerDrowned(GameObject player)
    {
        // Invoke("RestartLevel", restartDelay);
        CheckLives();
    }

    public void UpdateCoinCount()
    {
        data.coinCount += 1;

        ui.txtCoinCount.text = " x " + data.coinCount;
    }

    public void BulletHitEnemy(Transform enemy)
    {
        // show the enemy explosion SFX
        Vector3 pos = enemy.position;
        pos.z = 20f;
        SFXCtrl.instance.EnemyExplosion(pos);

        // show the big coin
        Instantiate(bigCoin, pos, Quaternion.identity);

        // destroy the enemy
        Destroy(enemy.gameObject);

        AudioCtrl.instance.EnemyExplosion(pos);
    }

    public void UpdateScore(Item item) 
    {
        int itemValue = 0;

        switch (item)
        {
            case Item.BigCoin:
                itemValue = bigCoinValue;
                break;
            case Item.Coin:
                itemValue = coinValue;
                break;
            case Item.Enemy:
                itemValue = enemyValue;
                break;
            default:
                break;
        }

        data.score += itemValue;

        ui.txtScore.text = "Score: " + data.score;
    }

    public void UpdateKeyCount(int keyNumber)
    {
        data.keyFound[keyNumber] = true;

        if (keyNumber == 0)
            ui.key0.sprite = ui.key0Full;
        else if (keyNumber == 1)
            ui.key1.sprite = ui.key1Full;
        else if (keyNumber == 2)
            ui.key2.sprite = ui.key2Full;
    }

    void RestartLevel()
    {
        SceneManager.LoadScene("GamePlay");
    }

    void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;

        ui.txtTimer.text = "Timer: " + (int)timeLeft;

        if(timeLeft <=0)
        {
            ui.txtTimer.text = "Timer: 0";

            // inform the GameCtrl to do the needful
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerDied(player);
        }
    }

    void HandleFirstBoot()
    {
        if(data.isFirstBoot)
        {
            // set lives to 3
            data.lives = 3;

            // set number of coins to 0
            data.coinCount = 0;

            // set keys collected to 0
            data.keyFound[0] = false;
            data.keyFound[1] = false;
            data.keyFound[2] = false;

            // set score to 0
            data.score = 0;

            // set isFirstBoot to false
            data.isFirstBoot = false;
        }
    }

    void UpdateHearts()
    {
        if (data.lives == 3)
        {
            ui.heart1.sprite = ui.fullHeart;
            ui.heart2.sprite = ui.fullHeart;
            ui.heart3.sprite = ui.fullHeart;
        }

        if(data.lives == 2)
        {
            ui.heart1.sprite = ui.emptyHeart;
        }

        if (data.lives == 1)
        {
            ui.heart2.sprite = ui.emptyHeart;
            ui.heart1.sprite = ui.emptyHeart;
        }
    }

    void CheckLives()
    {
        int updatedLives = data.lives;
        updatedLives -= 1;
        data.lives = updatedLives;

        if(data.lives == 0)
        {
            data.lives = 3;
            SaveData();
            Invoke("GameOver", restartDelay);
        }
        else
        {   
            
            Debug.Log("Live Left: " + data.lives);

            SaveData();
            Invoke("RestartLevel", restartDelay);
        }
    }

    void GameOver()
    {
        // todo
        ui.panelGameOver.SetActive(true);
    }
}
