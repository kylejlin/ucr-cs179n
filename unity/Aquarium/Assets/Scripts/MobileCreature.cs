using UnityEngine;

public enum BehaviorState
{
    Idle,
    Hunting,
    Dying,
}

public class MobileCreature : Creature
{
    public bool canSwim = false;
    public int consumeRate;
    public float speed = 1;

    public static float maxEnergy = 100;

    public float energy = maxEnergy;

    public static float huntingEnergyThreshold = 50;


    public static float metabolismRate = 1;

    public BehaviorState state = BehaviorState.Idle;

    public static float maxEatingDistance = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {
        energy -= Time.deltaTime * metabolismRate;

        if (energy <= huntingEnergyThreshold)
        {
            state = BehaviorState.Hunting;
        }
        else if (energy <= 0)
        {
            // The creature starved to death. 
            state = BehaviorState.Dying;
        }
        else
        {
            state = BehaviorState.Idle;
        }


        if (state == BehaviorState.Idle)
        {
            UpdateIdle();
        }
        else if (state == BehaviorState.Hunting)
        {
            UpdateHunting();
        }
        else if (state == BehaviorState.Dying)
        {
            UpdateDying();
        }
    }

    void UpdateIdle()
    {
        // Ray, this is for you to implement.
    }

    void UpdateDying()
    {
        // TODO
    }

    void UpdateHunting()
    {
        // I can't find the algae class, so for now,
        // I'm just targeting the closest entity.
        ImmobileCreature closest = FindClosest<ImmobileCreature>();

        if (closest == null)
        {
            // Nothing to eat.
            return;
        }

        Vector3 delta = closest.transform.position - transform.position;
        float distance = delta.magnitude;

        if (distance <= maxEatingDistance)
        {
            // For now, I implemented eating as instantaneous.
            // We can always adjust this later if you want a more gradual shrinking.
            energy = maxEnergy;
            closest.beingEaten(closest.maxHealth);
            return;
        }

        Vector3 displacement = delta.normalized;
        float k = speed * Time.deltaTime;
        displacement.Scale(new Vector3(k, k, k));
        transform.position += displacement;
    }

}
