# 行為樹
[位置](https://github.com/jiowchern/Regulus/tree/master/Library/RegulusBehaviourTree)
[測試](https://github.com/jiowchern/Regulus/tree/master/Test/RegulusBehaviourTree.Tests)

## 使用說明
利用`TICKRESULT.SUCCESS` `TICKRESULT.RUNNING` `TICKRESULT.FAILURE` 控制行為樹方法
```csharp
[NUnit.Framework.Test()]
public void TestTick7()
{
    bool executee = false;

    int count = 0;
    var builder = new Builder();
    var node = builder
        .Selector()
            .Sequence()
                .Action((delta) => TICKRESULT.SUCCESS)
                .Action((delta) => TICKRESULT.SUCCESS)                        
                .Action(
                    (delta) =>
                    {
                        count ++;
                        if(count > 3)
                            return TICKRESULT.SUCCESS;
                        return TICKRESULT.RUNNING;
                    })
            .End()
        .End().Build();

    NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, node.Tick(0));
    NUnit.Framework.Assert.AreEqual(false, executee);
}
```

利用`Regulus.BehaviourTree.Yield.Success` `Regulus.BehaviourTree.Yield.Failure` `Regulus.BehaviourTree.Yield.WaitSeconds` `Regulus.BehaviourTree.Yield.Wait` `Regulus.BehaviourTree.Yield.WaitUntil` 控制行為樹類似Unity的Coroutine
```csharp
[NUnit.Framework.Test()]
        [NUnit.Framework.Timeout(5000)]
        public void TestParallel3()
        {
            List<int> tokens = new List<int>();
            int token1 = 1;
            int token2 = 2;
            int token3 = 3;
            int token4 = 4;
            int token5 = 5;
            var builder = new Builder();
            var ticker = builder.Selector()
                    .Parallel(true)
                        .Action(() => _AddToken(tokens, token1,0))                
                        .Action(() => _AddToken(tokens, token2,1))
                        .Action(() => _AddToken(tokens, token3,0))
                    .End()
                    .Parallel(true)
                        .Action(() => _AddToken(tokens, token4,0))
                        .Action(() => _AddToken(tokens, token5,0))
                    .End()
                .End().Build();

            while (tokens.Count < 5)
            {
                ticker.Tick(0);
            }
            Assert.AreEqual(token1, tokens[0]);
            Assert.AreEqual(token2, tokens[1]);
            Assert.AreEqual(token3, tokens[2]);
            Assert.AreEqual(token4, tokens[3]);
            Assert.AreEqual(token5, tokens[4]);

        }

        private IEnumerable<IInstructable> _AddToken(List<int> tokens, int token , float time)
        {
            tokens.Add(token);
            yield return new WaitSeconds(time);
            yield return new Failure();
        }
```
