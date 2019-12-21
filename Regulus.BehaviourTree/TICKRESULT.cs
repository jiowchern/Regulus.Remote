using Regulus.BehaviourTree;

namespace Regulus.BehaviourTree
{
    public enum TICKRESULT
    {    
        SUCCESS,

        FAILURE,

        RUNNING
    }
}


namespace Regulus.Extension
{

    static class BehaviourTree
    {
        public static TICKRESULT Not(this TICKRESULT result)
        {
            if(result == TICKRESULT.FAILURE)
                return TICKRESULT.SUCCESS;
            else if(result == TICKRESULT.SUCCESS)
            {
                return TICKRESULT.FAILURE;
            }
            return result;
        }
    }
}