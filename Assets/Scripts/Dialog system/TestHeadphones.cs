using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHeadphones : Headphones
{
    protected override bool CheckBellsRinging()
    {
        return false;
    }
    protected override void Update()
    {
    }
}
