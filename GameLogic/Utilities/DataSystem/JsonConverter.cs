using System;

namespace GameLogic.Utilities.DataSystem {
	public interface IJsonConverterRaw {
		string Serialize(object obj);
		T Deserialize<T>(string json);
	}

	public class JsonException : Exception {
		protected int _errID;
		protected string _errMessage;

		public int ErrID => _errID;
		public string ErrMessage => _errMessage;

		protected static string FullMessage(int errID, string errMessage) {
			return "Json Converting Error (" + errID + "): " + errMessage;
		}

		public JsonException(int errID, string errMessage) :
			base(FullMessage(errID, errMessage)) {
			_errID = errID;
			_errMessage = errMessage;
		}

	}

	public sealed class JsonConverter {
		private readonly IJsonConverterRaw _converter;

		public JsonConverter(IJsonConverterRaw converter) {
			this._converter = converter ?? throw new ArgumentNullException(nameof(converter));
		}

		public string Serialize(object obj) {
			return _converter.Serialize(obj);
		}

		public T Deserialize<T>(string json) {
			return _converter.Deserialize<T>(json);
		}
	}
}
