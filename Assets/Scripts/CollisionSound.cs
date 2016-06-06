using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

/* Put this on the player character. On collision
 with something, call PlaySound() to play the
 sound, which will rapidly fade out over the course 
 of 0.5 seconds. */
public class CollisionSound : MonoBehaviour {
    public AudioSource sound;
    bool fadeOut = false;

    // Use this for initialization
    void Start()
    {
    }

    public void PlaySound()
    {
        sound.Stop();
        sound.volume = 1f;
        sound.Play();
        fadeOut = true;
    }

    public void PlaySound(AudioClip clip) {
        sound.Stop();
        sound.clip = clip;
        sound.volume = 1f;
        sound.Play();
        fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            sound.volume -= 2*Time.deltaTime;

            if (sound.volume <= 0.00f)
            {
                fadeOut = false;
            }
        }
    }
}
