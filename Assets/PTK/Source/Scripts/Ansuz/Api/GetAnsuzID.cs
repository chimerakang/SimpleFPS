using System;

namespace PTK
{
    public static partial class ObservableAusuz
    {
        [Serializable]
        public class GetArenaIDRequest : AnsuzRequest
        {
        }

        [Serializable]
        public class GetAansuzIDResponse : AnsuzRequest
        {
            public string ArenaID;
            public string SessionToken;
            public bool IsMaster;
        }

        public static UniRx.IObservable<GetAansuzIDResponse> GetArenaID()
        {
            var request = new GetArenaIDRequest
            {
                RequestID = (int)AnsuzRequestID.GetArenaID,
            };

            return SendRequest<GetAansuzIDResponse>(request);
        }

    }

}

