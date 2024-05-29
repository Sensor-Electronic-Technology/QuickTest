﻿cd $env:USERPROFILE\RiderProjects\QuickTest\QuickTest.Data\

[int]$type=0 <#0=patch,1=minor,2=major#>

$csprojfilename = $env:USERPROFILE+"\RiderProjects\QuickTest\QuickTest.Data\QuickTest.Data.csproj"
"Project file to update " + $csprojfilename
[xml]$csprojcontents = Get-Content -Path $csprojfilename;
"Current version number is " + $csprojcontents.Project.PropertyGroup.Version

[string]$oldversionNumber = $csprojcontents.Project.PropertyGroup.Version
$split=$oldversionNumber -split "\."
[int]$major = $split[0]
[int]$minor = $split[1]
[int]$patch = $split[2]

switch ($type) {
    0 { $patch = $patch + 1 }
    1 { $minor = $minor + 1 }
    2 { $major = $major + 1 }
}

$newversionNumber = $major.ToString() + "." + $minor.ToString() + "." + $patch.ToString()
"New version number is " + $newversionNumber
$outputPath=$env:USERPROFILE+"\RiderProjects\QuickTest\QuickTest.Data\bin\Release\QuickTest.Data." + $newversionNumber + ".nupkg"
$csprojcontents.Project.PropertyGroup.Version = $newversionNumber
$csprojcontents.Save($csprojfilename)
dotnet pack $csprojfilename --configuration Release
$json = dotnet user-secrets list --json
$secrets = $json | % { $_ -replace '//(BEGIN|END)' } | ConvertFrom-Json
dotnet nuget push $outputPath --api-key $secrets.'GithubPackage:Token' --source "github"




