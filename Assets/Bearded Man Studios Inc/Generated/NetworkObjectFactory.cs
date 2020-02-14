using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using MainThreadManager = BeardedManStudios.Forge.Networking.Unity.MainThreadManager;

namespace BeardedManStudios.Forge.Networking.Generated
{
	public partial class NetworkObjectFactory : NetworkObjectFactoryBase
	{
		public override void NetworkCreateObject(NetWorker networker, int identity, uint id, FrameStream frame, Action<NetworkObject> callback)
		{
            if (NetworkManager.Instance.IsMaster)
            {
                if (frame.Sender != null && frame.Sender != networker.Me)
				{
					if (!ValidateCreateRequest(networker, identity, id, frame))
						return;
				}
			}
			
			bool availableCallback = false;
			NetworkObject obj = null;
			MainThreadManager.Run(() =>
			{
				switch (identity)
				{
					case ChatManagerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new ChatManagerNetworkObject(networker, id, frame);
						break;
					case GameModeNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new GameModeNetworkObject(networker, id, frame);
						break;
					case PlayerNetworkObject.IDENTITY:
						availableCallback = true;
                        ///obj = new PlayerNetworkObject(networker, id, frame);
                        obj = new PlayerNetworkObject();
                        PlayerNetworkObject playerObj = (PlayerNetworkObject)obj;
                        playerObj.SetupUID((int)id);
                        break;
					case WeaponPickupNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new WeaponPickupNetworkObject(networker, id, frame);
						break;
				}

				if (!availableCallback)
					base.NetworkCreateObject(networker, identity, id, frame, callback);
				else if (callback != null)
					callback(obj);
			});
		}

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}