using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The HUD
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField]
    Text scoreText;

    // game timer support
    float elapsedSeconds = 0;
    bool running = true;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
        scoreText.text = "0";
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update()
	{
        if (running)
        {
            elapsedSeconds += Time.deltaTime;
            scoreText.text = ((int)elapsedSeconds).ToString();
        }
	}

    /// <summary>
    ///  Stops the game timer
    /// </summary>
    public void StopGameTimer()
    {
        running = false;
    }
}
