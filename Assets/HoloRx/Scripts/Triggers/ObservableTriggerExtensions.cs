using System;
using UniRx;
using UnityEngine;

public static partial class ObservableTriggerExtensions {

    public static IObservable<RaycastHit?> GazeHitAsObservable(this Component component)
    {
        if (component == null || component.gameObject == null)
            return Observable.Empty<RaycastHit?>();

        // let there be only one instance of the trigger (singleton) so that only one raycast is performed per update
        return GetSingletonComponent<ObservableGazeHitTrigger>(component.gameObject).GazeHitAsObservable();
    }

    public static IObservable<GameObject> FocusedObjectAsObservable(this Component component)
    {
        if (component == null || component.gameObject == null)
            return Observable.Empty<GameObject>();

        return GazeHitAsObservable(component)
            .Select(hitInfo =>
            {
                if (!hitInfo.HasValue || hitInfo.Value.collider == null)
                    return null;

                return hitInfo.Value.collider.gameObject;
            })
            .DistinctUntilChanged();
    }

    static T GetOrAddComponent<T>(GameObject gameObject)
        where T : Component
    {
        var instance = gameObject.GetComponent<T>();
        if (instance == null)
        {
            instance = gameObject.AddComponent<T>();
        }

        return instance;
    }

    static T GetSingletonComponent<T>(GameObject gameObject)
        where T : Component
    {
        var instance = UnityEngine.Object.FindObjectOfType<T>();
        if (instance == null)
        {
            instance = gameObject.AddComponent<T>();
        }

        return instance;
    }
}
