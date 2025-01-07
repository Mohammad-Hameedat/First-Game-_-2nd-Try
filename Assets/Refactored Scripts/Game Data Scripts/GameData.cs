using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransferableGameData", menuName = "Game Data/Transferable Game Data", order = 2)]
public class GameData : ScriptableObject
{
    public int selectedLevel;

    public List<GameObject> selectedSpecialPetsList = new();

    public List<int> selectedSpecialPetsIDs = new();
}
