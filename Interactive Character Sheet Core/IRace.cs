using System;

namespace InteractiveCharacterSheetCore
{
    public interface IRace
    {
        string ToString();
        Enum Value { get; }
    }
}
