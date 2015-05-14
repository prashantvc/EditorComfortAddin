using System.Linq;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace TwinTechs.EditorExtensions
{
	public static class IdeHelper
	{

		/// <summary>
		/// Encapsulated as I might need to do some more clever stuff here later for caching/reuse; but for now it 
		/// seems that Xamarin handles all the cases I need.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public static void OpenDocument (string fileName)
		{
			IdeApp.Workbench.OpenDocument (fileName, IdeApp.Workbench.ActiveDocument.Project, OpenDocumentOptions.TryToReuseViewer | OpenDocumentOptions.BringToFront);
		}


	}
}


