using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EncounterFilter
{
    #nullable enable
    public EncounterTag[]? includeEncounterTags;
    public EncounterTag[]? excludeEncounterTags;
    public int? minTier;
    public int? maxTier;

    #nullable disable
    public EncounterFilter(ScenarioData sd)
    {
        includeEncounterTags = sd.includeEncounters;
        excludeEncounterTags = sd.excludeEncounters;
        minTier = sd.minTier;
        maxTier = sd.maxTier;
    }

    public static bool Filterer(EncounterData ed, EncounterFilter ef)
    {
        if (ef.excludeEncounterTags != null &&      ef.excludeEncounterTags.Length > 0      && ed.encounterTags.Intersect(ef.excludeEncounterTags).Any())       return false;
        if (ef.includeEncounterTags != null &&      ef.includeEncounterTags.Length > 0      && !ed.encounterTags.Intersect(ef.includeEncounterTags).Any())      return false;
        if (ef.maxTier != null              &&      ef.maxTier < ed.tier)                                                                                       return false;
        if (ef.minTier != null              &&      ef.minTier > ed.tier)                                                                                       return false;

        return true;
    }
}

