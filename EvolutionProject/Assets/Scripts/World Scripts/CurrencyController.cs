﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController Instance;
    
    [SerializeField]
    private float timeBetweenPopulationCurrencyIncrease;
    
    [SerializeField, Min(0)]
    private int currencyAmountUpperBound;

    [SerializeField]
    private int currentCurrencyAmount;

    private void Start()
    {
        if (Instance == null) Instance = this;
        MainUIController.Instance.UpdateCurrencyDisplay(currentCurrencyAmount);
        StartCoroutine(nameof(PopulationCurrencyIncrease));
    }

    private IEnumerator PopulationCurrencyIncrease ()
    {
        AddCurrency(50);
        yield return new WaitForSeconds(timeBetweenPopulationCurrencyIncrease);
        StartCoroutine(nameof(PopulationCurrencyIncrease));
    }

    private void OnValidate ()
    {
        if (currencyAmountUpperBound < 0) currencyAmountUpperBound = -1;
    }

    public bool AddCurrency (int amount)
    {
        bool unclamped = currencyAmountUpperBound == -1 || !(currentCurrencyAmount + Mathf.Abs(amount) > currencyAmountUpperBound);

        currentCurrencyAmount += Mathf.Abs(amount);

        if (currencyAmountUpperBound != -1) currentCurrencyAmount = Mathf.Clamp(currentCurrencyAmount, 0, currencyAmountUpperBound);

        MainUIController.Instance.UpdateCurrencyDisplay(currentCurrencyAmount);

        return unclamped;
    }

    public bool RemoveCurrency (int amount, bool cancelOnInsufficientCurrency)
    {
        bool sufficientCurrency = !(currentCurrencyAmount < Mathf.Abs(amount));
        if (cancelOnInsufficientCurrency && !sufficientCurrency) return false;
        
        currentCurrencyAmount -= Mathf.Abs(amount);
        
        currentCurrencyAmount = Mathf.Clamp(currentCurrencyAmount, 0, currencyAmountUpperBound);
        
        MainUIController.Instance.UpdateCurrencyDisplay(currentCurrencyAmount);

        return sufficientCurrency;
    }
}
