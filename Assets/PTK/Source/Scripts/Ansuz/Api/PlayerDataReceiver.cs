using System;
using UnityEngine;

namespace PTK
{
    public static partial class ArenaObservable
    {
        [Serializable]
        public class PlayerData : ArenaData, IMsgReceived
        {
            public Vector3 _position;
            public Vector3 _spineRotation;
            public Quaternion _rotation;
            public float _vertical;
            public float _horizontal;
            public bool _isMoving;
        }
    }

}

