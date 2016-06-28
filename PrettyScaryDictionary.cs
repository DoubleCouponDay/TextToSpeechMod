using System.Collections.Generic;

namespace SETextToSpeechMod
{
    public static class PrettyScaryDictionary
    {
        public const string AEE = "AEE";
        public const string AHH = "AHH"; 
        public const string AWW = "AWW"; 
        public const string BIH = "BIH"; 
        public const string DIH = "DIH"; 
        public const string EEE = "EEE"; 
        public const string EHH = "EHH"; 
        public const string EYE = "EYE"; 
        public const string FIH = "FIH"; 
        public const string GIH = "GIH"; 
        public const string HIH = "HIH"; 
        public const string HOH = "HOH"; 
        public const string IHH = "IHH"; 
        public const string JIH = "JIH"; 
        public const string KIH = "KIH"; 
        public const string KSS = "KSS"; 
        public const string LIH = "LIH"; 
        public const string MIH = "MIH"; 
        public const string NIH = "NIH"; 
        public const string OOO = "OOO"; 
        public const string OWE = "OWE"; 
        public const string PIH = "PIH"; 
        public const string RIH = "RIH"; 
        public const string SIH = "SIH"; 
        public const string THI = "THI"; 
        public const string TIH = "TIH"; 
        public const string UHH = "UHH"; 
        public const string VIH = "VIH"; 
        public const string WIH = "WIH"; 
        public const string YIH = "YIH"; 
        public const string ZIH = "ZIH";

        public static readonly string[] conversionTable = { //helps the ingame block convert phonemes to speaker numbers
            AEE, AHH, AWW, BIH, DIH, EEE, EHH, EYE, FIH, GIH, HIH, HOH, IHH, JIH, KIH, KSS, LIH, MIH, NIH, OOO, OWE, PIH, RIH, SIH, THI, TIH, UHH, VIH, WIH, YIH, ZIH 
        };

        /*
        SUPPORTED EXTENSIONS 
        this is a useless array but it lets me know which extensions can be added to dictionary words.
            "E",
            "S", "ES", "IES",
            "D", "ED", "IED",
            "Y", "RY", "LY", "ALLY",
            "R", "ER", "BER", "LER", "MER", "NER", "PER", "TER",
            "L", "AL",
            "ING", "BING", "LING", "MING", "NING", "PING", "TING",
            "EST",
            "ABLE", "BABLE", "LABLE", "MABLE", "NABLE", "PABLE", "TABLE",
        */        
             
        public static readonly string[] row = new string[0]; //for readability only.                                                   

        //use dictionary if you just cant get the syllable spacing you want.
        //its very easy to enter a word wrong. try to get entry values to the same length as its key.                                                         
        public static Dictionary <string, string[]> ordered = new Dictionary <string, string[]>() //ACRONYMS ARE NOT SUPPORTED
        {
            {"_A_", row}, { "ALL", new string[]{ AWW, " ", LIH } }, { "ALSO", new string[]{ AWW, LIH, SIH, OWE } }, { "AUTOMATIC", new string[]{ AWW, TIH, " ", OWE, MIH, AHH, TIH, IHH, KIH, } }, { "ALERT", new string[]{ UHH, " ", LIH, RIH, TIH} }, { "ABOARD", new string[]{ UHH, " ", BIH, AWW, " ", DIH} },
            {"_B_", row}, { "BEFORE", new string[]{ BIH, EEE, FIH, AWW, RIH, "", } },
            {"_C_", row}, { "COULD", new string[]{ KIH, OOO, " ", DIH, "", } }, { "CHINESE", new string[]{ KIH, HIH, EYE, NIH, EEE, " ", ZIH, } },
            {"_D_", row}, { "DO", new string[]{ DIH, OOO, } },
            {"_E_", row}, { "ENGINE", new string[]{ EHH, NIH, " ", JIH, IHH, NIH, } }, { "EARTHQUAKE", new string[]{ RIH, THI, " ", KIH, WIH, AEE, KIH, "", "", "", } }, { "END", new string[]{ EHH, NIH, DIH, } }, { "EVERY", new string[]{ EHH, VIH, RIH, "", EEE, } },
            {"_F_", row}, { "FIND", new string[]{ FIH, EYE, NIH, DIH, } },
            {"_G_", row}, { "GO", new string[]{ GIH, OWE, } }, { "GOOD", new string[]{ GIH, " ", OOO, DIH, } }, { "GIANT", new string[]{ JIH, EYE, UHH, NIH, TIH, } }, { "GET", new string[]{ GIH, EHH, TIH, } },
            {"_H_", row}, { "HELL", new string[]{ HIH, EHH, LIH, "", } }, { "HEY", new string[]{ HIH, AEE, "", } }, { "HARDWARE", new string[]{ HIH, UHH, RIH, DIH, " ", WIH, EHH, RIH } },
            {"_I_", row}, { "IM", new string[]{ EYE, MIH, } }, { "ILL", new string[]{ EYE, LIH, "", } }, { "INFINIT", new string[]{ IHH, NIH, FIH, IHH, NIH, IHH, TIH, } }, { "INTEREST", new string[]{ IHH, NIH, TIH, RIH, " ", EHH, SIH, TIH, } }, 
            {"_J_", row},
            {"_K_", row},
            {"_L_", row}, { "LAZY", new string[]{ LIH, AEE, ZIH, } }, { "LOAD", new string[]{ LIH, OWE, " ", DIH, } }, { "LIMIT", new string[]{ LIH, IHH, MIH, IHH, TIH, } }, { "LOOP", new string[]{ LIH, OOO, " ", PIH, } },
            {"_M_", row}, { "MADDEN", new string[]{ MIH, AHH, DIH, EHH, NIH, "", } }, { "MEME", new string[]{ MIH, EEE, " ", MIH, } }, { "MANUAL", new string[]{ MIH, AHH, NIH, OOO, LIH, ""} },
            {"_N_", row}, { "NIGHT", new string[]{ NIH, EYE, " ", TIH, "", } },  { "NURSERY", new string[]{ NIH, RIH, " ", SIH, UHH, RIH, EEE, } }, { "NOW", new string[]{ NIH, OWE, WIH, } },
            {"_O_", row}, { "OKAY", new string[]{ OWE, " ", KIH, AEE, } }, { "OVER", new string[]{ OWE, " ", VIH, RIH, } }, { "ON", new string[]{ HOH, NIH, } }, { "ONE", new string[]{ WIH, UHH, NIH, } }, { "ONCE", new string[]{ WIH, UHH, NIH, SIH, } },
            {"_P_", row}, { "PUT", new string[]{ PIH, OOO, TIH, } }, { "POSSIBILIT", new string[]{ PIH, HOH, SIH, IHH, BIH, " ", IHH, LIH, IHH, TIH, } }, { "PASTE", new string[]{ PIH, AEE, " ", SIH, TIH, } }, { "PRIVATE", new string[]{ PIH, RIH, EYE, VIH, UHH, TIH, "", } },
            {"_Q_", row},
            {"_R_", row}, { "RUN", new string[]{ RIH, UHH, NIH, } }, { "ROSA", new string[]{ RIH, OWE, SIH, UHH, } }, { "READ", new string[]{ RIH, EEE, " ", DIH, } },
            {"_S_", row}, { "SIR", new string[]{ SIH, RIH, "", } }, { "SCIENCE", new string[]{ SIH, EYE, " ", EHH, NIH, SIH, "", } }, { "SHE", new string[]{ SIH, HIH, EEE, } }, { "SHOULD", new string[]{ SIH, HIH, " ", OOO, DIH, "", } }, { "SMALL", new string[]{ SIH, MIH, AWW, LIH, "", } }, { "SOLUTION", new string[]{ SIH, HOH, LIH, OOO, SIH, HIH, UHH, NIH,} }, { "SURE", new string[]{ SIH, HIH, OOO, RIH,} },
            {"_T_", row}, { "THING", new string[]{ THI, IHH, " ", NIH, GIH, } }, { "TODAY", new string[]{ TIH, OOO, " ", DIH, AEE} }, { "THAT", new string[]{ THI, AHH, " ", TIH, } }, { "TO", new string[]{ TIH, OOO, } }, { "TWO", new string[]{ TIH, OOO, "", } }, { "TOO", new string[]{ TIH, OOO, "", } }, { "THERE", new string[]{ THI, EHH, " ", RIH, "", } }, { "THEIR", new string[]{ THI, EHH, " ", RIH, "", } },
            {"_U_", row}, { "UNIVERSE", new string[]{ YIH, OOO, " ", NIH, EEE, VIH, RIH, SIH, } }, { "USE", new string[]{ YIH, OOO, SIH, } },
            {"_V_", row}, { "VIDEO", new string[]{ VIH, IHH, DIH, EEE, OWE, } }, { "VARIATION", new string[]{ VIH, EHH, RIH, EEE, AEE, " ", HIH, UHH, NIH, } }, { "VERSION", new string[]{ VIH, RIH, " ", ZIH, HIH, UHH, NIH, } },
            {"_W_", row}, { "WOULD", new string[]{ WIH, OOO, " ", DIH, "", } }, { "WORD", new string[]{ WIH, RIH, " ", DIH, } },
            {"_X_", row},
            {"_Y_", row}, { "YOUR", new string[]{ YIH, " ", AWW, "", } },
            {"_Z_", row},
        };
    }
}
