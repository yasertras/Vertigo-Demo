using UnityEngine;
using UnityEngine.U2D;

public enum CollectableObjects
{
    Melee,
    Pistol,
    Shotgun,
    SubMachine,
    AssaultRifle,
    SniperRifle,
    MachineGun,
    Armor,
    Grenade,
    Gold,
    Money,
    Skin,     
    Death,
    Health
}

[System.Serializable]
public class Slot
{
    public string spriteName;
    public int value;
    public CollectableObjects weaponType;
    public Sprite mainSprite;
}


[CreateAssetMenu(fileName = "Slots", menuName = "Create/SlotsContainer", order = 1)]
public class SlotContainer : ScriptableObject
{
    public Slot[] slots;
}
