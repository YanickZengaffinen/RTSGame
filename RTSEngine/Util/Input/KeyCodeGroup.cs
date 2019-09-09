using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KeyCodeGroup
{
    [SerializeField]
    private KeyCode[] keyCodes;

    public KeyCodeGroup(params KeyCode[] keyCodes)
    {
        this.keyCodes = keyCodes;
    }

    public bool Any()
        => keyCodes.Any(x => Input.GetKey(x));

    public bool All()
        => keyCodes.All(x => Input.GetKey(x));
}
