using System;
using UniRx;
using UniRx.Triggers;
using UniRx.Diagnostics;
using UnityEngine;

public class GazeGestureManager 
    : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var recognizer = new ReactiveGestureRecognizer();
        var tapped = recognizer.TappedAsObservable()
            .Publish()
            .RefCount();

        // get an observable for all object focus changes
        var focusedObject = this.FocusedObjectAsObservable()
            .Publish()
            .RefCount();

        // detect Select gestures while object in focus
        focusedObject
            .Where(obj => obj != null)
            .SelectMany(obj => Observable.Start(() => recognizer.StartCapturingGestures(), Scheduler.MainThread)
                .SelectMany(_ => tapped
                    // Send an OnSelect message to the focused object and its ancestors.
                    .Do(__ => obj.SendMessageUpwards("OnSelect"))
                    .TakeUntil(focusedObject.Skip(1))
                    .Finally(() => recognizer.CancelGestures())
                )
            )
            .Subscribe()
            .AddTo(this);
    }
}