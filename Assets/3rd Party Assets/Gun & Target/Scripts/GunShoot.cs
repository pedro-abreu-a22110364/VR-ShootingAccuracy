using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private float shootDelay = 0.1f;

    [Space, SerializeField] private AudioSource audiosource;

    [SerializeField] private GameInfoManager gameInfoManager;
    [SerializeField] private Logger logger;

    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] private float maxLineDistance = 5f;
    [SerializeField] private float lineShowTimer = 0.25f;

    [SerializeField] private LayerMask targetLayer; 

    private float lastShot;

    public void Shoot()
    {
        if (lastShot > Time.time) return;

        lastShot = Time.time + shootDelay;

        GunShotAudio();

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, bulletPosition.position);

        Vector3 endpoint = bulletPosition.position + bulletPosition.forward * maxLineDistance;

        Ray ray = new Ray(bulletPosition.position, bulletPosition.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxLineDistance, targetLayer))
        {
            endpoint = hit.point;

            Target target = hit.collider.GetComponent<Target>();

            if (target != null)
            {
                target.OnRaycastHit(hit.point);
            }

            gameInfoManager.shotsOnTarget++;

            if (Vector3.Distance(hit.point, hit.collider.bounds.center) < 0.2f)
            {
                gameInfoManager.bullseyeHits++;
            }
        }


        line.SetPosition(1, endpoint);

        Destroy(line.gameObject, lineShowTimer);

        gameInfoManager.shotsFired++;
        logger.shotsFiredPerTarget++;
    }

    private void GunShotAudio()
    {
        var random = Random.Range(0.8f, 1.2f);
        audiosource.pitch = random;

        audiosource.Play();
    }
}
