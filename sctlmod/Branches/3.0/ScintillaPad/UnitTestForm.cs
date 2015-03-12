#region Using Directives

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNet;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

#endregion Using Directives


namespace ScintillaPad
{
	// Rather than use a testing framework like NUnit or even Visual Studio
	// with their custom attributes and runners, this is just a simple unit
	// test than anyone can understand and contribute to.
	//
	// Naturally, tests should be ordered so that they don't depend
	// on functions that haven't previously been tested.
	internal partial class UnitTestForm : Form
	{
		#region Constants

		private const string QUICK_BROWN_FOX = "The quick brown fox jumps over the lazy dog";
		private const string HELLO_WORLD = "Hello World!";
		private const string PEACE_BE_UPON_YOU = "السلام عليكم";
		private const string PLETHORA = "πλείων";

		#endregion Constants


		#region Fields

		private Scintilla _scintilla;
		private bool _eventFired;
		private volatile bool _stop;

		#endregion Fields


		#region Methods

		private void AddTest(string name, TestMethod method)
		{
			Test t = new Test();
			t.Name = name;
			t.Method = method;

			ListViewItem lvi = new ListViewItem();
			lvi.UseItemStyleForSubItems = false;
			lvi.Text = t.Name;
			lvi.Tag = t;
			lvi.SubItems.Add("Ready");

			listView.Items.Add(lvi);
		}


		private void AssertEvent(MethodInvoker method)
		{
			_eventFired = false;
			method.Invoke();
			if (!_eventFired)
				throw new AssertFailedException();
		}


		private static void AssertIsTrue(bool condition)
		{
			if (!condition)
				throw new AssertFailedException();
		}


		private static void AssertThrows(Type type, MethodInvoker method)
		{
			try
			{
				method.Invoke();
			}
			catch (Exception ex)
			{
				if (ex.GetType() != type)
					throw new AssertFailedException();
			}
		}


		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDetails();
		}


		private void resetButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvi in listView.Items)
			{
				lvi.SubItems[1].Text = "Ready";
				lvi.SubItems[1].ResetStyle();
				Test t = (Test)lvi.Tag;
				t.Exception = null;
			}

			UpdateDetails();
		}


		private void SendKey(char ch, bool doEvents)
		{
			//short vk = Win32.VkKeyScan(ch);
			//if (!Win32.PostMessage(_scintilla.Handle, Win32.WM_KEYDOWN, (IntPtr)vk, IntPtr.Zero))
			//    throw new Win32Exception(Marshal.GetLastWin32Error());

			if (!NativeMethods.PostMessage(_scintilla.Handle, NativeMethods.WM_CHAR, (IntPtr)ch, IntPtr.Zero))
				throw new Win32Exception(Marshal.GetLastWin32Error());

			if (doEvents)
				Application.DoEvents();
		}


		private void startButton_Click(object sender, EventArgs e)
		{
			startButton.Enabled = false;
			resetButton.Enabled = false;
			stopButton.Enabled = true;
			_stop = false;

			bool failed = false;
			foreach (ListViewItem lvi in listView.Items)
			{
				// Ya, I know what you're thinking,
				// but this is just a test
				Application.DoEvents();
				if (_stop)
					break;

				// Skip already run items
				if (lvi.SubItems[1].Text != "Ready")
					continue;

				Test t = (Test)lvi.Tag;

				try
				{
					t.Method.Invoke();
					lvi.SubItems[1].Text = "Pass";
					lvi.SubItems[1].ForeColor = Color.Green;
				}
				catch (AssertFailedException afe)
				{
					failed = true;
					t.Exception = afe;
					lvi.SubItems[1].Text = "Fail";
					lvi.SubItems[1].ForeColor = Color.Red;
				}
				catch (Exception ex)
				{
					failed = true;
					t.Exception = ex;
					lvi.SubItems[1].Text = "Error";
					lvi.SubItems[1].ForeColor = Color.Red;
				}
			}


			if (failed && !_stop)
				MessageBox.Show(this, "One or more tests did not pass.", Util.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

			_stop = false;
			stopButton.Enabled = false;
			resetButton.Enabled = true;
			startButton.Enabled = true;

			UpdateDetails();
		}

		private void stopButton_Click(object sender, EventArgs e)
		{
			_stop = true;
		}


		//private void TestDeletingChars()
		//{
		//    // This test must be run after TestInsertingChars
		//    for (int i = 0; i < QUICK_BROWN_FOX.Length; i++)
		//        SendKey('\b', false);
		//    Application.DoEvents();
		//    AssertIsTrue(_scintilla.Text.Length == 0);
		//}


		private void TestAppendTextMethod()
		{
			_scintilla.Text = null;
			_scintilla.AppendText(QUICK_BROWN_FOX);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX);

			_scintilla.AppendText(PEACE_BE_UPON_YOU);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX + PEACE_BE_UPON_YOU);
		}


		private void TestDeleteTextMethods()
		{
			_scintilla.Text = QUICK_BROWN_FOX + PLETHORA;
			_scintilla.DeleteText(0, 3);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX.Substring(3) + PLETHORA);

			_scintilla.DeleteText(new Range(_scintilla.Text.Length - PLETHORA.Length, _scintilla.TextLength));
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX.Substring(3));

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.DeleteText(-1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.DeleteText(1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.DeleteText(0, _scintilla.TextLength + 1));

			AssertThrows(typeof(ArgumentException), () => _scintilla.DeleteText(new Range(-1, 0)));
			AssertThrows(typeof(ArgumentException), () => _scintilla.DeleteText(new Range(1, 0)));
			AssertThrows(typeof(ArgumentException), () => _scintilla.DeleteText(new Range(0, _scintilla.TextLength + 1)));
		}


		private void TestEncodingProperty()
		{
			// Assumes it hasn't already been changed
			AssertIsTrue(_scintilla.Encoding is UTF8Encoding);

			_scintilla.Encoding = Encoding.ASCII;
			AssertIsTrue(_scintilla.Encoding is ASCIIEncoding);

			_scintilla.Encoding = Encoding.GetEncoding(936);
			AssertIsTrue(_scintilla.Encoding.CodePage == 936);

			AssertThrows(typeof(ArgumentNullException), () => _scintilla.Encoding = null);
			AssertThrows(typeof(ArgumentException), () => _scintilla.Encoding = Encoding.UTF32);

			_scintilla.Encoding = Encoding.UTF8;
			AssertIsTrue(_scintilla.Encoding is UTF8Encoding);
		}


		private void TestFindMethod()
		{
			Range range = new Range();

			_scintilla.Text = HELLO_WORLD + PEACE_BE_UPON_YOU + QUICK_BROWN_FOX;
			range = _scintilla.Find("Fox", FindFlags.None, 0, _scintilla.TextLength, true);
			AssertIsTrue(range.StartIndex == _scintilla.Text.IndexOf("fox"));
			AssertIsTrue(range.Length == 3);
			AssertIsTrue(_scintilla.SelectedText == "fox");
			
			range = _scintilla.Find("Fox", FindFlags.MatchCase, 0, _scintilla.TextLength, false);
			AssertIsTrue(range.IsEmpty);
			AssertIsTrue(_scintilla.SelectedText == "fox"); // From previous find



			//AssertThrows(typeof(ArgumentOutOfRangeException), 
		}


		private void TestGetTextMethods()
		{
			string text = null;

			_scintilla.Text = null;
			text = _scintilla.GetText(0, _scintilla.TextLength);
			AssertIsTrue(text == String.Empty);

			_scintilla.Text = HELLO_WORLD + PEACE_BE_UPON_YOU + PLETHORA;
			text = _scintilla.GetText(HELLO_WORLD.Length, _scintilla.TextLength);
			AssertIsTrue(text == PEACE_BE_UPON_YOU + PLETHORA);

			text = _scintilla.GetText(HELLO_WORLD.Length, _scintilla.TextLength - PLETHORA.Length);
			AssertIsTrue(text == PEACE_BE_UPON_YOU);

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.GetText(-1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.GetText(1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.GetText(0, _scintilla.TextLength + 1));

			AssertThrows(typeof(ArgumentException), () => _scintilla.GetText(new Range(-1, 0)));
			AssertThrows(typeof(ArgumentException), () => _scintilla.GetText(new Range(1, 0)));
			AssertThrows(typeof(ArgumentException), () => _scintilla.GetText(new Range(0, _scintilla.TextLength + 1)));
		}


		private void TestHideSelectionProperty()
		{
			// Assumes default
			AssertIsTrue(_scintilla.HideSelection == false);

			_scintilla.HideSelection = true;
			AssertIsTrue(_scintilla.HideSelection == true);

			AssertEvent(() => _scintilla.HideSelection = false);
			AssertIsTrue(_scintilla.HideSelection == false);
		}


		private void TestInsertingChars()
		{
			_scintilla.Text = null;
			foreach (char c in QUICK_BROWN_FOX)
				SendKey(c, false);
			Application.DoEvents();
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX);
		}


		private void TestInsertTextMethod()
		{
			_scintilla.Text = null;
			_scintilla.InsertText(0, "World");
			AssertIsTrue(_scintilla.Text == "World");

			_scintilla.InsertText(1, null);
			_scintilla.InsertText(2, String.Empty);
			AssertIsTrue(_scintilla.Text == "World");

			_scintilla.InsertText(0, "Hello");
			AssertIsTrue(_scintilla.Text == "HelloWorld");

			_scintilla.InsertText(5, PLETHORA);
			AssertIsTrue(_scintilla.Text == "Hello" + PLETHORA + "World");

			_scintilla.InsertText(_scintilla.TextLength, "!");
			_scintilla.InsertText(_scintilla.TextLength - 6, " ");
			_scintilla.InsertText(5, " ");
			AssertIsTrue(_scintilla.Text == "Hello " + PLETHORA + " World!");

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.InsertText(-1, null));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.InsertText(_scintilla.TextLength + 1, null));
		}


		private void TestRangeStructure()
		{
			Range r = new Range();
			AssertIsTrue(r.StartIndex == 0);
			AssertIsTrue(r.EndIndex == 0);
			AssertIsTrue(r.Length == 0);
			AssertIsTrue(r.IsEmpty);

			r = new Range(10, 15);
			AssertIsTrue(r.StartIndex == 10);
			AssertIsTrue(r.EndIndex == 15);
			AssertIsTrue(r.Length == 5);
			AssertIsTrue(!r.IsEmpty);

			r.StartIndex += 5;
			r.EndIndex += 5;
			AssertIsTrue(r.StartIndex == 15);
			AssertIsTrue(r.EndIndex == 20);
			AssertIsTrue(r.Length == 5);
			AssertIsTrue(!r.IsEmpty);

			Range r2 = new Range(15, 20);
			AssertIsTrue(r2.Equals(r));
			AssertIsTrue(r2 == r);

			r.StartIndex = 0;
			r.EndIndex = 0;
			AssertIsTrue(!r.Equals(r2));
			AssertIsTrue(r != r2);
			AssertIsTrue(r == Range.Empty);
		}


		private void TestReadOnlyProperty()
		{
			_scintilla.ReadOnly = false;

			_scintilla.Text = QUICK_BROWN_FOX;
			AssertEvent(() => _scintilla.ReadOnly = true);
			AssertIsTrue(_scintilla.ReadOnly);

			_scintilla.AppendText(QUICK_BROWN_FOX);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX);

			AssertEvent(() => _scintilla.ReadOnly = false);
			_scintilla.AppendText(QUICK_BROWN_FOX);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX + QUICK_BROWN_FOX);
			AssertIsTrue(_scintilla.ReadOnly == false);
		}


		private void TestReplaceTextMethods()
		{
			_scintilla.Text = QUICK_BROWN_FOX + PLETHORA;
			_scintilla.ReplaceText(_scintilla.Text.Length - PLETHORA.Length, _scintilla.Text.Length, null);
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX);

			_scintilla.ReplaceText(new Range(4, 9), "slow");
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX.Replace("quick", "slow"));

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.ReplaceText(-1, 0, null));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.ReplaceText(1, 0, null));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.ReplaceText(0, _scintilla.TextLength + 1, null));

			AssertThrows(typeof(ArgumentException), () => _scintilla.ReplaceText(new Range(-1, 0), null));
			AssertThrows(typeof(ArgumentException), () => _scintilla.ReplaceText(new Range(1, 0), null));
			AssertThrows(typeof(ArgumentException), () => _scintilla.ReplaceText(new Range(0, _scintilla.TextLength + 1), null));
		}


		private void TestSelectMethodSelectedTextProperty()
		{
			_scintilla.Text = HELLO_WORLD;
			_scintilla.Select(0, 5);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD.Substring(0, 5));

			_scintilla.Select(11, 6);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD.Substring(6, 5));

			_scintilla.Select(0, _scintilla.TextLength);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD);

			_scintilla.AppendText(Environment.NewLine);
			_scintilla.AppendText(HELLO_WORLD);
			_scintilla.Select(HELLO_WORLD.Length + Environment.NewLine.Length, HELLO_WORLD.Length + Environment.NewLine.Length + 5);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD.Substring(0, 5));

			_scintilla.Text = PEACE_BE_UPON_YOU;
			_scintilla.Select(0, 5);
			AssertIsTrue(_scintilla.SelectedText == PEACE_BE_UPON_YOU.Substring(0, 5));

			_scintilla.AppendText(Environment.NewLine);
			_scintilla.AppendText(PEACE_BE_UPON_YOU);
			_scintilla.Select(PEACE_BE_UPON_YOU.Length + Environment.NewLine.Length, PEACE_BE_UPON_YOU.Length + Environment.NewLine.Length + 5);
			AssertIsTrue(_scintilla.SelectedText == PEACE_BE_UPON_YOU.Substring(0, 5));

			_scintilla.Select(0, _scintilla.TextLength);
			AssertIsTrue(_scintilla.SelectedText == _scintilla.Text);

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.Select(-1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.Select(_scintilla.TextLength + 1, 0));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.Select(0, -1));
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.Select(0, _scintilla.TextLength + 1));

			_scintilla.Select(0, 0);
			AssertIsTrue(_scintilla.SelectedText == String.Empty);

			_scintilla.Text = null;
			AssertIsTrue(_scintilla.SelectedText == String.Empty);
		}


		private void TestSelectionBackColorProperty()
		{
			// Assumes default
			AssertIsTrue(_scintilla.SelectionBackColor == SystemColors.Highlight);

			_scintilla.SelectionBackColor = Color.Red;
			AssertIsTrue(_scintilla.SelectionBackColor == Color.Red);

			AssertThrows(typeof(ArgumentException), () => _scintilla.SelectionBackColor = Color.Transparent);

			_scintilla.SelectionBackColor = Color.Empty;
			AssertIsTrue(_scintilla.SelectionBackColor == SystemColors.Highlight);
		}


		private void TestSelectionInactiveBackColorProperty()
		{
			// Assumes default
			AssertIsTrue(_scintilla.SelectionInactiveBackColor == SystemColors.Control);

			_scintilla.SelectionInactiveBackColor = Color.Red;
			AssertIsTrue(_scintilla.SelectionInactiveBackColor == Color.Red);

			AssertThrows(typeof(ArgumentException), () => _scintilla.SelectionInactiveBackColor = Color.Transparent);

			_scintilla.SelectionInactiveBackColor = Color.Empty;
			AssertIsTrue(_scintilla.SelectionInactiveBackColor == SystemColors.Control);
		}


		private void TestSelectionForeColorProperty()
		{
			// Assumes default
			AssertIsTrue(_scintilla.SelectionForeColor == SystemColors.HighlightText);

			_scintilla.SelectionForeColor = Color.Red;
			AssertIsTrue(_scintilla.SelectionForeColor == Color.Red);

			AssertThrows(typeof(ArgumentException), () => _scintilla.SelectionForeColor = Color.Transparent);

			_scintilla.SelectionForeColor = Color.Empty;
			AssertIsTrue(_scintilla.SelectionForeColor == SystemColors.HighlightText);
		}


		private void TestSelectionInactiveForeColorProperty()
		{
			// Assumes default
			AssertIsTrue(_scintilla.SelectionInactiveForeColor == SystemColors.ControlText);

			_scintilla.SelectionInactiveForeColor = Color.Red;
			AssertIsTrue(_scintilla.SelectionInactiveForeColor == Color.Red);

			AssertThrows(typeof(ArgumentException), () => _scintilla.SelectionInactiveForeColor = Color.Transparent);

			_scintilla.SelectionInactiveForeColor = Color.Empty;
			AssertIsTrue(_scintilla.SelectionInactiveForeColor == SystemColors.ControlText);
		}


		private void TestSelectionStartEndProperties()
		{
			_scintilla.Text = HELLO_WORLD;
			_scintilla.Select(0, 0);
			AssertIsTrue(_scintilla.SelectionStart == 0);
			AssertIsTrue(_scintilla.SelectionEnd == 0);

			_scintilla.SelectionStart = 5;
			AssertIsTrue(_scintilla.SelectionStart == 5);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD.Substring(0, 5));

			_scintilla.SelectionStart++;
			_scintilla.SelectionEnd = HELLO_WORLD.Length;
			AssertIsTrue(_scintilla.SelectionEnd == HELLO_WORLD.Length);
			AssertIsTrue(_scintilla.SelectedText == HELLO_WORLD.Substring(6));

			_scintilla.Text = PEACE_BE_UPON_YOU + Environment.NewLine + PEACE_BE_UPON_YOU;
			_scintilla.Select(0, _scintilla.TextLength);
			AssertIsTrue(_scintilla.SelectionEnd == 0);
			AssertIsTrue(_scintilla.SelectionStart == _scintilla.TextLength);

			_scintilla.Select(_scintilla.TextLength, 0);
			AssertIsTrue(_scintilla.SelectionStart == 0);
			AssertIsTrue(_scintilla.SelectionEnd == _scintilla.TextLength);

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.SelectionEnd = -1);
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.SelectionEnd = _scintilla.TextLength + 1);
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.SelectionStart = -1);
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.SelectionStart = _scintilla.TextLength + 1);
		}


		private void TestStyleFontProperty()
		{
			// Assumes default
			//AssertIsTrue(_scintilla.Styles[0].Font == _scintilla.Font);
			AssertIsTrue(_scintilla.Styles[1].Font.Name == "Verdana");

			Font f = new Font("Tahoma", 12f);
			_scintilla.Styles[0].Font = f;
			AssertIsTrue(_scintilla.Styles[0].Font.Name == "Tahoma");
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETBOLD, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETITALIC, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETUNDERLINE, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETCHARACTERSET, IntPtr.Zero, IntPtr.Zero) == (IntPtr)f.GdiCharSet);

			f = new Font("Tahoma", 12f, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point, 2);
			_scintilla.Styles[0].Font = f;
			AssertIsTrue(_scintilla.Styles[0].Font.Name == "Tahoma");
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETBOLD, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETITALIC, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETUNDERLINE, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
			AssertIsTrue(_scintilla.DirectMessage(Constants.SCI_STYLEGETCHARACTERSET, IntPtr.Zero, IntPtr.Zero) == (IntPtr)f.GdiCharSet);

			_scintilla.Styles[0].Font = null;
			AssertIsTrue(_scintilla.Styles[0].Font.Name == "Verdana");

			//_scintilla.Font = null;
		}


		private void TestTextLengthProperty()
		{
			_scintilla.Text = QUICK_BROWN_FOX;
			AssertIsTrue(_scintilla.TextLength == QUICK_BROWN_FOX.Length);

			_scintilla.Text = null;
			AssertIsTrue(_scintilla.TextLength == 0);
		}


		private void TestTextProperty()
		{
			_scintilla.Text = null;
			AssertIsTrue(_scintilla.Text.Length == 0);

			_scintilla.Text = QUICK_BROWN_FOX;
			AssertIsTrue(_scintilla.Text == QUICK_BROWN_FOX);

			_scintilla.Text = String.Empty;
			AssertIsTrue(_scintilla.Text.Length == 0);
		}


		private void TestZoom()
		{
			// Assumes default
			AssertIsTrue(_scintilla.ZoomFactor == 0);

			_scintilla.ZoomFactor++;
			AssertIsTrue(_scintilla.ZoomFactor == 1);

			_scintilla.ZoomFactor -= 2;
			AssertIsTrue(_scintilla.ZoomFactor == -1);

			_scintilla.ZoomIn();
			_scintilla.ZoomIn();
			AssertIsTrue(_scintilla.ZoomFactor == 1);

			_scintilla.ZoomOut();
			_scintilla.ZoomOut();
			AssertIsTrue(_scintilla.ZoomFactor == -1);

			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.ZoomFactor = -100);
			AssertThrows(typeof(ArgumentOutOfRangeException), () => _scintilla.ZoomFactor = 200);

			AssertEvent(() => _scintilla.ZoomFactor = 0);
			AssertEvent(() => _scintilla.ZoomIn());
			AssertEvent(() => _scintilla.ZoomOut());
		}


		private void UpdateDetails()
		{
			// Update the details text box
			if (listView.SelectedItems.Count <= 0)
			{
				textBox.Text = null;
				return;
			}

			using (StringWriter sw = new StringWriter())
			{
				ListViewItem lvi = listView.SelectedItems[0];
				Test t = (Test)lvi.Tag;
				sw.WriteLine("**{0}**", lvi.SubItems[1].Text);
				sw.WriteLine();
				sw.WriteLine("Test: " + t.Name);
				sw.WriteLine("Method: " + t.Method.Method.DeclaringType.FullName + "." + t.Method.Method.Name);
				sw.WriteLine("Exception: " + (t.Exception == null ? "N/A" : t.Exception.ToString()));
				textBox.Text = sw.ToString();
			}
		}


		private void UnitTestForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_stop = true;
		}

		#endregion Methods


		#region Constructors

		public UnitTestForm()
		{
			InitializeComponent();

			// Create a Scintilla control off-screen to use with the tests
			_scintilla = new Scintilla();
			_scintilla.Bounds = new Rectangle(-500, -500, 250, 250);
			_scintilla.HideSelectionChanged += (s, e) => _eventFired = true;
			_scintilla.ZoomFactorChanged += (s, e) => _eventFired = true;
			_scintilla.ReadOnlyChanged += (s, e) => _eventFired = true;
			this.Controls.Add(_scintilla);

			// Add the tests
			AddTest("Encoding Property", new TestMethod(TestEncodingProperty));
			AddTest("Text Property", new TestMethod(TestTextProperty));
			AddTest("GetText Method(s)", new TestMethod(TestGetTextMethods));
			AddTest("AppendText Method", new TestMethod(TestAppendTextMethod));
			AddTest("DeleteText Method(s)", new TestMethod(TestDeleteTextMethods));
			AddTest("InsertText Method", new TestMethod(TestInsertTextMethod));
			AddTest("ReplaceText Method(s)", new TestMethod(TestReplaceTextMethods));
			AddTest("Inserting Characters", new TestMethod(TestInsertingChars));
			AddTest("TextLength Property", new TestMethod(TestTextLengthProperty));
			AddTest("Select Method/SelectedText Property", new TestMethod(TestSelectMethodSelectedTextProperty));
			AddTest("SelectionStart Property/SelectionEnd Property", new TestMethod(TestSelectionStartEndProperties));
			AddTest("SelectionBackColor Property", new TestMethod(TestSelectionBackColorProperty));
			AddTest("SelectionInactiveBackColor Property", new TestMethod(TestSelectionInactiveBackColorProperty));
			AddTest("SelectionForeColor Property", new TestMethod(TestSelectionForeColorProperty));
			AddTest("SelectionInactiveForeColor Property", new TestMethod(TestSelectionInactiveForeColorProperty));
			AddTest("HideSelection Property", new TestMethod(TestHideSelectionProperty));
			AddTest("Style.Font Property", new TestMethod(TestStyleFontProperty));
			AddTest("ZoomIn/ZoomOut Methods/ZoomFactor Property", new TestMethod(TestZoom));
			AddTest("Range Strucure", new TestMethod(TestRangeStructure));
			AddTest("Find Method", new TestMethod(TestFindMethod));
			AddTest("ReadOnly Property, ReadOnlyChanged Event", new TestMethod(TestReadOnlyProperty));

			// Select the first one
			listView.Items[0].Selected = true;
		}

		#endregion Constructors


		#region Types

		internal class AssertFailedException : Exception
		{
		}

		private delegate void TestMethod();

		private class Test
		{
			public string Name { get; set; }
			public TestMethod Method { get; set; }
			public Exception Exception { get; set; }
		}

		#endregion Types
	}
}
