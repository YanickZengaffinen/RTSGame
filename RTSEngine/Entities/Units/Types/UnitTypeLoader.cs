using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitTypeLoader
{
    /// <summary>
    /// Load all unittypes that are stored in a resource json file
    /// </summary>
    /// <param name="path">Path without a file ending</param>
    public IList<UnitType> LoadFromJsonResource(string path)
    {
        var textFile = Resources.Load<TextAsset>(path);
        return JsonArrayHelper.FromJson<UnitType>(textFile.text).ToList();
    }
}
