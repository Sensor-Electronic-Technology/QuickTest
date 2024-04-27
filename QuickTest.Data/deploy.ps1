cd C:\Users\aelmendo\RiderProjects\QuickTest\QuickTest.Data\

[int]$type=0 <#0=patch,1=minor,2=major#>

$csprojfilename = "C:\Users\aelmendo\RiderProjects\QuickTest\QuickTest.Data\QuickTest.Data.csproj"
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
$outputPath="C:/Users/aelmendo/RiderProjects/QuickTest/QuickTest.Data/bin/Release/QuickTest.Data." + $newversionNumber + ".nupkg"
$csprojcontents.Project.PropertyGroup.Version = $newversionNumber
$csprojcontents.Save($csprojfilename)
dotnet pack $csprojfilename --configuration Release 
<#dotnet nuget push $outputPath --source "github" #>
dotnet nuget push -s http://172.20.4.15:8081/v3/index.json $outputPath



<# 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
# "bin/Release/QuickTest.Data.1.0.2.nupkg" 
# $csprojcontents.Project.PropertyGroup.Version = $Env:BUILD_BUILDNUMBER #>
<# $csprojcontents.Save($csprojfilename) #>
<# "Version number has been udated from " + $oldversionNumber + " to " + $Env:BUILD_BUILDNUMBER #>
