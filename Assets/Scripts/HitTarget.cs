using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class HitTarget 
    : MonoBehaviour
{
    // These public fields become settable properties in the Unity editor.
    public GameObject underworld;
    public GameObject objectToHide;

    void Start()
    {
        // Occurs when this object starts colliding with another object
        this.OnCollisionEnterAsObservable()
            .Subscribe(_ =>
            {
                // Hide the stage and show the underworld.
                objectToHide.SetActive(false);
                underworld.SetActive(true);

                // Disable Spatial Mapping to let the spheres enter the underworld.
                SpatialMapping.Instance.MappingEnabled = false;
            })
            .AddTo(this);
    }
}