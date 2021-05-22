# Intor
This is a project builder for regulus.remote, which will create the following projects.  
  
>YourNamespace.Common/YourNamespace.Common.csproj  
YourNamespace.Main/YourNamespace.Main.csproj  
YourNamespace.Protocol/YourNamespace.Protocol.csproj  

# Prepare
Include [Regulus.Remote](https://github.com/jiowchern/Regulus.Remote.git) to your repo. 
# Build
```ps1
dotnet run --project ./Regulus.Remote.TemplateCreator.csproj -- --sln-namespace YourNamespace --library-dir ./Regulus.Remote --output-dir ./
```