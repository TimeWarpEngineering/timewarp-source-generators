namespace TimeWarp.SourceGenerators.TestConsole;

public partial class PartialPropertyTest
{
    // Declaration part
    public partial string TestProperty { get; set; }
}

public partial class PartialPropertyTest
{
    // Implementation part
    private string _testProperty = "";
    
    public partial string TestProperty 
    { 
        get => _testProperty;
        set => _testProperty = value;
    }
}