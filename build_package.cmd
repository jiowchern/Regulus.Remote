md artifact 
md artifact\Library 
md artifact\Library\Server
md artifact\Library\Client
md artifact\Library\Client\Remote
md artifact\Library\Client\Standalone
md artifact\Library\BehaviourTree
md artifact\Library\Serialization

md artifact\Tool
md artifact\Tool\Server
md artifact\Tool\ProtocolBuilder


copy Regulus.BehaviourTree\bin\Release\netstandard2.0\*.* artifact\Library\BehaviourTree
copy Regulus.Remote.Soul\bin\Release\netstandard2.0\*.* artifact\Library\Server
copy Regulus.Remote.Ghost\bin\Release\netstandard2.0\*.* artifact\Library\Client\Remote
copy Regulus.Remote.Standalone\bin\Release\netstandard2.0\*.* artifact\Library\Client\Standalone
copy Regulus.Serialization\bin\Release\netstandard2.0\*.* artifact\Library\Serialization


copy Regulus.Application.Server\bin\Release\netcoreapp2.1\*.* artifact\Tool\Server
copy Regulus.Application.Protocol.CodeBuilder\bin\Release\netcoreapp2.1\*.* artifact\Tool\ProtocolBuilder
copy Regulus.Application.Protocol.CodeWriter\bin\Release\netcoreapp2.1\*.* artifact\Tool\ProtocolBuilder
