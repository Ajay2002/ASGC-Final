using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;
using TMPro;
public class GeneticUIController : MonoBehaviour
{

    public Transform sidePanel;
    public PlayerController controller;

    public static GeneticUIController Instance;

    public List<Toggle> selectionType = new List<Toggle>();
    
    public Transform toggleGeneticPanel;
    public Transform openButton;

    [Header("Average State")]
    public TextMeshProUGUI heading;
    public TextMeshProUGUI age;
    public TextMeshProUGUI health;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI hunger;
    public TextMeshProUGUI fear;
    public TextMeshProUGUI thirst;
    public TextMeshProUGUI sleepiness;

    [Header("Traits")]
    public TextMeshProUGUI traitHeading;
    public TextMeshProUGUI traitStateHeading;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI size;
    public TextMeshProUGUI attractiveness;
    public TextMeshProUGUI dangerSense;
    public TextMeshProUGUI sightRange;
    public TextMeshProUGUI strength;
    public TextMeshProUGUI heatResistance;
    public TextMeshProUGUI intelligence;
    public TextMeshProUGUI HI;
    public TextMeshProUGUI HUI;
    public TextMeshProUGUI FI;
    public TextMeshProUGUI SI;
    public TextMeshProUGUI RI;
    public TextMeshProUGUI TI;

    [Header("Creation of Entities")]
    public GeneticTraits currentCreationTrait;
    public BiomeType typeOfBiome;
    public List<SliderText> slText = new List<SliderText>();

    [Header("Modification of Genetics")]
    public List<StepperText> stText = new List<StepperText>();



    private void Awake() {
        selectionMethod = 1;

        if (Instance == null)
            Instance = this;

    }

    [Header("Selection")]
    public Slider slider;
    public int selectionMethod;
    public void EntitySelectionTypeSwitch (int selection) {

        if (selection == 0) {
            selectionMethod = selection;
        }
        else if (selection == 1) {
            selectionMethod = selection;
        }
        else if (selection == 2) {
            selectionMethod = selection;
        }

    }

    private float percModify {
        get {
            return slider.value;
        }
    }

    public List<EntityManager> entities = new List<EntityManager>();
    private void Start() {
        selectionMethod = 1;
        for (int i = 0; i < slText.Count; i++) {
            slText[i].guiText.text = slText[i].originalText + " (" + Math.Round(slText[i].slider.value,1).ToString() + ") ";
            String s = slText[i].guiText.text;
                if (s == "Speed") {
                    currentCreationTrait.speed = Mathf.Clamp(slText[i].getValue,0f,5f);
                }
                else if (s == "Size") {
                    currentCreationTrait.size = Mathf.Clamp(slText[i].getValue,0f,3f);
                }
                else if (s == "Sight Range") {
                    currentCreationTrait.sightRange = Mathf.Clamp(slText[i].getValue,0f,5f);
                }
                else if (s == "Attractiveness") {
                    currentCreationTrait.attractiveness = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Danger Sense") {
                    currentCreationTrait.dangerSense = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Strength") {
                    currentCreationTrait.strength = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Heat Resistance") {
                    currentCreationTrait.heatResistance = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Intelligence") {
                    currentCreationTrait.intellect = Mathf.Clamp(slText[i].getValue,0f,1f);
                }

                else if (s == "Hunger Importance") {
                    currentCreationTrait.HUI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Health Importance") {
                    currentCreationTrait.HI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Sleep Importance") {
                    currentCreationTrait.SI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Merge Importance") {
                    currentCreationTrait.RI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Fear Importance") {
                    currentCreationTrait.FI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
        }

        for (int i = 0; i < stText.Count; i++) {
                
            float a = 0f;
            if (stText[i].valueName == "Food Regeneration Time") {
                a = MapManager.Instance.worldSpawnedFoodSpawnPeriods[0];
            }
            else if (stText[i].valueName == "Total Amount of Food") {
                a = MapManager.Instance.maxAmountOfFood;
            }
            else if (stText[i].valueName == "Energy Per Food") {
                a = MapManager.Instance.worldFoodValue;
            }
            else if (stText[i].valueName == "Mutation Chance") {
                a = MapManager.Instance.mutationChance;
            }
            else if (stText[i].valueName == "Decision Time") {
                EntityManager[] m = EntityManager.FindObjectsOfType<EntityManager>();
                float avg = 0;
                int inc = 0;
                for (int c = 0; c < m.Length; c++) {
                    
                    if (m[c].type == GTYPE.Creature) {
                        avg += m[c].traits.surroundingCheckCooldown;
                        inc++;
                    }

                }
                avg/=inc;
                a = avg;
            }
           
            stText[i].guiText.text = "($"+Math.Round(stText[i].getPrice,1)+") "+stText[i].valueName + " ["+Math.Round(a,1).ToString()+"]";
        }
    }

    public TMP_InputField inp;
    private void Update() {
        int val = 0;
        int.TryParse(inp.text, out val);

        float addedPrice = 500;
        
        for (int i = 0; i < slText.Count; i++) {

            addedPrice += slText[i].price;

        }

        float finalPrice = addedPrice*val;
        costText.text = "Cost : $" + finalPrice.ToString();
        if (finalPrice > CurrencyController.Instance.currentCurrencyAmount) {
            costText.color = Color.red;
        }
        else
            costText.color = Color.green;
        
        entities.Clear();
        if (selectionMethod == 0) {
            
            if (controller.selectedEntityTransforms.Count > 0) {
                entities.Clear();

                for (int i = 0; i < controller.selectedEntityTransforms.Count; i++) {
                    
                    if (controller.selectedEntityTransforms[i] != null)
                    entities.Add(controller.selectedEntityTransforms[i].GetComponent<EntityManager>());
                    
                }   

            }

        }
        else if (selectionMethod == 1) {
            entities.Clear();
            EntityManager[] m = EntityManager.FindObjectsOfType<EntityManager>();
            
            for (int i = 0; i < m.Length; i++) {
                
                if (m[i].type == GTYPE.Creature)
                entities.Add(m[i]);

            }
            
        }
        else if (selectionMethod == 2) {
            entities.Clear();
            EntityManager[] m = EntityManager.FindObjectsOfType<EntityManager>();
            int amount = Mathf.RoundToInt(m.Length*percModify);

            for (int i = 0; i < amount; i++) {
                
                if (m[i].type == GTYPE.Creature)
                entities.Add(m[i]);

            }

        }

        if (traitHeading.color != Color.white) {
            traitHeading.color = Color.Lerp(traitHeading.color,Color.white,0.04f);
        }

        
        if (entities.Count > 0) {
            sidePanel.gameObject.SetActive(true);

            CurrentState averageState = new CurrentState();
            GeneticTraits averageTraits = new GeneticTraits();
            if (entities.Count > 1) {
                heading.text = "Average States";
                traitHeading.text = "Average Traits";
                traitStateHeading.text = "Average State Importance";
            }
            else {
                heading.text = "Selected States";
                traitHeading.text = "Selected Traits";
                traitStateHeading.text = "Selected State Importance";
            }

            int L = 0;
            //TODO: Base it on the average of the selection type
            for (int i = 0; i < entities.Count; i++) {

                if (entities[i] == null)
                continue;

                StateManager st = entities[i].stateManagement;
                GeneticTraits t = entities[i].traits;

                if (st == null)
                    continue;
                
                CurrentState s = st.state;

                

                L++;

                averageTraits.speed += t.speed;
                averageTraits.size += t.size;
                averageTraits.sightRange += t.sightRange;
                averageTraits.attractiveness += t.attractiveness;
                averageTraits.dangerSense += t.dangerSense;
                averageTraits.strength += t.strength;
                averageTraits.heatResistance += t.heatResistance;
                averageTraits.intellect += t.intellect;

                averageTraits.HUI += t.HUI;
                averageTraits.HI += t.HI;
                averageTraits.SI += t.SI;
                averageTraits.RI += t.RI;
                averageTraits.FI += t.FI;

                averageState.ageView += s.ageView;
                averageState.healthView += s.healthView;
                averageState.fearView += s.fearView;
                averageState.energyView += s.energyView;
                averageState.hungerView += s.hungerView;
                averageState.sleepView += s.sleepView;


            }

            if (L > 0) {
                averageState.ageView = Mathf.Round(averageState.ageView/L);
                averageState.healthView = Mathf.Round(averageState.healthView/L);
                averageState.fearView = Mathf.Round(averageState.fearView/L);
                averageState.energyView = Mathf.Round(averageState.energyView/L);
                averageState.hungerView = Mathf.Round(averageState.hungerView/L);
                averageState.sleepView = Mathf.Round(averageState.sleepView/L);

                averageTraits.HUI   /=    L;
                averageTraits.HI    /=    L;
                averageTraits.SI    /=    L;
                averageTraits.RI    /=    L;
                averageTraits.FI    /=    L;

                averageTraits.speed         /=L;
                averageTraits.size              /=L;
                averageTraits.sightRange    /=L;
                averageTraits.attractiveness    /=L;
                averageTraits.dangerSense       /=L;
                averageTraits.strength          /=L;
                averageTraits.heatResistance    /=L;
                averageTraits.intellect         /=L;


                speed.text =            Mathf.RoundToInt((averageTraits.speed*100)/5).ToString()+"/100";
                size.text =             Mathf.RoundToInt((averageTraits.size*100)/3).ToString()+"/100";
                attractiveness.text =   Mathf.RoundToInt(averageTraits.attractiveness*100).ToString()+"/100";
                sightRange.text =       Mathf.RoundToInt((averageTraits.sightRange*100)/5).ToString()+"/100";
                dangerSense.text =      Mathf.RoundToInt(averageTraits.dangerSense*100).ToString()+"/100";
                strength.text =         Mathf.RoundToInt(averageTraits.strength*100).ToString()+"/100";
                heatResistance.text =   Mathf.RoundToInt(averageTraits.heatResistance*100).ToString()+"/100";
                intelligence.text =     Mathf.RoundToInt(averageTraits.intellect*100).ToString()+"/100";
                HI.text =               Mathf.RoundToInt(averageTraits.HI*100).ToString()+"/100";
                HUI.text =              Mathf.RoundToInt(averageTraits.HUI*100).ToString()+"/100";
                FI.text =               Mathf.RoundToInt(averageTraits.FI*100).ToString()+"/100";
                SI.text =               Mathf.RoundToInt(averageTraits.SI*100).ToString()+"/100";
                RI.text =               Mathf.RoundToInt(averageTraits.RI*100).ToString()+"/100";
            

                // age.text =          "Age : " + averageState.ageView.ToString();
                // health.text =       "Health : " + averageState.healthView.ToString();
                // fear.text =         "Fear : " + averageState.fearView.ToString();
                // energy.text =       "Energy : " + averageState.energyView.ToString();
                // hunger.text =       "Hunger : " + averageState.hungerView.ToString();
                // sleepiness.text =   "Sleepiness : " + averageState.sleepView.ToString();
            }

        }
    }


    private void LateUpdate() {
        for (int i = 0; i < stText.Count; i++) {
                
            float a = 0f;
            if (stText[i].valueName == "Food Regeneration Time") {
                a = MapManager.Instance.worldSpawnedFoodSpawnPeriods[0];
            }
            else if (stText[i].valueName == "Total Amount of Food") {
                a = MapManager.Instance.maxAmountOfFood;
            }
            else if (stText[i].valueName == "Energy Per Food") {
                a = MapManager.Instance.worldFoodValue;
            }
            else if (stText[i].valueName == "Mutation Chance") {
                a = MapManager.Instance.mutationChance;
            }
            else if (stText[i].valueName == "Decision Time") {
                EntityManager[] m = EntityManager.FindObjectsOfType<EntityManager>();
                float avg = 0;
                int inc = 0;
                for (int c = 0; c < m.Length; c++) {
                    
                    if (m[c].type == GTYPE.Creature) {
                        avg += m[c].traits.surroundingCheckCooldown;
                        inc++;
                    }

                }
                avg/=inc;
                a = avg;
            }
           
            stText[i].guiText.text = "($"+Math.Round(stText[i].getPrice,1)+") "+stText[i].valueName + " ["+Math.Round(a,2).ToString()+"]";
        }
    }

    public StepperText PriceValue (Stepper s) {

        for (int i = 0; i < stText.Count; i++) {

            if (stText[i].stepper == s) {
                return stText[i];
            }

        }

        return null;

    }

    public void CarryOut (string v, float increment, StepperText t) {

        if (v == "Food Regeneration Time") {
               MapManager.Instance.worldSpawnedFoodSpawnPeriods[0] = t.value;
            }
            else if (v == "Total Amount of Food") {
                MapManager.Instance.maxAmountOfFood = Mathf.RoundToInt(t.value);
            }
            else if (v == "Energy Per Food") {
                MapManager.Instance.worldFoodValue = t.value;
            }
            else if (v == "Mutation Chance") {
                MapManager.Instance.mutationChance = t.value;
            }
            else if (v == "Decision Time") {
                EntityManager[] m = EntityManager.FindObjectsOfType<EntityManager>();
                for (int c = 0; c < m.Length; c++) {
                    
                    if (m[c].type == GTYPE.Creature) {
                        m[c].traits.surroundingCheckCooldown = Mathf.Clamp(m[c].traits.surroundingCheckCooldown+increment,0.1f,3f);
                    }

                }
            }
           
            
    }

    

    public void BiomeChange (string biome) {

        BiomeType t = BiomeType.Grass;

        if (biome == "grass")
            t = BiomeType.Grass;
        else if (biome == "desert")
            t = BiomeType.Desert;
        else if (biome == "snow") 
            t = BiomeType.Snow;
        else if (biome == "forest")
            t = BiomeType.Forest;
        

        float price = 100 * entities.Count;

        if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(price),true)==true) {
            for (int i = 0; i < entities.Count; i++) {

                if (entities[i] == null)
                continue;

                entities[i].creatureBiomeType = t;
                entities[i].ModifyPhysicalAttributes(true);

            }
        }
       


    }

    public void IncrementParams (string fullS) {

        string[] temp = fullS.Split(',');
        string action=temp[0];
        float modification= float.Parse(temp[1]);
        float price = float.Parse(temp[2]);
        Increment(action,modification/5,price);
    }

    public void IncrementParamsPositive (string fullS) {
       
        string[] temp = fullS.Split(',');
        string action=temp[0];
        float modification= float.Parse(temp[1]);
        float price = Mathf.Abs(float.Parse(temp[2]));
    //     print ("HIT ME" + price + ":"+modification/50+":"+action);
        Increment(action,modification,price);
    }

    public void IncrementParamsNegative (string fullS) {
        
        string[] temp = fullS.Split(',');
        string action=temp[0];
        float modification= float.Parse(temp[1]);
        float price = -Mathf.Abs(float.Parse(temp[2]));
      //  print ("HIT ME" + price + ":"+modification/50+":"+action);
        Increment(action,modification,price);
    }

    public void Duplicate() {
        
        //200 be the price of deletion per entity
        float charge = entities.Count*10000;

        if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(charge),true)) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == null)
                continue;
                //Add bred with
                EntityManager m = Instantiate(entities[i].gameObject,MapManager.Instance.GetRandomPoint(),Quaternion.identity).GetComponent<EntityManager>();
                m.initial = false;
                m.GetComponent<EntityGlowOnSelect>().SetSelected(false);
                m.controller.ActionCompletion();

                for (int c = 0; c < entities.Count; c++) {
                    if (entities[c] != null)
                    m.bredWith.Add(entities[c]);
                }

            }
        }
        else {
            //Send Warning
        }

    }

    public void Delete() {

        //200 be the price of deletion per entity
        float charge = entities.Count*500;

        if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(charge),true)) {
            for (int i = 0; i < entities.Count; i++) {
                
                if (entities[i]!=null)
                GameObject.Destroy(entities[i].gameObject);

            }
        }
        else {
            //Send Warning
        }

    }

    public void Close() {
        sidePanel.gameObject.SetActive(false);
    }

    public void Increment (string s, float amount, float expensePerAmount) {
        if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(expensePerAmount),true)==true) {
            for (int i = 0; i < entities.Count; i++) {

                if (entities[i] == null)
                continue;

                GeneticTraits averageTraits = entities[i].traits;

                if (averageTraits == null)
                    continue;

                
                    
                    if (s == "speed") {
                        averageTraits.speed = Mathf.Clamp(averageTraits.speed+amount,0f,5f);
                    }
                    else if (s == "size") {
                        averageTraits.size = Mathf.Clamp(averageTraits.size+amount,0f,3f);
                    }
                    else if (s == "sightRange") {
                        averageTraits.sightRange = Mathf.Clamp(averageTraits.sightRange+amount,0f,5f);
                    }
                    else if (s == "attractiveness") {
                        averageTraits.attractiveness = Mathf.Clamp(averageTraits.attractiveness+amount,0f,1f);
                    }
                    else if (s == "dangerSense") {
                        averageTraits.dangerSense = Mathf.Clamp(averageTraits.dangerSense+amount,0f,1f);
                    }
                    else if (s == "strength") {
                        averageTraits.strength = Mathf.Clamp(averageTraits.strength+amount,0f,1f);
                    }
                    else if (s == "heatResistance") {
                        averageTraits.heatResistance = Mathf.Clamp(averageTraits.heatResistance+amount,0f,1f);
                    }
                    else if (s == "intellect") {
                        averageTraits.intellect = Mathf.Clamp(averageTraits.intellect+amount,0f,1f);
                    }

                    else if (s == "HUI") {
                        averageTraits.HUI = Mathf.Clamp(averageTraits.HUI+amount,0f,1f);
                    }
                    else if (s == "HI") {
                        averageTraits.HI = Mathf.Clamp(averageTraits.HI+amount,0f,1f);
                    }
                    else if (s == "SI") {
                        averageTraits.SI = Mathf.Clamp(averageTraits.SI+amount,0f,1f);
                    }
                    else if (s == "RI") {
                        averageTraits.RI = Mathf.Clamp(averageTraits.RI+amount,0f,1f);
                    }
                    else if (s == "FI") {
                        averageTraits.FI = Mathf.Clamp(averageTraits.FI+amount,0f,1f);
                    }

                    entities[i].ModifyPhysicalAttributes(true);
                    // averageTraits.size += t.size;
                    // averageTraits.sightRange += t.sightRange;
                    // averageTraits.attractiveness += t.attractiveness;
                    // averageTraits.dangerSense += t.dangerSense;
                    // averageTraits.strength += t.strength;
                    // averageTraits.heatResistance += t.heatResistance;
                    // averageTraits.intellect += t.intellect;

                    // averageTraits.HUI += t.HUI;
                    // averageTraits.HI += t.HI;
                    // averageTraits.SI += t.SI;
                    // averageTraits.RI += t.RI;
                    // averageTraits.FI += t.FI;

                
            }
        }
        
       

    }

    public void ConnectValue (Slider v) {

        for (int i = 0; i < slText.Count; i++) {

            if (slText[i].slider == v) {
                slText[i].guiText.text = slText[i].originalText + " (" + (Math.Round(v.value,1)).ToString() + ") ";
                String s = slText[i].guiText.text;
                if (s == "Speed") {
                    currentCreationTrait.speed = Mathf.Clamp(slText[i].getValue,0f,5f);
                }
                else if (s == "Size") {
                    currentCreationTrait.size = Mathf.Clamp(slText[i].getValue,0f,3f);
                }
                else if (s == "Sight Range") {
                    currentCreationTrait.sightRange = Mathf.Clamp(slText[i].getValue,0f,5f);
                }
                else if (s == "Attractiveness") {
                    currentCreationTrait.attractiveness = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Danger Sense") {
                    currentCreationTrait.dangerSense = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Strength") {
                    currentCreationTrait.strength = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Heat Resistance") {
                    currentCreationTrait.heatResistance = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Intelligence") {
                    currentCreationTrait.intellect = Mathf.Clamp(slText[i].getValue,0f,1f);
                }

                else if (s == "Hunger Importance") {
                    currentCreationTrait.HUI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Health Importance") {
                    currentCreationTrait.HI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Sleep Importance") {
                    currentCreationTrait.SI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Merge Importance") {
                    currentCreationTrait.RI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
                else if (s == "Fear Importance") {
                    currentCreationTrait.FI = Mathf.Clamp(slText[i].getValue,0f,1f);
                }
            
            }
        }

    }

    public TextMeshProUGUI costText;
    public void TextChange (TMP_InputField inp) {

        int val = 0;
        int.TryParse(inp.text, out val);

        float addedPrice = 500;
        
        for (int i = 0; i < slText.Count; i++) {

            addedPrice += slText[i].price;

        }

        float finalPrice = addedPrice*val;
        costText.text = "Cost : $" + finalPrice.ToString();
        if (finalPrice > CurrencyController.Instance.currentCurrencyAmount) {
            costText.color = Color.red;
        }
        else
            costText.color = Color.green;
    }
    

    public void AnotherBiomeChange (string biome) {
        BiomeType t = BiomeType.Grass;

        if (biome == "grass")
            t = BiomeType.Grass;
        else if (biome == "desert")
            t = BiomeType.Desert;
        else if (biome == "snow") 
            t = BiomeType.Snow;
        else if (biome == "forest")
            t = BiomeType.Forest;

        typeOfBiome = t;
    }

    public void BuyIT (TMP_InputField inp) {

        int val = 0;
        int.TryParse(inp.text, out val);

        float addedPrice = 500;
        
        for (int i = 0; i < slText.Count; i++) {

            addedPrice += slText[i].price;

        }

        float finalPrice = addedPrice*val;

        if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(finalPrice),true)==true) {

            for (int i = 0; i < val+1; i++) {

                MapManager.Instance.SpawnEntity(MapManager.Instance.GetRandomPoint(),currentCreationTrait,typeOfBiome);

            }

        }

    }

    public void ShowGeneticPanel (bool show) {

        if (show) {

            openButton.gameObject.SetActive(false);
            toggleGeneticPanel.gameObject.SetActive(true);

        }
        else {

            openButton.gameObject.SetActive(true);
            toggleGeneticPanel.gameObject.SetActive(false);
        
        }

    }


}

[System.Serializable]
public class SliderText {

    public Slider slider;
    public TextMeshProUGUI guiText;
    public float priceMax;
    public float distanceFrom;
    public float maxDistance;
    public string originalText;

    public float getValue {
        get {return slider.value;}
    }
    
    public float price {

        get {

            float perc = Mathf.Abs(slider.value-distanceFrom)/maxDistance;
            float val = priceMax*perc;
            return val/10;
        }

    }

}

[System.Serializable]
public class StepperText {

    public Stepper stepper;
    public TextMeshProUGUI guiText;
    public string valueName;
    public float priceIncrement;
    public float maxPrice;
    public float initialPrice;
    private int increment=1;
    private float currentValue;

    public float getPrice {

        get {

            float a = Mathf.Clamp(initialPrice + priceIncrement*increment,0,maxPrice);
            currentValue = a;
            return currentValue;

        }

    }

    public void Inc () {
        increment ++;
    }

    public float value {
        get {
            return stepper.value;
        }
    }

}