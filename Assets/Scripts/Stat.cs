using System;
using UnityEngine;

[Serializable]
class Stat
{
    [SerializeField]
    private BarScript bar;

    [SerializeField]
    private float maxVal;

    [SerializeField]
    private float currentVal;

    public float CurrentValue
    {
        get { return currentVal; }
        set
        {
            this.currentVal = Mathf.Clamp(value, 0, MaxVal);
            Bar.Value = currentVal;
        }
    }

    public float MaxVal
    {
        get { return maxVal; }
        set
        {
            Bar.MaxValue = value;
            this.maxVal = value;
        }
    }

    public BarScript Bar
    {
        get { return bar; }
    }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentValue = currentVal;
    }
}
