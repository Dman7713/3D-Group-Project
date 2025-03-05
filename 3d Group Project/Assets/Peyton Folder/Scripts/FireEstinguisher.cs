using UnityEngine;

public class GunShooting : MonoBehaviour
{
    public GameObject bulletPrefab;    // Bullet prefab to be instantiated
    public Transform shootPoint;       // Point from where the bullet will be shot
    public float fireRate = 0.1f;      // Time between shots
    public float bulletSpeed = 20f;    // Speed at which the bullet moves
    public int bulletsPerShot = 5;     // Number of bullets fired per shot
    public float spreadAngle = 15f;    // The angle spread for the bullets

    private float nextFireTime;        // Time until next shot

    void Update()
    {
        // Handle shooting (right mouse button to shoot)
        if (Input.GetMouseButton(1) && Time.time >= nextFireTime) // 1 is right mouse button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Fire multiple bullets in a spread pattern
        for (int i = 0; i < bulletsPerShot; i++)
        {
            // Calculate spread direction for each bullet
            float spread = Random.Range(-spreadAngle, spreadAngle);  // Random spread within the specified angle
            Quaternion bulletRotation = Quaternion.Euler(0, spread, 0) * shootPoint.rotation;  // Apply spread to the bullet's rotation

            // Create a bullet and set its velocity
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, bulletRotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = bulletRotation * Vector3.forward * bulletSpeed;

            // Set the next time you can shoot
            nextFireTime = Time.time + fireRate;
        }
    }
}
