using UnityEngine;

public class Target : MonoBehaviour
{
    private MeshRenderer _renderer;
    private SphereCollider _sphereCollider;
    private AudioSource _audioSource;

    private bool _isDisabled;
    private bool _isRotating;

    public LevelManager levelManager;

    [SerializeField] private Logger logger;
    [SerializeField] private GameInfoManager gameInfoManager;

    private Vector3 _targetCenter;

    [SerializeField] private float rotationSpeed = 180f;
    private float _currentRotation;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _sphereCollider = GetComponent<SphereCollider>();
        _audioSource = GetComponent<AudioSource>();

        _targetCenter = transform.position;
    }

    private void Update()
    {
        if (_isRotating)
        {
            RotateTarget();
        }
    }

    public void OnRaycastHit(Vector3 hitPoint)
    {
        if (_isDisabled) return;

        if (IsBullseyeHit(hitPoint))
        {
            gameInfoManager.bullseyeHits++;
        }

        _audioSource.Play();
        StartDestroyAnimation();
    }

    private bool IsBullseyeHit(Vector3 hitPoint)
    {
        Vector3 projectedHitPoint = new Vector3(hitPoint.x, hitPoint.y, _targetCenter.z);

        float distance = Vector3.Distance(_targetCenter, projectedHitPoint);

        return distance < 0.2f;
    }

    private void StartDestroyAnimation()
    {
        _isRotating = true;
        _currentRotation = 0f; // Reset rotation tracker
    }

    private void RotateTarget()
    {
        Debug.Log("Rotating target...");

        float rotationStep = rotationSpeed * Time.deltaTime;
        float remainingRotation = -90f - _currentRotation;

        if (Mathf.Abs(rotationStep) > Mathf.Abs(remainingRotation))
        {
            rotationStep = remainingRotation;
        }

        // Calculate pivot point at y + 0.5 (top edge of the target)
        Vector3 pivot = transform.position + new Vector3(0f, 0.5f, 0f);

        // Rotate around the pivot
        transform.RotateAround(pivot, transform.right, rotationStep);

        _currentRotation += rotationStep;

        // Stop rotating when reaching -90 degrees
        if (Mathf.Abs(_currentRotation) >= 90f)
        {
            _isRotating = false;
            DestroyTarget();
        }
    }


    public void DestroyTarget()
    {
        logger.distance = GetDistance();

        _renderer.enabled = false;
        _sphereCollider.enabled = false;
        _isDisabled = true;

        levelManager.OnTargetDestroyed();
    }

    public void ResetTarget()
    {
        _renderer.enabled = true;
        _sphereCollider.enabled = true;
        _isDisabled = false;
        _isRotating = false;
        transform.rotation = Quaternion.identity;
    }

    private string GetDistance()
    {
        switch (_targetCenter.z)
        {
            case 5:
                return "Close";
            case 10:
                return "Medium";
            case 15:
                return "Far";
            default:
                return "Unknown distance";
        }
    }
}
