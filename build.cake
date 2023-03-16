///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var solutionName = "PineMigration";
var solutionFolder = "./PineMigration";
var targetFramework = "net7";
var outputFolder = "./artifacts";
var runtime = targetFramework;
var runtimeFolder = outputFolder + $"/{runtime}";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	CleanDirectory($"./{solutionName}/bin/{configuration}");
	CleanDirectory(outputFolder);
});

Task("Restore")
	.Does(()=>{
		DotNetRestore(solutionFolder);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	DotNetBuild(solutionFolder, new DotNetBuildSettings
	{
		NoRestore = true,
		Configuration = configuration,
	});
});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
{
	DotNetTest(solutionFolder, new DotNetTestSettings
	{
		NoBuild = true,
		NoRestore = true,
		Configuration = configuration,
	});
});

Task("Publish")
	.IsDependentOn("Test")
	.Does(()=>
{
		Information($"\nRuntime: {runtime}");

		var settings = new DotNetPublishSettings
		{
			OutputDirectory = runtimeFolder,
			Configuration = configuration,
		};

		DotNetPublish(solutionFolder, settings);
		DeleteFile($"{runtimeFolder}/{solutionName}.pdb");
});


Task("NuGet")
	.IsDependentOn("Publish")
	.Does(()=>
{
    DotNetPack(solutionFolder);
});

Task("PublishAndZip")
	.IsDependentOn("Publish")
	.Does(()=>
{
		Zip(runtimeFolder, $"{outputFolder}/{solutionName}_{runtime}.zip");
		DeleteDirectory(runtimeFolder, new DeleteDirectorySettings {
    		Recursive = true,
    		Force = true
		});
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);