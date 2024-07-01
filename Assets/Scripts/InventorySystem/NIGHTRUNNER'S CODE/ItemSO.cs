using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    [TextArea] public string itemDescription;
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;

    public bool UseItem(PlayerHealth playerHealth)
    {
        switch (statToChange)
        {
            case StatToChange.Health:
                if (playerHealth.currentHealth == playerHealth.maxHealth)
                {
                    return false;
                }
                else
                {
                    playerHealth.ChangeHealth(amountToChangeStat);
                    return true;
                }
            case StatToChange.Damage:
                // Implement damage increase logic
                return false;
            case StatToChange.Speed:
                // Implement speed increase logic
                return false;
            default:
                return false;
        }
    }

    public enum StatToChange
    {
        None,
        Health,
        Damage,
        Speed
    };
}
