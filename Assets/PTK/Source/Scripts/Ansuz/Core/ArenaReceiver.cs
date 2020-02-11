using System;
using System.Collections;
using System.Text;
using UniRx;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace PTK
{
    [Serializable]
    public class ArenaData
    {
        public int RequestID;
        public string Timestamp;
        public string ArenaID;
        public string StringData;

        public ArenaData()
        {
            Timestamp = DateTime.Now.Ticks.ToString();
        }
    }

    

    

    public class ArenaReceiver<T> where T : ArenaData, new()
    {
        Ansuz ansuz;
        String topic;
        String ownArenaID;
        int ownRequestID;
        bool _msgReceived = false;
        string _receivedMsg;

        public ArenaReceiver(Ansuz instance, String subscribeTopic, string arenaID, int requestID)
        {
            ansuz = instance;
            topic = subscribeTopic;
            ownArenaID = arenaID;
            ownRequestID = requestID;
        }

        void OnReceivedMsg(MqttMsgPublishEventArgs mqttMsg)
        {
            var receivedMsg = Encoding.UTF8.GetString(mqttMsg.Message);
            var data = JsonUtility.FromJson<T>(receivedMsg);
            /*
            if (data.RequestID == ownRequestID && !data.ArenaID.Equals(ownArenaID) )
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
            */
        }

        public IEnumerator Worker(IObserver<T> observer)
        {
            ansuz.OnReceivedMsg += OnReceivedMsg;

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
        public static IObservable<T> RegisterReceiver<T>(String subscribeTopic, string arenaID, int requestID) where T : ArenaData, new()
        {
            return Observable.FromCoroutine<T>(observer =>
               new ArenaReceiver<T>(Ansuz.Instance, subscribeTopic, arenaID, requestID)
                    .Worker(observer))
                    .SubscribeOnMainThread();
        }
    }

}
