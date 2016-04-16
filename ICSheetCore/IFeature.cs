namespace ICSheetCore
{
    public interface IFeature
    {
        string Name { get; }
        int StartsFromLevel { get; }
        bool IsMulticlassInheritable { get; }
        string Description { get; }
    }
}