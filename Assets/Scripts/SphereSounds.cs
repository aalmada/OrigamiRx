using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SphereSounds 
    : MonoBehaviour
{
    void Start()
    {
        var rigid = this.gameObject.GetComponent<Rigidbody>();

        // Add an AudioSource component and set up some defaults
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialize = true;
        audioSource.spatialBlend = 1.0f;
        audioSource.dopplerLevel = 0.0f;
        audioSource.rolloffMode = AudioRolloffMode.Custom;

        // Load the Sphere sounds from the Resources folder
        var impactClip = Resources.Load<AudioClip>("Impact");
        var rollingClip = Resources.Load<AudioClip>("Rolling");

        // Play an impact sound if the sphere impacts strongly enough.
        this.OnCollisionEnterAsObservable()
            .Where(collision => collision.relativeVelocity.magnitude >= 0.1f)
            .SelectMany(_ =>
            {
                audioSource.clip = impactClip;
                audioSource.loop = false;
                audioSource.Play();
                return Observable.Timer(TimeSpan.FromSeconds(audioSource.clip.length));
            })
            .Do(_ =>
            {
                audioSource.clip = rollingClip;
                audioSource.loop = true;
                audioSource.Play();
            })
            .TakeWhile(_ => rigid.velocity.magnitude >= 0.01f)
            .TakeUntil(this.OnCollisionExitAsObservable())
            .Finally(() => audioSource.Stop())
            .Subscribe()
            .AddTo(this);
    }
}