using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    [SerializeField]
    private List<TableObjectData> tableObjects;

    [SerializeField]
    private List<TableObjectDataList> tableObjectsList;

    private List<TableObjectData> toActivate;

    private void Awake() {
        toActivate = new List<TableObjectData>();
        TableObjectDataList dataList = tableObjectsList.Find(list => list.day == SaveManager.sv.currentDay);
        if (dataList == null)
            dataList = tableObjectsList.Find(list => list.day == -1);

        foreach (var toSpawn in dataList.tableObjectsToSpawn) {
            toActivate.Add(tableObjects.Find(data => data.name == toSpawn));
        }
    }


    private void Start() {
        foreach (var tableObject in toActivate) {
            tableObject.gameObject.SetActive(true);
        }
    }
}

[Serializable]
internal class TableObjectDataList {
    public int day;
    public List<string> tableObjectsToSpawn;
}


[Serializable]
internal class TableObjectData {
    public string name;
    public GameObject gameObject;
}