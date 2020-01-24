namespace Regulus.Remote.Tests
{
    public class PropertyTest
    {
        public int Field;
        public int Property { get { return Field; } }
    }
    public class SoulPropertyTest
    {
        [NUnit.Framework.Test()]
        public void UpdateTest()
        {
            var propertyTest = new PropertyTest();
            propertyTest.Field = 1;
            var info = typeof(PropertyTest).GetProperty(nameof(PropertyTest.Property));
            var handler = new Regulus.Remote.SoulProvider.Soul.PropertyHandler(info , 1);
            object val;
            handler.TryUpdate(propertyTest, out val);
            NUnit.Framework.Assert.AreEqual(1, val);

            propertyTest.Field = 2;

            handler.TryUpdate(propertyTest, out val);
            NUnit.Framework.Assert.AreEqual(2, val);

            NUnit.Framework.Assert.AreEqual(1, handler.Id);
        }
    }
}