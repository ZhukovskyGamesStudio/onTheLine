using System;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    public static TagManager instance;
    public List<string> tags;

    private void Awake()
    {
        instance = this;
        if (tags == null)
            tags = new List<string>();
    }

    public static void AddTag(string newTag)
    {
        string tag = (string)Enum.Parse(typeof(string), newTag);
        AddTag(tag);
    }

    public static bool CheckTag(string toCheck)
    {
        return instance.tags.Contains(toCheck);
    }
}
   [System.Serializable]
public enum Tags
{
    none, toysRobbery, toysSaved, girlFound, girlFoundWrong
}
