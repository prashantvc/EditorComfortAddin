using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;

namespace TwinTechs.EditorExtensions
{
	public class ShowMembers : CommandHandler
	{
		protected override void Run ()
		{
			if (IdeApp.Workbench?.ActiveDocument != null) {
				var memberListWindow = new MemberListWindow ("Members", IdeApp.Workbench.RootWindow, Gtk.DialogFlags.Modal);
				memberListWindow.Run ();
			}
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}


	public class NextMember : CommandHandler
	{
		protected override void Run ()
		{
			MemberExtensionsHelper.Instance.GotoNextEntity ();
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}

	public class PreviousMember : CommandHandler
	{
		protected override void Run ()
		{
			MemberExtensionsHelper.Instance.GotoPreviousEntity ();

		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}

	public class ToggleViewModel : CommandHandler
	{
		protected override void Run ()
		{
			MemberExtensionsHelper.Instance.ToggleVMXamlCs ();
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}

	public class ToggleViewModelPreferCodeBehind : CommandHandler
	{
		protected override void Run ()
		{
			MemberExtensionsHelper.Instance.ToggleVMXamlCs (true);
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}

	public class ShowRecentDocumentBrowser : CommandHandler
	{
		protected override void Run ()
		{
			if (IdeApp.Workspace.IsOpen) {
				var recentDocumentsWindow = new RecentFileListWindow ("Browse Recent Documents", IdeApp.Workbench.RootWindow, Gtk.DialogFlags.Modal);
				recentDocumentsWindow.Run ();
			}
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
			info.Visible = true;
		}
	}

}

