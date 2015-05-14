using System;
using System.Diagnostics;
using MonoDevelop.Ide;
using Atk;
using MonoDevelop.Ide.Gui;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoDevelop.Projects;

namespace TwinTechs.EditorExtensions
{
	public class FileHistoryHelper
	{
		static FileHistoryHelper _instance;
		Collection<MonoDevelop.Ide.Gui.Document> _recentDocuments = new Collection<MonoDevelop.Ide.Gui.Document> ();
		const int MaxDocuments = 30;

		const string PathDelimeter = "#:#";

		public static FileHistoryHelper Instance {
			get {
				if (_instance == null) {
					_instance = new FileHistoryHelper ();
				}
				return _instance;
			}
		}

		public FileHistoryHelper ()
		{
			IdeApp.Workbench.ActiveDocumentChanged += IdeApp_Workbench_ActiveDocumentChanged;
			IdeApp.Exiting += IdeApp_Exiting;
			IdeApp.Initialized += IdeApp_Initialized;
		}

		void IdeApp_Exiting (object sender, ExitEventArgs args)
		{
			SaveHistory ();
		}

		#region events

		void IdeApp_Initialized (object sender, EventArgs e)
		{
			LoadHistory ();
		}



		void IdeApp_Workbench_ActiveDocumentChanged (object sender, EventArgs e)
		{
			var newDocument = IdeApp.Workbench.ActiveDocument;
			if (_recentDocuments.Contains (newDocument)) {
				_recentDocuments.Remove (newDocument);
				_recentDocuments.Insert (0, newDocument);
			} else {
				_recentDocuments.Insert (0, newDocument);
				if (_recentDocuments.Count > MaxDocuments) {
					_recentDocuments.RemoveAt (_recentDocuments.Count - 1);
				}
			}
		}

		#endregion

		#region public api

		/// <summary>
		/// Gets the recent documents.
		/// </summary>
		/// <returns>The recent documents.</returns>
		public Collection<MonoDevelop.Ide.Gui.Document> GetRecentDocuments ()
		{
			return new Collection<MonoDevelop.Ide.Gui.Document> (_recentDocuments);
		}

		#endregion

		#region private impl

		/// <summary>
		/// Loads the history.
		/// </summary>
		void LoadHistory ()
		{
			try {
				var paths = File.ReadAllLines (GetSavedStatePath ());
				var splitItems = paths.Select ((arg) => arg.Split (new string[] { PathDelimeter }, StringSplitOptions.None));
				//TODO - need to refactor the _recentDocuments to store FileOpenInformation objects
			} catch (Exception e) {
				Console.WriteLine ("error loading history " + e.Message);
			}
		}


		/// <summary>
		/// Saves the history.
		/// </summary>
		void SaveHistory ()
		{
//			try {
			var newItems = _recentDocuments.Select ((e) => new Tuple<string,Project> (e.FileName.FullPath, e.Project)).Where (tuple => File.Exists (tuple.Item1));
			var concatanated = newItems.Select ((Tuple<string, Project> arg) => arg.Item1 + PathDelimeter + arg.Item2.ItemId);
			File.WriteAllLines (GetSavedStatePath (), concatanated);
//			} catch (Exception e) {
//				Console.WriteLine ("error loading history " + e.Message);
//			}
		}

		/// <summary>
		/// Gets the project with identifier.
		/// </summary>
		/// <returns>The project with identifier.</returns>
		/// <param name="itemId">Item identifier.</param>
		Project GetProjectWithId (string itemId)
		{
			return IdeApp.Workspace.GetAllProjects ().FirstOrDefault ((project) => project.ItemId == itemId);
		}

		/// <summary>
		/// Saveds the state path.
		/// </summary>
		/// <returns>The state path.</returns>
		string GetSavedStatePath ()
		{
			//TODO get a workspace specific base directory
			var tempPath = IdeApp.Workspace.BaseDirectory.Combine (".#FileHistory");
			return tempPath;
		}

		#endregion
	}
}

