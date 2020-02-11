using System;
using System.Collections;
using System.Text;
using UniRx;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace PTK
{
    public class AnsuzTask<T> where T : AnsuzRequest, new()
    {
        static float TimeoutSpan = 15f;

        Ansuz ansuz;
        float _timer;

        bool _msgReceived = false;
        string _receivedMsg;
        string timestamp;
        string _requestJson;

        public AnsuzTask(Ansuz instance, AnsuzRequest request)
        {
            ansuz = instance;
            timestamp = request.Timestamp;
            _requestJson = JsonUtility.ToJson(request);
        }

        void OnReceivedMsg(MqttMsgPublishEventArgs mqttMsg)
        {
            var receivedMsg = Encoding.UTF8.GetString(mqttMsg.Message);
            var data = JsonUtility.FromJson<T>(receivedMsg);

            if (data.Timestamp.Equals(timestamp))
            {
                _msgReceived = true;
                _receivedMsg = receivedMsg;

                Debug.Log("AnsuzTask Received Msg |> = " + _receivedMsg);

                AnsuzResponse jsonData = JsonUtility.FromJson<AnsuzResponse>(_receivedMsg); 

                if (jsonData.ResponseID < 0 )
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

        public IEnumerator PublishMessage(IObserver<T> observer)
        {
            bool published = Ansuz.Instance.Publish(_requestJson);
            if (!published)
                yield break;

            ansuz.OnReceivedMsg += OnReceivedMsg;

            while (_timer < TimeoutSpan && !_msgReceived)
            {
                _timer += Time.deltaTime;
                yield return null;
            }

            if (_msgReceived)
            {
                T response = new T();
                JsonUtility.FromJsonOverwrite(_receivedMsg, response);
                observer.OnNext(response);
            }
            else
            {
                if( _receivedMsg == null )
                {
                    observer.OnError(new Exception("receive message is null"));
                    yield break;
                }
                else
                {
                    AnsuzResponse jsonData = JsonUtility.FromJson<AnsuzResponse>(_receivedMsg);
                    if (jsonData.ResponseID == 7)
                    {
                        yield break;
                    }

                    observer.OnError(new Exception("Timeout |> msg = " + jsonData.ResponseMsg + " | requestJson:" + _requestJson));
                }

                Ansuz.Instance.GettimeOut();
            }

            ansuz.OnReceivedMsg -= OnReceivedMsg;
            observer.OnCompleted();
        }
    }
}