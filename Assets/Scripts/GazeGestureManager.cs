using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager 
    : MonoBehaviour
{
    public struct TappedEvent
    {
        public InteractionSourceKind Source;
        public int TapCount;
        public Ray HeadRay;
    }

    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var recognizer = new GestureRecognizer();

        // Update is called once per frame
        this.UpdateAsObservable()
            .Select(_ =>
            {
                // Do a raycast into the world based on the user's
                // head position and orientation.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
                    return hitInfo.collider.gameObject;

                return null;
            })
            .DistinctUntilChanged()
            .Do(focusedObject => FocusedObject = focusedObject)
            .Where(focusedObject => focusedObject != null)
            .Subscribe(focusedObject =>
            {
                recognizer.CancelGestures();
                recognizer.StartCapturingGestures();
            })
            .AddTo(this);

        // Set up a GestureRecognizer to detect Select gestures.
        Observable.FromEvent<GestureRecognizer.TappedEventDelegate, TappedEvent>(
            h => (source, tapCount, ray) => h(new TappedEvent { Source = source, TapCount = tapCount, HeadRay = ray }),
            h => recognizer.TappedEvent += h, h => recognizer.TappedEvent -= h)
            .Where(_ => FocusedObject != null)
            .Subscribe(_ =>
            {
                // Send an OnSelect message to the focused object and its ancestors.
                FocusedObject.SendMessageUpwards("OnSelect");
            })
            .AddTo(this);

        recognizer.StartCapturingGestures();
    }
}