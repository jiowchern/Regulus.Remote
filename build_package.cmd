md artifact 
md artifact\Library 
md artifact\Library\Server
md artifact\Library\Client
md artifact\Library\Client\Remote
md artifact\Library\Client\Standalone
md artifact\Library\Serialization

md artifact\Tool
md artifact\Tool\ProtocolBuilder

dotnet publish .\Regulus.Remote.Soul -o .\artifact\Library\Server -f netstandard2.0
dotnet publish .\Regulus.Remote.Ghost -o .\artifact\Library\Client\Remote -f netstandard2.0
dotnet publish .\Regulus.Remote.Standalone -o .\artifact\Library\Client\Standalone -f netstandard2.0
dotnet publish .\Regulus.Serialization -o \.artifact\Library\Serialization -f netstandard2.0

dotnet publish .\Regulus.Application.Protocol.CodeWriter -o .\artifact\Tool\ProtocolBuilder  -f netcoreapp2.1
