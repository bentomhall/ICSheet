using System;
namespace ICSheet5e.Model
{
    interface IRace
    {
        string ToString();
        Race.RaceType Value { get; }
    }
}
