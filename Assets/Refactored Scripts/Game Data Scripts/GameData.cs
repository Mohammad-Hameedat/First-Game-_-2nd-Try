using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransferableGameData", menuName = "GameData/Transferable Game Data", order = 1)]
public class GameData : ScriptableObject
{
    public int selectedLevel;

    public List<GameObject> selectedSpecialPetsList = new();

    public List<int> selectedSpecialPetsIDs = new();
}
