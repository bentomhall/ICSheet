﻿using System;
using System.Collections.Generic;

namespace ICSheetCore
{
    public interface IRace
    {
        /// <summary>
        /// The number of feet that this race can move per round before adding modifiers.
        /// </summary>
        int BaseMovement { get; }
        /// <summary>
        /// The fully-qualified name of the race (including subrace if any).
        /// </summary>
        string RaceName { get; }

        /// <summary>
        /// Indicates the subrace (if any). Will be null for races without subraces.
        /// </summary>
        string SubraceName { get; }

        /// <summary>
        /// A collection of all racial features (including subrace features).
        /// </summary>
        IEnumerable<IFeature> Features { get; }

        void AddFeature(IFeature feature);
        Tuple<string, string> GetInformation();
    }
}
