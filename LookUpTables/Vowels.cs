using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.LookUpTables
{
    public static class Vowels
    {
        private static readonly string[] table = new string[]
        {
            PrettyScaryDictionary.AEE,
            PrettyScaryDictionary.AHH,
            PrettyScaryDictionary.AWW,
            PrettyScaryDictionary.EEE,
            PrettyScaryDictionary.EHH,
            PrettyScaryDictionary.ERR,
            PrettyScaryDictionary.EYE,
            PrettyScaryDictionary.HOH,
            PrettyScaryDictionary.IHH,
            PrettyScaryDictionary.OOO,
            PrettyScaryDictionary.OWE,
            PrettyScaryDictionary.UHH,
        };
        public static readonly ReadOnlyCollection <string> TABLE = new ReadOnlyCollection <string> (table); //static properties must be declared after their dependencies. else, will crash on startup.
    }
}
