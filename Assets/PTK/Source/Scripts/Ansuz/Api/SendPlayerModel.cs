using System;
using BeardedManStudios;

namespace PTK
{
    public static partial class ObservableAusuz
    {
        [Serializable]
        public class SendPlayerModelRequest : AnsuzRequest
        {
            public string ModelData;
        }

        [Serializable]
        public class SendPlayerModelResponse : AnsuzRequest
        {
        }

        public static UniRx.IObservable<SendPlayerModelResponse> SendPlayerModel(string modelData)
        {
            var request = new SendPlayerModelRequest
            {
                RequestID = (int)AnsuzRequestID.SendPlayerModel,
                ArenaID = Ansuz.Instance.ArenaID,
                SessionToken = Ansuz.Instance.SessionToken,
                ModelData = modelData,
            };

            return SendRequest<SendPlayerModelResponse>(request);
        }

    }

}

