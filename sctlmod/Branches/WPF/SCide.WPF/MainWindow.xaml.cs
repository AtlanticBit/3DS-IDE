using System;
using System.Linq;
using System.Windows;
using ScintillaNET;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Windows.Input;

namespace SCide.WPF
{
	public partial class MainWindow : Window
	{
		private const string NEW_DOCUMENT_TEXT = "Untitled";
		private const int LINE_NUMBERS_MARGIN_WIDTH = 35; // TODO Don't hardcode this

		#region Commands
		public static RoutedCommand NewFileCommand = new RoutedCommand();
		public static RoutedCommand OpenFileCommand = new RoutedCommand();
		public static RoutedCommand SaveFileCommand = new RoutedCommand();
		public static RoutedCommand SaveAllFilesCommand = new RoutedCommand();
		public static RoutedCommand PrintFileCommand = new RoutedCommand();
		public static RoutedCommand UndoCommand = new RoutedCommand();
		public static RoutedCommand RedoCommand = new RoutedCommand();
		public static RoutedCommand CutCommand = new RoutedCommand();
		public static RoutedCommand CopyCommand = new RoutedCommand();
		public static RoutedCommand PasteCommand = new RoutedCommand();
		public static RoutedCommand SelectAllCommand = new RoutedCommand();
		public static RoutedCommand FindCommand = new RoutedCommand();
		public static RoutedCommand ReplaceCommand = new RoutedCommand();
		#endregion

		public MainWindow()
		{
			InitializeComponent();
			// Why this has to be here, I have no idea.
			// All I know is that it doesn't work properly 
			// if put in the xaml file.
			lineNumbersMenuItem.IsChecked = true;
		}

		private static Visibility Toggle(Visibility v)
		{
			if (v == Visibility.Visible)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}

		public IEnumerable<DocumentForm> Documents
		{
			get { return documentsRoot.Children.Cast<DocumentForm>(); }
		}

		private int _newDocumentCount = 0;
		private DocumentForm NewDocument()
		{
			DocumentForm doc = new DocumentForm();
			SetScintillaToCurrentOptions(doc);
			doc.Title = String.Format(CultureInfo.CurrentCulture, "{0}{1}", NEW_DOCUMENT_TEXT, ++_newDocumentCount);
			documentsRoot.Children.Add(doc);
			doc.DockAsDocument();
			doc.IsActive = true;
			incrementalSearcher.Scintilla = doc.Scintilla;

			return doc;
		}

		private void OpenFile()
		{
			bool? res = openFileDialog.ShowDialog();
			if (res == null || !(bool)res)
				return;

			foreach (string filePath in openFileDialog.FileNames)
			{
				// Ensure this file isn't already open
				bool isOpen = false;
				foreach (DocumentForm documentForm in Documents)
				{
					if (filePath.Equals(documentForm.FilePath, StringComparison.OrdinalIgnoreCase))
					{
						documentForm.IsActive = true;
						isOpen = true;
						break;
					}
				}

				// Open the files
				if (!isOpen)
					OpenFile(filePath);
			}
		}

		private DocumentForm OpenFile(string filePath)
		{
			DocumentForm doc = new DocumentForm();
			SetScintillaToCurrentOptions(doc);
			doc.Scintilla.Text = File.ReadAllText(filePath);
			doc.Scintilla.UndoRedo.EmptyUndoBuffer();
			doc.Scintilla.Modified = false;
			doc.Title = Path.GetFileName(filePath);
			doc.FilePath = filePath;
			documentsRoot.Children.Add(doc);
			doc.DockAsDocument();
			doc.IsActive = true;
			incrementalSearcher.Scintilla = doc.Scintilla;

			return doc;
		}

		private void SetScintillaToCurrentOptions(DocumentForm doc)
		{
			// Turn on line numbers?
			if (lineNumbersMenuItem.IsChecked)
				doc.Scintilla.Margins.Margin0.Width = LINE_NUMBERS_MARGIN_WIDTH;
			else
				doc.Scintilla.Margins.Margin0.Width = 0;

			// Turn on white space?
			if (whitespaceMenuItem.IsChecked)
				doc.Scintilla.Whitespace.Mode = WhitespaceMode.VisibleAlways;
			else
				doc.Scintilla.Whitespace.Mode = WhitespaceMode.Invisible;

			// Turn on word wrap?
			if (wordWrapMenuItem.IsChecked)
				doc.Scintilla.LineWrapping.Mode = LineWrappingMode.Word;
			else
				doc.Scintilla.LineWrapping.Mode = LineWrappingMode.None;

			// Show EOL?
			doc.Scintilla.EndOfLine.IsVisible = endOfLineMenuItem.IsChecked;

			// Set the zoom
			doc.Scintilla.ZoomFactor = _zoomLevel;
		}


		#region Menus

		// These sections are in the same order
		// they appear in the actual menu.

		#region File
		private void newMenuItem_Click(object sender, RoutedEventArgs e) { NewDocument(); }
		private void openMenuItem_Click(object sender, RoutedEventArgs e) { OpenFile(); }
		private void saveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Save();
		}
		private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.SaveAs();
		}
		private void saveAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			foreach (DocumentForm doc in Documents)
			{
				doc.Save();
			}
		}

		#region Export
		private void exportAsHTMLMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.ExportAsHtml();	
		}
		#endregion

		private void printMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Printing.Print();
		}
		private void printPreviewMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Printing.PrintPreview();
		}
		private void exitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		#endregion

		#region Edit

		#region Undo/Redo
		private void undoMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.UndoRedo.Undo();
		}
		private void redoMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.UndoRedo.Redo();
		}
		#endregion

		#region Cut/Copy/Paste
		private void cutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Clipboard.Cut();
		}
		private void copyMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Clipboard.Copy();
		}
		private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Clipboard.Paste();
		}
		#endregion
		
		private void selectAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Selection.SelectAll();
		}
		
		#region Find and Replace
		private void findMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.FindReplace.ShowFind(); }
		private void replaceMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.FindReplace.ShowReplace(); }
		private void findInFilesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Coming someday...
		}
		private void replaceInFilesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//	Coming someday...
		}
		#endregion
		
		private void gotoMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.GoTo.ShowGoToDialog(); }

		#region Bookmarks
		private void toggleBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Line currentLine = ActiveDocument.Scintilla.Lines.Current;
			if (ActiveDocument.Scintilla.Markers.GetMarkerMask(currentLine) == 0)
			{
				currentLine.AddMarker(0);
			}
			else
			{
				currentLine.DeleteMarker(0);
			}
		}
		private void previousBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//	 I've got to redo this whole FindNextMarker/FindPreviousMarker Scheme
			Line l = ActiveDocument.Scintilla.Lines.Current.FindPreviousMarker(1);
			if (l != null)
				l.Goto();
		}
		private void nextBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//	 I've got to redo this whole FindNextMarker/FindPreviousMarker Scheme
			Line l = ActiveDocument.Scintilla.Lines.Current.FindNextMarker(1);
			if (l != null)
				l.Goto();
		}
		private void clearBookmarksMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Markers.DeleteAll(0); }
		#endregion
		
		#region Drop Markers
		private void dropMarkerMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.DropMarkers.Drop(); }
		private void collectMarkerMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.DropMarkers.Collect(); }
		#endregion

		#region Advanced

		private void makeUpperCaseMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Commands.Execute(BindableCommand.UpperCase); }
		private void makeLowerCaseMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LowerCase); }

		#region Comment/Uncomment
		private void commentStreamMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Commands.Execute(BindableCommand.StreamComment); }
		private void commentLineMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LineComment); }
		private void uncommentMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LineUncomment); }
		#endregion

		#endregion

		private void autocompleteMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.AutoComplete.Show(); }

		#region Snippets
		private void insertSnippetMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Snippets.ShowSnippetList(); }
		private void surroundWithSnippetMenuItem_Click(object sender, RoutedEventArgs e) { ActiveDocument.Scintilla.Snippets.ShowSurroundWithList(); }
		#endregion

		#endregion

		#region View

		private void toolbarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle the visibility of the tool bar
			toolStrip.Visibility = Toggle(toolStrip.Visibility);
			toolbarMenuItem.IsChecked = toolStrip.Visibility == Visibility.Visible;
		}
		private void statusBarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle the visibility of the status strip
			statusStrip.Visibility = Toggle(statusStrip.Visibility);
			statusBarMenuItem.IsChecked = statusStrip.Visibility == Visibility.Visible;
		}

		#region Control Character Visibility
		private void whitespaceMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle the whitespace mode for all open files
			whitespaceMenuItem.IsChecked = !whitespaceMenuItem.IsChecked;
			foreach (DocumentForm doc in Documents)
			{
				if (whitespaceMenuItem.IsChecked)
					doc.Scintilla.Whitespace.Mode = WhitespaceMode.VisibleAlways;
				else
					doc.Scintilla.Whitespace.Mode = WhitespaceMode.Invisible;
			}
		}
		private void wordWrapMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle word wrap for all open files
			wordWrapMenuItem.IsChecked = !wordWrapMenuItem.IsChecked;
			foreach (DocumentForm doc in Documents)
			{
				if (wordWrapMenuItem.IsChecked)
					doc.Scintilla.LineWrapping.Mode = LineWrappingMode.Word;
				else
					doc.Scintilla.LineWrapping.Mode = LineWrappingMode.None;
			}
		}
		private void endOfLineMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle EOL visibility for all open files
			endOfLineMenuItem.IsChecked = !endOfLineMenuItem.IsChecked;
			foreach (DocumentForm doc in Documents)
			{
				doc.Scintilla.EndOfLine.IsVisible = endOfLineMenuItem.IsChecked;
			}	
		}
		#endregion

		#region Zoom
		private int _zoomLevel = 0;
		private void UpdateAllScintillaZoom()
		{
			// Update zoom level for all files
			foreach (DocumentForm doc in dockPanel.DocumentsSource)
				doc.Scintilla.ZoomFactor = _zoomLevel;
		}
		private void zoomInMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Increase the zoom for all open files
			_zoomLevel++;
			UpdateAllScintillaZoom();
		}
		private void zoomOutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			_zoomLevel--;
			UpdateAllScintillaZoom();
		}
		private void resetZoomMenuItem_Click(object sender, RoutedEventArgs e)
		{
			_zoomLevel = 0;
			UpdateAllScintillaZoom();
		}
		#endregion

		private void lineNumbersMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Toggle the line numbers margin for all documents
			lineNumbersMenuItem.IsChecked = !lineNumbersMenuItem.IsChecked;
			foreach (DocumentForm docForm in Documents)
			{
				if (lineNumbersMenuItem.IsChecked)
					docForm.Scintilla.Margins.Margin0.Width = LINE_NUMBERS_MARGIN_WIDTH;
				else
					docForm.Scintilla.Margins.Margin0.Width = 0;
			}
		}

		#region Folding
		private void foldLevelMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Lines.Current.FoldExpanded = true;	
		}
		private void unfoldLevelMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.Lines.Current.FoldExpanded = false;
		}
		private void foldAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
			{
				foreach (Line l in ActiveDocument.Scintilla.Lines)
				{
					l.FoldExpanded = true;
				}
			}
		}
		private void unfoldAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
			{
				foreach (Line l in ActiveDocument.Scintilla.Lines)
				{
					l.FoldExpanded = true;
				}
			}
		}
		#endregion

		#region Navigation
		private void navigateForwardMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.DocumentNavigation.NavigateForward();	
		}
		private void navigateBackwardMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Scintilla.DocumentNavigation.NavigateBackward();
		}
		#endregion

		#endregion

		#region Language

		private void csLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("cs"); }
		private void htmlLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("html"); }
		private void plainTextLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage(String.Empty); }
		private void pythonLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("python"); }
		private void mssqlLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("mssql"); }
		private void vbScriptLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("vbscript"); }
		private void xmlLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("xml"); }
		private void iniLanguageMenuItem_Click(object sender, RoutedEventArgs e) { SetLanguage("ini"); }

		private void SetLanguage(string language)
		{
			// Use a built-in lexer and configuration
			ActiveDocument.Scintilla.ConfigurationManager.Language = language;

			// Smart indenting...
			if ("cs".Equals(language, StringComparison.OrdinalIgnoreCase))
				ActiveDocument.Scintilla.Indentation.SmartIndentType = SmartIndent.CPP;
			else
				ActiveDocument.Scintilla.Indentation.SmartIndentType = SmartIndent.None;
		}

		#endregion

		#region Window

		private void bookmarkWindowMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// These currently are hidden.
		}
		private void findResultsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
		}
		private void closeWindowMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDocument != null)
				ActiveDocument.Close();
		}
		private void closeAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
		}

		#endregion

		#region Help
		
		private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		#endregion

		#endregion

		public DocumentForm ActiveDocument
		{
			get { return documentsRoot.Children.Where(c => c.Content == dockPanel.ActiveContent).FirstOrDefault() as DocumentForm; }
		}

		private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
		{
			// Update the main form _text to show the current document
			if (ActiveDocument != null)
			{
				this.Title = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", ActiveDocument.Title, Program.Title);
				incrementalSearcher.Scintilla = ActiveDocument.Scintilla;
			}
			else
				this.Title = Program.Title;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NewDocument();
		}
	}
}
