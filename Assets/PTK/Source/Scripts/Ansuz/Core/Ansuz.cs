using CielaSpike;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UniRx;
using UniRx.Triggers;

namespace PTK
{
    public class Ansuz : MonoBehaviour
    {
        public static bool Ansuzinitialized = false;
        public static Ansuz Instance { get; private set; }

        public int UID;
        public string ArenaID;
        public string SessionToken;
        public bool IsMaster = false;
        public string DeviceID = "";

        string BrokerHostName = "notice.com.tw";
        int BrokerPort = 29001;

        public event Action<MqttMsgPublishEventArgs> OnReceivedMsg;
        public event Action OnDisconnected;
        public event Action OnConnected;
        public event Action GetErrorCode;
        public event Action GetErrorStr;
        public event Action GetTimeOut;

        public float reconnectInterval = 1f;
        public bool Connecting = false;

        bool timeout = false;
        bool hasopenwindow = false;
        bool hasstartreconnect = false;
        int timeouttime = 0;
        private Dictionary<string, ArenaData> _receiverMap = new Dictionary<string, ArenaData>();
        private Dictionary<string, byte> _qosMap = new Dictionary<string, byte>();

        void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            Debug.Log("[Arena] OnDestroy");
            OnConnected -= ReadyReConnect;
            UnsubsribeEvents();
            Disconnect();
            Instance = null;
        }

        public void Init()
        {
            if (Instance != null)
            {
                enabled = false;
                DestroyImmediate(this);
                return;
            }
            Instance = this;

            GetDeviceToken();
            
            string defaultTopic = "arena/" + DeviceID;
            _qosMap[defaultTopic] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
            
            Ansuzinitialized = true;
        }

        public void StartConnect()
        {
            _waitForReconnect = new WaitForSeconds(reconnectInterval);
            Observable.FromCoroutine(CreateClient).SelectMany(ConnectLoop(DeviceID, "", "")).Subscribe().AddTo(this);
            OnConnected += ReadyReConnect;
        }


        string PublishTopic
        {
            get { return "arena/user/" + DeviceID; }
        }

        string SubscribeTopic
        {
            get { return "arena/" + DeviceID + "/#"; }
        }

        string DeviceToken
        {
            get
            {
                if (_deviceToken.Length == 0)
                    _deviceToken = GetDeviceToken();

                return _deviceToken;
            }
        }

        WaitForSeconds _waitForReconnect;
        MqttClient _client;
        bool _subscribed;
        bool _privousIsConnected;
        string _deviceToken = "";

        public bool Publish(string message)
        {
            if (_client == null)
                return false;

            if (!_client.IsConnected)
                return false;

            ushort msgId = _client.Publish(PublishTopic,
                Encoding.UTF8.GetBytes(message),
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                false);

            Debug.Log("Publish :> Id ( " + msgId + " ) msg => " + message);

            return true;
        }

        public bool PublishToTopic(string topic, string message, byte qos )
        {
            if (_client == null)
                return false;

            if (!_client.IsConnected)
                return false;

            ushort msgId = _client.Publish(topic, Encoding.UTF8.GetBytes(message), qos, false);
            Debug.Log("PublishToTopic :> Id ( " + msgId + " ) msg => " + message);

            return true;
        }

        public bool PublishBytes(string topic, byte[] compress, byte qos)
        {
            if (_client == null)
                return false;

            if (!_client.IsConnected)
                return false;

            ushort msgId = _client.Publish(topic, compress, qos, false);
            return true;
        }


        public void RegisterReceiver(string topic, byte qos, ArenaData reveiver)
        {
            _receiverMap.Add(topic, reveiver);
            _qosMap.Add(topic, qos);
        }

        public void UnregisterReceiver(string topic)
        {
            _receiverMap.Remove(topic);
        }


        void Disconnect()
        {
            if (_client == null)
                return;

            if (!_client.IsConnected)
                return;

            _client.Disconnect();
            Debug.Log("Broker Disconnected");
        }

        IEnumerator CreateClient()
        {
            while (_client == null)
            {
                yield return Ninja.JumpBack;
                try
                {
                    _client = new MqttClient(BrokerHostName, BrokerPort, false, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                yield return Ninja.JumpToUnity;

                if (_client == null)
                    yield return _waitForReconnect;
            }
        }

        IEnumerator ConnectLoop(string clientId, string username, string password)
        {
            while (true)
            {
                yield return _waitForReconnect;
                if (_client.IsConnected)
                    continue;

                if (_privousIsConnected)
                {
                    _privousIsConnected = false;
                    if (OnDisconnected != null)
                    {
                        Connecting = false;
                        OnDisconnected();
                    }
                    Debug.Log("Broker OnDisconnected");
                }

                yield return Ninja.JumpBack;
                try
                {
                    _client.Connect(clientId, username, password);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                yield return Ninja.JumpToUnity;

                if (_client != null)
                {
                    if (_client.IsConnected)
                    {
                        _privousIsConnected = true;
                        SubscribeEvents();
                        if (OnConnected != null)
                        {
                            Connecting = true;
                            Debug.Log("OnConnected!!!!");
                            OnConnected();
                        }
                        hasstartreconnect = false;
                        Debug.Log("Broker OnConnected");
                    }
                }
            }
        }

        void OnMqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            /*
                */
            string topic = e.Topic;
            if( topic != null && topic.Length > 0 && _receiverMap.ContainsKey(topic) )
            {
                ArenaData arenaData = _receiverMap[topic];
                if( arenaData != null)
                {
                    var receivedMsg = Encoding.UTF8.GetString(e.Message);
                    arenaData.OnReceivedMsg(receivedMsg);
                }
            }
            else
            {
                if (OnReceivedMsg != null)
                    OnReceivedMsg(e);
            }
        }

        void SubscribeEvents()
        {
            if (_subscribed)
                return;

            List<string> topicList = new List<string>();
            List<byte> qosList = new List<byte>();
            foreach ( var qosMap in _qosMap )
            {
                topicList.Add(qosMap.Key);
                qosList.Add(qosMap.Value);
            }
            _client.Subscribe(topicList.ToArray(), qosList.ToArray());
            _client.MqttMsgPublishReceived += OnMqttMsgReceived;
            _subscribed = true;
        }

        void UnsubsribeEvents()
        {
            if (_subscribed)
            {
                List<string> topicList = new List<string>();
                foreach (var qosMap in _qosMap)
                {
                    topicList.Add(qosMap.Key);
                }
                _client.Unsubscribe(topicList.ToArray());

                ///_client.Unsubscribe(new string[] { SubscribeTopic });
                _client.MqttMsgPublishReceived -= OnMqttMsgReceived;

                _subscribed = false;
            }
        }

        public string GetDeviceToken()
        {
#if UNITY_IOS
            byte[] token = NotificationServices.deviceToken;
            if (token == null)
            {
                Debug.LogWarning("iOS deviceToken is null");
                return "null";
            }
                
            DeviceID = BitConverter.ToString(token).Replace("-", "");
#else
            DeviceID = SystemInfo.deviceUniqueIdentifier;
#endif
            return DeviceID;
        }

        public void Geterrorcode()
        {
            if (GetErrorCode != null)
            {
                GetErrorCode();
            }
            Debug.Log("GetErrorCode");
        }

        public void Geterrorstring()
        {
            if (GetErrorStr != null)
            {
                GetErrorStr();
            }
        }

        public void GettimeOut()
        {
            if (GetTimeOut != null)
            {
                GetTimeOut();
            }

            Debug.Log("GetTimeOut");
            if (hasstartreconnect == false)
            {
                if (timeouttime > 2) // 0 1 2 3次
                {
                    StopAllCoroutines();
                    timeouttime = 1;
                }
                else
                {
                    Debug.LogError("GetTimeOut & timeouttime ++ :" + timeouttime);
                    timeouttime++;
                    hasstartreconnect = true;
                    StopCoroutine(StartReconnrct());
                    StartCoroutine(StartReconnrct());
                }
            }
        }

        IEnumerator StartReconnrct()
        {
            Debug.Log("StartReconnrct");
            if (!timeout)
            {
                StopCoroutine(WaitToOpenWindow());
                StartCoroutine(WaitToOpenWindow());
                yield return new WaitForSeconds(5.0f);
                Disconnect();
                Connecting = false;
                _client = null;
                timeout = true;
                Init();
            }
            else
            {
                Debug.Log("return null");
                yield return null;
            }
        }

        public void ReadyReConnect()
        {
            Debug.Log("ReadyReConnect");
            if (timeout)
            {
                timeout = false;
                StopCoroutine(WaitToOpenWindow());
                if (hasopenwindow)
                {
                    hasopenwindow = false;
                }
                hasstartreconnect = false;
            }
        }

        IEnumerator WaitToOpenWindow()
        {
            yield return new WaitForSeconds(3.0f);
            hasopenwindow = true;
        }

    }
}