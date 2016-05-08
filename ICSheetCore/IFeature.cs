namespace ICSheetCore
{
    /// <summary>A feature of a character.</summary>
    public interface IFeature 
    {
        /// <summary></summary>
        string Name { get; }
        /// <summary>The first level this feature applies.</summary>
        int StartsFromLevel { get; }
        /// <summary>Can be gained by a multi-class character on a second/later class.</summary>
        bool IsMulticlassInheritable { get; }
        /// <summary>A basic description.</summary>
        string Description { get; }
        /// <summary>The full description including the name.</summary>
        string FullDescription { get; }

    }
}