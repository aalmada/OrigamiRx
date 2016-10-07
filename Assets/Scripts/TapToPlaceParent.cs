using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TapToPlaceParent 
    : MonoBehaviour
{
    bool placing = false;

    void Start()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.
        this.UpdateAsObservable()
            .Where(_ => placing)
            .Select<Unit, RaycastHit?>(_ =>
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
                    return hitInfo;

                return null;
            })
            .Where(hitInfo => hitInfo.HasValue)
            .Select(hitInfo => hitInfo.Value)
            .DistinctUntilChanged()
            .Subscribe(hitInfo =>
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                this.transform.parent.position = hitInfo.point;

                // Rotate this object's parent object to face the user.
                var toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                this.transform.parent.rotation = toQuat;
            })
            .AddTo(this);
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // On each Select gesture, toggle whether the user is in placing mode.
        placing = !placing;

        // If the user is in placing mode, display the spatial mapping mesh.
        if (placing)
        {
            SpatialMapping.Instance.DrawVisualMeshes = true;
        }
        // If the user is not in placing mode, hide the spatial mapping mesh.
        else
        {
            SpatialMapping.Instance.DrawVisualMeshes = false;
        }
    }
}