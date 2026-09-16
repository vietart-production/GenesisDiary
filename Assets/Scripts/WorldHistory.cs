using System.Collections.Generic;
using UnityEngine;

// Minimal WORLD-history log (GDD section 14): records births/deaths so the
// population's story is traceable, not just its current snapshot. Plain
// serializable fields on purpose - they survive domain reloads, unlike a
// static list would.
[System.Serializable]
public class HistoryEvent
{
    public string type;
    public string creatureName;
    public int generation;
}

public class WorldHistory : MonoBehaviour
{
    public List<HistoryEvent> events = new List<HistoryEvent>();
    public int totalBirths;
    public int totalDeaths;
    public int maxGenerationSeen;

    public void RecordBirth(string creatureName, int generation)
    {
        events.Add(new HistoryEvent { type = "birth", creatureName = creatureName, generation = generation });
        totalBirths++;
        if (generation > maxGenerationSeen)
        {
            maxGenerationSeen = generation;
        }
    }

    public void RecordDeath(string creatureName, int generation)
    {
        events.Add(new HistoryEvent { type = "death", creatureName = creatureName, generation = generation });
        totalDeaths++;
    }
}
