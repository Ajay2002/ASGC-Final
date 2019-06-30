using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Random = UnityEngine.Random;

public class GeneticEntity : MonoBehaviour
{
	#region  Variables

	public enum GeneticType
	{
		Predator,
		Prey
	};

	public GeneticType type;

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