using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorEntity : GeneticEntity_T
{
    
    #region  Action Methods

    GeneticEntity_T currentFood;
    public override void Eat(){
        if (friends.Count > 0) {
            int i = UnityEngine.Random.Range(0,friends.Count-1);
            GeneticEntity_T t = friends[i];

            if (t != null) {
                controller.MoveTo(t.transform.position,traits.speed*1.5f,"ateFood", 0f);
                currentFood = t;
            }
            else {
                friends.RemoveAt(i);
                Eat();
            }

        }
        else {

            Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
            controller.MoveTo(randomPoint,traits.speed/2,"ateFood", 0f);
            actionUpdateString = "FoodSearching";

        }
    }

    public override void Fight(GeneticEntity_T fighter){
        if (fighter != null) {
            fighter.state.energy -= 20;
            fighter.state.health -= 30;
            fighter.state.sleepiness += 10;
        }
        state.energy -= 30;
        state.health -= 40;
        state.sleepiness += 20;
        
        
        pausedState = false;
        PauseFor(1);
    }

    public override void Flight(){
        Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
        controller.MoveTo(randomPoint,traits.speed,"running", 0f);
    }

    public override void Breed(GeneticEntity_T e) {
        GeneticEntity_T newEntity = manager.SpawnEntityEnemy(manager.GetRandomPointAwayFrom(transform.position,traits.sightRange)).GetComponent<GeneticEntity_T>();
        
        #region  Trait Modification
        newEntity.traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.1f,2f) : ((traits.surroundingCheckCooldown+e.traits.surroundingCheckCooldown)/2);
    
        newEntity.traits.decisionCoolDown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.1f,5f) : ((traits.decisionCoolDown+e.traits.decisionCoolDown)/2);
        
        newEntity.traits.speed = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? (UnityEngine.Random.Range(0.01f,1f)*5) : ((traits.speed+e.traits.speed)/2);
    
        newEntity.traits.size = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f)*3 : ((traits.size+e.traits.size)/2);
        
        newEntity.traits.attractiveness = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.attractiveness+e.traits.attractiveness)/2);
    
        newEntity.traits.sightRange = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,10f)*10 : ((traits.sightRange+e.traits.sightRange)/2);
    
        newEntity.traits.dangerSense = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.dangerSense+e.traits.dangerSense)/2);
    
        newEntity.traits.strength = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.strength+e.traits.strength)/2);
    
        newEntity.traits.heatResistance = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.heatResistance+e.traits.heatResistance)/2);
    
        newEntity.traits.intellect  =UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.intellect+e.traits.intellect)/2);
    
        newEntity.traits.brute = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.brute+e.traits.brute)/2);
        
        newEntity.traits.HI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HI+e.traits.HI)/2);
    
        newEntity.traits.AI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.AI+e.traits.AI)/2);

        newEntity.traits.FI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.FI+e.traits.FI)/2);
    
        newEntity.traits.HUI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HUI+e.traits.HUI)/2);

        newEntity.traits.SI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.SI+e.traits.SI)/2);
    
        newEntity.traits.RI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.RI+e.traits.RI)/2);
        #endregion

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

         //State changing with regards to breeding
        e.state.energy -= 20;
        e.state.hunger += 20;
        e.state.sleepiness += 10;
        e.state.reproductiveness -= 10;

        state.energy -= 20;
        state.hunger += 20;
        state.sleepiness += 10;
        state.reproductiveness -= 10;

    }



    public override void Death(){GameObject.Destroy(this.gameObject);}
    public override void Sleep(){

        if (friends.Count > 0) {

            Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
            controller.MoveTo(randomPoint,traits.speed,"sleepChecking", 0f);

        }
        else {

       //     Debug.DrawLine(transform.position,transform.position+Vector3.up*10,Color.red,10f);
//            print ("Sleeping");
            state.energy += 60;
            state.sleepiness -= 100;
            state.health += 20;

            pausedState = false;
            PauseFor(5);
        }

    }
    public override void Roam(){}

    #endregion

    #region  Decision Making

    public override void LowHealth() {

        pausedState = true;

        if (state.hunger > state.sleepiness) {
            Eat();
        }
        else if (state.hunger <= state.sleepiness) {
            Sleep();
        }

    }

    public override void LowEnergy() {
        pausedState = true;
        
        if (state.hunger > state.sleepiness) {
            Eat();
        }
        else if (state.hunger <= state.sleepiness) {
            Sleep();
        }

    }

    public override void HighFear() {
        pausedState = true;
        if (friends.Count > 0) {

            int fightIndex =0;

            for (int i = 0; i < friends.Count; i++) {
                if (friends[i] != null)
                if (friends[i].traits.strength < traits.strength) {
                    fightIndex = i;
                    break;
                }
            }

            Flight();
            //Fight(friends[fightIndex]);

        }
        else {
            Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
            controller.MoveTo(randomPoint,traits.speed,"running", 0f);
        }

    }

    
    public override void HighReproductivity() {
        pausedState = true;
        Reproduce();
    }

    private void Reproduce() {
        //Debug.DrawLine(transform.position,transform.position+Vector3.up*5,Color.green,1);
        if (enemies.Count > 0) {
            bool foundBreeding = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i] == null)
                    {continue;}

                if (bredWith.Contains(enemies[i]))
                    {continue;}
                
                if (enemies[i].state.age < 20)
                    {continue;}

                if (parentA != null && parentB != null) {
                    if (enemies[i] == parentA)
                        continue;

                    if (enemies[i] == parentB)
                        continue;

                    if (enemies[i].parentA != null && enemies[i].parentB != null) {
                        if (enemies[i].parentA == parentA || enemies[i].parentA == parentB)
                            continue;
                        if (enemies[i].parentB == parentB || enemies[i].parentB == parentB)
                            continue;

                        
                        if (enemies[i].parentA == this)
                        continue;

                        if (enemies[i].parentB == this)
                        continue;

                    }

                        

                }

                if (enemies[i].traits.attractiveness*enemies[i].state.age*enemies[i].state.health*enemies[i].traits.strength < traits.attractiveness*state.health*state.age*traits.strength)
                continue;

                enemies[i].bredWith.Add(this);
                bredWith.Add(enemies[i]);
                foundBreeding = true;
//                print ("Breeeeedingg");
                Breed(enemies[i]);
                //Debug.DrawLine(transform.position,transform.position+Vector3.up*5,Color.red,10);
                break;
            }

            if (!foundBreeding) {
                pausedState = true;
                Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
                controller.MoveTo(randomPoint,traits.speed,"reproduceCheck", 0f);
            }
            else {
                pausedState = false;
                PauseFor(2);
            }
        }
        else {
            pausedState = true;
            Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
            controller.MoveTo(randomPoint,traits.speed,"reproduceCheck", 0f);
        }
    }

    //Roam until food is found...
    public override void FoodRequired() {
        pausedState = true;
        Eat();
    }

    #endregion

    public override void StateUpdate() {
        //TODO: These are really simple at the moment and more complex behaviours will have to emerge
        
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += ageRate * Time.deltaTime;

        if (state.age >= 100) {
            Death();
        }
        
        // if (friends != null)
        // state.fear = Mathf.Clamp(friends.Count*10 - (traits.strength*10)/2 + traits.dangerSense*10,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+0.3f*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 20) {
            state.reproductiveness += 0.9f * Time.deltaTime;
        }
        else if (state.age >= 50 && state.age <= 100) {
            state.reproductiveness -= 0.9f * Time.deltaTime;
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*0.2f*traits.size),0f,100f);


        //Consequences
        if (state.health <= 0) {
            Death();
        }

        if (state.age >= 100) {
            Death();
        }

        if (state.hunger >= 100) {
            state.energy -= 2*Time.deltaTime;
            state.health -= 2*Time.deltaTime;
        }

        if (state.sleepiness >= 100) {
            state.energy -= 1.5f*Time.deltaTime;
            state.health -= 2f*Time.deltaTime;
        }

        if (!currentlyPausedState && !pausedState)
        StateActionConversion();
    }

    public override void StateActionConversion() {
        base.StateActionConversion();


    }

    public override void CompletedAction(string invocationTarget){

        if (invocationTarget == "ateFood") {
            
            if (currentFood !=null && friends.Contains(currentFood) && Vector3.Distance(currentFood.transform.position,transform.position) < 0.7f) {
                float h = currentFood.state.health-25;
                if (h <= 0) {
                    currentFood.Death();
                }
                else {
                    currentFood.state.health = h;
                }
                state.energy += 10;
                state.health += 10;
                state.hunger -= 70;
                pausedState = false;
            }
            else {
                pausedState = false;
                //Eat();
            }
        }

        if (invocationTarget == "running") {
            pausedState = false;
        }

        if (invocationTarget == "sleepChecking") {
            //Sleep();
            Eat();
            pausedState = false;
        }

        if (invocationTarget == "reproduceCheck") {
            Reproduce();
        }
    }

    public override void StateActionUpdate (string invocationTarget) {
        
    }

    #region Energy Management

    public override float EnergyMovementCalculation (float movementSpeed) {
        return movementSpeed*traits.size*0.5f+state.age*0.1f;
    }


    #endregion
}
