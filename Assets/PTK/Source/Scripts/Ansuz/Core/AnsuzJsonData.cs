using System;

namespace PTK
{
    [Serializable]
    public class AnsuzRequest
    {
        public int RequestID; 
        public int UID;
        public string SessionToken;
        public string ArenaID;

        public AnsuzRequest()
        {
            UID = -1;
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
