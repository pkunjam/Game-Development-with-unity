using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The event manager
/// </summary>
public class EventManager : MonoBehaviour
{
    #region Fields

    // save lists of invokers and listeners
    static List<PickupSpawner> invokers = new List<PickupSpawner> ();
    static List<UnityAction<GameObject>> listeners = new List<UnityAction<GameObject>> ();

    #endregion

    #region Public methods

    /// <summary>
    /// Adds the given script as an invoker
    /// </summary>
    /// <param name="invoker">the invoker</param>
    public static void AddInvoker(PickupSpawner invoker)
    {
        // add invoker to list and add all listeners to invoker
        invokers.Add(invoker);
        foreach (UnityAction<GameObject> listener in listeners)
        {
            invoker.AddListener(listener);
        }
    }

    /// <summary>
    /// Adds the given event handler as a listener
    /// </summary>
    /// <param name="handler">the event handler</param>
    public static void AddListener(UnityAction<GameObject> handler)
    {       
        // add listener to list and to all invokers
        listeners.Add(handler);
        foreach (PickupSpawner invoker in invokers)
        {
            invoker.AddListener(handler);
        }
    }

    #endregion
}
