﻿using System;
using UniRx;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class ReactiveGestureRecognizer 
    : IDisposable
{
    public struct GestureErrorArgs
    {
        public string Error { get; set; }
        public int HResult{ get; set; }
    }

    public struct GestureArgs
    {
        public InteractionSourceKind Source { get; set; }
        public Ray HeadRay { get; set; }
    }

    public struct CumulativeDeltaGestureArgs
    {
        public InteractionSourceKind Source { get; set; }
        public Vector3 CumulativeDelta { get; set; }
        public Ray HeadRay { get; set; }
    }

    public struct NormalizedOffsetGestureArgs
    {
        public InteractionSourceKind Source { get; set; }
        public Vector3 NormalizedOffset { get; set; }
        public Ray HeadRay { get; set; }
    }

    public struct TapppedGestureArgs
    {
        public InteractionSourceKind Source { get; set; }
        public int TapCount { get; set; }
        public Ray HeadRay { get; set; }
    }

    public class GestureRecognitionException
        : Exception
    {
        public GestureRecognitionException(string message, int hresult)
            : base(message)
        {
            HResult = hresult;
        }
    }

    readonly GestureRecognizer recognizer;
    readonly IObservable<GestureErrorArgs> gestureError;
    readonly IObservable<GestureArgs> holdCanceled;
    readonly IObservable<GestureArgs> holdCompleted;
    readonly IObservable<GestureArgs> holdStarted;
    readonly IObservable<CumulativeDeltaGestureArgs> manipulationCanceled;
    readonly IObservable<CumulativeDeltaGestureArgs> manipulationCompleted;
    readonly IObservable<CumulativeDeltaGestureArgs> manipulationStarted;
    readonly IObservable<CumulativeDeltaGestureArgs> manipulationUpdated;
    readonly IObservable<NormalizedOffsetGestureArgs> navigationCanceled;
    readonly IObservable<NormalizedOffsetGestureArgs> navigationCompleted;
    readonly IObservable<NormalizedOffsetGestureArgs> navigationStarted;
    readonly IObservable<NormalizedOffsetGestureArgs> navigationUpdated;
    readonly IObservable<GestureArgs> recognitionEnded;
    readonly IObservable<GestureArgs> recognitionStarted;
    readonly IObservable<TapppedGestureArgs> tapped;

    public ReactiveGestureRecognizer()
        : this(new GestureRecognizer())
    {
    }

    public ReactiveGestureRecognizer(GestureRecognizer recognizer)
    {
        if (recognizer == null)
            throw new ArgumentNullException();

        this.recognizer = recognizer;

        gestureError = Observable.FromEvent<GestureRecognizer.GestureErrorDelegate, GestureErrorArgs>(
                    h => (error, hresult) => h(new GestureErrorArgs { Error = error, HResult = hresult }),
                    h => recognizer.GestureErrorEvent += h, h => recognizer.GestureErrorEvent -= h);

        holdCanceled = Observable.FromEvent<GestureRecognizer.HoldCanceledEventDelegate, GestureArgs>(
                        h => (source, headRay) => h(new GestureArgs { Source = source, HeadRay = headRay }),
                        h => recognizer.HoldCanceledEvent += h, h => recognizer.HoldCanceledEvent -= h);

        holdCompleted = Observable.FromEvent<GestureRecognizer.HoldCompletedEventDelegate, GestureArgs>(
                        h => (source, headRay) => h(new GestureArgs { Source = source, HeadRay = headRay }),
                        h => recognizer.HoldCompletedEvent += h, h => recognizer.HoldCompletedEvent -= h);

        holdStarted = Observable.FromEvent<GestureRecognizer.HoldStartedEventDelegate, GestureArgs>(
                        h => (source, headRay) => h(new GestureArgs { Source = source, HeadRay = headRay }),
                        h => recognizer.HoldStartedEvent += h, h => recognizer.HoldStartedEvent -= h);

        manipulationCanceled = Observable.FromEvent<GestureRecognizer.ManipulationCanceledEventDelegate, CumulativeDeltaGestureArgs>(
                        h => (source, cumulativeDelta, headRay) => h(new CumulativeDeltaGestureArgs { Source = source, CumulativeDelta = cumulativeDelta, HeadRay = headRay }),
                        h => recognizer.ManipulationCanceledEvent += h, h => recognizer.ManipulationCanceledEvent -= h);

        manipulationCompleted = Observable.FromEvent<GestureRecognizer.ManipulationCompletedEventDelegate, CumulativeDeltaGestureArgs>(
                        h => (source, cumulativeDelta, headRay) => h(new CumulativeDeltaGestureArgs { Source = source, CumulativeDelta = cumulativeDelta, HeadRay = headRay }),
                        h => recognizer.ManipulationCompletedEvent += h, h => recognizer.ManipulationCompletedEvent -= h);

        manipulationStarted = Observable.FromEvent<GestureRecognizer.ManipulationStartedEventDelegate, CumulativeDeltaGestureArgs>(
                        h => (source, cumulativeDelta, headRay) => h(new CumulativeDeltaGestureArgs { Source = source, CumulativeDelta = cumulativeDelta, HeadRay = headRay }),
                        h => recognizer.ManipulationStartedEvent += h, h => recognizer.ManipulationStartedEvent -= h);

        manipulationUpdated = Observable.FromEvent<GestureRecognizer.ManipulationUpdatedEventDelegate, CumulativeDeltaGestureArgs>(
                        h => (source, cumulativeDelta, headRay) => h(new CumulativeDeltaGestureArgs { Source = source, CumulativeDelta = cumulativeDelta, HeadRay = headRay }),
                        h => recognizer.ManipulationUpdatedEvent += h, h => recognizer.ManipulationUpdatedEvent -= h);

        navigationCanceled = Observable.FromEvent<GestureRecognizer.NavigationCanceledEventDelegate, NormalizedOffsetGestureArgs>(
                        h => (source, normalizedOffset, headRay) => h(new NormalizedOffsetGestureArgs { Source = source, NormalizedOffset = normalizedOffset, HeadRay = headRay }),
                        h => recognizer.NavigationCanceledEvent += h, h => recognizer.NavigationCanceledEvent -= h);

        navigationCompleted = Observable.FromEvent<GestureRecognizer.NavigationCompletedEventDelegate, NormalizedOffsetGestureArgs>(
                        h => (source, normalizedOffset, headRay) => h(new NormalizedOffsetGestureArgs { Source = source, NormalizedOffset = normalizedOffset, HeadRay = headRay }),
                        h => recognizer.NavigationCompletedEvent += h, h => recognizer.NavigationCompletedEvent -= h);

        navigationStarted = Observable.FromEvent<GestureRecognizer.NavigationStartedEventDelegate, NormalizedOffsetGestureArgs>(
                        h => (source, normalizedOffset, headRay) => h(new NormalizedOffsetGestureArgs { Source = source, NormalizedOffset = normalizedOffset, HeadRay = headRay }),
                        h => recognizer.NavigationStartedEvent += h, h => recognizer.NavigationStartedEvent -= h);

        navigationUpdated = Observable.FromEvent<GestureRecognizer.NavigationUpdatedEventDelegate, NormalizedOffsetGestureArgs>(
                        h => (source, normalizedOffset, headRay) => h(new NormalizedOffsetGestureArgs { Source = source, NormalizedOffset = normalizedOffset, HeadRay = headRay }),
                        h => recognizer.NavigationUpdatedEvent += h, h => recognizer.NavigationUpdatedEvent -= h);

        recognitionEnded = Observable.FromEvent<GestureRecognizer.RecognitionEndedEventDelegate, GestureArgs>(
                        h => (source, headRay) => h(new GestureArgs { Source = source, HeadRay = headRay }),
                        h => recognizer.RecognitionEndedEvent += h, h => recognizer.RecognitionEndedEvent -= h);

        recognitionStarted = Observable.FromEvent<GestureRecognizer.RecognitionStartedEventDelegate, GestureArgs>(
                        h => (source, headRay) => h(new GestureArgs { Source = source, HeadRay = headRay }),
                        h => recognizer.RecognitionStartedEvent += h, h => recognizer.RecognitionStartedEvent -= h);

        tapped = Observable.FromEvent<GestureRecognizer.TappedEventDelegate, TapppedGestureArgs>(
                        h => (source, tapCount, headRay) => h(new TapppedGestureArgs { Source = source, TapCount = tapCount, HeadRay = headRay }),
                        h => recognizer.TappedEvent += h, h => recognizer.TappedEvent -= h);
    }

    public GestureSettings GetRecognizableGestures()
    {
        return recognizer.GetRecognizableGestures();
    }

    public GestureSettings SetRecognizableGestures(GestureSettings newMaskValue)
    {
        return recognizer.SetRecognizableGestures(newMaskValue);
    }

    public bool IsCapturingGestures
    {
        get { return recognizer.IsCapturingGestures(); }
    }

    public void StartCapturingGestures()
    {
        recognizer.StartCapturingGestures();
    }

    public void StopCapturingGestures()
    {
        recognizer.StopCapturingGestures();
    }

    public void CancelGestures()
    {
        recognizer.CancelGestures();
    }

    public IObservable<GestureArgs> HoldCanceledAsObservable()
    {
        return Observable.Create<GestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                holdCanceled.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<GestureArgs> HoldCompletedAsObservable()
    {
        return Observable.Create<GestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                holdCompleted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<GestureArgs> HoldStartedAsObservable()
    {
        return Observable.Create<GestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                holdStarted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<CumulativeDeltaGestureArgs> ManipulationCanceledAsObservable()
    {
        return Observable.Create<CumulativeDeltaGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                manipulationCanceled.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<CumulativeDeltaGestureArgs> ManipulationCompletedAsObservable()
    {
        return Observable.Create<CumulativeDeltaGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                manipulationCompleted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<CumulativeDeltaGestureArgs> ManipulationStartedAsObservable()
    {
        return Observable.Create<CumulativeDeltaGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                manipulationStarted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<CumulativeDeltaGestureArgs> ManipulationUpdatedAsObservable()
    {
        return Observable.Create<CumulativeDeltaGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                manipulationUpdated.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<NormalizedOffsetGestureArgs> NavigationCanceledAsObservable()
    {
        return Observable.Create<NormalizedOffsetGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                navigationCanceled.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<NormalizedOffsetGestureArgs> NavigationCompletedAsObservable()
    {
        return Observable.Create<NormalizedOffsetGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                navigationCompleted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<NormalizedOffsetGestureArgs> NavigationStartedAsObservable()
    {
        return Observable.Create<NormalizedOffsetGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                navigationStarted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<NormalizedOffsetGestureArgs> NavigationUpdatedAsObservable()
    {
        return Observable.Create<NormalizedOffsetGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                navigationUpdated.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<GestureArgs> RecognitionEndedAsObservable()
    {
        return Observable.Create<GestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                recognitionEnded.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<GestureArgs> RecognitionStartedAsObservable()
    {
        return Observable.Create<GestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                recognitionStarted.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    public IObservable<TapppedGestureArgs> TappedAsObservable()
    {
        return Observable.Create<TapppedGestureArgs>(observer =>
        {
            return new CompositeDisposable(new IDisposable[] {
                tapped.Subscribe(observer.OnNext),
                gestureError.Subscribe(error => observer.OnError(new GestureRecognitionException(error.Error, error.HResult))),
            });
        });
    }

    #region IDisposable Support

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                recognizer.Dispose();
            }

            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        Dispose(true);
    }

    #endregion

}

