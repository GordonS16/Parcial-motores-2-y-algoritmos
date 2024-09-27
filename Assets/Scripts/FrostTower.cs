using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostTower : Tower
{

    [SerializeField] private float slowingFactor;

    private void Start()
    {
        ElementType = Element.FROST;
    }

    public override Debuff GetDebuff()
    {
        return new FrostDebuff(slowingFactor, DebuffDuration, Target);
    }
}
