using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogsCollection", menuName = "ScriptableObjects/DialogsCollection", order = 5)]
[Serializable]
public class DialogsCollection : ScriptableObject
{
    public List<Dialog> allDialogs;
    //TODO addAutoGenerate by menuButton
}
