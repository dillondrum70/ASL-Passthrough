using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastComponent : MonoBehaviour
{
    TwoHandPoseTracker twoHandPoseTracker;

    [SerializeField] List<Spell> spells;

    //Most recent signs inserted at index 0
    public List<Gesture> recentHandGestures = new List<Gesture>();

    //Seconds to wait before allowing the same sign again
    [SerializeField] float signDelay = 1f;
    float currentDelay = 0f;

    [SerializeField] Transform spellSpawnPoint;

    private void Start()
    {
        for(int i = 0; i < spells.Count; i++)
        {
            //Set and reverse gestures list
            Spell spell = spells[i];
            spell.reversedGestures = new(spell.gestures);
            spell.reversedGestures.Reverse();
            spells[i] = spell;
        }
    }

    private void OnEnable()
    {
        twoHandPoseTracker = GetComponent<TwoHandPoseTracker>();
        twoHandPoseTracker.OnGestureEnter.AddListener(AddRecentGesture);
        twoHandPoseTracker.leftTracker.OnGestureEnter.AddListener(AddRecentGesture);
        twoHandPoseTracker.rightTracker.OnGestureEnter.AddListener(AddRecentGesture);
        
    }

    private void OnDisable()
    {
        twoHandPoseTracker.OnGestureEnter.RemoveListener(AddRecentGesture);
        twoHandPoseTracker.leftTracker.OnGestureEnter.RemoveListener(AddRecentGesture);
        twoHandPoseTracker.rightTracker.OnGestureEnter.RemoveListener(AddRecentGesture);
    }

    private void Update()
    {
        //Update delay timer
        if(currentDelay > 0f)
        {
            currentDelay -= Time.deltaTime;
        }
    }

    void AddRecentGesture(Gesture handGesture)
    {
        if(recentHandGestures.Count > 0 &&          //Check there is a count
            recentHandGestures[0] == handGesture && //if current gesture equals the one on top
            currentDelay > 0)                       //and currentDelay has not elapsed
        {
            //Then do nothing
            return;
        }

        //Add gesture to list
        recentHandGestures.Insert(0, handGesture);
        Debug.Log($"Add gesture - {recentHandGestures.Count}");

        currentDelay = signDelay;

        //Iterate over all spells
        foreach (Spell spellEffect in spells)
        {
            //Check if there are less letters than gestuers in spell effect
            if (recentHandGestures.Count < spellEffect.reversedGestures.Count)
            {
                //Skip this gesture
                continue;
            }

            //Check order of poses, ensure they match the most recent poses in the stack
            bool match = true;
            for (int i = 0; i < spellEffect.reversedGestures.Count; i++)
            {
                //Exit loop if a pose does not match, move to next pose or exit and accept if at end of pose list
                if (spellEffect.reversedGestures[i] != recentHandGestures[i])
                {
                    match = false;
                    break;
                }
            }

            //All poses match
            if (match)
            {
                spellEffect.effect.CastSpell(spellSpawnPoint);
                recentHandGestures.Clear();
            }
        }
    }
}
