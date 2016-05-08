using System.Runtime.Serialization;

namespace ICSheetCore
{

    /// <summary>
    /// Contains the basic class feature details.
    /// </summary>
    [DataContract]
    public class ClassFeature : IFeature
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startingLevel"></param>
        /// <param name="inheritable"></param>
        /// <param name="description"></param>
        public ClassFeature(string name, int startingLevel, bool inheritable, string description)
        {
            Name = name;
            StartsFromLevel = startingLevel;
            IsMulticlassInheritable = inheritable;
            Description = description;
        }


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsMulticlassInheritable
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int StartsFromLevel
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullDescription
        {
            get
            {
                return Description;
            }
        }
    }
}