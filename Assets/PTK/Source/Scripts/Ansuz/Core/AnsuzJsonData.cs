using System;

namespace PTK
{
    [Serializable]
    public class AnsuzRequest
    {
        public int RequestID; 
        public string Timestamp;
        public string SessionToken;
        public string ArenaID;

        public AnsuzRequest()
        {
            Timestamp = DateTime.Now.Ticks.ToString();
        }
    }

    [Serializable]
    public class AnsuzResponse
    {
        public int ResponseID;
        public string Timestamp;
        public string ResponseMsg;
    }
}
