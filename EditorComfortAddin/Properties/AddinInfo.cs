using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"Editor Comfort Additions", 
	Namespace = "TwinTechs.EditorExtensions",
	Version = "1.0"
)]

[assembly:AddinName ("Editor Comfort Additions")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Provides some further comfort features for user's of Xamarin's excellent IDE. These ar navigation features which will be familiar to JetBrains users, and allow for more and quick navigation without the mouse")]
[assembly:AddinAuthor ("George James Edward Cook")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
