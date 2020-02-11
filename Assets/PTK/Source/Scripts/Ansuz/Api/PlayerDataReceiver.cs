using System;

namespace PTK
{
    public static partial class ArenaObservable
    {
        [Serializable]
        public class PlayerData : ArenaData
        {
            public bool IsMaster;
        }

        public static UniRx.IObservable<PlayerData> PlayerDataReceiver(String subscribeTopic, string arenaID, int requestID)
        {
            return RegisterReceiver<PlayerData>(subscribeTopic, arenaID, requestID);
        }

    }

}

