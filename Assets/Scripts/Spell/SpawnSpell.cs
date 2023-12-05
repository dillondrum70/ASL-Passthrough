using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpell : SpellEffect
{
    [SerializeField] GameObject prefab;

    public override void CastSpell(HandPoseTracker handPoseTracker, Transform spellSpawn)
    {
        Debug.Log("CastSpell");
        ThrowSpell tSpell = Instantiate(prefab, spellSpawn.position, spellSpawn.rotation).GetComponent<ThrowSpell>();
        tSpell.SetPalmTransform(handPoseTracker.GetPalmTransform());
    }
}
