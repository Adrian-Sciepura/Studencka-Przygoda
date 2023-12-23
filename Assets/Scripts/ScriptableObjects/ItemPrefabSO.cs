using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPrefab", menuName = "ScriptableObjects/ItemPrefab", order = 1)]
public class ItemPrefabSO : ScriptableObject, IAutoGenerated
{
    [SerializeField]
    public string itemName;

    [SerializeField]
    public GameObject itemPrefab;

    [SerializeReference, SubclassSelector]
    public IAttackAction attackAction;

    [SerializeReference, SubclassSelector]
    public IUseAction useAction;

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
               attackAction != null &&
               useAction != null;
    }
}