﻿using System;
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
            public int GroupID;
        }

        [Serializable]
        public class SpawnData : ArenaData, IMsgReceived
        {
            public int SpawnID;
        }

    }

}

