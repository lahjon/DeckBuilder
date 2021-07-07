using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class StartingCardSet
{
    public string name;
    public CharacterClassType characterClass;
    public Profession profession = Profession.Base;
    public List<CardData> startingCards = new List<CardData>();
}

