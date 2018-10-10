[xml]$doc = Get-Content .\src\Directory.Build.props
$version = $doc.Project.PropertyGroup.VersionPrefix # the version under development, update after a release
$versionSuffix = '-build.0' # manually incremented for local builds

function isVersionTag($tag){
    $v = New-Object Version
    [Version]::TryParse($tag, [ref]$v)
}

if ($env:appveyor){
    $versionSuffix = '-build.' + $env:appveyor_build_number
    if ($env:appveyor_repo_tag -eq 'true' -and (isVersionTag($env:appveyor_repo_tag_name))){
        $version = $env:appveyor_repo_tag_name
        $versionSuffix = ''
    }
    Update-AppveyorBuild -Version "$version$versionSuffix"
}

dotnet build -c Release freya-polyfills.sln /p:Version=$version$versionSuffix
dotnet pack --no-build -c Release src/Freya.Polyfills /p:Version=$version$versionSuffix -o $psscriptroot/bin
dotnet pack --no-build -c Release src/Freya.Polyfills.Hopac /p:Version=$version$versionSuffix -o $psscriptroot/bin
dotnet pack --no-build -c Release src/Freya.Polyfills.Katana /p:Version=$version$versionSuffix -o $psscriptroot/bin
dotnet pack --no-build -c Release src/Freya.Polyfills.Katana.Hopac /p:Version=$version$versionSuffix -o $psscriptroot/bin
dotnet pack --no-build -c Release src/Freya.Polyfills.Kestrel /p:Version=$version$versionSuffix -o $psscriptroot/bin
dotnet pack --no-build -c Release src/Freya.Polyfills.Kestrel.Hopac /p:Version=$version$versionSuffix -o $psscriptroot/bin
