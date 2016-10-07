using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class WorldCursor 
    : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var raycast = this.UpdateAsObservable()
            .Select<Unit, RaycastHit?>(_ =>
            {
                // Do a raycast into the world based on the user's
                // head position and orientation.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
                    return hitInfo;

                return null;
            });

        // Grab the mesh renderer that's on the same object as this script.
        var meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();

        // display cursor when raycast has value
        raycast
            .Select(hitInfo => hitInfo.HasValue)
            .DistinctUntilChanged()
            .Subscribe(hasValue => meshRenderer.enabled = hasValue)
            .AddTo(this);

        // move cursor when raycast has value
        raycast
            .Where(hitInfo => hitInfo.HasValue)
            .Select(hitInfo => hitInfo.Value)
            .Subscribe(hitInfo => {
                // Move the cursor to the point where the raycast hit.
                this.transform.position = hitInfo.point;

                // Rotate the cursor to hug the surface of the hologram.
                this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            })
            .AddTo(this);
    }
}