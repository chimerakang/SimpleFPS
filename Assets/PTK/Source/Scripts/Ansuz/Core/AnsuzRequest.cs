using System;
using UniRx;
using UnityEngine;

namespace PTK
{
    public enum AnsuzRequestID
    {
        GetArenaID = 2,
        AnsuzLogin = 3,
        SendDeviceToken = 4,
        SendStatistics = 5,
        SendSongRecord = 6,
        SendChallengeRecord = 7,
        GetAppVersion = 8,
        SendPlayerModel = 9,
    }


    public static partial class ObservableAusuz
    {
        public static IObservable<T> SendRequest<T>(AnsuzRequest request) where T : AnsuzRequest, new()
        {
            return Observable.FromCoroutine<T>(observer =>
               new AnsuzTask<T>(Ansuz.Instance, request)
                  .PublishMessage(observer))
                  .SubscribeOnMainThread();
        }
    }
}
