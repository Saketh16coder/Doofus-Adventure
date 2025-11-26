using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Doofus Diary JSON")]
    public TextAsset diaryJsonFile;

    [Header("Pulpit Prefab")]
    public GameObject pulpitPrefab;

    [Header("Loaded Values")]
    public float playerSpeed;
    public float minPulpitDestroyTime;
    public float maxPulpitDestroyTime;
    public float pulpitSpawnTime;

    [Header("Gameplay")]
    public int score = 0;
    public bool isGameStarted = false;
    public bool isGameOver = false;

    [Header("UI (TMP)")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;

    [Header("High Score")]
    public int highScore = 0;
    public TMP_Text startHighScoreText;
    public TMP_Text gameOverHighScoreText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveClip;        
    public AudioClip warningClip;    
    public AudioClip gameOverClip;   

    private DoofusDiary diaryData;
    private List<PulpitController> activePulpits = new List<PulpitController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadDiary();
    }

    private void Start()
    {
        score = 0;
        isGameStarted = false;
        isGameOver = false;

       
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateScoreUI();
        UpdateHighScoreUI();

        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        
        SpawnInitialPulpit();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void LoadDiary()
    {
        diaryData = JsonUtility.FromJson<DoofusDiary>(diaryJsonFile.text);

        playerSpeed = diaryData.player_data.speed;
        minPulpitDestroyTime = diaryData.pulpit_data.min_pulpit_destroy_time;
        maxPulpitDestroyTime = diaryData.pulpit_data.max_pulpit_destroy_time;
        pulpitSpawnTime = diaryData.pulpit_data.pulpit_spawn_time;
    }

    private void SpawnInitialPulpit()
    {
        Instantiate(pulpitPrefab, Vector3.zero, Quaternion.identity);
    }

    
    public void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void RegisterPulpit(PulpitController pulpit)
    {
        activePulpits.Add(pulpit);

        
        if (activePulpits.Count > 2)
        {
            Destroy(activePulpits[0].gameObject);
            activePulpits.RemoveAt(0);
        }
    }

    public void UnregisterPulpit(PulpitController pulpit)
    {
        if (activePulpits.Contains(pulpit))
            activePulpits.Remove(pulpit);
    }

    public void SpawnPulpitAdjacent(Vector3 currentPosition)
    {
        Vector3[] dirs =
        {
            new Vector3(9,0,0),
            new Vector3(-9,0,0),
            new Vector3(0,0,9),
            new Vector3(0,0,-9)
        };

        Vector3 chosenPos = currentPosition;
        for (int i = 0; i < 20; i++)
        {
            Vector3 np = currentPosition + dirs[Random.Range(0, dirs.Length)];
            bool exists = false;

            foreach (var p in activePulpits)
            {
                if (p == null) continue;
                if (Mathf.Approximately(p.transform.position.x, np.x) &&
                    Mathf.Approximately(p.transform.position.z, np.z))
                    exists = true;
            }

            if (!exists)
            {
                chosenPos = np;
                break;
            }
        }

        Instantiate(pulpitPrefab, chosenPos, Quaternion.identity);
    }

    
    public void StartGame()
    {
        isGameStarted = true;
        isGameOver = false;
        startPanel.SetActive(false);
    }

    public void AddScore(int a)
    {
        if (isGameOver) return;

        score += a;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateHighScoreUI()
    {
        if (startHighScoreText != null)
            startHighScoreText.text = "Best Score: " + highScore;

        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = "Best Score: " + highScore;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

       
        PlaySFX(gameOverClip);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateHighScoreUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + score;
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
