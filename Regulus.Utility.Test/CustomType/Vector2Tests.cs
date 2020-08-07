namespace Regulus.Utility.Tests
{

    public class Vector2Tests
    {
        [NUnit.Framework.Test()]
        public void VectorToAngleTest()
        {
            Vector2 vec = Vector2.AngleToVector(45.0f);
            float angle = Vector2.VectorToAngle(vec);
            NUnit.Framework.Assert.AreEqual(45.0f, angle);
        }
    }
}