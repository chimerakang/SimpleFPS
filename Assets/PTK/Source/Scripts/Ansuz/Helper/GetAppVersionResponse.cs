using System;
using System.Collections.Generic;
using UnityEngine;

namespace PTK
{
    [Serializable]
    public class GetAppVersionAuthResponse
    {
        public Credential credential;

        [Serializable]
        public class Credential{ }
    }
}
