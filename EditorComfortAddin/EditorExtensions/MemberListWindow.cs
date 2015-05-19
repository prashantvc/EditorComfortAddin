using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components;
using Gdk;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;
using Mono.TextEditor.PopupWindow;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide;
using Xwt;
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.CSharp;
using Mono.CSharp.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace TwinTechs.EditorExtensions
{

	public class MemberListWindow : Gtk.Dialog
	{
		Gtk.TreeView _treeView;
		Gtk.Entry _searchView;
		Gtk.ListStore _listStore;
		bool keyHandled = false;
		Collection<IUnresolvedEntity> _filteredEntities;
		IUnresolvedEntity _selectedEntry;

		int _selectedIndex;

		public MemberListWindow (string title, Gtk.Window parent, DialogFlags flags, params object[] button_data) : base (title, parent, flags, button_data)
		{
			if (IdeApp.Workbench == null) {
				return;
			}
			var rootWindow = IdeApp.Workbench.RootWindow;
			_searchView = new Gtk.Entry ("");
			_searchView.SetSizeRequest (500, 40);
			_searchView.Changed += _searchView_Changed;
			_searchView.KeyReleaseEvent += HandleSearchViewKeyReleaseEvent;
			_searchView.KeyPressEvent += HandleKeyPressEvent;
			_searchView.FocusOutEvent += HandleFocusOutEvent;
			VBox.Add (_searchView);

			CreateTree ();
			VBox.Add (_treeView);

			MemberExtensionsHelper.Instance.IsDirty = true;
			UpdateMembers ();
			var editor = IdeApp.Workbench.ActiveDocument.Editor;
			var visualInsertLocation = editor.LogicalToVisualLocation (editor.Caret.Location);
			var targetView = IdeApp.Workbench.RootWindow;

			this.SetSizeRequest (500, 700);

			CanFocus = true;
			_searchView.CanFocus = true;
			_searchView.IsEditable = true;
			_searchView.GrabFocus ();
			ShowAll ();

		}

		#region key events

		void HandleKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			keyHandled = false;

			var keyChar = (char)args.Event.Key;
			var keyValue = args.Event.KeyValue;
			var modifier = args.Event.State;
			var key = args.Event.Key;

			var processResult = PreProcessKey (key, keyChar, modifier);
			if ((processResult & KeyActions.CloseWindow) != 0) {
				var moveToLocation = (processResult & KeyActions.CloseWindow) != 0;
				CloseWindow (moveToLocation);
			}
			var handled = (processResult & KeyActions.Process) != 0;
			handled = true;
			args.RetVal = keyHandled = handled;
		}

		void HandleSearchViewKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			switch (args.Event.Key) {
			case Gdk.Key.Return:
			case Gdk.Key.ISO_Enter:
			case Gdk.Key.Key_3270_Enter:
			case Gdk.Key.KP_Enter:
				CloseWindow (true);
				break;
			default:
				break;
			}

			args.RetVal = keyHandled = true;
		}

		void HandleKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			args.RetVal = keyHandled = true;
		}

		void HandleFocusOutEvent (object o, FocusOutEventArgs args)
		{
			_searchView.GrabFocus ();
		}

		TreeIter GetTreeIterForRow (int index)
		{
			TreeIter iter;

			_listStore.GetIter (out iter, new TreePath (new int[]{ index }));
			return iter;
		}

		public KeyActions PreProcessKey (Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			bool isSelected;

			switch (key) {
			case Gdk.Key.Home:
				if ((modifier & ModifierType.ShiftMask) == ModifierType.ShiftMask)
					return KeyActions.Process;
				//TODO
				return KeyActions.Ignore;
			case Gdk.Key.End:
				if ((modifier & ModifierType.ShiftMask) == ModifierType.ShiftMask)
					return KeyActions.Process;
				//TODO
				return KeyActions.Ignore;

			case Gdk.Key.Up:
				if (_selectedIndex - 1 >= 0) {
					SelectRowIndex (_selectedIndex - 1);
				}
				return KeyActions.Ignore;

			case Gdk.Key.Tab:
				//tab always completes current item even if selection is disabled
				goto case Gdk.Key.Return;
//
			case Gdk.Key.Return:
			case Gdk.Key.ISO_Enter:
			case Gdk.Key.Key_3270_Enter:
			case Gdk.Key.KP_Enter:
				if (_selectedEntry != null) {
					return KeyActions.Complete | KeyActions.Ignore | KeyActions.CloseWindow;
				}
				return KeyActions.Ignore;
			case Gdk.Key.Escape:
				_selectedEntry = null;
				return KeyActions.Ignore | KeyActions.CloseWindow;
			case Gdk.Key.Down:
				if (_selectedIndex + 1 < _filteredEntities.Count) {
					SelectRowIndex (_selectedIndex + 1);
				}
				return KeyActions.Ignore;

			case Gdk.Key.Page_Up:
				if ((modifier & ModifierType.ShiftMask) == ModifierType.ShiftMask)
					return KeyActions.Process;
				//TODO
				return KeyActions.Ignore;

			case Gdk.Key.Page_Down:
				if ((modifier & ModifierType.ShiftMask) == ModifierType.ShiftMask)
					return KeyActions.Process;
				//TODO
				return KeyActions.Ignore;

			case Gdk.Key.Left:
				//if (curPos == 0) return KeyActions.CloseWindow | KeyActions.Process;
				//curPos--;
				return KeyActions.Process;

			case Gdk.Key.Right:
				//if (curPos == word.Length) return KeyActions.CloseWindow | KeyActions.Process;
				//curPos++;
				return KeyActions.Process;

			case Gdk.Key.Caps_Lock:
			case Gdk.Key.Num_Lock:
			case Gdk.Key.Scroll_Lock:
				return KeyActions.Ignore;

			case Gdk.Key.Control_L:
			case Gdk.Key.Control_R:
			case Gdk.Key.Alt_L:
			case Gdk.Key.Alt_R:
			case Gdk.Key.Shift_L:
			case Gdk.Key.Shift_R:
			case Gdk.Key.ISO_Level3_Shift:
				// AltGr
				return KeyActions.Process;
			default:
				return KeyActions.Ignore;
			}
			if (keyChar == '\0')
				return KeyActions.Process;

			if (keyChar == ' ' && (modifier & ModifierType.ShiftMask) == ModifierType.ShiftMask)
				return KeyActions.CloseWindow | KeyActions.Process;

			// special case end with punctuation like 'param:' -> don't input double punctuation, otherwise we would end up with 'param::'
			if (char.IsPunctuation (keyChar) && keyChar != '_') {
				return KeyActions.Ignore;
			}
			return KeyActions.Process;
		}


		void _searchView_Changed (object sender, EventArgs e)
		{
			UpdateMembers ();
		}

		#endregion


		#region private impl

		void SelectRowIndex (int index)
		{
//			Console.WriteLine ("SelectRowIndex " + index);
			_treeView.Selection.SelectIter (GetTreeIterForRow (index));
		}

		void CreateTree ()
		{
			_treeView = new Gtk.TreeView ();
			_treeView.SetSizeRequest (500, 600);

			//type
			var typeColumn = new Gtk.TreeViewColumn ();
			typeColumn.Title = "Type";
			typeColumn.MaxWidth = 50;
			// Do the same for the song title column
			var typeCell = new Gtk.CellRendererText ();
			typeColumn.PackStart (typeCell, true);

			//name
			var nameColumn = new Gtk.TreeViewColumn ();
			nameColumn.Title = "Name";
			nameColumn.MaxWidth = 300;
			var nameCell = new Gtk.CellRendererText ();
			nameColumn.PackStart (nameCell, true);

			//info
			var infoColumn = new Gtk.TreeViewColumn ();
			infoColumn.Title = "Info";
			infoColumn.MaxWidth = 150;
			var infoCell = new Gtk.CellRendererText ();
			infoColumn.PackStart (infoCell, true);



			// Add the columns to the TreeView
			_treeView.AppendColumn (typeColumn);
			_treeView.AppendColumn (nameColumn);
			_treeView.AppendColumn (infoColumn);

			typeColumn.AddAttribute (typeCell, "text", 0);
			nameColumn.AddAttribute (nameCell, "text", 1);
			infoColumn.AddAttribute (infoCell, "text", 2);

			_treeView.Selection.Mode = Gtk.SelectionMode.Single;
			_treeView.Selection.Changed += _treeView_Selection_Changed;


			_treeView.KeyPressEvent += HandleKeyPressEvent;
			_treeView.KeyReleaseEvent += HandleKeyReleaseEvent;
			_treeView.ButtonReleaseEvent += (o, args) => {
				if (_selectedEntry != null) {
					CloseWindow (true);
				}
			};

		}

		void _treeView_Selection_Changed (object sender, EventArgs e)
		{
			var selectedRows = _treeView.Selection.GetSelectedRows ();
			if (selectedRows != null && selectedRows.Length > 0) {
				var row = selectedRows [0];
				var index = row.Indices [0];
				_selectedEntry = _filteredEntities [index];
				_selectedIndex = index;
			}
			_searchView.GrabFocus ();
			_searchView.SelectRegion (_searchView.Text.Length, _searchView.Text.Length);
		}


		void CloseWindow (bool moveToSelectedEntry = false)
		{
			if (_selectedEntry != null) {
				MemberExtensionsHelper.Instance.GotoMember (_selectedEntry);
			}

			Gtk.Application.Invoke ((s, ev) => DestroyWindow ());
		}

		void DestroyWindow ()
		{
			_searchView.KeyReleaseEvent -= HandleSearchViewKeyReleaseEvent;
			_searchView.KeyPressEvent -= HandleKeyPressEvent;
			_searchView.FocusOutEvent -= HandleFocusOutEvent;

			Visible = false;
			Destroy ();
		}

		void UpdateMembers ()
		{
			_listStore = new Gtk.ListStore (typeof(string), typeof(string), typeof(string));
			var entities = MemberExtensionsHelper.Instance.GetEntities (); //TODO filter
			var filterText = _searchView.Text;
			if (string.IsNullOrEmpty (filterText)) {
				_filteredEntities = entities;
			} else {
				
				var filteredEntities = entities.Where (e => {
					return MemberMatchingHelper.GetMatchWithSearchText (filterText, e.Name);
				}).ToList ();
				_filteredEntities = new Collection<IUnresolvedEntity> (filteredEntities);
			}
			foreach (var entity in _filteredEntities) {
				//TODO improve this
				var info = "";
				var method = entity as DefaultUnresolvedMethod;
				if (method != null) {
					
					var typeInfo = method.Parameters.Select ((arg) => {
						return arg.Name + ":" + arg.Type.ToString ();
					}
					               );
					info = "(" + String.Join (" , ", typeInfo) + " )";
				}
				_listStore.AppendValues (entity.SymbolKind.ToString ().Substring (0, 1), entity.Name, info);
			}
			_treeView.Model = _listStore;

			if (_selectedEntry != null) {
				var newIndex = _filteredEntities.IndexOf (_selectedEntry);
				SelectRowIndex (newIndex == -1 ? 0 : newIndex);
			} else {
				SelectRowIndex (0);
			}
		}

		#endregion

	}
}

