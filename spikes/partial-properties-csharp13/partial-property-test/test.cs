partial class C
{
    // Defining declaration
    public partial string Prop { get; set; }

    // Implementing declaration
    public partial string Prop { get => field ??= ""; set => field = value; }
}