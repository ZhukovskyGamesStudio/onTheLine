using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Headphones : TableItemBehaviour {
    public static Headphones instance;
    public AudioSource bellSource;
    public AudioSource[] talkingSource;
    public Commutator Commutator;
    public Dictionary<GameObject, int> talksDict;

    private bool _isOnHead;

    protected override void Awake() {
        base.Awake();
        talksDict = new Dictionary<GameObject, int>();
        instance = this;
        for (int i = 0; i < talkingSource.Length; i++) {
            talkingSource[i].Play();
            talkingSource[i].Pause();
        }
    }

    public static void PlayStopBell(bool isPlaying) {
        if (instance.bellSource.isPlaying == isPlaying)
            return;

        if (isPlaying)
            instance.bellSource.Play();
        else
            instance.bellSource.Stop();
    }

    public static void PlayStopTalking(GameObject objIndex, bool isPlaying) {
        if (!instance.talksDict.ContainsKey(objIndex))
            instance.talksDict.Add(objIndex, Random.Range(0, instance.talkingSource.Length));

        if (instance.talkingSource[instance.talksDict[objIndex]].isPlaying == isPlaying)
            return;

        if (isPlaying)
            instance.talkingSource[instance.talksDict[objIndex]].UnPause();
        else {
            instance.talkingSource[instance.talksDict[objIndex]].Pause();
            //instance.talksDict.Remove(objIndex);
        }
    }

    public override void OnmouseDown() {
        if (_isOnHead)
            return;
        base.OnmouseDown();
        PutOn();
    }

    private void PutOn() {
        if (_isOnHead)
            return;
        //_animation.Play("PutOn");
        AddTag();
        _isOnHead = true;
    }

    public void AddTag() {
        TagManager.AddTag("Headphone on");
    }

    protected override void Update() {
        base.Update();
        PlayStopBell(CheckBellsRinging());
    }

    protected virtual bool CheckBellsRinging() {
        for (int i = 0; i < Commutator.Levers.Length; i++) {
            if (Commutator.Levers[i].isRinging)
                return true;
        }

        return false;
    }
}