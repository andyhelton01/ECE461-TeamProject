#!/bin/sh

packageid="Microsoft.Build.Mono.Debug"
version="14.1.0.0-prerelease" # update as needed

mono path/to/nuget.exe install $packageid -Version \
    $version -Source "https://www.myget.org/F/dotnet-buildtools/"

# run MSBuild
mono $packageid.$version/lib/MSBuild.exe Foo.sln