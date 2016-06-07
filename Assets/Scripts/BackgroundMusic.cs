using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class BackgroundMusic : MonoBehaviour {

    public AudioSource music;
    bool fadeOut = false;
    bool fadeIn = false;
    int fadeDuration = 4;

    // Use this for initialization
    void Start() {
        music.Play();
        music.loop = true;
    }

    public void ChangeMusic(AudioClip clip) {
        fadeOut = true;
        StartCoroutine(WaitAndSetMusic(clip));
    }

    void SetMusic(AudioClip clip) {
        music.Stop();
        music.clip = clip;
        music.Play();
        fadeIn = true;
    }

    IEnumerator WaitAndSetMusic(AudioClip clip) {
        yield return new WaitForSeconds(fadeDuration);
        SetMusic(clip);
    }

    // Update is called once per frame
    void Update() {
        if (fadeOut) {
            music.volume -= Time.deltaTime / fadeDuration;

            if (music.volume <= 0.00f) {
                fadeOut = false;
            }
        } else if (fadeIn) {
            music.volume += Time.deltaTime / fadeDuration;

            if (music.volume >= 1.0f) {
                fadeIn = false;
            }
        }
    }
}
