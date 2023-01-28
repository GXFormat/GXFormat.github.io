#!/bin/bash
dotnet build -c Release GXFormat.Generator
dotnet run -c Release --project GXFormat.Generator/GXFormat.Generator.csproj
dotnet publish -c Release GXFormat.Website/GXFormat.Website.csproj

