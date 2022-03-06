namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{

    public class PropertyTester : IPropertyable
    {
        public readonly Property<int> Property1 = new Property<int>(1);
        public readonly Property<int> Property2 = new Property<int>(2);
        Property<int> IPropertyable2.Property2 => Property2;

        Property<int> IPropertyable1.Property1 => Property1;
    }
}
