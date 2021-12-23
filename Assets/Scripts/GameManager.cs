using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private GameObject virus0Perfab;
    [SerializeField] private GameObject virus1Perfab;

    [SerializeField] private GameObject bulletItemPrefab;
    [SerializeField] private GameObject toiletPaperItemPrefab;
    [SerializeField] private GameObject fastFirePillItemPrefab;
    [SerializeField] private GameObject disinfectantItemPrefab;

    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private Animator bulletTextAnimator;
    [SerializeField] private TextMeshProUGUI toiletPaperText;

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject pauseCanvas;

    [SerializeField] private Animator UIAnimator;
    [SerializeField] private Animator musicAnimator;

    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private int mainMenuSceneIndex = 0;

    [SerializeField] private GameObject[] heartSprites;
    [SerializeField] private GameObject heartDisappearEffect;
    [SerializeField] private GameObject disinfectantEffect;

    [SerializeField] private float virusSpawnIntervall;
    [SerializeField] private float virusSpawnIntervallRandomness;
    [SerializeField] private float virusSpawnIntervallFactor;

    [SerializeField] private float powerupSpawnProbability = .001f;

    [SerializeField] private float itemSpawnVelocityRange;

    [SerializeField] private Vector2 spawnPos0;
    [SerializeField] private Vector2 spawnPos1;

    private float timeSinceLastSpawn = 0f;
    private float currentVirusSpawnIntervall;
    private PlayerController player;

    private int bulletAmount = 20;
    private int toiletPaperAmount = 0;
    private int highScore = 0;

    private bool firstTime = false;

    public bool gamePaused = false;
    public bool cancleNextShot = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("ERROR: Found other GameManager!");

        currentVirusSpawnIntervall = virusSpawnIntervall;

        player = FindObjectOfType<PlayerController>();

        highScore = PlayerPrefs.GetInt("highScore", 0);
    }

    private void Start()
    {
        ManageTutorial();
    }

    private void FixedUpdate()
    {
        if (player != null && !gamePaused)
        {
            ManageVirusSpawning();
            ManagePowerUpSpawning();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            PauseGame();
    }

    private void ManageTutorial()
    {
        firstTime = PlayerPrefs.GetInt("firstTime", 1) == 1;

        if (firstTime)
        {
            Time.timeScale = 0f;
            gamePaused = true;
            tutorialCanvas.SetActive(true);
        }
    }

    public void EndTutorial(bool showTutorialAgain)
    {
        cancleNextShot = true;

        if (showTutorialAgain)
            PlayerPrefs.SetInt("firstTime", 1);
        else PlayerPrefs.SetInt("firstTime", 0);

        tutorialCanvas.SetActive(false);
        gamePaused = false;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0f;
            gamePaused = true;
            pauseCanvas.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        cancleNextShot = true;

        pauseCanvas.SetActive(false);
        gamePaused = false;
        Time.timeScale = 1f;
    }

    private void ManageVirusSpawning()
    {
        virusSpawnIntervall *= virusSpawnIntervallFactor;
        virusSpawnIntervallRandomness *= virusSpawnIntervallFactor;

        if (timeSinceLastSpawn <= 0f)
        {
            SpawnVirus(Random.Range(0, 2), Random.value > .5f ? spawnPos0 : spawnPos1);

            currentVirusSpawnIntervall = virusSpawnIntervall + Random.Range(-virusSpawnIntervallRandomness, virusSpawnIntervallRandomness);
            timeSinceLastSpawn = currentVirusSpawnIntervall;
        }

        timeSinceLastSpawn -= Time.fixedDeltaTime;
    }

    private void ManagePowerUpSpawning()
    {
        if (Random.value <= powerupSpawnProbability)
        {
            Vector3 itemPosition = new Vector3(Random.Range(-3f, 3f), 10f, 0f);

            if (Random.value > .5f)
                 SpawnFastFirePillItem(itemPosition);
            else SpawnDisinfectantItem(itemPosition);
        }
    }

    public GameObject SpawnBullet(Vector3 pos, Vector3 dir)
    {
        GameObject newBullet = null;

        HandleBulletInitiation(ref newBullet, pos, dir);

        return newBullet;
    }

    public GameObject SpawnVirus(int type, Vector2 position)
    {
        GameObject virusPrefab;

        if (type == 0) virusPrefab = virus0Perfab;
        else           virusPrefab = virus1Perfab;

        GameObject virusInstance = null;

        if (virusPrefab != null)
            virusInstance = Instantiate(virusPrefab, position, Quaternion.identity);
        else Debug.LogError("ERROR: No Virus Prefab assigned!");

        return virusInstance;
    }

    public GameObject SpawnBulletItem(Vector3 pos)
    {
        GameObject newBulletItem = null;

        if (bulletItemPrefab != null)
        {
            newBulletItem = Instantiate(bulletItemPrefab, pos, Quaternion.identity);
            AssignRandomVelocity(ref newBulletItem);
        }
        else Debug.LogError("ERROR: No Bullet-Item Prefab assigned!");

        return newBulletItem;
    }

    public GameObject SpawnToiletPaperItem(Vector3 pos)
    {
        GameObject newToiletPaperItem = null;

        if (toiletPaperItemPrefab != null)
        {
            newToiletPaperItem = Instantiate(toiletPaperItemPrefab, pos, Quaternion.identity);
            AssignRandomVelocity(ref newToiletPaperItem);
        }
        else Debug.LogError("ERROR: No Toilet-Paper-Item Prefab assigned!");

        return newToiletPaperItem;
    }

    public GameObject SpawnFastFirePillItem(Vector3 pos)
    {
        GameObject newFastFirePillItem = null;

        if (fastFirePillItemPrefab != null)
        {
            newFastFirePillItem = Instantiate(fastFirePillItemPrefab, pos, Quaternion.identity);
            AssignRandomVelocity(ref newFastFirePillItem);
        }
        else Debug.LogError("ERROR: No Fast-Fire-Pill-Item Prefab assigned!");

        return newFastFirePillItem;
    }

    public GameObject SpawnDisinfectantItem(Vector3 pos)
    {
        GameObject newDisinfectantItem = null;

        if (disinfectantItemPrefab != null)
        {
            newDisinfectantItem = Instantiate(disinfectantItemPrefab, pos, Quaternion.identity);
            AssignRandomVelocity(ref newDisinfectantItem);
        }
        else Debug.LogError("ERROR: No Disinfectant-Item Prefab assigned!");

        return newDisinfectantItem;
    }

    private void AssignRandomVelocity(ref GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.AddForce(new Vector2(Random.Range(-itemSpawnVelocityRange, itemSpawnVelocityRange),
                Random.Range(-itemSpawnVelocityRange, itemSpawnVelocityRange)));

        else Debug.LogError("ERROR: No RigidBody2D assigned to Item");
    }

    private void HandleBulletInitiation(ref GameObject newBullet, Vector3 pos, Vector3 dir)
    {
        if (bulletPrefab != null)
        {
            pos.y += Random.Range(-.1f, .1f);
            newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);

            if (dir.x < 0f)
            {
                Bullet bulletComp = newBullet.GetComponent<Bullet>();

                if (bulletComp != null)
                    bulletComp.SetDirection(-1f);
                else Debug.LogError("ERROR: Bullet Component not found!");
            }
        }
        else Debug.LogError("ERROR: No Bullet Prefab assigned!");
    }

    public void CollectToiletPaper(int amount)
    {
        toiletPaperAmount += amount;
        toiletPaperText.text = toiletPaperAmount.ToString();
    }

    public void CollectBullet(int amount)
    {
        bulletAmount += amount;
        bulletText.text = bulletAmount.ToString();
    }

    public bool Shoot()
    {
        if (cancleNextShot)
        {
            cancleNextShot = false;
            return false;
        }

        bulletAmount = Mathf.Max(bulletAmount - 1, 0);
        bulletText.text = bulletAmount.ToString();

        if (bulletAmount == 0)
        {
            bulletTextAnimator.SetTrigger("outOfBullets");
            return false;
        }
        return true;
    }

    public void ChangeHealth(int health, int oldHealth)
    {
        for (int i = health; i < oldHealth; i++)
        {
            GameObject heartSprite = heartSprites[i];
            if (heartSprite != null)
            {
                Transform parent = heartSprite.transform.parent;
                Vector3 position = heartSprite.transform.position;

                Destroy(heartSprite);

                SpawnHeartDisappearEffect(position, parent);
            }
            else Debug.LogError("ERROR: Heart Sprite missing!");
        }
    }

    private void SpawnHeartDisappearEffect(Vector3 pos, Transform parent)
    {
        pos.z = -1f;

        if (heartDisappearEffect != null)
            Instantiate(heartDisappearEffect, pos, Quaternion.identity, parent);
        else Debug.LogError("ERROR: No Heart-Disappear Effect assigned!");
    }

    private void SpawnDisinfectantEffect()
    {
        Vector3 pos = new Vector3(0f, 0f, -1f);

        if (disinfectantEffect != null)
            Instantiate(disinfectantEffect, pos, Quaternion.identity);
        else Debug.LogError("ERROR: No Disinfectant Effect assigned!");
    }

    public void DisinfectantArea()
    {
        Virus[] viruses = FindObjectsOfType<Virus>();

        for (int i = 0; i < viruses.Length; i++)
            viruses[i].Die();

        SpawnDisinfectantEffect();
    }

    public void ActivateFastFire()
    {
        player.AddFastFireTime(5f);
    }

    public void PlayerDied()
    {
        int score = toiletPaperAmount;
        highScore = Mathf.Max(highScore, toiletPaperAmount);
        PlayerPrefs.SetInt("highScore", highScore);

        Invoke("ChangeToGameOverCanvas", 1f);

        highScoreText.text = highScore.ToString();
        scoreText.text = score.ToString();
    }

    private void ChangeToGameOverCanvas()
    {
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        cancleNextShot = true;

        Time.timeScale = 1f;
        gamePaused = false;

        UIAnimator.SetTrigger("getBlack");
        musicAnimator.SetTrigger("calmDown");

        Invoke("LoadMainMenuScene", .5f);
    }

    public void TryAgain()
    {
        cancleNextShot = true;

        Time.timeScale = 1f;
        gamePaused = false;

        UIAnimator.SetTrigger("getBlack");
        musicAnimator.SetTrigger("calmDown");

        Invoke("ReloadScene", .5f);
    }

    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
