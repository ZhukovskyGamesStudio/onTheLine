using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinanceBook : TableItemBehaviour
{
    public GameObject linePrefab;
    public Transform linesParent;
    public FinanceLine sumLine;
    FinanceLine foodLine;

    int calculatedMoney;
    bool isBuyingFood;

    private void Start()
    {
        SaveManager.instance.OnUnloadScene += ApproveFinancePlan;
        DisplayDayResult(SaveManager.sv.dayResult);
    }
    public void DisplayDayResult(DayResult dayResult)
    {
        int money = SaveManager.sv.money;
        calculatedMoney = money;
        AddLine("¡€ÀŒ", money);

        if(dayResult != null)
        {
            calculatedMoney += Settings.config.payForDay;
            AddLine("—“¿¬ ¿ “≈À≈‘ŒÕ»—“ »", Settings.config.payForDay);

            int bonusForCalls = Settings.config.payForCall * dayResult.callsServed;
            calculatedMoney += bonusForCalls;
            AddLine("¡ŒÕ”— «¿ «¬ŒÕ »  x" + dayResult.callsServed, bonusForCalls);

            int penaltySum = 0;
            for (int i = 0; i < dayResult.penaltyAmount; i++)
            {
                penaltySum += (i - Settings.config.freePenalties) * Settings.config.penaltyIncrease;
            }
            if (penaltySum < 0)
                penaltySum = 0;
            if (dayResult.penaltyAmount > 0)
            {
                calculatedMoney -= penaltySum;
                AddLine("ÿ“–¿‘€ x" + dayResult.penaltyAmount, penaltySum);
            }
        }
       

        isBuyingFood = false;
        foodLine = AddLine("œ–Œœ»“¿Õ»≈ ", Settings.config.foodPrice*-1, ChangeBuyingFood);

        ChangeBuyingFood();
        UpdateSumLine();
    }

    void ChangeBuyingFood()
    {
        if (isBuyingFood)
        {
            calculatedMoney += Settings.config.foodPrice;
            isBuyingFood = false;
            foodLine.SetCrossed(true);
        }
        else
        {
            if (calculatedMoney >= Settings.config.foodPrice)
            {
                calculatedMoney -= Settings.config.foodPrice;
                isBuyingFood = true;
                foodLine.SetCrossed(false);
            }
        }
        UpdateSumLine();
    }

    void UpdateSumLine()
    {
        sumLine.SetValues("»“Œ√Œ", calculatedMoney);
    }

    FinanceLine AddLine(string text, int price, UnityAction onPressed = null)
    {
        GameObject go = Instantiate(linePrefab, linesParent);
        go.SetActive(true);
        FinanceLine line = go.GetComponent<FinanceLine>();

        if (onPressed != null)
        {
            line.SetCrossed(true);
            line.button.onClick.AddListener(onPressed);
            line.button.interactable = true;
        }

        line.SetValues(text, price);
        return line;
    }

    public void ApproveFinancePlan()
    {
        SaveManager.ChangeMoney(calculatedMoney);
        SaveManager.sv.dayResult = null;
    }

    private void OnDestroy()
    {
        SaveManager.instance.OnUnloadScene -= ApproveFinancePlan;
    }

}
[System.Serializable]
public class PurchasableItem
{
    public string name;
    public int price;
    public UnityEvent onPurchase;
}
