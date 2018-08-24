using GameLogic.Core;
using GameLogic.Utilities.ScriptSystem;

namespace GameLogic.Utilities {
	public interface IIdentifiable : IDescribable, IJSContextProvider {
		string ID { get; }
	}

	public abstract class AutogenIdentifiable : IIdentifiable {
		private static ulong _autoIncrement = 0L;

		private readonly ulong _thisNumber;

		protected abstract string BaseID { get; }

		public string ID => this.BaseID + "_" + _thisNumber;

		public abstract string Name { get; set; }
		public abstract string Description { get; set; }

		public AutogenIdentifiable() {
			_thisNumber = _autoIncrement++;
		}

		public abstract IJSContext GetContext();
		public abstract void SetContext(IJSContext context);
	}
}
