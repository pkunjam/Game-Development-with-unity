using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A pickup spawner
/// </summary>
public class PickupSpawner : MonoBehaviour
{
	// needed for spawning
	[SerializeField]
	GameObject prefabPickup;

	// spawn control
    const float SpawnDelay = 0.3f;
	Timer spawnTimer;
    const int MaxNumPickups = 20;
    const int TotalNumPickups = 50;
    int numPickupsSpawned = 0;

	// spawn location support
    Vector3 location = Vector3.zero;
	float minSpawnX;
	float maxSpawnX;
	float minSpawnY;
	float maxSpawnY;

	// collision-free spawn support
	const int MaxSpawnTries = 20;
	float pickupColliderRadius;
	Vector2 min = new Vector2();
	Vector2 max = new Vector2();

	// resolution support
	const int BaseWidth = 800;
	const int BaseHeight = 600;
	const int BaseBorderSize = 100;

    // events invoked by the class
    PickupSpawnedEvent pickupSpawnedEvent = new PickupSpawnedEvent();

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
		float widthRatio = (float)Screen.width / BaseWidth;
		float heightRatio = (float)Screen.height / BaseHeight;
		float resolutionRatio = (widthRatio + heightRatio) / 2;
		int spawnBorderSize = (int)(BaseBorderSize * resolutionRatio);

		// save spawn boundaries for efficiency
        Vector3 upperLeftCornerScreen = new Vector3(spawnBorderSize,
            spawnBorderSize, -Camera.main.transform.position.z);
        Vector3 lowerRightCornerScreen = new Vector3(
            Screen.width - spawnBorderSize,
            Screen.height - spawnBorderSize, 
            -Camera.main.transform.position.z);
        Vector3 upperLeftCorner = Camera.main.ScreenToWorldPoint(upperLeftCornerScreen);
        Vector3 lowerRightCorner = Camera.main.ScreenToWorldPoint(lowerRightCornerScreen);
		minSpawnX = upperLeftCorner.x;
        maxSpawnX = lowerRightCorner.x;
		minSpawnY = upperLeftCorner.y;
        maxSpawnY = lowerRightCorner.y;

		// spawn and destroy a pickup to cache collider value
        GameObject tempPickup = Instantiate(prefabPickup) as GameObject;
		CircleCollider2D collider = tempPickup.GetComponent<CircleCollider2D>();
        pickupColliderRadius = collider.radius;
		Destroy(tempPickup);

		// create and start timer
		spawnTimer = gameObject.AddComponent<Timer>();
		spawnTimer.AddTimerFinishedEventListener(HandleSpawnTimerFinishedEvent);
        spawnTimer.Duration = SpawnDelay;
        spawnTimer.Run();

        // add as invoker of pickup spawned event
        EventManager.AddInvoker(this);
	}

	/// <summary>
	/// Handles the spawn timer finished event
	/// </summary>
	void HandleSpawnTimerFinishedEvent()
    {
		// only spawn a pickup if below max number
		if (GameObject.FindGameObjectsWithTag("Pickup").Length < MaxNumPickups)
        {
			SpawnPickup();
		}

        // don't start the timer if we've spawned all the pickups
        if (numPickupsSpawned < TotalNumPickups)
        {
            spawnTimer.Run();
        }
	}

	/// <summary>
	/// Spawns a new pickup at a random location
	/// </summary>
	void SpawnPickup()
    {		
		// generate random location and calculate pickup collision rectangle
		location.x = Random.Range(minSpawnX, maxSpawnX);
		location.y = Random.Range(minSpawnY, maxSpawnY);
        SetMinAndMax(location);

		// make sure we don't spawn into a collision
		int spawnTries = 1;
		while (Physics2D.OverlapArea(min, max) != null &&
			spawnTries < MaxSpawnTries)
        {
			// change location and calculate new rectangle points
			location.x = Random.Range(minSpawnX, maxSpawnX);
			location.y = Random.Range(minSpawnY, maxSpawnY);
			SetMinAndMax (location);

			spawnTries++;
		}

		// create new pickup if found collision-free location
		if (Physics2D.OverlapArea(min, max) == null)
        {
            GameObject pickup = Instantiate<GameObject>(prefabPickup,
                                    location, Quaternion.identity);
            pickupSpawnedEvent.Invoke(pickup);

            // increment total pickups count
            numPickupsSpawned++;
		}
	}

	/// <summary>
	/// Sets min and max for a pickup collision rectangle
	/// </summary>
	/// <param name="location">location of the pickup</param>
	void SetMinAndMax(Vector3 location)
    {
		min.x = location.x - pickupColliderRadius;
        min.y = location.y - pickupColliderRadius;
		max.x = location.x + pickupColliderRadius;
        max.y = location.y + pickupColliderRadius;
	}

    /// <summary>
    /// Adds the given listener for the pickup spawned event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddListener(UnityAction<GameObject> listener)
    {
        pickupSpawnedEvent.AddListener(listener);
    }
}
