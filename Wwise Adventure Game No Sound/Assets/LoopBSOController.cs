using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBSOController : MonoBehaviour {

    private AudioSource source;

    public float preEntrySeconds = 0f;
    public float preExitSeconds = 16f;

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (source.time > preExitSeconds)
            source.time = preEntrySeconds;
	}
}
