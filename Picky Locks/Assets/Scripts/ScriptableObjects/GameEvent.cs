using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();
    
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}

//How would the UI reference this?
//Need to have a controller class for each UI. 
//OnDeathUI controller - ShowUI on Level Fail
//Next Level UI controller - ShowUI() on Level Complete 
//Next Zone UI controller - ShowUI( on Level Complete && Zone Complete - this differentiation should be made in the LevelGenerator
    //ScriptableObject to store the levels in a zone, need to contain references to the relevant methods as well. 
    /*** OnLevelComplete() - check if any levels are left in the zone. Seperate playerpref needs to be kept for zone level count.
     ***/
//StartUI controller - ShowUI() called on Level Start - HideUI() On Play Start
public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }
    public void OnEventRaised()
    {
        Response.Invoke();
    }
}