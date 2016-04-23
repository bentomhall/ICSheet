using System;
using System.Collections.Generic;

namespace ICSheetCore
{
    /// <summary>
    /// Contains the basic class feature details.
    /// </summary>
    public class ClassFeature : IFeature
    {
        public ClassFeature(string name, int startingLevel, bool inheritable, string description)
        {
            Name = name;
            StartsFromLevel = startingLevel;
            IsMulticlassInheritable = inheritable;
            Description = description;
        }

        public string Description
        {
            get; private set;
        }

        public bool IsMulticlassInheritable
        {
            get; private set;
        }


        public string Name
        {
            get; private set;
        }

        public int StartsFromLevel
        {
            get; private set;
        }

        public string FullDescription
        {
            get
            {
                return Description;
            }
        }
    }
}