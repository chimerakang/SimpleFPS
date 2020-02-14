using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace PTK
{
    interface IMsgReceived
    {
        void OnReceivedMsg(string msg);
    }

    [Serializable]
    public class ArenaData: IMsgReceived
    {
        public int UID;
        public int RequestID;
        public string BMSData;
        public event Action<string> MessageReceived;

        public ArenaData()
        {
            UID = -1;
        }

        public void OnReceivedMsg(string msg)
        {
            if( MessageReceived != null)
            {
                MessageReceived(msg);
            }
        }

        public T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    public class ArenaReceiver<T> where T : ArenaData, new()
    {
        Ansuz ansuz;
        int uid;
        string topic;
        string arenaID;
        int ownRequestID;
        bool _msgReceived = false;
        string _receivedMsg;

        public ArenaReceiver(Ansuz instance, string subscribeTopic, int requestID)
        {
            ansuz = instance;
            topic = subscribeTopic;

            arenaID = instance.ArenaID;
            uid = instance.UID;
            ownRequestID = requestID;
            ///instance.RegisterReceiver(subscribeTopic, this);
        }

        void OnReceivedMsg(string receivedMsg)
        {
            var data = JsonUtility.FromJson<T>(receivedMsg);
            if ( (data.RequestID == ownRequestID) && (data.UID != uid ) )
            {
                Debug.Log("ArenaReceiver Received Msg |> = " + receivedMsg);
                _msgReceived = true;
                _receivedMsg = receivedMsg;

                Debug.Log("AnsuzTask Received Msg |> = " + _receivedMsg);

                AnsuzResponse jsonData = JsonUtility.FromJson<AnsuzResponse>(_receivedMsg);

                if (jsonData.ResponseID < 0)
                {
                    Debug.Log("Errcode Number:" + jsonData.ResponseID);
                    Ansuz.Instance.Geterrorcode();
                }

                string errcodestr = jsonData.ResponseMsg;
                if (errcodestr == "time too short or too long")
                {
                    Debug.Log("Errcode String:" + errcodestr);
                    Ansuz.Instance.Geterrorstring();
                }
            }
            
        }

        public IEnumerator Receiver(IObserver<T> observer)
        {
            /// ansuz.OnReceivedMsg += OnReceivedMsg;

            if (_msgReceived)
            {
                T response = new T();
                JsonUtility.FromJsonOverwrite(_receivedMsg, response);
                observer.OnNext(response);
            }

            yield return null;
        }
    }


    public static partial class ArenaObservable
    {
        public static IObservable<T> RegisterReceiver<T>(String subscribeTopic, int requestID) where T : ArenaData, new()
        {
            ArenaReceiver<T> receiver = new ArenaReceiver<T>(Ansuz.Instance, subscribeTopic, requestID);
            return Observable.FromCoroutine<T>(observer =>
                receiver
                .Receiver(observer))
                .SubscribeOnMainThread();
        }
    }
}
