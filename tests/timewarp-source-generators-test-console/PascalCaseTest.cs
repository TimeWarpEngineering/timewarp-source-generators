#region Purpose
// PascalCase file fixture that TW0001 can flag when that rule is enabled.
#endregion

namespace TimeWarp.SourceGenerators.TestConsole;

public class PascalCaseTest
{
    public string GetPascalMessage()
    {
        return "This message is from a PascalCase source file!";
    }
}