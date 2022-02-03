using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static SettingsConfig config;
    public SettingsConfig configuration;
    void Awake()
    {
        config = configuration;
    }

}

