using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Windows.Speech;

public struct ResetWorldArgs
{
    // empty
}

public class SpeechManager 
    : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var keywords = new Dictionary<string, System.Action>();

        keywords.Add("Reset world", () =>
        {
            // Publish a reset world message.
            MessageBroker.Default.Publish(new ResetWorldArgs());
        });

        keywords.Add("Drop Sphere", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the OnDrop method on just the focused object.
                focusObject.SendMessage("OnDrop");
            }
        });

        // Tell the KeywordRecognizer about our keywords.
        var keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        Observable.FromEvent<PhraseRecognizer.PhraseRecognizedDelegate, PhraseRecognizedEventArgs>(
            h => args => h(args),
            h => keywordRecognizer.OnPhraseRecognized += h,
            h => keywordRecognizer.OnPhraseRecognized -= h)
            .Select(args =>
            {
                Action action;
                if (keywords.TryGetValue(args.text, out action))
                    return action;

                return null;
            })
            .Where(action => action != null)
            .Subscribe(action => action.Invoke())
            .AddTo(this);

        keywordRecognizer.Start();
    }
}