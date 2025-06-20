using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections.ObjectModel;


public enum Rarity
{
    Common,
    Rare,
    Epic
};
[System.Serializable]
public class GameManager : MonoBehaviour
{
    private CameraController cameraController;
    public GameObject aquariumPrefab; 
    public Camera mainCamera;
    // we can have a list of all the creatures but maybe also three lists, I just feel like maybe it is easier to change from three list to one if we really need to, so I started with three lists
    public List<Entity> creatures = new List<Entity>(); //list of all the prefabs of all entities
    public List<Entity> decorations = new List<Entity>();


    private static Dictionary<Entity, bool> collection = new();

    [SerializeField] float money = 5f;
    private float moneyRate = 0f;
    public int level = 1; // todo:
    public int xpCap = 0;
    public int xp = 0;
    public List<Aquarium> tanks = new List<Aquarium>();
    public int selectedTank;
    public List<LevelData> levels = new List<LevelData>(); // this will be used to unlock new creatures and decorations

    void Awake()
    {
        List<GameObject> creaturesPrefabs = new List<GameObject>();
        creaturesPrefabs.AddRange(Resources.LoadAll<GameObject>("Algaes"));
        creaturesPrefabs.AddRange(Resources.LoadAll<GameObject>("Trilobites"));
        creaturesPrefabs.AddRange(Resources.LoadAll<GameObject>("Decorations"));
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        creatures = InitIDs(creaturesPrefabs);
        InitAABBs(creatures);

        // List<GameObject> decorationprefabs = new List<GameObject>();
        // decorationprefabs.AddRange(Resources.LoadAll<GameObject>("Decorations"));
        // decorations = InitIDs(decorationprefabs);


        levels.Add(new LevelData(100, new List<int>() { 0 }, new List<int>() { 0 }, new List<int>() { 0 }));
        levels.Add(new LevelData(200, new List<int>() { 1 }, new List<int>() { 1 }, new List<int>() { 1 }));

        this.xpCap = levels[level - 1].xpCap;

        aquariumPrefab = Resources.Load<GameObject>("Aquarium/Aquarium");
        GameObject aquariumObject = Instantiate(aquariumPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        tanks.Add(aquariumObject.GetComponent<Aquarium>());
        selectTank(0);

        // if (GameObject.Find("Main Camera")) mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // else Debug.Log("Could not find main camera");
    }

    void Update()
    {
        calcMoneyTick(Time.deltaTime);
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
    public void calcMoneyTick(float timeStep)
    {
        float moneyGained = 0f;
        foreach (Aquarium aquarium in tanks)
        {
            moneyGained += aquarium.calcMoney() * timeStep * 0.01f;
        }
        money += moneyGained;
        moneyRate = moneyGained / timeStep;
        // this.levelUp(1); // todo: fix this to caluelate an xp
    }

    // public void addEntity(Entity entity, Aquarium aquarium, bool playerDragNDrop = true)
    // {
    //     collection[entity] = true;
    //     if (!playerDragNDrop) {aquarium.addEntity(entity); return; }
    //     if (mainCamera && mainCamera.GetComponent<MouseUIManager>()) mainCamera.GetComponent<MouseUIManager>().startPreview(entity, aquarium);

    // }
    public void addEntity(Entity entity, bool playerDragNDrop = true)
    {
        collection[entity] = true;
        if (!playerDragNDrop) { tanks[selectedTank].addEntity(entity); return; }
        if (mainCamera && mainCamera.GetComponent<MouseUIManager>()) mainCamera.GetComponent<MouseUIManager>().startPreview(entity, tanks[selectedTank]);
        else Debug.LogWarning("Could not find mainCamera or DragNDrop script");
    }
    public void breedTrilobites() //todo: later
    {
        return;
    }
    public int getHunger()
    {
        return tanks[selectedTank].getHunger();
    }
    public int getAlgaesHealth()
    {
        return tanks[selectedTank].getAlgaesHealth();
    }
    public float getHappinessRatio()
    {
        float happiness = 0;
        float maxHappiness = 0;
        foreach (Aquarium aquarium in tanks)
        {
            happiness += aquarium.getHappiness();
            maxHappiness += aquarium.getMaxHappiness();
        }
        return happiness / maxHappiness;
    }
    public float getMoney()
    {
        return money;
    }
    public float getMoneyRate()
    {
        return moneyRate;
    }
    public bool CanBuy(float price)
    {
        return money >= price;
    }
    public void buy(float price)
    {
        money -= price;
    }
    private List<Entity> InitIDs(List<GameObject> objects) // this will assign an id for each elements in the list 
    {
        List<Entity> entities = new List<Entity>();
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];
            Entity entity = obj.GetComponent<Entity>();
            if (entity != null)
            {
                entity.id = i;
                entities.Add(entity);
                collection[entity] = false;
            }
            else Debug.LogWarning("Non entity in entity list");
        }
        return entities;
    }
    private void InitAABBs(List<Entity> allEntities){ //List is automatically passed by reference
        foreach(Entity e in allEntities){
            Entity spawnedEntity = Instantiate(e.gameObject, Vector3.zero, Quaternion.identity, transform.root).GetComponent<Entity>(); //this feels silly but they need to be active to get bounds
            if(spawnedEntity) e.setAABB(spawnedEntity.getAllCollidersBoundingBox()); //set the AABB value of the prefabs. So all future entities spawned will be able to use this value and not recalculate it
            Destroy(spawnedEntity.gameObject); 
        }
    }
    public bool isCollected(Entity entity)
    {
        return collection[entity];
    }
    public void addTank()
    {
        GameObject aquariumObject = Instantiate(aquariumPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Aquarium aquarium = aquariumObject.GetComponent<Aquarium>();
        aquarium.setID(tanks.Count);
        aquariumObject.transform.position = new Vector3(tanks.Count * (aquarium.dimensions[0] + 10), 0, 0); // move the aquarium to the right
        tanks.Add(aquarium);
    }
    public void selectTank(int index)
    {
        if (index >= 0 && index < tanks.Count)
        {
            selectedTank = index;
            cameraController.setTarget(tanks[selectedTank].transform);
        }
        else
        {
            Debug.LogError("Invalid tank index: " + index);
        }
    }
    public void nextTank()
    {
        selectedTank = (selectedTank + 1) % tanks.Count;
        cameraController.setTarget(tanks[selectedTank].transform);
    }
    public void PrevTank()
    {
        selectedTank = (selectedTank - 1 + tanks.Count) % tanks.Count;
        cameraController.setTarget(tanks[selectedTank].transform);
    }
    public Aquarium getTank()
    {
        if (selectedTank >= 0 && selectedTank < tanks.Count)
        {
            return tanks[selectedTank];
        }
        else
        {
            Debug.LogError("Invalid tank index: " + selectedTank);
            return null;
        }
    }
    public void SelectTank(int index)
    {
        if (index >= 0 && index < tanks.Count)
        {
            selectedTank = index;
        }
        else
        {
            Debug.LogError("Invalid tank index: " + index);
        }
    }
}
