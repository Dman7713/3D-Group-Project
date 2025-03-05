using UnityEngine;

public class CutBurgerBun : MonoBehaviour
{
    [SerializeField]
    GameObject topBunPrefab;  // The top half of the bun
    [SerializeField]
    GameObject bottomBunPrefab; // The bottom half of the bun

    [SerializeField]
    GameObject burntTopBunPrefab;  // The burnt top half of the bun
    [SerializeField]
    GameObject burntBottomBunPrefab; // The burnt bottom half of the bun

    Vector3 spawnPos;
    [SerializeField]
    Vector3 spawnDiff;  // Used to slightly offset the spawn position for the two pieces
    Quaternion spawnRot;
    [SerializeField]
    Vector3 spawnScale;
    [SerializeField]
    int spawnCount = 1;
    [SerializeField]
    int cutCount = 1;
    [SerializeField]
    Vector3 scaleDecrease;

    [SerializeField]
    AudioClip cutSound; // Audio clip for the cut sound
    AudioSource audioSource; // AudioSource to play the sound

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRot = Quaternion.identity;
        transform.localScale = spawnScale; // Make sure the bun starts with the correct scale

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Knife"))
        {
            cutCount -= 1;
            transform.localScale -= scaleDecrease;

            // Play the cut sound when the object is cut
            if (audioSource && cutSound)
            {
                audioSource.PlayOneShot(cutSound);
            }

            // Check if the object is tagged as "Burnt"
            if (CompareTag("Burnt"))
            {
                // Spawn the burnt versions of the top and bottom buns
                if (spawnCount <= 1)
                {
                    Instantiate(burntTopBunPrefab, spawnPos, spawnRot); // Spawn burnt top bun piece
                    Instantiate(burntBottomBunPrefab, spawnPos + spawnDiff, spawnRot); // Spawn burnt bottom bun piece
                    Destroy(gameObject);
                }
                else
                {
                    Instantiate(burntTopBunPrefab, spawnPos + spawnDiff, spawnRot); // Spawn burnt top bun piece at offset
                    Instantiate(burntBottomBunPrefab, spawnPos + spawnDiff * 2, spawnRot); // Spawn burnt bottom bun piece at different offset
                    spawnCount -= 1;
                }
            }
            else
            {
                // Regular cut behavior for non-"burnt" objects
                if (cutCount >= 0)
                {
                    if (spawnCount <= 1)
                    {
                        Instantiate(topBunPrefab, spawnPos, spawnRot); // Spawn top bun piece
                        Instantiate(bottomBunPrefab, spawnPos + spawnDiff, spawnRot); // Spawn bottom bun piece
                        Destroy(gameObject);
                    }
                    else
                    {
                        Instantiate(topBunPrefab, spawnPos + spawnDiff, spawnRot); // Spawn top bun piece at offset
                        Instantiate(bottomBunPrefab, spawnPos + spawnDiff * 2, spawnRot); // Spawn bottom bun piece at different offset
                        spawnCount -= 1;
                    }
                }
            }
        }
    }
}
