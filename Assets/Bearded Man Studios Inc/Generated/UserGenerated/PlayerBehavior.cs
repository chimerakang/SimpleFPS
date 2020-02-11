using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"Vector3\", \"Vector3\"][\"string\"][\"int\"][\"int\", \"Vector3\", \"Vector3\"][\"int\", \"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"origin\", \"camForward\"][\"killedBy\"][\"weaponIndex\"][\"amount\", \"hitPoint\", \"hitNormal\"][\"skinIndex\", \"playerName\"]]")]
	public abstract partial class PlayerBehavior : NetworkBehavior
	{
		public const byte RPC_SHOOT = 0 + 5;
		public const byte RPC_DIE = 1 + 5;
		public const byte RPC_SWITCH_WEAPON = 2 + 5;
		public const byte RPC_TAKE_DAMAGE = 3 + 5;
		public const byte RPC_SETUP_PLAYER = 4 + 5;
		
		public PlayerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (PlayerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("Shoot", Shoot, typeof(Vector3), typeof(Vector3));
			networkObject.RegisterRpc("Die", Die, typeof(string));
			networkObject.RegisterRpc("SwitchWeapon", SwitchWeapon, typeof(int));
			networkObject.RegisterRpc("TakeDamage", TakeDamage, typeof(int), typeof(Vector3), typeof(Vector3));
			networkObject.RegisterRpc("SetupPlayer", SetupPlayer, typeof(int), typeof(string));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new PlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new PlayerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// Vector3 origin
		/// Vector3 camForward
		/// </summary>
		public abstract void Shoot(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string killedBy
		/// </summary>
		public abstract void Die(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int weaponIndex
		/// </summary>
		public abstract void SwitchWeapon(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int amount
		/// Vector3 hitPoint
		/// Vector3 hitNormal
		/// </summary>
		public abstract void TakeDamage(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int skinIndex
		/// string playerName
		/// </summary>
		public abstract void SetupPlayer(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}