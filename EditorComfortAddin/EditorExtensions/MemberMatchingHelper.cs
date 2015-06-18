using System;
using System.Text;

namespace TwinTechs.EditorExtensions
{
	public static class MemberMatchingHelper
	{

		static string[] GetParts (string sourceString)
		{
			StringBuilder builder = new StringBuilder ();
			foreach (char c in sourceString) {
				if ((Char.IsUpper (c) || !Char.IsLetter (c)) && builder.Length > 0)
					builder.Append ('%');
				builder.Append (c);

			}
			var text = builder.ToString ();
			return text.Split ('%');
		}

		public static bool GetMatchWithSearchText (string searchText, string targetText)
		{
			var sourceParts = GetParts (searchText);
			var parts = GetParts (targetText);
			//now iterate over to see if each part is contained

			var startPartIndex = 0;
			var numberOfSourceParts = sourceParts.Length;
			bool didMatch = false;
		

			while (!didMatch && (startPartIndex + numberOfSourceParts <= parts.Length)) {

				int endIndex = startPartIndex + numberOfSourceParts;
				bool failedMatch = false;
				for (int index = startPartIndex; index < endIndex; index++) {
					if (index - startPartIndex == numberOfSourceParts) {
						break;
					}
					var currentItemPart = parts [index];
					var sourcePart = sourceParts [index - startPartIndex];
					if (!char.IsUpper (sourcePart, 0)) {
						if (!(currentItemPart.ToUpper ().Contains (sourcePart.ToUpper ()))) {
							failedMatch = true;
						}
						continue;
					}

					var currentItemPartHead = currentItemPart.Substring (0, 1);
					var currentItemPartTail = currentItemPart.Substring (1, currentItemPart.Length - 1);

					var sourcePartHead = sourcePart.Substring (0, 1);
					var sourcePartTail = sourcePart.Substring (1, sourcePart.Length - 1);
					if (!string.IsNullOrEmpty (currentItemPartTail) && !string.IsNullOrEmpty (sourcePartTail)) {
						if (sourcePartHead != currentItemPartHead || !(currentItemPartTail.ToUpper ().Contains (sourcePartTail.ToUpper ()))) {
							failedMatch = true;
						}
					} else {
						if (sourcePartHead != currentItemPartHead) {
							failedMatch = true;
						}
					}
				}
				startPartIndex++;
				didMatch = !failedMatch;
			}
			if (!didMatch) {
				var upperSearchText = searchText.ToUpper ();
				var upperTargetText = targetText.ToUpper ();
				if (upperSearchText == upperTargetText) {
					didMatch = true;
				}

				if (upperTargetText.Contains (upperSearchText)) {
					didMatch = true;
				}
			}

			return didMatch;
		}

	}
}

