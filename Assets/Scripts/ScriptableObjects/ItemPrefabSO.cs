using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPrefab", menuName = "ScriptableObjects/ItemPrefab", order = 1)]
public class ItemPrefabSO : ScriptableObject, IAutoGenerated
{
    [SerializeField]
    public string itemName;

    [SerializeField]
    public string displayName;

    [SerializeField]
    public GameObject itemPrefab;

    [SerializeReference, SubclassSelector]
    public IItemBehaviour itemBehaviour;

    [SerializeReference, SubclassSelector]
    public List<ItemData> data;


    public string GetGeneratedName()
    {
        return itemName;
    }

    public bool Validate()
    {
        return itemName != null &&
               itemPrefab != null &&
               itemBehaviour != null;
    }
}