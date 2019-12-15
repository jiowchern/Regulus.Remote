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

copy Library\Regulus.Remote.Soul\bin\Release\*.* artifact\Library\Server
copy Library\Regulus.BehaviourTree\bin\Release\*.* artifact\Library\BehaviourTree
copy Library\Regulus.Serialization\bin\Release\*.* artifact\Library\Serialization
copy Library\Regulus.Remote.Ghost\bin\Release\*.* artifact\Library\Client\Remote
copy Library\Regulus.Remote.Standalone\bin\Release\*.* artifact\Library\Client\Standalone

copy Tool\Regulus.Application.Server\bin\Release\*.* artifact\Tool\Server
copy Tool\Regulus.Application.Protocol.Generator\bin\Release\*.* artifact\Tool\ProtocolBuilder
