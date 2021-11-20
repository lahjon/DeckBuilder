// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class Character : MonoBehaviour, ISaveableCharacter
// {
//     //public int level;
//     //public int experience;
//     //public CharacterClassType classType;
//     public PlayableCharacterData characterData;
//     public ProfessionType profession;

//     public void SetCharacterData(int index)
//     {
//         SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), index);
//         characterData = WorldSystem.instance.characterManager.allCharacterData[index];
//         WorldSystem.instance.characterManager.characterStats.Init();
//     }

//     public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
//     {
//         //level = a_SaveData.level;
        
//         if (a_SaveData.profession != ProfessionType.Base)
//         {
//             profession = a_SaveData.profession;
//         }
//         else
//         {
//             switch (WorldSystem.instance.characterManager.selectedCharacterClassType)
//             {
//                 case CharacterClassType.Berserker:
//                     profession = ProfessionType.Berserker1;
//                     break;
//                 case CharacterClassType.Rogue:
//                     profession = ProfessionType.Rogue1;
//                     break;
//                 case CharacterClassType.Splicer:
//                     profession = ProfessionType.Splicer1;
//                     break;
//                 case CharacterClassType.Beastmaster:
//                     profession = ProfessionType.Beastmaster1;
//                     break;
                
//                 default:
//                     break;
//             }
//         }
//     }
//     public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
//     {
//         //a_SaveData.level = level;
//         a_SaveData.profession = profession;
//     }
// }