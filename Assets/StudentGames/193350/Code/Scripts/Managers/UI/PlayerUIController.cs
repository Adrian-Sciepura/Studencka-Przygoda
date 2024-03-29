using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private Slider _healthBarSlider;

    [SerializeField]
    private Image[] _inventorySlots;

    [SerializeField]
    private Sprite _defaultSprite;

    [SerializeField]
    private TextMeshProUGUI _gameTimeText;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private Image[] keysImage;

    private void Awake()
    {
        EventManager.Subscribe<OnEntityChangeHeldItemEvent>(PlayerChangedHeldItem);
        EventManager.Subscribe<OnEntityDamageEvent>(OnPlayerDamage);
        EventManager.Subscribe<OnEntityHealEvent>(OnPlayerHeal);
        EventManager.Subscribe<OnEntityPickupItemEvent>(OnPlayerPickupItem);
        EventManager.Subscribe<OnPointsValueChanged>(OnPointsValueChanged);
        EventManager.Subscribe<OnKeysValueChanged>(OnKeysValueChanged);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe<OnEntityChangeHeldItemEvent>(PlayerChangedHeldItem);
        EventManager.Unsubscribe<OnEntityDamageEvent>(OnPlayerDamage);
        EventManager.Unsubscribe<OnEntityHealEvent>(OnPlayerHeal);
        EventManager.Unsubscribe<OnEntityPickupItemEvent>(OnPlayerPickupItem);
        EventManager.Unsubscribe<OnPointsValueChanged>(OnPointsValueChanged);
        EventManager.Unsubscribe<OnKeysValueChanged>(OnKeysValueChanged);
    }

    private void Update()
    {
        int time = (int)GameManager.GameTime;
        _gameTimeText.text = string.Format("{0:00}:{1:00}", time / 60, time % 60);
    }

    private void Start()
    {
        UpdatePlayerHealth();
        UpdatePlayerInventory();
        UpdateKeys();
        UpdatePoints();
    }

    #region Event handlers

    private void OnKeysValueChanged(OnKeysValueChanged onKeysValueChanged) => UpdateKeys();

    private void OnPointsValueChanged(OnPointsValueChanged onPointsValueChangedEvent) => UpdatePoints();

    private void OnPlayerPickupItem(OnEntityPickupItemEvent entityPickupItemEvent)
    {
        if (entityPickupItemEvent.Entity.EntityCategory != GameEntityCategory.Player)
            return;

        UpdatePlayerInventory();
    }

    private void PlayerChangedHeldItem(OnEntityChangeHeldItemEvent entityChangeHeldItemEvent)
    {
        if (entityChangeHeldItemEvent.Entity.EntityCategory != GameEntityCategory.Player)
            return;

        foreach (var slot in _inventorySlots)
            slot.color = Color.white;

        int currentSlot = entityChangeHeldItemEvent.Entity.HeldItemInventorySlot;

        _inventorySlots[currentSlot].sprite = ItemTypeToSprite(LevelManager.PlayerEntity.Inventory.Items[currentSlot]);
        _inventorySlots[currentSlot].color = Color.red;
    }

    private void OnPlayerDamage(OnEntityDamageEvent entityDamageEvent)
    {
        if (entityDamageEvent.Entity.EntityCategory != GameEntityCategory.Player)
            return;

        UpdatePlayerHealth();
    }

    private void OnPlayerHeal(OnEntityHealEvent entityHealEvent)
    {
        if (entityHealEvent.Entity.EntityCategory != GameEntityCategory.Player)
            return;

        UpdatePlayerHealth();
    }

    #endregion

    private void UpdatePoints()
    {
        _scoreText.text = GameManager.Points.ToString("0000000");
    }

    private void UpdateKeys()
    {
        int collectedKeys = GameManager.Keys;
        for (int i = 0; i < collectedKeys; i++)
            keysImage[i].color = Color.white;

        for (int i = collectedKeys; i < keysImage.Length; i++)
            keysImage[i].color = Color.black;
    }

    private void UpdatePlayerHealth()
    {
        LivingEntityData playerHealthData = LevelManager.PlayerEntity.EntityData.GetData<LivingEntityData>();
        float val = playerHealthData.health / (float)playerHealthData.maxHealth;
        _healthBarSlider.value = val > 0 ? val : 0;
    }

    private void UpdatePlayerInventory()
    {
        GameEntity playerEntity = LevelManager.PlayerEntity;


        for(int i = 0; i < 4; i++)
        {
            if (playerEntity.Inventory.MaxSize < i)
                break;

            _inventorySlots[i].color = Color.white;
            _inventorySlots[i].sprite = ItemTypeToSprite(playerEntity.Inventory.Items[i]);
        }

        _inventorySlots[playerEntity.HeldItemInventorySlot].color = Color.red;
    }

    private Sprite ItemTypeToSprite(ItemType itemType)
    {
        return itemType != ItemType.NONE
                ? GameDataManager.itemRegistry[itemType].itemPrefab.GetComponent<SpriteRenderer>().sprite
                : _defaultSprite;
    }
}