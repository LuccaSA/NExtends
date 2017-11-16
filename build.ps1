
param(
    [string]$nuget_path= $("C:\nuget")
    )
    
	wget "https://raw.githubusercontent.com/rducom/ALM/master/build/ComputeVersion.ps1" -outfile "ComputeVersion.ps1"
    . .\ComputeVersion.ps1 
    
    $version = Compute .\NExtends\NExtends.csproj
    $props = "/p:Configuration=Debug,VersionPrefix="+($version.Prefix)+",VersionSuffix="+($version.Suffix)
    $propack = "/p:PackageVersion="+($version.Semver) 
 
    dotnet restore
    dotnet build .\NExtends.sln $props
    dotnet pack .\NExtends\NExtends.csproj --configuration Debug $propack -o $nuget_path
 
    
