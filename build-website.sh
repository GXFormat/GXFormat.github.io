#!/bin/bash
rm -rf GXFormat.Website/wwwroot/data/GXFormat_2099.01.01.lflist.conf
dotnet build -c Release GXFormat.Generator
dotnet run -c Release --project GXFormat.Generator/GXFormat.Generator.csproj
dotnet publish -c Release GXFormat.Website/GXFormat.Website.csproj

