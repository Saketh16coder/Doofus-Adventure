using UnityEngine;

public class PulpitController : MonoBehaviour
{
    private float lifeTime;
    private float spawnNextTime;
    private float timer;
    private bool hasRequestedNext = false;

    [Header("Warning Effect")]
    public Color warningColor = Color.red;
    public float warningDuration = 0.8f; 

    [Header("Bonus Settings")]
    public bool isBonus = false;
    public Color bonusColor = Color.yellow;
    public int normalScoreValue = 1;
    public int bonusScoreValue = 3;
    [Range(0f, 1f)]
    public float bonusChance = 0.2f; 

    private Renderer rend;
    private Color baseColor;         
    private bool inWarningPhase = false;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PulpitController: No GameManager found in scene!");
            return;
        }

        
        lifeTime = Random.Range(
            GameManager.Instance.minPulpitDestroyTime,
            GameManager.Instance.maxPulpitDestroyTime
        );

        spawnNextTime = GameManager.Instance.pulpitSpawnTime;

       
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            baseColor = rend.material.color;
        }

        
        if (Random.value < bonusChance)
        {
            isBonus = true;

            if (rend != null)
            {
                baseColor = bonusColor;
                rend.material.color = bonusColor;
            }
        }

      
        GameManager.Instance.RegisterPulpit(this);
    }

    private void Update()
    {
        
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
            {
                return;
            }
        }

       
        timer += Time.deltaTime;

        
        if (!inWarningPhase && lifeTime - timer <= warningDuration)
        {
            inWarningPhase = true;

           
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlaySFX(GameManager.Instance.warningClip);
            }
        }

        
        if (inWarningPhase && rend != null)
        {
            float t = Mathf.PingPong(Time.time * 8f, 1f); 
            rend.material.color = Color.Lerp(baseColor, warningColor, t);
        }

        
        if (!hasRequestedNext && timer >= spawnNextTime)
        {
            hasRequestedNext = true;
            GameManager.Instance.SpawnPulpitAdjacent(transform.position);
        }

       
        if (timer >= lifeTime)
        {
            GameManager.Instance.UnregisterPulpit(this);

           
            if (rend != null)
            {
                rend.material.color = baseColor;
            }

            Destroy(gameObject);
        }
    }

  
    public int GetScoreValue()
    {
        return isBonus ? bonusScoreValue : normalScoreValue;
    }
}
