using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelManager
{
    public static readonly Dictionary<string, GameEntity> spawnedEntities = new Dictionary<string, GameEntity>();
    public static GameEntity playerEntity { get; private set; }

    private static GameObject _entityParent;

    public static void Setup()
    {
        EventManager.Instance.Subscribe<OnHighPriorityLevelLoadEvent>(SetupEntitiesOnScene);
        EventManager.Instance.Subscribe<OnEntityDieEvent>(EntityDeath);
        EventManager.Instance.Subscribe<OnInteractionItemStartEvent<SpawnEntityInteractionItem>>(BuildFromGameObject);
    }

    private static void SetupEntitiesOnScene(OnHighPriorityLevelLoadEvent e)
    {
        spawnedEntities.Clear();

        _entityParent = GameObject.Find("Entity");
        SpawnInfo[] spawnInfos = Object.FindObjectsOfType<SpawnInfo>();

        for (int i = 0; i < spawnInfos.Length; i++)
        {
            if (spawnInfos[i].entityType == GameEntityType.Player)
            {
                SpawnInfo temp = spawnInfos[0];
                spawnInfos[0] = spawnInfos[i];
                spawnInfos[i] = temp;
                break;
            }
        }

        foreach (SpawnInfo spawnInfo in spawnInfos)
            if (!spawnInfo.doNotCreateImmediately)
                BuildFromSpawnInfo(spawnInfo);

        EventManager.Instance.Publish(new OnLevelSetupComplete());
    }

    private static void BuildFromGameObject(OnInteractionItemStartEvent<SpawnEntityInteractionItem> onSpawnEntityInteractionStarted)
    {
        SpawnInfo spawnInfo = onSpawnEntityInteractionStarted.Data.spawnObject.GetComponent<SpawnInfo>();

        if (spawnInfo != null)
            BuildFromSpawnInfo(spawnInfo);

        EventManager.Instance.Publish(new OnInteractionItemFinishEvent<SpawnEntityInteractionItem>());
    }

    private static void BuildFromSpawnInfo(SpawnInfo spawnInfo)
    {
        GameEntity createdEntity = Factory.Build(spawnInfo.guid, spawnInfo.entityType, spawnInfo.gameObject.transform.position);
        createdEntity.transform.parent = _entityParent.transform;
        spawnedEntities.Add(spawnInfo.guid, createdEntity);

        if (spawnInfo.entityType == GameEntityType.Player)
        {
            playerEntity = createdEntity;
            playerEntity.inventory.Resize(4);
        }

        for (int i = spawnInfo.transform.childCount - 1; i >= 0; i--)
            spawnInfo.transform.GetChild(i).transform.parent = createdEntity.transform;

        Object.Destroy(spawnInfo.gameObject);
    }

    private static void EntityDeath(OnEntityDieEvent entityDieEvent)
    {
        spawnedEntities[entityDieEvent.Entity.GUID] = null;
        Object.Destroy(entityDieEvent.Entity.gameObject);

        Debug.Log("Entity died");
    }
}
