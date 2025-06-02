using UnityEngine;

public class Trilobite : MobileCreature
{
    protected new void Awake()
    {
        setValues(true); //things in creature and mobile creature need these values
        base.Awake(); //call Creature Start()
        setValues(true); //but also the standard for other classes has been the base awake is called first
        //maybe better to set them in the editor? hm
        

    }
    void FixedUpdate() {
        updateFSM();
    }
    protected void setValues(bool demo)
    {
        if(demo){
            growthRate = 0.1f;
            adultEnergy = 40;
            breedingCooldown = 50; //change 

            spawnSize = .5f; 
            energy = maxEnergy = spawnSize*adultEnergy;
            huntingEnergyThreshold = .75f * maxEnergy;
            consumeRate = .5f * maxEnergy; //changed

            spawnRadius = 20;
            minSpawnSpace = 5;
            minCMCubedPer = 10000; //changed
        }else{
            growthRate = 0.1f;
            adultEnergy = 40;
            breedingCooldown = 100; //change 

            spawnSize = .5f; 
            energy = maxEnergy = spawnSize*adultEnergy;
            huntingEnergyThreshold = .75f * maxEnergy;
            consumeRate = .5f * maxEnergy; //changed

            spawnRadius = 5;
            minSpawnSpace = 1;
            minCMCubedPer = 200; //changed
        }
    }
}
