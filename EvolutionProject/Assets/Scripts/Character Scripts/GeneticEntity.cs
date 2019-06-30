using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Random = UnityEngine.Random;

public class GeneticEntity : MonoBehaviour
{
<<<<<<< HEAD
	#region  Variables

	public enum GeneticType
	{
		Predator,
		Prey
	};

	public GeneticType type;
=======
   #region  Variables
    
    GeneticEntity parentA;
    GeneticEntity parentB;

    public bool initial;

    public enum GeneticType {Predator, Prey};
    public GeneticType type;
    //References
    [Header("External References")]
    public MapManager manager;
    public GeneticController controller;

    [Header("Internal References")]
    public GeneticTraits traits;
    public CurrentState state;
    public Brain brain;

    //Private Bools
    [Header("Program State")]
    public float fitness;
    public bool currentlyPerformingAction;
    public float timeSinceAction;

    //Sensory Variables
    public List<GeneticEntity> enemies = new List<GeneticEntity>(); //Enemy Tag
    public List<GeneticEntity> friends = new List<GeneticEntity>(); //Player Tag
    public List<Transform> food = new List<Transform>(); //Food Tag

    public List<GeneticEntity> bredWith = new List<GeneticEntity>();
    #endregion

    #region  Default Methods

    private void Start() {
        if (initial)
        Randomise();
        manager = GameObject.FindObjectOfType<MapManager>();
        controller = GetComponent<GeneticController>();
        //Just start to raom
        Roam();
        controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
        timerSense = traits.surroundingCheckCooldown;
        timerEnabled = true;
    }

    float timerSense = 0f;
    bool timerEnabled = false;
    bool completionCheck = false;
    private void Update() {
        if (timerEnabled == true) {
            if (timerSense > 0) {
                timerSense -= Time.deltaTime;
            }
            else if (timerSense <= 0) {
                controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
                timerSense = traits.surroundingCheckCooldown;
            }
        }

        /*if (currentlyPerformingAction == true) {
            if (completionCheck == false) {
                timeSinceAction = traits.decisionCoolDown;
                completionCheck = true;
            }

            if (completionCheck == true) {

                if (timeSinceAction > 0) {
                    timeSinceAction -= Time.deltaTime;
                }

                if (timeSinceAction < 0) {
                    currentlyPerformingAction = false;
                }

            }

        }*/

        //This is a test of breeding
       
        
        if (state.health > 0){
            StateUpdate();
        }

    }

    #endregion

    #region  Actions
    
    //Pick a random enemy/player
    public void Fight() {
        //Havent done predator
        if (type == GeneticType.Prey) {

            //This is a test for fight (not real)

            if (enemies.Count > 0) {
                currentlyPerformingAction = true;
                int i = UnityEngine.Random.Range(0,enemies.Count-1);
              //  GeneticEntity t = enemies[i];
                
                // t.state.energy -= 20;
                // t.state.health -= 30;
                // t.state.sleepiness += 10;

                state.energy -= 30;
                state.health -= 40;
                state.sleepiness += 20;

            }

        }
    }

    private void OverrideAction() {
        currentlyPerformingAction = true;
    }
    
    private void Death() {
        GameObject.Destroy(this.gameObject);
    }

    //Energy Boost, Hunger Reduction + Health
    int currentlyEating = 0;
    public void Eat() {
        if (food.Count > 0) {
            currentlyPerformingAction = true;
            int i = UnityEngine.Random.Range(0,food.Count-1);
            Transform t = food[i];
            currentlyEating = i;

            controller.MoveTo(t,traits.speed,"ateFood", 0f);

        }
    }

    public void Sleep() {
        currentlyPerformingAction = true;
        controller.MoveTo(manager.GetRandomPointAwayFrom(transform.position,traits.sightRange),traits.speed,"sleepingPointFound",0f);
    }

    public void Nothing() {
        currentlyPerformingAction = true;
    }
    
    //Loop through each tag's network for Entity A and Entity B...
    
    public GeneticEntity Breed (GeneticEntity e) {
        
        currentlyPerformingAction = true;

        Vector3 breedPosition = Vector3.Cross(transform.position,e.transform.position);
        Vector3 avgPosition = (transform.position+e.transform.position)/2;
        avgPosition.y = e.transform.position.y;

        Vector3 creationPosition = avgPosition+breedPosition*1.2f;

        GeneticEntity newEntity = manager.SpawnEntity(transform.position+transform.forward*0.6f).GetComponent<GeneticEntity>();
        
        e.state.energy -= 40;
        e.state.sleepiness += 20;
        e.state.reproductiveness -= 10;

        state.energy -= 40;
        state.sleepiness += 20;
        state.reproductiveness -= 10;

        //FIXME: This assumes that e.brain.tags.Count = brain.tags.Count
        for (int i = 0; i < e.brain.tags.Count; i++) {

            NNetwork A = e.brain.correspondingNetwork[i].network;
            NNetwork B = brain.correspondingNetwork[i].network;
            
            NNetwork Child1 = new NNetwork();
            Child1.Initialise(2,10,8,brain.correspondingNetwork[i].possibleActions.Count);

            for (int w = 0; w < Child1.weights.Count; w++)
            {
                if (UnityEngine.Random.Range(0.01f,1.0f) < manager.mutationChance) {
                    for (int x = 0; x < Child1.weights[w].RowCount; x++)
                    {
                        for (int y = 0; y < Child1.weights[w].ColumnCount; y++)
                        {
                            Child1.weights[w][x, y] = UnityEngine.Random.Range(-1f, 1f);
                        }
                    }
                }
                else {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                    {//(Matrix<float> A, Matrix<float> B) = CrossOver(population[AIndex].weights[w], population[BIndex].weights[w]);
                        Child1.weights[w]=(A.weights[w]);
                    }
                    else {
                        Child1.weights[w]=(B.weights[w]);
                    }
                }
            }

            for (int b = 0; b < Child1.biases.Count; b++)
            {
                if (UnityEngine.Random.Range(0.01f,1.0f) < manager.mutationChance) {
                    Child1.biases[b] = UnityEngine.Random.Range(-1f,1f);
                }
                else {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        Child1.biases[b]=(A.biases[b]);
                    }
                    else
                    {
                        Child1.biases[b] = (B.biases[b]);
                    }
                }
            }

            newEntity.brain.correspondingNetwork[i].network = Child1;
            
            //Creation & Mutation of Traits

            newEntity.traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.3f,2f) : ((traits.surroundingCheckCooldown+e.traits.surroundingCheckCooldown)/2);
        
            newEntity.traits.decisionCoolDown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.5f,5f) : ((traits.decisionCoolDown+e.traits.decisionCoolDown)/2);
            
            newEntity.traits.speed = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.speed+e.traits.speed)/2);
        
            newEntity.traits.size = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,5f) : ((traits.size+e.traits.size)/2);
            
            newEntity.traits.attractiveness = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,100f) : ((traits.attractiveness+e.traits.attractiveness)/2);
        
            newEntity.traits.sightRange = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,5f) : ((traits.sightRange+e.traits.sightRange)/2);
        
            newEntity.traits.dangerSense = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.dangerSense+e.traits.dangerSense)/2);
        
            newEntity.traits.strength = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.strength+e.traits.strength)/2);
        
            newEntity.traits.heatResistance = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.heatResistance+e.traits.heatResistance)/2);
        
            newEntity.traits.intellect  =UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.intellect+e.traits.intellect)/2);
        
            newEntity.traits.brute = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f) : ((traits.brute+e.traits.brute)/2);
            
            newEntity.traits.HI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HI+e.traits.HI)/2);
        
            newEntity.traits.AI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.AI+e.traits.AI)/2);

            newEntity.traits.FI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.FI+e.traits.FI)/2);
        
            newEntity.traits.HUI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HUI+e.traits.HUI)/2);

            newEntity.traits.SI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.SI+e.traits.SI)/2);
        
            newEntity.traits.RI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.RI+e.traits.RI)/2);
            
            //State Resets
            newEntity.state.age = 0;
            newEntity.state.energy = 100;
            newEntity.state.fear = 0;
            newEntity.state.sleepiness = 0;
            newEntity.state.hunger = 0;
            newEntity.state.health = 100;
            newEntity.state.reproductiveness = 0f;

            newEntity.parentA = this;
            newEntity.parentB = e;

            currentlyPerformingAction = false;

        }



        return null;

    }

    public void Roam() {
        Vector3 p = manager.GetRandomPointAwayFrom(transform.position, traits.sightRange);
        controller.MoveTo(p,traits.speed/2,"roamCompleted",0);

        currentlyPerformingAction = true;
        timeSinceAction = 0;
    }

    public void Flight(Vector3 position) {
        Vector3 p = manager.GetRandomPointAwayFrom(position, traits.sightRange);
        controller.MoveTo(p,traits.speed,"flightCompleted",0);

        currentlyPerformingAction = true;
        timeSinceAction = 0;
    }

    public void CompletedAction (string actionType) {
        
        if (actionType == "flightCompleted") {
            currentlyPerformingAction = false;
        }
        else if (actionType == "roamCompleted") {

            //TODO: If nothing is in sight range (this is important)
            //Roam();
            currentlyPerformingAction = false;
        }
        else if (actionType == "ateFood") {
            
            if (currentlyEating > food.Count-1 || currentlyEating < 0) {
                currentlyPerformingAction = false;
                return;
            }
            if (food[currentlyEating] != null) {

                GameObject.Destroy(food[currentlyEating].gameObject);
                state.energy += 30;
                state.health += 10;
                state.hunger -= 40;

            }

            currentlyPerformingAction = false;
        }
        else if (actionType == "sleepingPointFound") {

            if (type == GeneticType.Prey) {
                if (enemies.Count <= 0) {
                    
                    //Found a good place to sleep [engage sleeping... (don't do anything for a couple of seconds)]
                    state.sleepiness -= 50;
                    state.energy += 50;
                    state.fear = 0;
                    state.health += 10;
                    
                }
                else {
                    Sleep();
                }
            }

        }
    }

    #endregion

    #region  Energy Calculations
    public float EnergyMovementCalculation (float movementSpeed) {
        //NOTE: This is a temporary function        
        return movementSpeed*traits.size*0.5f+state.age*0.1f;
    }

    public float EnergyMovementCalculation (float movementSpeed, float d) {
        //NOTE: This is a temporary function        
        return d*(movementSpeed*traits.size-Mathf.Pow(state.age,2));
    }
    #endregion

    #region  State Management

    private void StateUpdate() {
        //TODO: These are really simple at the moment and more complex behaviours will have to emerge
        
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += 0.2f * Time.deltaTime;

        if (state.age >= 100) {
            Death();
        }
        
        if (enemies != null)
        state.fear = Mathf.Clamp(enemies.Count*10 - traits.strength/2 + traits.dangerSense/3,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 25) {
            state.reproductiveness += 3 * Time.deltaTime;
        }
        else if (state.age >= 50 && state.age <= 75) {
            state.reproductiveness -= 3 * Time.deltaTime;
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*0.2f*traits.size),0f,100f);

        StateActionConversion();
    }

    private void StateActionConversion() {

        if (state.hunger >= 90) {
            //Force eat (until it finds food reduce health and energy)
            state.health -= 2 * Time.deltaTime;
            state.energy -= 2 * Time.deltaTime;
        }
        
        if (state.sleepiness >= 90) {
            state.sleepiness = 100;
            //Force sleep (until it finds a place to sleep & settles in reduce health)
            state.health -= 2 * Time.deltaTime;
            state.energy -= 2 * Time.deltaTime;
        }

        if (state.reproductiveness >= 100) {
            //Force reproduction
        }

        if (state.energy <= 5) {
            state.health -= 2 * Time.deltaTime;
        }

        if (state.health <= 0) {
            Death();
        }

        // help.Plot(t,state.age,"Age");
        // help.Plot(t,state.energy,"Energy");
        // help.Plot(t, state.fear,"Fear");
        // help.Plot(t, state.sleepiness,"Sleepiness");
        // help.Plot(t, state.hunger,"Hunger");
        // help.Plot(t, state.reproductiveness,"Reproductive Urge");

    }
    
    #endregion

    #region  Sensory Control

    //Run netowork (Don't forget about state importance trait)
    public void SensoryUpdate(List<GeneticEntity> enemies, List<Transform> food, List<GeneticEntity> friends) {
        if (currentlyPerformingAction) {
            //FIXME: Must return here

            // this.enemies = enemies;
            // this.food = food;
            // this.friends = friends;
            //return;
        }
        this.enemies = enemies;
        this.food = food;
        this.friends = friends;
        RunNetwork();
        //Do the network code here
        //TODO: Don't forget the 'nothing' network 
       // print ("Detected : " + enemies.Count + " enemies, " + friends.Count + " friends, " + food.Count + " items!");
        
    }

    //TODO: Add customisable input numbers (Fix in Random & Breeding)
    private void RunNetwork() {
        float highestOuptut = 0f;
        string highestDecision = "Nothing";
        int brainTag = 0;
        for (int i = 0; i < brain.tags.Count; i++) {
            
            List<float> inputs = new List<float>();
            

            if (brain.tags[i] == "Food") {
                if (food.Count <= 0)
                continue;
                inputs.Add(state.age*traits.AI);
                inputs.Add(state.fear * traits.FI);
                inputs.Add(state.energy);
                inputs.Add(state.health*traits.HI);
                inputs.Add(state.hunger*traits.HUI);
                inputs.Add(state.reproductiveness * traits.RI);
                inputs.Add(state.sleepiness*traits.SI);
                inputs.Add(food.Count);
            }
            else if (brain.tags[i] == "Enemy") {
                if (enemies.Count <= 0)
                continue;
                inputs.Add(state.age*traits.AI);
                inputs.Add(state.fear * traits.FI);
                inputs.Add(state.energy);
                inputs.Add(state.health*traits.HI);
                inputs.Add(state.hunger*traits.HUI);
                inputs.Add(state.reproductiveness * traits.RI);
                inputs.Add(state.sleepiness*traits.SI);
                inputs.Add(enemies.Count);
            }
            else if (brain.tags[i] == "Player") {
                if (friends.Count <= 0)
                continue;
                inputs.Add(state.age*traits.AI);
                inputs.Add(state.fear * traits.FI);
                inputs.Add(state.energy);
                inputs.Add(state.health*traits.HI);
                inputs.Add(state.hunger*traits.HUI);
                inputs.Add(state.reproductiveness * traits.RI);
                inputs.Add(state.sleepiness*traits.SI);
                inputs.Add(friends.Count);
            }
            else if (brain.tags[i] == "Nothing") {
                if (food.Count >= 0 || enemies.Count >= 0 || friends.Count >= 0)
                continue;
                inputs.Add(state.age*traits.AI);
                inputs.Add(state.fear * traits.FI);
                inputs.Add(state.energy);
                inputs.Add(state.health*traits.HI);
                inputs.Add(state.hunger*traits.HUI);
                inputs.Add(state.reproductiveness * traits.RI);
                inputs.Add(state.sleepiness*traits.SI);
                inputs.Add(0);
            }

            brain.correspondingNetwork[i].inputValues = inputs;
            List<float> outputs = brain.correspondingNetwork[i].network.RunNetwork(inputs);
            brain.correspondingNetwork[i].outputValues.Clear();
            for (int r = 0; r < outputs.Count; r++) {
                brain.correspondingNetwork[i].outputValues.Add(outputs[r]);
            }
            int best = 0;
            for (int d = 0; d < outputs.Count; d++) {
                //  print (net.possibleActions[d] + " : " + outputs[d]);
                if (outputs[best] < outputs[d]) {
                    best = d;
                }
            }

            if (highestOuptut < outputs[best]) {
                highestOuptut = outputs[best];
                highestDecision = brain.correspondingNetwork[i].possibleActions[best];
                brainTag = i;
            }

        }

        string selection = highestDecision;

        if (selection == "Fight") {
            Fight();
        }
        else if (selection == "Flight") {
            Flight(transform.position);
        }
        else if (selection == "Nothing") {
            Nothing();
        }
        else if (selection == "Roam") {
            Roam();
        }
        else if (selection == "Eat") {
            Eat();
        }
        else if (selection == "Sleep") {
            Sleep();
        }
        else if (selection == "Breed") {

            
             if (state.health > 0 && state.age >= 25) {
            

                if (friends.Count > 0) {

                    for (int i = 0; i < friends.Count; i++) {
                        if (friends[i] == null)
                        {Roam(); continue;}

                        if (bredWith.Contains(friends[i]))
                            {Roam(); continue;}
                        
                        if (friends[i].state.age <= 25)
                            {Roam(); continue;}

                        if (parentA != null && parentB != null) {
                            if (friends[i] == parentA)
                                continue;

                            if (friends[i] == parentB)
                                continue;

                            if (friends[i].parentA != null && friends[i].parentB != null) {
                            if (friends[i].parentA == parentA || friends[i].parentA == parentB)
                                continue;
                            if (friends[i].parentB == parentB || friends[i].parentB == parentB)
                                continue;

                            
                            if (friends[i].parentA == this)
                            continue;

                            if (friends[i].parentB == this)
                            continue;

                            }

                            

                        }
                        friends[i].bredWith.Add(this);
                        bredWith.Add(friends[i]);

                        Breed(friends[i]);
                    }

                }

            }
            else {
                Roam();
            }
        }

    }

    #endregion

    #region  Other

    public void Randomise() {
        for (int i = 0; i < brain.tags.Count; i++) {
            SenseNetwork net = brain.correspondingNetwork[i];
            net.network.Initialise(2,10,8,net.possibleActions.Count);
        }

        traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.3f,2f);
        
        traits.decisionCoolDown = UnityEngine.Random.Range(0.5f,5f);
        
        traits.speed = UnityEngine.Random.Range(0.01f,10f);
    
        traits.size = UnityEngine.Random.Range(0.01f,5f);
        
        traits.attractiveness = UnityEngine.Random.Range(0.01f,100f);
    
        traits.sightRange = UnityEngine.Random.Range(0.01f,5f);
    
        traits.dangerSense = UnityEngine.Random.Range(0.01f,10f);
    
        traits.strength = UnityEngine.Random.Range(0.01f,10f);
    
        traits.heatResistance = UnityEngine.Random.Range(0.01f,10f);
    
        traits.intellect = UnityEngine.Random.Range(0.01f,10f);
    
        traits.brute = UnityEngine.Random.Range(0.01f,10f);
        
        traits.HI = UnityEngine.Random.Range(0f,1f);
    
        traits.AI = UnityEngine.Random.Range(0f,1f);

        traits.FI = UnityEngine.Random.Range(0f,1f);
    
        traits.HUI = UnityEngine.Random.Range(0f,1f);

        traits.SI = UnityEngine.Random.Range(0f,1f);
    
        traits.RI = UnityEngine.Random.Range(0f,1f);
    }

    #endregion
>>>>>>> Code

	//References
	[Header("External References")]
	public MapManager manager;

	public GeneticController controller;

	[Header("Internal References")]
	public GeneticTraits traits;

	public CurrentState state;
	public Brain        brain;

	//Private Bools
	[Header("Program State")]
	public float fitness;

	public bool  currentlyPerformingAction;
	public float timeSinceAction;

	//Sensory Variables
	public List<GeneticEntity> enemies = new List<GeneticEntity>(); //Enemy Tag
	public List<GeneticEntity> friends = new List<GeneticEntity>(); //Player Tag
	public List<Transform>     food    = new List<Transform>();     //Food Tag

	//Current Action Targets
	private Transform     fightActionTarget;
	private GeneticEntity fightActionTargetEntity;
	private Transform     eatActionTarget;

	//Planned Actions
	private bool plannedSleep;

	#endregion

	#region  Default Methods

	private void Start ()
	{
		//Just start to roam
		Roam();
		controller.FOVChecker(traits.surroundingCheckCooldown, traits.sightRange);
		timerSense   = traits.surroundingCheckCooldown;
		timerEnabled = true;
	}

	float timerSense   = 0f;
	bool  timerEnabled = false;

	private void Update ()
	{
		if (timerEnabled == true)
		{
			if (timerSense > 0)
			{
				timerSense -= Time.deltaTime;
			}
			else if (timerSense <= 0)
			{
				controller.FOVChecker(traits.surroundingCheckCooldown, traits.sightRange);
				timerSense = traits.surroundingCheckCooldown;
			}
		}

		if (state.health > 0)
		{
			StateUpdate();
		}
	}

	#endregion

	#region  Actions

	//Pick a random enemy/player
	public void Fight ()
	{
		switch (type)
		{
			case GeneticType.Prey when fightActionTargetEntity == null:
				fightActionTargetEntity = enemies[Random.Range(0, enemies.Count)];
				break;

			case GeneticType.Predator when fightActionTargetEntity == null:
				fightActionTargetEntity = friends[Random.Range(0, friends.Count)];
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}

		if (fightActionTargetEntity == null) return;

		fightActionTarget = fightActionTargetEntity.transform;

		controller.MoveTo(fightActionTarget, traits.speed, "fightCompleted", 0);

		currentlyPerformingAction = true;
		timeSinceAction           = 0;

		//Make the fight target fight back
		//TODO: Determine if this is necessary
		fightActionTargetEntity.OverrideAction();
		fightActionTargetEntity.fightActionTargetEntity = this;
		fightActionTargetEntity.fightActionTarget       = transform;

		fightActionTargetEntity.Fight();
	}

	private void OverrideAction ()
	{
		currentlyPerformingAction = true;
	}

	private void Death ()
	{
		print("death");

		//TODO: Spawn Food if prey
	}

	//Energy Boost, Hunger Reduction + Health
	public void Eat ()
	{
		Food foodScript;

		foreach (Transform t in food)
		{
			if ((foodScript = t.GetComponent<Food>()) == null || foodScript.canPreyEat == false) continue;

			eatActionTarget = t;
			break;
		}

		if (eatActionTarget == null)
		{
			if (type == GeneticType.Predator) Fight();
			return;
		}

		controller.MoveTo(eatActionTarget.position, traits.speed, "eatCompleted", 0);

		currentlyPerformingAction = true;
		timeSinceAction           = 0;
	}

	public void Sleep ()
	{
		if (enemies.Count == 0)
		{
			//can sleep now

			//wait a couple seconds, then increase energy and health, reset sleepiness
			Invoke(nameof(EndSleep), 5);
			currentlyPerformingAction = true;
			timeSinceAction           = 0;
			return;
		}

		//otherwise, have to find somewhere safe to sleep
		Flight(enemies[0].transform.position);
		plannedSleep = true;
	}

	private void EndSleep ()
	{
		CompletedAction("sleepCompleted");
	}

	public void Nothing () { }

	//Loop through each tag's network

	// public GeneticEntity Breed (GeneticEntity e) {
	//     NNetwork Child1 = new NNetwork();
	//     NNetwork Child2 = new NNetwork();
	//     Child1.Initialise(carController.LAYERS, carController.NEURONS);
	//     Child2.Initialise(carController.LAYERS, carController.NEURONS);

	//     Child1.fitness = 0;
	//     Child2.fitness = 0;

	//     for (int w = 0; w < Child1.weights.Count; w++)
	//     {
	//         if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
	//         {//(Matrix<float> A, Matrix<float> B) = CrossOver(population[AIndex].weights[w], population[BIndex].weights[w]);
	//             Child1.weights[w]=(population[AIndex].weights[w]);
	//             Child2.weights[w]=(population[BIndex].weights[w]);
	//         }
	//         else {
	//             Child1.weights[w]=(population[BIndex].weights[w]);
	//             Child2.weights[w]=(population[AIndex].weights[w]);
	//         }
	//     }

	//     for (int b = 0; b < Child1.biases.Count; b++)
	//     {
	//         if (Random.Range(0.0f, 1.0f) < 0.5f)
	//         {
	//             Child1.biases[b]=(population[AIndex].biases[b]);
	//             Child2.biases[b]=(population[BIndex].biases[b]);
	//         }
	//         else
	//         {
	//             Child1.biases[b] = (population[BIndex].biases[b]);
	//             Child2.biases[b] = (population[AIndex].biases[b]);
	//         }
	//     }


	//     newPopulation[newPopulationIndex] = Child1;
	//     newPopulationIndex++;

	//     newPopulation[newPopulationIndex] = Child2;
	//     newPopulationIndex++;

	//     return null;
	// }

	public void Roam ()
	{
		Vector3 p = manager.GetRandomPointAwayFrom(transform.position, traits.sightRange);
		controller.MoveTo(p, traits.speed / 2, "roamCompleted", 0);

		currentlyPerformingAction = true;
		timeSinceAction           = 0;
	}

	public void Flight (Vector3 position)
	{
		Vector3 p = manager.GetRandomPointAwayFrom(position, traits.sightRange);
		controller.MoveTo(p, traits.speed, "flightCompleted", 0);

		currentlyPerformingAction = true;
		timeSinceAction           = 0;
	}

	public void CompletedAction (string actionType)
	{
		if (actionType == "fightCompleted" && fightActionTarget != null)
		{
			//TODO: Determine proper damage function
			fightActionTargetEntity.state.health -=
				(traits.strength + traits.size) /
				Mathf.Max(fightActionTargetEntity.traits.speed - traits.speed + 1, 0.5f) *
				Time.deltaTime;

			//TODO: Determine proper energy loss function
			state.energy -= (traits.strength + traits.size + traits.speed) / 5f * Time.deltaTime;
		}
		else if (actionType == "eatCompleted" && eatActionTarget != null)
		{
			Food foodScript = eatActionTarget.GetComponent<Food>();

			//eat the food and increase energy + health, reduce hunger			
			state.energy += foodScript.energyValue;
			state.health += foodScript.healthValue; //TODO: Potentially for balance food should only increase energy and sleep only increase health
			state.hunger -= foodScript.hungerValue;

			foodScript.Eat();
		}
		else if (actionType == "sleepCompleted")
		{
			//TODO: Determine values these should be.
			state.energy     += 10;
			state.health     =  100; //TODO: Potentially for balance sleep only increase health and food should only increase energy 
			state.sleepiness =  0;
		}
		else if (actionType == "flightCompleted")
		{
			currentlyPerformingAction = false;
		}
		else if (actionType == "roamCompleted")
		{
			//TODO: If nothing is in sight range (this is important)
			//should Roam only be called after roam if there is nothing in sight range,
			//or should it be called after everything if there is nothing in sight range
			//(i.e. outside of any if statements)
			Roam();

			//should currentlyPerformingAction be set to false after calling roam again?
			currentlyPerformingAction = false;
		}
	}

	#endregion

	#region  Energy Calculations

	public float EnergyMovementCalculation (float movementSpeed)
	{
		//NOTE: This is a temporary function        
		return movementSpeed * traits.size + state.age;
	}

	public float EnergyMovementCalculation (float movementSpeed, float d)
	{
		//NOTE: This is a temporary function        
		return d * (movementSpeed * traits.size - state.age * state.age);
	}

	#endregion

	#region  State Management

	private void StateUpdate ()
	{
		//TODO: These are really simple at the moment and more complex behaviours will have to emerge

		state.energy = Mathf.Clamp(state.energy, 0f, 100f);

		state.age += 0.2f * Time.deltaTime;

		if (state.age >= 100)
		{
			Death();
		}

		if (enemies != null)
			state.fear = Mathf.Clamp(enemies.Count * 10 - traits.strength / 2 + traits.dangerSense / 3, 0f, 100f);

		if (state.energy <= 50)
		{
			state.sleepiness = Mathf.Clamp(state.sleepiness + 2 * Time.deltaTime, 0f, 100f);
		}

		if (state.age <= 50 && state.age >= 25)
		{
			state.reproductiveness += 3 * Time.deltaTime;
		}
		else if (state.age >= 50 && state.age <= 75)
		{
			state.reproductiveness -= 3 * Time.deltaTime;
		}


		state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime * 0.2f * traits.size), 0f, 100f);

		StateActionConversion();
	}

	private void StateActionConversion ()
	{
		if (state.hunger >= 90)
		{
			//Force eat (until it finds food reduce health and energy)
			state.health -= 2 * Time.deltaTime;
			state.energy -= 2 * Time.deltaTime;
		}

		if (state.sleepiness >= 90)
		{
			state.sleepiness = 100;

			//Force sleep (until it finds a place to sleep & settles in reduce health)
			state.health -= 2 * Time.deltaTime;
			state.energy -= 2 * Time.deltaTime;
		}

		if (state.reproductiveness >= 100)
		{
			//Force reproduction
		}

		if (state.energy <= 5)
		{
			state.health -= 2 * Time.deltaTime;
		}

		if (state.health <= 0)
		{
			Death();
		}

		// help.Plot(t,state.age,"Age");
		// help.Plot(t,state.energy,"Energy");
		// help.Plot(t, state.fear,"Fear");
		// help.Plot(t, state.sleepiness,"Sleepiness");
		// help.Plot(t, state.hunger,"Hunger");
		// help.Plot(t, state.reproductiveness,"Reproductive Urge");
	}

	#endregion

	#region  Sensory Control

	public void SensoryUpdate (List<GeneticEntity> enemies, List<Transform> food, List<GeneticEntity> friends)
	{
		if (currentlyPerformingAction)
		{
			// this.enemies = enemies;
			// this.food = food;
			// this.friends = friends;
			// //return;
		}

		//Do the network code here
		// print ("Detected : " + enemies.Count + " enemies, " + friends.Count + " friends, " + food.Count + " items!");
		this.enemies = enemies;
		this.food    = food;
		this.friends = friends;
	}

	#endregion

	#region  Other

	public void Randomise ()
	{
		for (int i = 0; i < brain.tags.Count; i++)
		{
			SenseNetwork net = brain.correspondingNetwork[i];
			net.network.Initialise(2, 10, 6, net.possibleActions.Count);
		}

		traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.3f, 2f);

		traits.decisionCoolDown = UnityEngine.Random.Range(0.5f, 5f);

		traits.speed = UnityEngine.Random.Range(0.01f, 10f);

		traits.size = UnityEngine.Random.Range(0.01f, 5f);

		traits.attractiveness = UnityEngine.Random.Range(0.01f, 100f);

		traits.sightRange = UnityEngine.Random.Range(0.01f, 5f);

		traits.dangerSense = UnityEngine.Random.Range(0.01f, 10f);

		traits.strength = UnityEngine.Random.Range(0.01f, 10f);

		traits.heatResistance = UnityEngine.Random.Range(0.01f, 10f);

		traits.intellect = UnityEngine.Random.Range(0.01f, 10f);

		traits.brute = UnityEngine.Random.Range(0.01f, 10f);

		traits.HI = UnityEngine.Random.Range(0f, 1f);

		traits.AI = UnityEngine.Random.Range(0f, 1f);

		traits.FI = UnityEngine.Random.Range(0f, 1f);

		traits.HUI = UnityEngine.Random.Range(0f, 1f);

		traits.SI = UnityEngine.Random.Range(0f, 1f);

		traits.RI = UnityEngine.Random.Range(0f, 1f);
	}

	#endregion
}

[System.Serializable]
public class GeneticTraits
{
	public float surroundingCheckCooldown;
	public float decisionCoolDown;
	public float speed;
	public float size;
	public float attractiveness;
	public float sightRange;
	public float dangerSense;
	public float strength;
	public float heatResistance;
	public float intellect;
	public float brute;

	public float HI;
	public float AI;
	public float FI;
	public float HUI;
	public float SI;
	public float RI;
}

[System.Serializable]
public struct CurrentState
{
	public float energy;
	public float health;
	public float age;
	public float fear;
	public float hunger;
	public float sleepiness;
	public float reproductiveness;
}


[System.Serializable]
public class SenseNetwork
{
	public List<string> possibleActions = new List<string>();
	public List<float>  inputValues     = new List<float>();
	public List<float>  outputValues    = new List<float>();
	public NNetwork     network;
}

[System.Serializable]
public class Brain
{
	[Header("Vision Sense")]
	public float fov;

	public float              range;
	public List<string>       tags                 = new List<string>();
	public List<SenseNetwork> correspondingNetwork = new List<SenseNetwork>();
}