using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Person", menuName = "ScriptableObjects/Person", order = 2)]
[System.Serializable]
public class Person : ScriptableObject
{
    public string Name, Surname;
    public Work Work;
    public Temperament Temperament;
    public Age Age;
    public Sex Sex;
    public Criminal Criminal;
    public bool isDead;
    public int roomNumber;
    readonly static string[] womanNames = { "Наташа", "Диана", "Саша", "Лиза", "София", "Ксения", "Зухра", "Анна" };
    readonly static string[] manNames = { "Борис", "Георгий", "Ярослав", "Иосиф", "Владимир", "Фёдор", "Максим", "Виктор" };
    readonly static string[] surnames = { "Гриневич", "Полищук", "Есин(а)", "Колегов(а)", "Семёнов(а)", "Карпов(а)", "Лучинов(а)", "Сидоров(а)" };

    public List<LineAnswer> answers;
    public Person()
    {

    }
    public Person(Work _work, Temperament _temperament, Age _age, Sex _sex, Criminal _criminal = Criminal.None)
    {
        this.Work = _work;
        this.Temperament = _temperament;
        this.Age = _age;
        this.Sex = _sex;
        this.Criminal = _criminal;
    }

    public bool HasAnswer(string line)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i].line == line)
            {
                return true;
            }
        }
        return false;
    }

    public string Hear(string line)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i].line == line)
            {
                if(answers[i].newTag != Tags.none)
                    TagManager.AddTag(answers[i].newTag);
                return answers[i].answer;
            }
        }
        return null;
    }


    /// <summary>
    /// Генерирует только не криминальных людей. Криинальность надо задать вручную.
    /// </summary>
    /// <returns></returns>
    public static Person Generate()
    {
        Person person = new Person();
        person.Sex = (Sex)Random.Range(0, 2);
        person.Name = person.Sex == Sex.Woman ? womanNames[Random.Range(0, womanNames.Length)] : manNames[Random.Range(0, manNames.Length)];
        person.Surname = surnames[Random.Range(0, surnames.Length)];
        person.Work = (Work)Random.Range(0, 4);
        person.Temperament = (Temperament)Random.Range(0, 3);
        person.Age = (Age)Random.Range(0, 3);
        person.Criminal = Criminal.None;


        return person;
    }
    public static Person Generate(PersonShablon shablon)
    {
        Person person = new Person();

        if (shablon.Work == Work.Any)
            person.Work = (Work)Random.Range(0, 4);
        else
            person.Work = shablon.Work;

        if (shablon.Temperament == Temperament.Any)
            person.Temperament = (Temperament)Random.Range(0, 3);
        else
            person.Temperament = shablon.Temperament;

        if (shablon.Age == Age.Any)
            person.Age = (Age)Random.Range(0, 3);
        else
            person.Age = shablon.Age;

        if (shablon.Sex == Sex.Any)
            person.Sex = (Sex)Random.Range(0, 2);
        else
            person.Sex = shablon.Sex;

        if (shablon.Criminal == Criminal.Any)
            person.Criminal = Criminal.None;
        else
            person.Criminal = Criminal.Criminal;


        person.Name = person.Sex == Sex.Woman ? womanNames[Random.Range(0, womanNames.Length)] : manNames[Random.Range(0, manNames.Length)];
        person.Surname = Random.Range(0, 2) == 1 ? "Гриневич" : "Полищук";


        return person;
    }

    public bool CheckShablon(PersonShablon shablon)
    {
        if (isDead)
            return false;
        if (shablon.Work != Work.Any && shablon.Work != this.Work)
            return false;
        if (shablon.Temperament != Temperament.Any && shablon.Temperament != this.Temperament)
            return false;
        if (shablon.Age != Age.Any && shablon.Age != this.Age)
            return false;
        if (shablon.Sex != Sex.Any && shablon.Sex != this.Sex)
            return false;
        if (shablon.Criminal != Criminal.Any && shablon.Criminal != this.Criminal)
            return false;
        return true;
    }

}

public enum Work
{
    Any = -1,
    None = 0,
    PhoneOperator,
    Cashier,
    Policeman,
    Teacher,
    Miner
}

public enum Temperament
{
    Any = -1,
    Funny = 0,
    Friendly,
    Dumb,
    Angry
}

public enum Age
{
    Any = -1,
    Young = 0,
    Adult,
    Old
}

public enum Sex
{
    Any = -1,
    Woman = 0,
    Man
}


public enum Criminal
{
    Any = -1,
    None = 0,
    Criminal
}

[System.Serializable]
public class PersonShablon
{
    public string Name, Surname;
    public Work Work;
    public Temperament Temperament;
    public Age Age;
    public Sex Sex;
    public Criminal Criminal;
    public bool isDead;
    public int roomNumber;
    readonly static string[] womanNames = { "Наташа", "Диана", "Саша", "Лиза", "София", "Ксения", "Зухра", "Анна" };
    readonly static string[] manNames = { "Борис", "Георгий", "Ярослав", "Иосиф", "Владимир", "Фёдор", "Максим", "Виктор" };
    readonly static string[] surnames = { "Гриневич", "Полищук", "Есин(а)", "Колегов(а)", "Семёнов(а)", "Карпов(а)", "Лучинов(а)", "Сидоров(а)" };

    public PersonShablon()
    {

    }

    public PersonShablon(Work _work, Temperament _temperament, Age _age, Sex _sex, Criminal _criminal = Criminal.None)
    {
        this.Work = _work;
        this.Temperament = _temperament;
        this.Age = _age;
        this.Sex = _sex;
        this.Criminal = _criminal;
    }


    public static PersonShablon GenerateEmptyShablon()
    {
        PersonShablon shablon = new PersonShablon();
        shablon.roomNumber = -1;
        shablon.Name = "";
        shablon.Surname = "";
        shablon.Work = Work.Any;
        shablon.Temperament = Temperament.Any;
        shablon.Age = Age.Any;
        shablon.Sex = Sex.Any;
        shablon.Criminal = Criminal.Any;
        return shablon;
    }



}


[System.Serializable]
public class LineAnswer
{
    public string line, answer;
    public Tags newTag = Tags.none;
    public Tags requireTag = Tags.none;
}
//добавить известность - чем более известен персонаж, тем чаще о нём говорят другие в диалогах