using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An audio source for the entire game
/// </summary>
public class GameAudioSource : MonoBehaviour
{
	/// <summary>
	/// Awake is called before Start
	/// </summary>
	void Awake()
    {
        // initialize audio manager
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Initialize(audioSource);
    }
}
