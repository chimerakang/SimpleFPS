using CielaSpike;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UniRx;

namespace PTK
{
    public class Ansuz  : MonoBehaviour
    {
        public static bool Ansuzinitialized = false;
        public static Ansuz Instance { get; private set; }

        public string ArenaID;
        public string SessionToken;
        public bool IsMaster = false;

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

        string PublishTopic
        {
            get { return "arena/user/" + SystemInfo.deviceUniqueIdentifier; }
        }

        string SubscribeTopic
        {
            get { return "arena/" + SystemInfo.deviceUniqueIdentifier + "/#"; }
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
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                false);

            Debug.Log("Publish :> Id ( " + msgId + " ) msg => " + message);

            return true;
        }


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

            _waitForReconnect = new WaitForSeconds(reconnectInterval);
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            Observable.FromCoroutine(CreateClient).SelectMany(ConnectLoop(deviceId, "", "")).Subscribe().AddTo(this);
            OnConnected += ReadyReConnect;
            Ansuzinitialized = true;
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
            if (OnReceivedMsg != null)
                OnReceivedMsg(e);
        }

        void SubscribeEvents()
        {
            if (_subscribed)
                return;

            _client.Subscribe(new string[] { SubscribeTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            _client.MqttMsgPublishReceived += OnMqttMsgReceived;

            _subscribed = true;
        }

        void UnsubsribeEvents()
        {
            if (_subscribed)
            {
                _client.Unsubscribe(new string[] { SubscribeTopic });
                _client.MqttMsgPublishReceived -= OnMqttMsgReceived;

                _subscribed = false;
            }
        }

        public static string GetDeviceToken()
        {
#if UNITY_IOS
            byte[] token = NotificationServices.deviceToken;
            if (token == null)
            {
                Debug.LogWarning("iOS deviceToken is null");
                return "null";
            }
                        
            return BitConverter.ToString(token).Replace("-", "");
#endif
            return SystemInfo.deviceUniqueIdentifier;
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