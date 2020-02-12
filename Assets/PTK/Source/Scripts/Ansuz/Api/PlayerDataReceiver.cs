using System;
using UnityEngine;

namespace PTK
{
    public static partial class ArenaObservable
    {
        [Serializable]
        public class PlayerData : ArenaData, IMsgReceived
        {
        }

        [Serializable]
        public class FrameData : ArenaData, IMsgReceived
        {
            public int Group;
        }
    }

}

