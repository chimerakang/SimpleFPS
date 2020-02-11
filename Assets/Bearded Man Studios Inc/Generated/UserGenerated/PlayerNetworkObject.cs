using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;
using UniRx;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0.15,0.15,0.15,0]")]
	public partial class PlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 3;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private Vector3 _position;
		public event FieldEvent<Vector3> positionChanged;
		public InterpolateVector3 positionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		private Quaternion _rotation;
		public event FieldEvent<Quaternion> rotationChanged;
		public InterpolateQuaternion rotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion rotation
		{
			get { return _rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetrotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_rotation(ulong timestep)
		{
			if (rotationChanged != null) rotationChanged(_rotation, timestep);
			if (fieldAltered != null) fieldAltered("rotation", _rotation, timestep);
		}
		private Vector3 _spineRotation;
		public event FieldEvent<Vector3> spineRotationChanged;
		public InterpolateVector3 spineRotationInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 spineRotation
		{
			get { return _spineRotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_spineRotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_spineRotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetspineRotationDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_spineRotation(ulong timestep)
		{
			if (spineRotationChanged != null) spineRotationChanged(_spineRotation, timestep);
			if (fieldAltered != null) fieldAltered("spineRotation", _spineRotation, timestep);
		}
		private float _vertical;
		public event FieldEvent<float> verticalChanged;
		public InterpolateFloat verticalInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float vertical
		{
			get { return _vertical; }
			set
			{
				// Don't do anything if the value is the same
				if (_vertical == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_vertical = value;
				hasDirtyFields = true;
			}
		}

		public void SetverticalDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_vertical(ulong timestep)
		{
			if (verticalChanged != null) verticalChanged(_vertical, timestep);
			if (fieldAltered != null) fieldAltered("vertical", _vertical, timestep);
		}
		private float _horizontal;
		public event FieldEvent<float> horizontalChanged;
		public InterpolateFloat horizontalInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float horizontal
		{
			get { return _horizontal; }
			set
			{
				// Don't do anything if the value is the same
				if (_horizontal == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_horizontal = value;
				hasDirtyFields = true;
			}
		}

		public void SethorizontalDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_horizontal(ulong timestep)
		{
			if (horizontalChanged != null) horizontalChanged(_horizontal, timestep);
			if (fieldAltered != null) fieldAltered("horizontal", _horizontal, timestep);
		}
		private bool _isMoving;
		public event FieldEvent<bool> isMovingChanged;
		public Interpolated<bool> isMovingInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool isMoving
		{
			get { return _isMoving; }
			set
			{
				// Don't do anything if the value is the same
				if (_isMoving == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x20;
				_isMoving = value;
				hasDirtyFields = true;
			}
		}

		public void SetisMovingDirty()
		{
			_dirtyFields[0] |= 0x20;
			hasDirtyFields = true;
		}

		private void RunChange_isMoving(ulong timestep)
		{
			if (isMovingChanged != null) isMovingChanged(_isMoving, timestep);
			if (fieldAltered != null) fieldAltered("isMoving", _isMoving, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			rotationInterpolation.current = rotationInterpolation.target;
			spineRotationInterpolation.current = spineRotationInterpolation.target;
			verticalInterpolation.current = verticalInterpolation.target;
			horizontalInterpolation.current = horizontalInterpolation.target;
			isMovingInterpolation.current = isMovingInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _rotation);
			UnityObjectMapper.Instance.MapBytes(data, _spineRotation);
			UnityObjectMapper.Instance.MapBytes(data, _vertical);
			UnityObjectMapper.Instance.MapBytes(data, _horizontal);
			UnityObjectMapper.Instance.MapBytes(data, _isMoving);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			rotationInterpolation.current = _rotation;
			rotationInterpolation.target = _rotation;
			RunChange_rotation(timestep);
			_spineRotation = UnityObjectMapper.Instance.Map<Vector3>(payload);
			spineRotationInterpolation.current = _spineRotation;
			spineRotationInterpolation.target = _spineRotation;
			RunChange_spineRotation(timestep);
			_vertical = UnityObjectMapper.Instance.Map<float>(payload);
			verticalInterpolation.current = _vertical;
			verticalInterpolation.target = _vertical;
			RunChange_vertical(timestep);
			_horizontal = UnityObjectMapper.Instance.Map<float>(payload);
			horizontalInterpolation.current = _horizontal;
			horizontalInterpolation.target = _horizontal;
			RunChange_horizontal(timestep);
			_isMoving = UnityObjectMapper.Instance.Map<bool>(payload);
			isMovingInterpolation.current = _isMoving;
			isMovingInterpolation.target = _isMoving;
			RunChange_isMoving(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _spineRotation);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _vertical);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _horizontal);
			if ((0x20 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _isMoving);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

            string stringData = System.Text.Encoding.UTF8.GetString(dirtyFieldsData.CompressBytes());
            PTK.ObservableAusuz.SendPlayerModel(stringData)
                .Subscribe(result =>
                {
                    ///Debug.Log("receive player model data");
                },
                    e => Debug.Log(e)
                );

            return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (rotationInterpolation.Enabled)
				{
					rotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					rotationInterpolation.Timestep = timestep;
				}
				else
				{
					_rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (spineRotationInterpolation.Enabled)
				{
					spineRotationInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					spineRotationInterpolation.Timestep = timestep;
				}
				else
				{
					_spineRotation = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_spineRotation(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (verticalInterpolation.Enabled)
				{
					verticalInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					verticalInterpolation.Timestep = timestep;
				}
				else
				{
					_vertical = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_vertical(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (horizontalInterpolation.Enabled)
				{
					horizontalInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					horizontalInterpolation.Timestep = timestep;
				}
				else
				{
					_horizontal = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_horizontal(timestep);
				}
			}
			if ((0x20 & readDirtyFlags[0]) != 0)
			{
				if (isMovingInterpolation.Enabled)
				{
					isMovingInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					isMovingInterpolation.Timestep = timestep;
				}
				else
				{
					_isMoving = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_isMoving(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.UnityNear(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector3)positionInterpolation.Interpolate();
				//RunChange_position(positionInterpolation.Timestep);
			}
			if (rotationInterpolation.Enabled && !rotationInterpolation.current.UnityNear(rotationInterpolation.target, 0.0015f))
			{
				_rotation = (Quaternion)rotationInterpolation.Interpolate();
				//RunChange_rotation(rotationInterpolation.Timestep);
			}
			if (spineRotationInterpolation.Enabled && !spineRotationInterpolation.current.UnityNear(spineRotationInterpolation.target, 0.0015f))
			{
				_spineRotation = (Vector3)spineRotationInterpolation.Interpolate();
				//RunChange_spineRotation(spineRotationInterpolation.Timestep);
			}
			if (verticalInterpolation.Enabled && !verticalInterpolation.current.UnityNear(verticalInterpolation.target, 0.0015f))
			{
				_vertical = (float)verticalInterpolation.Interpolate();
				//RunChange_vertical(verticalInterpolation.Timestep);
			}
			if (horizontalInterpolation.Enabled && !horizontalInterpolation.current.UnityNear(horizontalInterpolation.target, 0.0015f))
			{
				_horizontal = (float)horizontalInterpolation.Interpolate();
				//RunChange_horizontal(horizontalInterpolation.Timestep);
			}
			if (isMovingInterpolation.Enabled && !isMovingInterpolation.current.UnityNear(isMovingInterpolation.target, 0.0015f))
			{
				_isMoving = (bool)isMovingInterpolation.Interpolate();
				//RunChange_isMoving(isMovingInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerNetworkObject() : base() { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
