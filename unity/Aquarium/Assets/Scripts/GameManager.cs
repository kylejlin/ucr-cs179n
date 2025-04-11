using UnityEngine;
using System.Collections.Generic; //list and dictionary definition

public enum Rarity
{
    Common,
    Rare,
    Epic
};
[System.Serializable]

public class GameManager : MonoBehaviour
{
    public Aquarium aquarium; // this will have to be changed: it should be spawned, not referenced, but this is convenient for the MVP
    // we can have a list of all the creatures but maybe also three lists, I just feel like maybe it is easier to change from three list to one if we really need to, so I started with three lists
    public List<GameObject> algaes = new List<GameObject>();
    public List<GameObject> trilobites = new List<GameObject>();
    public List<GameObject> decorations = new List<GameObject>();

    public int money = 1000; // todo: 
    public int level = 1; // todo:
    public int xpCap = 0;
    public int xp = 0;
    public List<LevelData> levels = new List<LevelData>(); // this will be used to unlock new creatures and decorations

    void Awake()
    {
        GameObject[] algaePrefabs = Resources.LoadAll<GameObject>("Agaes");
        algaes.AddRange(algaePrefabs);
        InitIDs(algaes);
        GameObject[] trilobitePrefabs = Resources.LoadAll<GameObject>("Trilobites");
        trilobites.AddRange(trilobitePrefabs);
        InitIDs(trilobites);
        GameObject[] decorationprefabs = Resources.LoadAll<GameObject>("Decorations");
        decorations.AddRange(decorationprefabs);
        InitIDs(decorations);


        levels.Add(new LevelData(100, new List<int>() { 0 }, new List<int>() { 0 }, new List<int>() { 0 }));
        levels.Add(new LevelData(200, new List<int>() { 1 }, new List<int>() { 1 }, new List<int>() { 1 }));

        this.xpCap = levels[level - 1].xpCap;
    }

    void Update()
    {

    }
    public void levelUp(int xp) // todo: fix this
    {
        this.xp += xp;
        if (this.xp >= this.xpCap)
        {
            this.level++;
            this.xp = 0;
            this.xpCap = levels[level - 1].xpCap;
        }
    }
    public void getCoin()
    {
        money += aquarium.calcCoin();
        this.levelUp(1); // todo: fix this to caluelate an xp
    }

    public void addEntity(GameObject entity, Aquarium aquarium)
    {
        aquarium.addEntity(entity);
    }
    public void breedTrilobites() //todo: later
    {
        return;
    }
    public int getHunger()
    {
        return aquarium.getHunger();
    }
    public int getAlgaesHealth()
    {
        return aquarium.getAlgaesHealth();
    }
    public float getHappiness()
    {
        return aquarium.getHappiness();
    }
    private void InitIDs(List<GameObject> objects) // this will assign an id for each elements in the list 
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];
            Entity entity = obj.GetComponent<Entity>();
            if (entity != null)
            {
                entity.id = i;
            }
        }
    }
}
