using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ObservableGazeHitTrigger 
    : ObservableTriggerBase
{
    readonly Subject<RaycastHit?> gazeHit = new Subject<RaycastHit?>();

    void Update()
    {
        var cameraTransform = Camera.main.transform;
        RaycastHit hitInfo;

        if (Physics.Raycast(
            cameraTransform.position, cameraTransform.forward,
            out hitInfo))
        {
            gazeHit.OnNext(hitInfo);
        }
        else
        {
            gazeHit.OnNext(null);
        }
    }

    public IObservable<RaycastHit?> GazeHitAsObservable()
    {
        return gazeHit;
    }

    protected override void RaiseOnCompletedOnDestroy()
    {
        gazeHit.OnCompleted();
    }
}
