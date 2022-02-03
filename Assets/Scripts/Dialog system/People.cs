using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class People 
{
    public List<Person> generatedPeople;
    List<Person> availablePeople;
    List<Person> roomedPeople;
    List<Person> doomedPeople;

    public void GenerateAdditionalPeople()
    {
        for (int i = 0; i < 100; i++)        
            generatedPeople.Add(Person.Generate());
    }
}
