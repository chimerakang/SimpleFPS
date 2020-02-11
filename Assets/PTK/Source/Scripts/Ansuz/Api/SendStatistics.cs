using System;

namespace PTK
{
    public static partial class ObservableAusuz
    {
        [Serializable]
        public class SendStatisticsRequest : AnsuzRequest
        {
            public string Statistics;
        }

        [Serializable]
        public class SendStatisticsResponse : AnsuzRequest
        {
        }

        public static UniRx.IObservable<SendStatisticsResponse> SendStatistics(string json)
        {
            var request = new SendStatisticsRequest
            {
                RequestID = (int)AnsuzRequestID.SendStatistics,
                ArenaID = Ansuz.Instance.ArenaID,
                SessionToken = Ansuz.Instance.SessionToken,
                Statistics = json,
            };

            return SendRequest<SendStatisticsResponse>(request);
        }

    }

}

