using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class WorldCursor 
    : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Grab the mesh renderer that's on the same object as this script.
        var meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();

        // display cursor when gazing at something
        this.GazeHitAsObservable()
            .Select(hitInfo => hitInfo.HasValue)
            .DistinctUntilChanged()
            .Subscribe(gazing =>
            {
                meshRenderer.enabled = gazing;
            })
            .AddTo(this);

        // move cursor when gazing at something
        this.GazeHitAsObservable()
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