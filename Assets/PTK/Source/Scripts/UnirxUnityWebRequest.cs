using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public static class UnirxUnityWebRequest
{
    public enum ResponseType
    {
        Json,
        Html
    }

    // To Abort UnityWebRequest, call Dispose() to subscription. OnError will be called.
    private const string RequestCancelMessage = "request_cancelled";

    // Get Request
    public static IObservable<T> Get<T>(string url, ResponseType responseType, IProgress<float> progress = null,
        float timeoutSec = 0)
    {
        var headers = AddHeadersFor(responseType);
        return Get<T>(url, headers, progress, timeoutSec);
    }

    public static IObservable<T> Get<T>(string url, Dictionary<string, string> headers,
        IProgress<float> progress = null, float timeoutSec = 0)
    {
        if (timeoutSec > 0)
        {
            // timeout
        }
        return Observable.FromMicroCoroutine<T>(
            (observer, cancellationToken) => GetRequest(url, headers, observer, cancellationToken, progress));
    }

    private static IEnumerator GetRequest<T>(string url, Dictionary<string, string> headers, IObserver<T> observer,
        CancellationToken cancellationToken, IProgress<float> progress = null)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            SetRequestHeader(request, headers);
            request.Send();
            while (!request.isDone && !cancellationToken.IsCancellationRequested)
            {
                if (progress != null)
                {
                    try
                    {
                        progress.Report(request.downloadProgress);
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                        yield break;
                    }
                }
                yield return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                observer.OnError(new Exception(RequestCancelMessage));
                yield break;
            }

            if (progress != null)
            {
                try
                {
                    progress.Report(request.downloadProgress);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                    yield break;
                }
            }

            OnResponse(observer, request, headers);
        }
    }

    // Post Request
    public static IObservable<T> Post<T>(string url, ResponseType responseType,
        Dictionary<string, string> parameters = null, IProgress<float> progress = null, float timeoutSec = 0)
    {
        var headers = AddHeadersFor(responseType);
        return Post<T>(url, headers, parameters, progress, timeoutSec);
    }

    public static IObservable<T> Post<T>(string url, Dictionary<string, string> headers,
        Dictionary<string, string> parameters = null, IProgress<float> progress = null, float timeoutSec = 0)
    {
        if (timeoutSec > 0)
        {
            // timeout
        }
        return Observable.FromMicroCoroutine<T>(
            (observer, cancellationToken) => PostRequest(url, headers, observer, cancellationToken, progress, parameters));
    }

    private static IEnumerator PostRequest<T>(string url, Dictionary<string, string> headers, IObserver<T> observer,
        CancellationToken cancellationToken, IProgress<float> progress = null, Dictionary<string, string> parameters = null)
    {
        using (var request = UnityWebRequest.Post(url, parameters))
        {
            SetRequestHeader(request, headers);

            request.Send();
            while (!request.isDone && !cancellationToken.IsCancellationRequested)
            {
                if (progress != null)
                {
                    try
                    {
                        progress.Report(request.downloadProgress);
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                        yield break;
                    }
                }
                yield return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                observer.OnError(new Exception(RequestCancelMessage));
                yield break;
            }

            if (progress != null)
            {
                try
                {
                    progress.Report(request.downloadProgress);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                    yield break;
                }
            }

            OnResponse(observer, request, headers);
        }
    }

    private static void OnResponse<T>(IObserver<T> observer, UnityWebRequest request,
        Dictionary<string, string> headers)
    {
        if (request.isNetworkError)
        {
            observer.OnError(new Exception(request.error));
        }
        else
        {
            if (request.responseCode != 200)
            {
                observer.OnError(new Exception(request.responseCode.ToString()));
            }
            else
            {
                var text = request.downloadHandler.text;

                // TODO: find alternative way to detect response type. Content-type is not adequate.
                // TODO: Servers responding to POST method with multimedia sometimes do not accept application/json Content type
                // TODO: Better to use application/x-www-form-urlencoded or multipart/form-data with POST.
                if (headers.ContainsKey("Content-Type"))
                {
                    string headerValue;
                    if (headers.TryGetValue("Content-Type", out headerValue))
                    {
                        switch (headerValue.Trim())
                        {
                            case "application/json":
                                var res = JsonUtility.FromJson<T>(text);
                                observer.OnNext(res);
                                break;
                            case "text/html":
                                break;
                        }
                    }
                }

                observer.OnCompleted();
            }
        }
    }

    private static void SetRequestHeader(UnityWebRequest request, Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }
    }

    private static Dictionary<string, string> AddHeadersFor(ResponseType responseType)
    {
        var headers = new Dictionary<string, string>();
        switch (responseType)
        {
            case ResponseType.Json:
                headers.Add("Content-Type", "application/json");
                headers.Add("Accepted", "application/json");
                break;
            case ResponseType.Html:
                break;
            default:
                throw new ArgumentOutOfRangeException("responseType", responseType, null);
        }
        return headers;
    }
}