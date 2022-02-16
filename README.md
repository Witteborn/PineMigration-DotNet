
![image](https://user-images.githubusercontent.com/27786664/154346980-c3248615-09a4-4d15-9cbc-86677df1ac52.png)

# Description
This project aims towards fixing the "Project is not eligable for migration" issue in Visual Studio IDE for C# DotNet projects.
This tool works even tho Microsofts inhouse tool is not able to migrate your project.

Further acticles about this issue:

[Microsoft Documentation](https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference)

[Official Issue - Closed: Low Priority](https://developercommunity.visualstudio.com/t/migrate-packagesconfig-to-packagereference-not-ava/246759)

[GitHub Issue explaining the problem](https://github.com/NuGet/docs.microsoft.com-nuget/issues/860)

# Features
- Automated migration from package.config to PackageReferences


# Installation
This is a dotnet-tool. This project can be installed by:

if you dont have a manifest for tools yet

- dotnet new tool-manifest

followed by

- dotnet tool install PineMigration


# Usage
Simply use
- dotnet tool run PineMigration

or

- dotnet PineMigration

[Microsoft Documentation on how to use a dotnet tool](https://docs.microsoft.com/de-de/dotnet/core/tools/global-tools)

# How it works

It is recommended to backup your project before using it just in case something unexpected happens.

The tool will automatically detect your csproj files and their package.config's and automatically migrate.

PineMigration reads the content of your package.config and extracts information such as the name and version of each NuGet package and writes it into the needed PackageReference format. It will then find your csproj file and paste the result at end of you file right before closing "Project" element.


# Contribution
Pull request are very welcome, please open an issue in advance.

Please check out our [Contribution Guidelines](./CONTRIBUTING.md) and [Code of Conduct](./CODE_OF_CONDUCT.md) before creating a pull request.

# Support
If you need any help check out our [Support](./SUPPORT.md) file.

# Security

Have you found a security issue? a vulnerability? or simply have some concerns?
Checkout our [Security file](./SECURITY.md) to see how you can report them safely.

# Thank you
[Cake](https://github.com/cake-build/cake) for being a rolemodel in this project and providing us with various templates.

[GitReleaseManager](https://github.com/GitTools/GitReleaseManager) for helping us to create easy release drafts

[GitVersion](https://github.com/GitTools/GitVersion) for helping us to automatically generate a SemVer in our workflow

[ReactBoilerplate](https://github.com/react-boilerplate/react-boilerplate) for providing us with fresh ideas and being an inspiration for this project. 

Also thank you to all contributors, people who open issues and bring in new ideas and feedback and anyone who shares this project and helps us in anyway possible.
**Thank you!** without you this wouldn't be possible.
