using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ObservableMaskedGazeHitTrigger 
    : ObservableTriggerBase
{
    public const float DefaultMaxDistance = Mathf.Infinity;
    public const int DefaultLayerMask = 0;// Physics.DefaultRaycastLayers;
    public const QueryTriggerInteraction DefaultQueryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

    readonly Subject<RaycastHit?> gazeHit = new Subject<RaycastHit?>();

    public float MaxDistance { get; set; }
    public int LayerMask { get; set; }
    public QueryTriggerInteraction QueryTriggerInteraction { get; set; }

    public ObservableMaskedGazeHitTrigger()
    {
        this.MaxDistance = DefaultMaxDistance;
        this.LayerMask = DefaultLayerMask;
        this.QueryTriggerInteraction = DefaultQueryTriggerInteraction;
    }

    void Update()
    {
        var cameraTransform = Camera.main.transform;
        RaycastHit hitInfo;

        if (Physics.Raycast(
            cameraTransform.position, cameraTransform.forward,
            out hitInfo,
            MaxDistance, LayerMask, QueryTriggerInteraction))
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
