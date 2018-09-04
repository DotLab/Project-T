using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextyClient {
	public partial class AdvancedConsole : UserControl {
		private readonly TextBoxPrinter _textBoxPrinter;
		private Action<string> _callback = null;

		public AdvancedConsole() {
			InitializeComponent();
			_textBoxPrinter = new TextBoxPrinter(outputTbx);
		}

		public TextBoxPrinter TextBoxPrinter => _textBoxPrinter;

		public void SetCommandCallback(Action<string> callback) {
			_callback = callback;
		}

		private void inputTbx_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				string inputText = inputTbx.Text;
				inputTbx.Text = "";
				_textBoxPrinter.WriteLine(inputText);
				if (_callback != null) _callback(inputText);
			}
		}
	}

	public class TextBoxPrinter : TextWriter {
		private readonly TextBox _textBox;

		public override Encoding Encoding => Encoding.Unicode;

		public TextBoxPrinter(TextBox reference) {
			_textBox = reference;
		}

		public override void Write(char value) {
			_textBox.Text += value;
		}

		public override void Write(string value) {
			_textBox.Text += value;
		}
	}

}
