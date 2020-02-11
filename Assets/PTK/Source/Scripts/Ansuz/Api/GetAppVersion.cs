using System;

namespace PTK
{
    public static partial class ObservableAusuz
    {
        [Serializable]
        public class GetAppVersionRequest : AnsuzRequest
        {
        }

        [Serializable]
        public class GetAppVersionResponse: AnsuzRequest
        {
            public string AppVersion;
            public int Builds;
        }

        public static UniRx.IObservable<GetAppVersionResponse> GetAppVersion()
        {
            var request = new GetAppVersionRequest
            {
                RequestID = (int)AnsuzRequestID.GetAppVersion,
            };

            return SendRequest<GetAppVersionResponse>(request);
        }

    }

}

