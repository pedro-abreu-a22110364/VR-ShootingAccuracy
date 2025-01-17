using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public float frequency = 0.25f;
    public float movementRange = 5.0f;
    public float groundLevelOffset = -1.0f;

    private Vector3 initialPosition;
    private float amplitude;

    public void InitializeTarget()
    {
        initialPosition = transform.position;
        amplitude = 1.0f;
    }

    void Start()
    {
        InitializeTarget();
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * Mathf.PI * 2 * frequency) * amplitude;
        Vector3 targetPosition = initialPosition + new Vector3(0, offsetY, 0);
        targetPosition.y = Mathf.Max(targetPosition.y, initialPosition.y + groundLevelOffset);
        transform.position = targetPosition;
    }
}
