var sln = "./src/Ideine.Cake.Xamarin/Ideine.Cake.Xamarin.sln";
var projectDirectory = "./src/Ideine.Cake.Xamarin/Ideine.Cake.Xamarin/";
var projectName = "Ideine.Cake.Xamarin";

const string DEFAULT_TARGET = "release";
const string DEFAULT_CONFIGURATION = "Release";
const string OUTPUT_FOLDER = "artifacts";

var target = Argument("target", DEFAULT_TARGET);
var configuration = Argument("configuration", DEFAULT_CONFIGURATION);


Task("build")
    .IsDependentOn("clean")
    .IsDependentOn("lib")
    .Does(() => {});

Task("release")
	.IsDependentOn("build")
	.IsDependentOn("copy_artifacts")
	.Does(() => { });

Task ("copy_artifacts")
	.Does(() => {
		if(!DirectoryExists(OUTPUT_FOLDER))
		{
			CreateDirectory(OUTPUT_FOLDER);
		}
		else
		{
			CleanDirectories(OUTPUT_FOLDER);
		}

		CopyFiles(string.Format("{0}bin/{1}/*.*", projectDirectory, configuration), OUTPUT_FOLDER);
	});

Task ("lib").Does (() => 
{
	NuGetRestore (sln);
	DotNetBuild (sln, c => c.Configuration = configuration);
});

Task ("clean").Does (() => 
{
	CleanDirectories ("./**/bin");
	CleanDirectories ("./**/obj");
});


Task("Default").Does(() => {
	RunTarget(DEFAULT_TARGET);
});

RunTarget(target);