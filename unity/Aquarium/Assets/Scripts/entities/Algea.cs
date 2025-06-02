using UnityEngine;

public class Algea : ImmobileCreature
{
    public bool demo = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        //creature variables are set in prefab editor
        base.Awake();
        if (demo) setDemoValues();
    }


    // Update is called once per frame
    void Update()
    {
        automaticGrowth(Time.deltaTime);
    }

    void setDemoValues()
    {
        growthCooldown = 2f;
    }
}
