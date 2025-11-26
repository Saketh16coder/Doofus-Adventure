using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;        
    public Vector3 offset;          
    public float smoothSpeed = 5f;  

    [Header("Game Over View")]
    public Vector3 gameOverOffset = new Vector3(0f, 30f, -30f);
    public float gameOverSmoothSpeed = 1.5f; 

    private void Start()
    {
        
        if (target != null && offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        bool isGameOver = false;

        if (GameManager.Instance != null)
        {
            isGameOver = GameManager.Instance.isGameOver;
        }

        Vector3 desiredPosition;
        float speed;

        if (isGameOver)
        {
            
            desiredPosition = target.position + gameOverOffset;
            speed = gameOverSmoothSpeed;
        }
        else
        {
            
            desiredPosition = target.position + offset;
            speed = smoothSpeed;
        }

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            speed * Time.deltaTime
        );

        transform.position = smoothedPosition;

        
    }
}
