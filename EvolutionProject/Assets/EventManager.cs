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
                    NotificationManager.Instance.CreateNotification(currentEvent.notificationToPrior);
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
                    NotificationManager.Instance.CreateNotification(currentEvent.notificationOnEnter);
                    currentEvent.Start(this);

                }

            }

        }

        if (cps == CurrentProgramState.RunningEvent) {
            currentTimer -= Time.deltaTime;
            if (currentEvent != null)
                currentEvent.Update();

            if (currentTimer <= 0f) {
                //Call Compeltion of TIme
                currentEvent.Completed();

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
    public float payoff;
    public bool endGame;
    public Notification notificationOnEnter;
    public Notification notificationToPrior;

    public abstract void Update();
    public abstract void Start(EventManager manager);
    public abstract void CompletionChecking();
    public abstract void Completed ();
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

        int maxFoxPopulation = WaveManager.Instance.foxWaves[WaveManager.Instance.currentWave].maxNumberOfFoxes;
        float perc = WaveManager.Instance.foxWaves[WaveManager.Instance.currentWave].maxPercentKill;
        int currentPopulation = 0;

        int currentCreaturePopulation = 0;

        EntityManager[] creatures = GameObject.FindObjectsOfType<EntityManager>();
        for (int i = 0; i < creatures.Length; i++) {
            if (creatures[i].type == GTYPE.Predator && creatures[i].isPartOfWave == true) {
                currentPopulation ++;
                if (currentPopulation > maxFoxPopulation) {
                    //Find a better way to do this than to instant kill them.
                    GameObject.Destroy(creatures[i].gameObject);
                    currentPopulation --;
                }

                continue;
            }

            if (creatures[i].type == GTYPE.Creature) {

                currentCreaturePopulation ++;

            }

        }

        if (currentCreaturePopulation <= startingPopulation*perc) {
            WaveManager.Instance.KillWave();
        }
    
    }

    int startingPopulation = 0;

    public override void Start(EventManager manager)  {

        EntityManager[] creatures = GameObject.FindObjectsOfType<EntityManager>();
        for (int i = 0; i < creatures.Length; i++) {
            if (creatures[i].type == GTYPE.Creature)
                startingPopulation ++;
        }

        WaveManager.Instance.CreateNewWave();
        
    }

    public override void Completed() {
        WaveManager.Instance.KillWave();
        EntityManager[] creatures = GameObject.FindObjectsOfType<EntityManager>();
        for (int i = 0; i < creatures.Length; i++) {
            if (creatures[i].type == GTYPE.Predator && creatures[i].isPartOfWave == true) {
                
                GameObject.Destroy(creatures[i].gameObject);
                continue;
            }

            
        }
        CurrencyController.Instance.AddCurrency(Mathf.RoundToInt(payoff));

        if (endGame)
            Application.LoadLevel(2);
    }

    private void CreateWave() {

    }

    public override void FunctionCall (string query) {

    }


}

public class FoodShortage : EventTemplate {

    public override void Update() {

    }

    float percentDrop = 0f;
    int initialFoodCount = 0;
    public override void Start(EventManager manager) {
        
        percentDrop = Random.Range(0.4f,0.6f);
        initialFoodCount = MapManager.Instance.maxAmountOfFood;
        MapManager.Instance.maxAmountOfFood = Mathf.RoundToInt(initialFoodCount*percentDrop);
        GeneticUIController.Instance.ResetValue("Total Amount of Food");
    }
    
    public override void Completed() {
        MapManager.Instance.maxAmountOfFood = initialFoodCount;        
        CurrencyController.Instance.AddCurrency(Mathf.RoundToInt(payoff));
        
        GeneticUIController.Instance.ResetValue("Total Amount of Food");
        if (endGame)
            Application.LoadLevel(2);
    }

    public override void CompletionChecking() {

    }

    public override void FunctionCall(string query) {

    }

}

/*
    public override void Update() {

    }

    public override void Start(EventManager manager) {

    }
    
    public override void Completed() {

    }

    public override void CompletionChecking() {

    }

    public override void FunctionCall(string query) {
        
    } 
*/

[System.Serializable]
public class FoxWaveTraits  {

    [Header("State Management")]
    public int maxAge=100;
    public float movementCost = 5;
    public float eatPower=1f;
    public float hungerIncrease=1.2f;
    public float sleepPower=1f;
    public float reproductiveIncrease=2;
    public float reproductiveCost=1f;

    [Header("Wave")]
    public int numberOfFoxes;
    public int maxNumberOfFoxes;
    public float maxPercentKill;
    
    [Header("Genetics")]
    public float variation;
    public GeneticTraits averageTraits;
    public CurrentState startingState;

}


[System.Serializable]
public class EPublic {

    public EType type;
    public string eventName;
    public float payoff;
    public float duration;
    public float durationTo;
    public bool endGame;

    public Notification notificationOnEnter;
    public Notification notificationToPrior;

    public EventTemplate GenerateTemplate () {

        if (type == EType.FoxWave) {

            FoxWave w = new FoxWave();
            w.payoff = payoff;
            w.eventName = eventName;
            w.type = type;
            w.duration = duration;
            w.durationTo = durationTo;
            w.notificationOnEnter = notificationOnEnter;
            w.notificationToPrior = notificationToPrior;
            w.endGame = endGame;

            return w;
        }
        else if (type == EType.FoodShortage) {
            FoodShortage w = new FoodShortage();
            w.eventName = eventName;
            w.type = type;
            w.payoff = payoff;
            w.duration = duration;
            w.durationTo = durationTo;
            w.notificationOnEnter = notificationOnEnter;
            w.notificationToPrior = notificationToPrior;
            w.endGame = endGame;
            return w;
        }

        Debug.LogError("This type of event has not been created yet...");
        return null;

    }

}