dotnet build;
if(-not $?) { exit; }
pushd "bin\Debug\netcoreapp3.1";
Invoke-Expression "./SBVTranslator.exe $args";
popd;