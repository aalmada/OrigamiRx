using UniRx;
using UnityEngine;

public class UnderworldReset 
    : MonoBehaviour
{
    void Start()
    {
        MessageBroker.Default.Receive<ResetWorldArgs>()
            .Subscribe(_ =>
            {
                // Show the stage and hide the underworld.
                // Grab all of the script files from this GameObject's parent
                var behaviours = transform.parent.gameObject.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (var behaviour in behaviours)
                {
                    // If the script's GameObject is disabled, enable it
                    if (!behaviour.gameObject.activeSelf)
                    {
                        behaviour.gameObject.SetActive(true);
                    }
                }
                gameObject.SetActive(false);

                // Enable Spatial Mapping again.
                SpatialMapping.Instance.MappingEnabled = true;
            })
            .AddTo(this);
    }
}
