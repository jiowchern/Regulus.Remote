md artifact 
md artifact\Library 
md artifact\Library\Server
md artifact\Library\Client
md artifact\Library\Client\Remote
md artifact\Library\Client\Standalone
md artifact\Library\BehaviourTree

md artifact\Tool
md artifact\Tool\Server
md artifact\Tool\ProtocolBuilder

copy Library\RemotingNativeSoul\bin\Release\*.* artifact\Library\Server
copy Library\RegulusBehaviourTree\bin\Release\*.* artifact\Library\BehaviourTree
copy Library\RemotingNativeGhost\bin\Release\*.* artifact\Library\Client\Remote
copy Library\RemotingStandalong\bin\Release\*.* artifact\Library\Client\Standalone

copy Tool\Server\bin\Release\*.* artifact\Tool\Server
copy Tool\GhostProviderGenerator\bin\Release\*.* artifact\Tool\ProtocolBuilder
