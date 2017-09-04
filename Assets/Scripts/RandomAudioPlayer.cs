using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour {
    public AudioClip[] sounds;
    public float intervalSecs = 5.0f;
    public bool startPlayingImmediately = true, noInstantRepeats = true;
    private float playTimer;
    private int newSound, lastSound = -1;
    private AudioSource audSrc;

    void Start()
    {
        audSrc = GetComponent<AudioSource>();
    }
	void OnEnable () {
        if (startPlayingImmediately)
            playTimer = 0.0f;
        else
            playTimer = intervalSecs;
	}

	void Update () {
		if(playTimer <= 0.0f)
        {
            if (noInstantRepeats)
            {
                do
                {
                    newSound = Random.Range(0, sounds.Length);
                } while (newSound == lastSound);
                lastSound = newSound;
            } else newSound = Random.Range(0, sounds.Length);

            audSrc.clip = sounds[newSound];
            audSrc.Play();
            playTimer = intervalSecs;
        }
        playTimer -= Time.deltaTime;
	}
}
