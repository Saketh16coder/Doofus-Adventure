using UnityEngine;

public class DoofusController3D : MonoBehaviour
{
    [Tooltip("Base move speed. Will be overridden by GameManager if available.")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private PulpitController currentPulpit;

    public PulpitController CurrentPulpit => currentPulpit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
       
        if (GameManager.Instance != null)
        {
            moveSpeed = GameManager.Instance.playerSpeed;
            Debug.Log($"DoofusController3D: Speed set from JSON = {moveSpeed}");
        }
        else
        {
            Debug.LogWarning("DoofusController3D: No GameManager found, using default moveSpeed.");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null)
        {
            
            if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
            {
                return;
            }
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");  

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 targetPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    private void Update()
    {
        
        if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            if (transform.position.y < -5f)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        PulpitController pulpit = collision.gameObject.GetComponent<PulpitController>();
        if (pulpit == null)
        {
            return;
        }

        
        if (currentPulpit == null)
        {
            currentPulpit = pulpit;
            return;
        }

        
        if (pulpit != currentPulpit)
        {
            currentPulpit = pulpit;

            if (GameManager.Instance != null)
            {
                int scoreValue = pulpit.GetScoreValue();
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.PlaySFX(GameManager.Instance.moveClip);
            }
        }
    }
}

