namespace Regulus.Utility.Tests
{

    public class Vector2Tests
    {
        [Xunit.Fact]
        public void VectorToAngleTest()
        {
            Vector2 vec = Vector2.AngleToVector(45.0f);
            float angle = Vector2.VectorToAngle(vec);
            Xunit.Assert.Equal(45.0f, angle);
        }
    }
}