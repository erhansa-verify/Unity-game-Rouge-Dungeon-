// Assets/Scripts/Data/CharacterDatabase.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters;
}
