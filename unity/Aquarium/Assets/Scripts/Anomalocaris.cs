using UnityEngine;

public class Anomalocaris : MobileCreature
{
    public bool demo; //set in editor for demo (faster) behavior
    protected new void Awake()
    {
        //creature and mobilecreature variables are set in prefab editor
        base.Awake(); //call Creature Start()
        if (demo) setDemoValues();
    }



    void FixedUpdate()
    {
        updateFSM();
    }

    protected override void UpdateHunting()
    {
        Trilobite closest = FindClosest<Trilobite>();

        if (closest == null)
        {
            // Nothing to eat.
            return;
        }

        Vector3 delta = closest.getClosestPointOnColliders(transform.position) - transform.position; //distance to closest point on the surface of the colliders
        float distanceSqr = delta.sqrMagnitude; //  sqr bc its faster

        if (distanceSqr <= (maxEatingDistance * getMaturity()) * (maxEatingDistance * getMaturity()))
        {
            //Eat based on consumeRate 
            predate(closest);
            return;
        }
        else
        {
            Vector3 displacement = delta.normalized;
            float k = speed * 3 * Time.deltaTime;
            displacement.Scale(new Vector3(k, k, k));
            transform.position += displacement;
            rotateTowards(delta);
        }
        
    }

    private void setDemoValues()
    {
        huntingEnergyThreshold = 80;
        breedingCooldown = 10;
    }
}
