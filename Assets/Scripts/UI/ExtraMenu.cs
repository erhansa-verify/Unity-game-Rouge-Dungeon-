using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ExtraMenu : MonoBehaviour
{
    public enum Category
    {
        Characters,
        Enemies,
        Items
    }

    [Header("Slots (Exactly 10)")]
    [SerializeField] private List<Image> slots;

    [Header("Data")]
    [SerializeField] private List<CharacterData> characters;
    [SerializeField] private List<ItemData> items;

    private const int MaxSlots = 10;

    private void Awake()
    {
        ClearSlots();
    }

    public void OnCharactersSelected()
    {
        ShowCharacters();
    }

    public void OnItemsSelected()
    {
        ShowItems();
    }

    public void OnEnemiesSelected()
    {
        ClearSlots(); // ignored for now
    }

    private void ShowCharacters()
    {
        ClearSlots();

        int count = Mathf.Min(characters.Count, MaxSlots);

        for (int i = 0; i < count; i++)
        {
            slots[i].sprite = characters[i].Icon;
            slots[i].enabled = true;
        }
    }

    private void ShowItems()
    {
        ClearSlots();

        int count = Mathf.Min(items.Count, MaxSlots);

        for (int i = 0; i < count; i++)
        {
            slots[i].sprite = items[i].icon;
            slots[i].enabled = true;
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            slot.sprite = null;
            slot.enabled = false;
        }
    }
}
