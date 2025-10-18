using System;
using System.Collections;
using UltEvents;
using UnityEngine;

[System.Serializable]
public sealed class StateMachineEvent : UltEvent<string, string> { }

[System.Serializable]
public sealed class TransitionsEvent : UltEvent<string, int, string, int> { }

[Serializable]
public class SubStateEvent
{
    public string nameSubEvent = default;
    public UltEvent EventTrigger = default;

    public UltEvent[] TransitionsTrigger = default;
}

[Serializable]
public class StateEvent
{
    public string nameEvent = default;

    public UltEvent EventTrigger = default;
    public SubStateEvent[] SubState = default;
    public UltEvent[] TransitionsTrigger = default;

}

public class EventsStates : MonoBehaviour
{
    [SerializeField] private StateEvent[] statesEvent = default;
    [SerializeField] private UltEvent EventTransitionTrigger = default;
    [SerializeField] private UltEvent SubEventTransitionTrigger = default;
    [SerializeField] private float timeBetwenStates = 1f;
    private WaitForSeconds delay = default;

    private void Awake()
    {
        delay = new WaitForSeconds(timeBetwenStates);
    }

    public void CallEvent(string nameEvent)
    {
        for (int i = 0; i < statesEvent.Length; i++)
        {
            if (statesEvent[i]?.nameEvent == nameEvent)
            {
                statesEvent[i]?.EventTrigger?.Invoke();
                //  CallSubStateEvent(nameSubEvent, i);
            }
        }
    }

    public void CallEventAndSubState(string nameEvent, string nameSubEvent)
    {

        for (int i = 0; i < statesEvent.Length; i++)
        {
            if (statesEvent[i].nameEvent == nameEvent)
            {
                Debug.Log("CallEvent");
                statesEvent[i].EventTrigger?.Invoke();
                CallSubStateEvent(nameSubEvent, i);
            }
        }
    }

    public void CallSubStateEvent(string nameSubEvent, int stateIndex)
    {

        for (int i = 0; i < statesEvent[stateIndex].SubState.Length; i++)
        {
            if (statesEvent[stateIndex]?.SubState[i]?.nameSubEvent == nameSubEvent)
            {
                Debug.Log("CallSubEvent");
                statesEvent[stateIndex]?.SubState[i]?.EventTrigger?.Invoke();
            }
        }
    }


    public void CallTransitionEvent(string transitionEvent, int indexTransition = 0)
    {
        StartCoroutine(CheckEventTransition(transitionEvent, indexTransition));
    }

    public IEnumerator CheckEventTransition(string transitionEvent, int indexTransition = 0)
    {
        yield return StartCoroutine("ExecuteEventTransition");
        yield return StartCoroutine(ExecuteTransition(transitionEvent, indexTransition));
        yield return null;
    }

    public IEnumerator ExecuteEventTransition()
    {
        EventTransitionTrigger?.Invoke();
        yield return delay;
    }

    public IEnumerator ExecuteSubEventTransition()
    {
        SubEventTransitionTrigger?.Invoke();
        yield return delay;
    }

    public IEnumerator ExecuteTransition(string transitionEvent, int indexTransition = 0)
    {
        Debug.Log("Execute transition");
        for (int i = 0; i < statesEvent.Length; i++)
        {
            if (statesEvent[i]?.nameEvent == transitionEvent)
            {
                statesEvent[i]?.TransitionsTrigger[indexTransition]?.Invoke();
            }
        }
        yield return null;
    }

    public void CallTransitionAndTransitionSubEvent(string transitionEvent, int indexTransition = 0, string transitionSubEvent = "", int indexTransitionSubEvent = 0)
    {
        StartCoroutine(CheckEventTransitionAndSubTransition(transitionEvent, indexTransition, transitionSubEvent, indexTransitionSubEvent));
    }

    public IEnumerator CheckEventTransitionAndSubTransition(string transitionEvent, int indexTransition = 0, string transitionSubEvent = "", int indexTransitionSubEvent = 0)
    {
        yield return StartCoroutine("ExecuteEventTransition");
        yield return StartCoroutine(ExecuteTransitionAndSubEventTransition(transitionEvent, indexTransition, transitionSubEvent, indexTransitionSubEvent));
        yield return null;
    }



    public IEnumerator ExecuteTransitionAndSubEventTransition(string transitionEvent, int indexTransition = 0, string transitionSubEvent = "", int indexTransitionSubEvent = 0)
    {
        Debug.Log("Execute transition");

        for (int i = 0; i < statesEvent.Length; i++)
        {
            if (statesEvent[i].nameEvent == transitionEvent)
            {
                if (statesEvent[i]?.TransitionsTrigger.Length > 0)
                {
                    statesEvent[i]?.TransitionsTrigger[indexTransition]?.Invoke();
                    yield return delay;
                    CallTransitionSubEvent(transitionSubEvent, i, indexTransitionSubEvent);
                }

            }
        }
        yield return null;
    }

    public void CallTransitionSubEvent(string transitionSubEvent, int stateIndex, int indexTransitionSubEvent = 0)
    {

        if (statesEvent[stateIndex].SubState.Length > 0)
        {
            for (int i = 0; i < statesEvent[stateIndex]?.SubState.Length; i++)
            {
                if (statesEvent[stateIndex]?.SubState[i]?.TransitionsTrigger.Length > 0)
                {

                    if (statesEvent[stateIndex]?.SubState[i]?.nameSubEvent == transitionSubEvent)
                    {
                        statesEvent[stateIndex]?.SubState[i]?.TransitionsTrigger[indexTransitionSubEvent]?.Invoke();
                    }
                }
            }
        }
    }
}
