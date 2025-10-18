using System.Collections;
using UltEvents;
using UnityEngine;

[System.Serializable]
public sealed class ConditionalEvent : UltEvent<bool> { }

[System.Serializable]
public sealed class ConditionalNamedEvent : UltEvent<bool, string> { }


[System.Serializable]
public class SequenceEvent
{
    public bool eventDone = false;
    public UltEvent EventTrigger = default;
    public ConditionalEvent ConditionalEvent = default;

    public IEnumerator CallEventTriggerCoroutine()
    {
        EventTrigger?.Invoke();
        yield return null;
    }

    public IEnumerator CheckEventDoneCoroutine()
    {
        ConditionalEvent?.Invoke(eventDone);
        yield return null;
    }

}

[System.Serializable]
public class NamedSequence
{
    public string nameSequence = default;
    public SequenceEvent[] SequenceEvents = default;
}


public class WaitEventSequence : MonoBehaviour
{
    [SerializeField] private NamedSequence[] NamedSequences = default;
    [SerializeField] private SequenceEvent[] SequenceEvents = default;

    [SerializeField] private bool testbool = false;


    public void SetBoolEvent(bool a)
    {
        testbool = a;
    }

    public void SetBoolEvent2(bool value)
    {
        if (SequenceEvents.Length > 0)
        {
            for (int i = 0; i < SequenceEvents.Length; i++)
            {

                if (SequenceEvents[i].eventDone == false)
                {

                    SequenceEvents[i].eventDone = value;
                    Debug.Log("Check " + i + "EventDone" + value);
                    break;
                }
            }
        }

    }
    public void SetBoolNamedEvent(bool value, string nameSequenceGroup)
    {

        //StartCoroutine(SetBoolNamedEventCoroutine(value, nameSequenceGroup));
        if (NamedSequences.Length > 0)
        {
            for (int i = 0; i < NamedSequences.Length; i++)
            {
                if (NamedSequences[i].nameSequence == nameSequenceGroup)
                {
                    for (int j = 0; j < NamedSequences[i].SequenceEvents.Length; j++)
                    {
                        if (NamedSequences[i].SequenceEvents[j].eventDone == false)
                        {
                            Debug.Log("Check " + j + " from " + NamedSequences[i].nameSequence + "done");
                            NamedSequences[i].SequenceEvents[j].eventDone = value;
                            break;
                        }
                    }
                }
            }
        }

    }

    public IEnumerator SetBoolNamedEventCoroutine(bool value, string nameSequenceGroup)
    {
        if (NamedSequences.Length > 0)
        {
            for (int i = 0; i < NamedSequences.Length; i++)
            {
                if (NamedSequences[i].nameSequence == nameSequenceGroup)
                {
                    for (int j = 0; j < NamedSequences[i].SequenceEvents.Length; j++)
                    {
                        if (NamedSequences[i].SequenceEvents[j].eventDone == false)
                        {
                            Debug.Log("Check " + j + " from " + NamedSequences[i].nameSequence + "done");
                            NamedSequences[i].SequenceEvents[j].eventDone = value;
                            yield break;
                        }
                        yield return null;
                    }
                }
            }
        }

    }

    public void ResetSequencer()
    {
        StopAllCoroutines();
        if (NamedSequences.Length > 0)
        {
            for (int i = 0; i < NamedSequences.Length; i++)
            {

                for (int j = 0; j < NamedSequences[i].SequenceEvents.Length; j++)
                {
                    NamedSequences[i].SequenceEvents[j].eventDone = false;
                }
            }
        }
        if (SequenceEvents.Length > 0)
        {
            for (int i = 0; i < SequenceEvents.Length; i++)
            {
                SequenceEvents[i].eventDone = false;
            }
        }
    }

    public void CallNamedSequenceGroup(string nameSequenceGroup)
    {
        StartCoroutine(CallNamedSequenceGroupCoroutine(nameSequenceGroup));
    }

    private IEnumerator CallNamedSequenceGroupCoroutine(string nameSequenceGroup)
    {
        if (NamedSequences.Length > 0)
        {
            for (int i = 0; i < NamedSequences.Length; i++)
            {
                if (NamedSequences[i]?.nameSequence == nameSequenceGroup)
                {
                    for (int j = 0; j < NamedSequences[i]?.SequenceEvents.Length; j++)
                    {
                        yield return ExecuteCurrentEvent(NamedSequences[i]?.SequenceEvents[j]);
                    }
                }
            }
        }

    }

    public void CallSequenceEvents()
    {
        StartCoroutine(CallSequenceEventsCoroutine());
    }

    private IEnumerator CallSequenceEventsCoroutine()
    {
        if (SequenceEvents.Length > 0)
        {
            for (int i = 0; i < SequenceEvents.Length; i++)
            {
                yield return ExecuteCurrentEvent(SequenceEvents[i]);
            }
        }

    }

    private IEnumerator ExecuteCurrentEvent(SequenceEvent currentSequence)
    {
        if (currentSequence.eventDone == false)
        {
            yield return StartCoroutine(currentSequence?.CallEventTriggerCoroutine());
            //yield return new WaitForSeconds(3f);
            // currentSequence.eventDone = true;
        }
        yield return StartCoroutine(currentSequence?.CheckEventDoneCoroutine());

        yield return new WaitUntil(() => currentSequence?.eventDone == true);
    }

    public void CheckTimeFinished(float time, string nameSequenceGroup)
    {
        StartCoroutine(CheckTimeFinishedCoroutine(time, nameSequenceGroup));
    }



    private IEnumerator CheckTimeFinishedCoroutine(float time, string nameSequenceGroup)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Se completo el tiempo");
        SetBoolNamedEvent(true, nameSequenceGroup);
    }

    private IEnumerator CheckTimeFinishedCoroutine2(float time, string nameSequenceGroup)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Se completo el tiempo");
        SetBoolNamedEvent(true, nameSequenceGroup);
    }

    public void CheckTimeFinishedSequence(float time)
    {
        StartCoroutine(CheckTimeFinishedSequenceCoroutine(time));
    }

    private IEnumerator CheckTimeFinishedSequenceCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Se completo el tiempo");
        SetBoolEvent(true);
    }
}
