////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnAnimationEvent : MonoBehaviour
{

    // HINT: Expose the sound to be played in this animation
    //public List<AK.Wwise.Event> Sounds = new List<AK.Wwise.Event>();
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip showRoll;
    public AudioClip hideRoll;

    public void PlaySoundWithIdx(int idx)
    {
        // HINT: Play sound with index idx synchronized with animation
        //Sounds[idx].Post(gameObject);

        if (idx == 0)
        {
            audioSource.clip = showRoll;
            audioSource.Play();
        }

        else if (idx == 2)
        {
            audioSource.clip = hideRoll;
            audioSource.Play();
        }

    }
}
