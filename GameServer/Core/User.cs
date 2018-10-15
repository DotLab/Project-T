using GameServer.CharacterComponents;
using GameServer.Client;
using GameUtil.Network;
using System;

namespace GameServer.Core {
	public abstract class User : IEquatable<User> {
		protected readonly bool _isDM;
		protected readonly string _id;
		protected readonly string _name;

		public bool IsDM => _isDM;
		public DM AsDM => (DM)this;
		public Player AsPlayer => (Player)this;
		public string ID => _id;
		public string Name => _name;
		public abstract ClientProxy Client { get; }

		protected User(string id, string name, bool isDM) {
			_id = id;
			_name = name;
			_isDM = isDM;
		}

		public void UpdateClientProxy() {
			this.Client.FlushUserInputBuffer();
		}

		public override bool Equals(object obj) {
			return Equals(obj as User);
		}

		public override int GetHashCode() {
			return _id.GetHashCode();
		}

		public bool Equals(User other) {
			return other != null && _id == other._id;
		}
	}

	public sealed class Player : User {
		private readonly PlayerClient _playerClient;
		private readonly Character _character;

		public PlayerClient PlayerClient => _playerClient;
		public Character Character => _character;
		
		public override ClientProxy Client => _playerClient;

		public Player(string id, string name, Connection connection, Character character) :
			base(id, name, false) {
			_character = character ?? throw new ArgumentNullException(nameof(character));
			_playerClient = new PlayerClient(connection, this);
			character.ControlPlayer = this;
		}

	}

	public sealed class DM : User {
		private readonly DMClient _dmClient;

		public DMClient DMClient => _dmClient;
		
		public override ClientProxy Client => _dmClient;

		public DM(string id, string name, Connection connection) :
			base(id, name, true) {
			_dmClient = new DMClient(connection, this);
		}
	}
}
