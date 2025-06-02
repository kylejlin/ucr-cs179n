using UnityEngine;

public class Decoration : Entity
{
    public int moneyBonus;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override float calcMoneyBonus()
    {
        switch (GetRarity())
        {
            case Rarity.Common:
                return 1 + moneyBonus;
            case Rarity.Rare:
                return 3 + moneyBonus;
            case Rarity.Epic:
                return 7 + moneyBonus;
            default:
                return 1 + moneyBonus;
        }
    }

}
