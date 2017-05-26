if [ ! -d "./ReleasePack" ]
	rm -rf "./ReleasePack"
fi

mkdir "./RegulusPack"

mkdir "./RegulusPack/Library"
mkdir "./RegulusPack/Library/BehaviourTree"
mkdir "./RegulusPack/Library/Server"
mkdir "./RegulusPack/Library/Client"

mkdir "./RegulusPack/Tool"
mkdir "./RegulusPack/Tool/Server"
mkdir "./RegulusPack/Tool/ProtocolBuilder"

cp "./Library/RegulusBehaviourTree/bin/Release/*.*" "./RegulusPack/Library/BehaviourTree" 

cp "./Library/RemotingNativeGhost/bin/Release/*.*" "./RegulusPack/Library/Client" 

cp "./Library/RemotingNativeSoul/bin/Release/*.*" "./RegulusPack/Library/Server" 

cp "./Tool/Server/bin/Release/*.*" "./RegulusPack/Tool/Server" 
cp "./Tool/Regulus.Remoting.Unity.ProtocolBuilder/bin/Release/*.*" "./RegulusPack/Tool/ProtocolBuilder" 








