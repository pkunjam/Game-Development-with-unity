using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collecting game object
/// </summary>
public class Collector : MonoBehaviour
{
	#region Fields

    // targeting support
    SortedList<Target> targets = new SortedList<Target>();
    Target targetPickup = null;

    // movement support
	const float BaseImpulseForceMagnitude = 2.0f;
    const float ImpulseForceIncrement = 0.3f;

	// saved for efficiency
    Rigidbody2D rb2d;

    #endregion

    #region Methods

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
		// center collector in screen
		Vector3 position = transform.position;
		position.x = 0;
		position.y = 0;
		position.z = 0;
		transform.position = position;

		// save reference for efficiency
		rb2d = GetComponent<Rigidbody2D>();

        // add as listener for pickup spawned event
        EventManager.AddListener(HandlePickupSpawnedEvent);
	}

    /// <summary>
    /// Called when another object is within a trigger collider
    /// attached to this object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        // only respond if the collision is with the target pickup
        if (other.gameObject == targetPickup.GameObject)
        {
            // remove collected pickup from list of targets and game
            int targetPickupIndex = targets.IndexOf(targetPickup);
            GameObject deadTarget = targets[targetPickupIndex].GameObject;
            targets.RemoveAt(targetPickupIndex);
            Destroy(deadTarget);

            // go to next target if there is one
            rb2d.velocity = Vector2.zero;
            if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].UpdateDistance(transform.position);
                }
                targets.Sort();
                SetTarget(targets[targets.Count - 1]);
            }
            else
            {
                targetPickup = null;
            }
		}
	}

    /// <summary>
    /// Handles the pickup spawned event
    /// </summary>
    /// <param name="pickup">pickup that was spawned</param>
    void HandlePickupSpawnedEvent(GameObject pickup)
    {
        // add new pickup to list of targets
        targets.Add(new Target(pickup, transform.position));

        // change target as appropriate
        float targetPickupDistance;
        if (targetPickup != null)
        {
            targetPickupDistance = Vector3.Distance(
                targetPickup.GameObject.transform.position, transform.position);
        }
        else
        {
            targetPickupDistance = float.MaxValue;
        }
        if (targets[targets.Count - 1].Distance < targetPickupDistance)
        {
            SetTarget(targets[targets.Count - 1]);
        }
    }

    /// <summary>
    /// Sets the target pickup to the provided pickup
    /// </summary>
    /// <param name="pickup">Pickup.</param>
    void SetTarget(Target pickup)
    {
        targetPickup = pickup;
        GoToTargetPickup();
    }

    /// <summary>
    /// Starts the collector moving toward the target pickup
    /// </summary>
    void GoToTargetPickup()
    {
        // calculate direction to target pickup and start moving toward it
        Vector2 direction = new Vector2(
            targetPickup.GameObject.transform.position.x - transform.position.x,
            targetPickup.GameObject.transform.position.y - transform.position.y);
        direction.Normalize();
        rb2d.velocity = Vector2.zero;
        float impulseForce = BaseImpulseForceMagnitude +
            ImpulseForceIncrement * targets.Count;
        rb2d.AddForce(direction * impulseForce, 
            ForceMode2D.Impulse);
    }

    /*
	/// <summary>
	/// Updates the pickup currently targeted for collection.
	/// If the provided pickup is closer than the currently
	/// targeted pickup, the provided pickup is set as the
	/// new target. Otherwise, the targeted pickup isn't
	/// changed.
	/// </summary>
	/// <param name="pickup">pickup</param>
	public void UpdateTarget(GameObject pickup)
    {
		if (targetPickup == null)
        {
			SetTarget(pickup);
		}
        else
        {
			float targetDistance = GetDistance(targetPickup);
			if (GetDistance(pickup) < targetDistance)
            {
				SetTarget(pickup);
			}
		} 
	}






	/// <summary>
	/// Gets the pickup in the scene that's closest to the teddy bear
	/// If there are no pickups in the scene, returns null
	/// </summary>
	/// <returns>closest pickup</returns>
	GameObject GetClosestPickup()
    {
        // initial setup
		List<GameObject> pickups = tedTheCollector.Pickups;
		GameObject closestPickup;
		float closestDistance;
		if (pickups.Count == 0)
        {
			return null;
		}
        else
        {
			closestPickup = pickups[0];
			closestDistance = GetDistance(closestPickup);
		}

		// find and return closest pickup
		foreach (GameObject pickup in pickups)
        {
			float distance = GetDistance(pickup);
			if (distance < closestDistance)
            {
				closestPickup = pickup;
				closestDistance = distance;
			}
		}
		return closestPickup;
	}

	/// <summary>
	/// Gets the distance between the teddy bear and the 
	/// provided pickup
	/// </summary>
	/// <returns>distance</returns>
	/// <param name="pickup">pickup</param>
	float GetDistance(GameObject pickup)
    {
		return Vector3.Distance(transform.position, pickup.transform.position);
	}
    */

	#endregion
}
