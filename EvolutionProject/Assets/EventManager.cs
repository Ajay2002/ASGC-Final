using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Events")]
    public int currentEventIndex = -1;
    public List<EPublic> eventList = new List<EPublic>();
    private EventTemplate currentEvent;

    //Instance
    public static EventManager Instance;



    [Header("Private")]
    //Private Variables
    public float currentTimer = 0f;
    private bool timerEnabled = false;

    public enum CurrentProgramState {RunningEvent, Finished, Waiting, Nothing};
    public CurrentProgramState cps;

    private void Awake() {
        if (Instance == null)
            Instance = this;

        
       EnableEventSystem(true);
    }

    public void EnableEventSystem(bool goBackToStart) {
        if (goBackToStart)
            currentEventIndex = -1;
        
        currentEvent = null;

        timerEnabled = true;
        currentTimer = 0f;
        cps = CurrentProgramState.Finished;
    }

    public void DisableEventSystem() {
        timerEnabled = false;
        currentEvent = null;
        currentTimer = 0f;
        cps = CurrentProgramState.Nothing;
    }

    private void Update() {

        if (cps == CurrentProgramState.Finished) {
            currentEventIndex++;
            if (currentEventIndex >= 0 && currentEventIndex < eventList.Count) {

                currentEvent = eventList[currentEventIndex].GenerateTemplate();
                if (currentEvent != null) {
                    currentTimer = currentEvent.durationTo;
                    cps = CurrentProgramState.Waiting;
                }
                else {
                    Debug.LogError("Something went wrong!");
                }

            }
            else 
            {
                DisableEventSystem();
            }

        }

        if (cps == CurrentProgramState.Waiting) {
            if (timerEnabled) {

                if (currentTimer > 0) {
                    currentTimer -= Time.deltaTime;
                }
                else {

                    //Start the event 
                    cps = CurrentProgramState.RunningEvent;
                    currentTimer = currentEvent.duration;
                    currentEvent.Start(this);

                }

            }

        }

        if (cps == CurrentProgramState.RunningEvent) {
            currentTimer -= Time.deltaTime;
            if (currentEvent != null)
                currentEvent.Update();

            if (currentTimer <= 0f) {
                currentEvent = null;
                currentTimer = 0f;
                cps = CurrentProgramState.Finished;
            }
        }
    }

    private void ChooseAction() {



    }

}

public enum EType {
    FoxWave,
    Fire,
    FoodShortage,
    Disease,
    LandDestruction
}

[System.Serializable]
public abstract class EventTemplate  {

    public string eventName;
    public EType type;
    public float duration;
    public float durationTo;
    public Notification notificationOnEnter;
    public Notification notificationToPrior;

    public abstract void Update();
    public abstract void Start(EventManager manager);
    public abstract void CompletionChecking();
    public abstract void FunctionCall (string query);

}

/* Wave Definitions : 

1. Increase the amount of foxes
2. Increase the starting states of the foxes
3. Increase the traits of the foxes slightly

*/

public class FoxWave : EventTemplate {

    public override void Update() {


        //Check for Completion
        CompletionChecking();
    }

    public override void CompletionChecking() {

    }

    public override void Start(EventManager manager)  {
        
    }

    private void CreateWave() {

    }

    public override void FunctionCall (string query) {

    }


}

public class FoxWaveTraits  {

    public int numberOfFoxes;
    public int maxNumberOfFoxes;
    public float maxPercentKill;
    
    public GeneticTraits averageTraits;
    public CurrentState startingState;

}


[System.Serializable]
public class EPublic {

    public EType type;
    public string eventName;

    public float duration;
    public float durationTo;

    public Notification notificationOnEnter;
    public Notification notificationToPrior;

    public EventTemplate GenerateTemplate () {

        if (type == EType.FoxWave) {

            FoxWave w = new FoxWave();
            w.eventName = eventName;
            w.type = type;
            w.duration = duration;
            w.durationTo = durationTo;
            w.notificationOnEnter = notificationOnEnter;
            w.notificationToPrior = notificationToPrior;

            return w;
        }

        Debug.LogError("This type of event has not been created yet...");
        return null;

    }

}