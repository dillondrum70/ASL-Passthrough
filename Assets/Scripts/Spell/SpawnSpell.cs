using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpell : SpellEffect
{
    [SerializeField] GameObject prefab;

    public override void CastSpell()
    {
        Debug.Log("CastSpell");
        Instantiate(prefab);
    }
}
