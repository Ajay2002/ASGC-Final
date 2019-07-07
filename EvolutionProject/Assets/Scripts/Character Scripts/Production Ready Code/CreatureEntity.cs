using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEntity : GeneticEntity_T
{
    
    #region  Action Methods

    Transform currentFood;
    public override void Eat(){
        if (food.Count > 0) {
            int i = UnityEngine.Random.Range(0,food.Count-1);
            Transform t = food[i];

            if (t != null) {
                controller.MoveTo(t.position,traits.speed,"ateFood", 0f);
                currentFood = t;
            }
            else {
                food.RemoveAt(i);
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
        base.Breed(e);
    }



    public override void Death(){GameObject.Destroy(this.gameObject);}
    public override void Sleep(){

        if (enemies.Count > 0) {

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
        if (enemies.Count > 0) {

            int fightIndex =0;

            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i] != null)
                if (enemies[i].traits.strength < traits.strength) {
                    fightIndex = i;
                    break;
                }
            }

            if (enemies.Count < friends.Count || enemies.Count <= 1) {
                
                Flight();
                //Fight(enemies[fightIndex]);

            }
            else {

                Flight();

            }

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
        if (friends.Count > 0) {
            bool foundBreeding = false;
            for (int i = 0; i < friends.Count; i++) {
                if (friends[i] == null)
                    {continue;}

                if (bredWith.Contains(friends[i]))
                    {continue;}
                
                if (friends[i].state.age < 20)
                    {continue;}

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

                if (friends[i].traits.attractiveness*friends[i].state.age*friends[i].state.health*friends[i].traits.strength < traits.attractiveness*state.health*state.age*traits.strength)
                continue;

                friends[i].bredWith.Add(this);
                bredWith.Add(friends[i]);
                foundBreeding = true;
                Breed(friends[i]);
                break;
            }

            if (!foundBreeding) {
                Vector3 randomPoint = manager.GetRandomPointAwayFrom(transform.position,traits.sightRange);
                controller.MoveTo(randomPoint,traits.speed,"reproduceCheck", 0f);
            }
            else {
                pausedState = false;
                PauseFor(2);
            }
        }
        else {
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
        base.StateUpdate();
    }

    public override void StateActionConversion() {
        base.StateActionConversion();


    }

    public override void CompletedAction(string invocationTarget){

        if (invocationTarget == "ateFood") {
            
            if (currentFood !=null && food.Contains(currentFood)) {
                GameObject.Destroy(currentFood.gameObject);
                state.energy += 10;
                state.health += 5;
                state.hunger -= 10;
                pausedState = false;
            }
            else {
                Eat();
            }
        }

        if (invocationTarget == "running") {
            pausedState = false;
        }

        if (invocationTarget == "sleepChecking") {
            Sleep();
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
