// Assets/Scripts/UI/CharacterCarouselUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCarouselUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterDatabase database;

    [Header("Character Slots")]
    [SerializeField] private RectTransform leftSlot;
    [SerializeField] private RectTransform centerSlot;
    [SerializeField] private RectTransform rightSlot;

    [SerializeField] private Image leftImage;
    [SerializeField] private Image centerImage;
    [SerializeField] private Image rightImage;

    [Header("Info UI")]
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponNameText;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private float scaleSelected = 1.15f;
    [SerializeField] private float scaleUnselected = 0.85f;

    private int currentIndex;
    private bool isAnimating;

    void Start()
    {
        currentIndex = 0;
        InstantRefresh();
    }

    public void Next()
    {
        if (isAnimating) return;
        currentIndex = Wrap(currentIndex + 1);
        AnimateCarousel(-1);
    }

    public void Previous()
    {
        if (isAnimating) return;
        currentIndex = Wrap(currentIndex - 1);
        AnimateCarousel(1);
    }

    public void ConfirmSelection()
    {
        CharacterSelector.instance.SelectCharacter(database.characters[currentIndex]);
    }

    private void AnimateCarousel(int direction)
    {
        isAnimating = true;

        Vector3 leftPos = new Vector3(-319, 100, 0);
        Vector3 centerPos = new Vector3(-4, 100, 0);
        Vector3 rightPos = new Vector3(319, 100, 0);

        if (direction == -1) // NEXT → slide LEFT
        {
            LeanTween.moveLocal(centerSlot.gameObject, leftPos, slideDuration);
            LeanTween.moveLocal(rightSlot.gameObject, centerPos, slideDuration);
            LeanTween.moveLocal(leftSlot.gameObject, rightPos, slideDuration);

            LeanTween.scale(centerSlot, Vector3.one * scaleUnselected, slideDuration);
            LeanTween.scale(rightSlot, Vector3.one * scaleSelected, slideDuration);
        }
        else // PREVIOUS → slide RIGHT
        {
            LeanTween.moveLocal(centerSlot.gameObject, rightPos, slideDuration);
            LeanTween.moveLocal(leftSlot.gameObject, centerPos, slideDuration);
            LeanTween.moveLocal(rightSlot.gameObject, leftPos, slideDuration);

            LeanTween.scale(centerSlot, Vector3.one * scaleUnselected, slideDuration);
            LeanTween.scale(leftSlot, Vector3.one * scaleSelected, slideDuration);
        }

        LeanTween.delayedCall(slideDuration, () =>
        {
            InstantRefresh();
            isAnimating = false;
        });
    }


    private void InstantRefresh()
    {
        int left = Wrap(currentIndex - 1);
        int right = Wrap(currentIndex + 1);

        leftImage.sprite = database.characters[left].Icon;
        centerImage.sprite = database.characters[currentIndex].Icon;
        rightImage.sprite = database.characters[right].Icon;

        centerSlot.localScale = Vector3.one * scaleSelected;
        leftSlot.localScale = Vector3.one * scaleUnselected;
        rightSlot.localScale = Vector3.one * scaleUnselected;

        leftSlot.localPosition = new Vector3(-319, 100, 0);
        centerSlot.localPosition = new Vector3(-4, 100, 0);
        rightSlot.localPosition = new Vector3(319, 100, 0);

        UpdateInfo(database.characters[currentIndex]);
    }

    private void UpdateInfo(CharacterData data)
    {
        characterNameText.text = data.name;
        weaponIcon.sprite = data.StartingWeapon.icon;
        weaponNameText.text = data.StartingWeapon.name;
    }

    private int Wrap(int index)
    {
        if (index < 0) return database.characters.Count - 1;
        if (index >= database.characters.Count) return 0;
        return index;
    }
}
