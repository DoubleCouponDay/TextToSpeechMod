using System;
using System.Text.RegularExpressions; //needed to match wildcard string matches simplify the rule based phoneme approach AdjacentEvaluation().

namespace SETextToSpeechMod
{
    class Pronunciation 
    {
        //pronunciation reference: http://www.englishleap.com/other-resources/learn-english-pronunciation
        const int NEW_WORD = -1;
        const int NO_MATCH = -2; 
        const int LAST_LETTER = -3;
        const int MAX_EXTENSION_SIZE = 5; 
        int placeholder = NEW_WORD;
        string[] dictionaryMatch;
        string surroundingPhrase;   

        WordCounter wordCounter;   

        public Pronunciation (string inputSentence)
        {
            this.wordCounter = new WordCounter (inputSentence);
        }

        //first searches the ditionary, then tries the secondary pronunciation if no match found.
        public string GetLettersPronunciation (string sentence, int letterIndex, out string secondary) 
        {
            secondary = "";
            string primaryPhoneme;        
            string currentWord = wordCounter.GetCurrentWord (ref placeholder); //this update is needed every time i increment a letter.          

            if (currentWord != " ")
            {                
                if (placeholder == NEW_WORD)
                {
                    string refinedQuery = ContinuallyRefineSearch (currentWord); //word ending simplifier to increase chances of a match.               
                
                    if (refinedQuery != "")
                    {
                        primaryPhoneme = TakeFromDictionary (true, sentence, letterIndex, out secondary);
                    }
            
                    else //if no match is found, use secondary pronunciation.
                    {
                        placeholder = NO_MATCH;
                        primaryPhoneme = AdjacentEvaluation (sentence, letterIndex, out secondary);
                    }
                }

                else if (placeholder != NO_MATCH) //takes over reading once a match is found in the dictionary.
                {
                    primaryPhoneme = TakeFromDictionary (false, sentence, letterIndex, out secondary);
                }

                else
                {
                    primaryPhoneme = AdjacentEvaluation (sentence, letterIndex, out secondary);
                }
            }

            else
            {
                primaryPhoneme = currentWord; //avoids setting placeholder in this scenario since an empty space cant reset it when needed.
                secondary = " ";
            }
            placeholder = wordCounter.CheckForEnd (placeholder, NEW_WORD); //script needed to reset to default state but the mod did not.
            return primaryPhoneme;
        }

        string ContinuallyRefineSearch (string currentWord) //removes word's extensions one after another until it has none.
        {
            string refinedQuery = currentWord;

            while (PrettyScaryDictionary.ordered.TryGetValue (currentWord, out dictionaryMatch) == false)
            {          
                string wordsExtension = EvaluateWordsEnding (currentWord); //returns / if the word is not long enough
                            
                if (wordsExtension != "/") //i must only remove an extension if one exists or the word is length 4 or greater.
                {
                    currentWord = currentWord.Remove ((currentWord.Length) - wordsExtension.Length, wordsExtension.Length);
                    refinedQuery = currentWord;
                }

                else
                {
                    return "";
                }
            }  
            return refinedQuery;
        }

        //returns the words extension if that phrase has been designed for, or / if word length is too small.
        string EvaluateWordsEnding (string currentWord) //unfortunately there is a ton of logic checks needed here because try catch does not work with in game scripts.
        {
            string searchResult = "/"; //prevents calling this method again until the next word.

            if (currentWord.Length >= MAX_EXTENSION_SIZE) //i dont really need to remove extensions unless the word is big enough.
            {
                bool E = (currentWord[currentWord.Length - 1] == 'E') ? true : false;
                bool S = (currentWord[currentWord.Length - 1] == 'S') ? true : false;
                bool D = (currentWord[currentWord.Length - 1] == 'D') ? true : false;
                bool Y = (currentWord[currentWord.Length - 1] == 'Y') ? true : false;
                bool R = (currentWord[currentWord.Length - 1] == 'R') ? true : false;                            
                bool L = (currentWord[currentWord.Length - 1] == 'L') ? true : false;                            

                bool ING = (currentWord[currentWord.Length - 3] == 'I' && 
                            currentWord[currentWord.Length - 2] == 'N' && 
                            currentWord[currentWord.Length - 1] == 'G') ? true : false;

                bool EST = (currentWord[currentWord.Length - 3] == 'E' && 
                            currentWord[currentWord.Length - 2] == 'S' && 
                            currentWord[currentWord.Length - 1] == 'T') ? true : false;

                bool ABLE = (currentWord[currentWord.Length - 4] == 'A' &&
                             currentWord[currentWord.Length - 3] == 'B' && 
                             currentWord[currentWord.Length - 2] == 'L' && 
                             currentWord[currentWord.Length - 1] == 'E') ? true : false;  
            
                if (E == true)
                {
                    searchResult = "E";
                }

                else if (S == true)
                {
                    bool ES = (currentWord[currentWord.Length - 2] == 'E') ? true : false;

                    if (ES == true)
                    {                                                     
                        bool IES = (currentWord[currentWord.Length - 3] == 'I') ? true : false;
                        
                        if (IES == true)
                        {
                            searchResult = "IES";
                        }

                        else
                        {
                            searchResult = "ES";
                        }
                    }
                
                    else
                    {
                        searchResult = "S";
                    }
                }

                else if (D == true)
                {
                    bool ED = (currentWord[currentWord.Length - 2] == 'E') ? true : false;

                    if (ED == true)
                    {
                        bool IED = (currentWord[currentWord.Length - 3] == 'I') ? true : false;

                        if (IED == true)
                        {
                            searchResult = "IED";
                        }
                    
                        else
                        {
                            searchResult = "ED";
                        }
                    }

                    else
                    {
                        searchResult = "D";    
                    }
                }

                else if (Y == true)
                {
                    bool RY = (currentWord[currentWord.Length - 2] == 'R') ? true : false;
                    bool LY = (currentWord[currentWord.Length - 2] == 'L') ? true : false;

                    if (LY == true)
                    {

                        bool ALLY = (currentWord[currentWord.Length - 4] == 'A' &&
                                        currentWord[currentWord.Length - 3] == 'L') ? true : false;  

                        if (ALLY == true)
                        {
                            searchResult = "ALLY";
                        }

                        else
                        {
                            searchResult = "LY";
                        }                        
                    }

                    else if (RY == true)
                    {
                        searchResult = "RY";
                    }

                    else
                    {
                        searchResult = "Y";
                    }                 
                }            

                else if (R == true)
                {
                    bool ER = (currentWord[currentWord.Length - 2] == 'E') ? true : false;  

                    if (ER == true)
                    {
                        bool BER = (currentWord[currentWord.Length - 3] == 'B') ? true : false;
                        bool LER = (currentWord[currentWord.Length - 3] == 'L') ? true : false;
                        bool MER = (currentWord[currentWord.Length - 3] == 'M') ? true : false;
                        bool NER = (currentWord[currentWord.Length - 3] == 'N') ? true : false;
                        bool PER = (currentWord[currentWord.Length - 3] == 'P') ? true : false;
                        bool TER = (currentWord[currentWord.Length - 3] == 'T') ? true : false;

                        if (TER == true)
                        {
                            searchResult = "TER";
                        }   
                  
                        else if (PER == true)
                        {
                            searchResult = "PER";
                        }

                        else if (NER == true)
                        {
                            searchResult = "NER";
                        }

                        else if (MER == true)
                        {
                            searchResult = "MER";
                        }

                        else if (LER == true)
                        {
                            searchResult = "LER";
                        }
                                     
                        else if (BER == true)
                        {
                            searchResult = "BER";
                        } 
                               
                        else 
                        {
                            searchResult = "ER";
                        } 
                    }

                    else
                    {
                        return "R";
                    }
                }                      

                if (L == true)
                {
                    bool AL = (currentWord[currentWord.Length - 2] == 'A') ? true : false;

                    if (AL == true)
                    {
                        searchResult = "AL";
                    }

                    else
                    {
                        searchResult = "L";
                    }
                }

                else if (ING == true)
                {
                    bool BING = (currentWord[currentWord.Length - 4] == 'B') ? true : false;
                    bool LING = (currentWord[currentWord.Length - 4] == 'L') ? true : false;
                    bool MING = (currentWord[currentWord.Length - 4] == 'M') ? true : false;
                    bool NING = (currentWord[currentWord.Length - 4] == 'N') ? true : false;                
                    bool PING = (currentWord[currentWord.Length - 4] == 'P') ? true : false;
                    bool TING = (currentWord[currentWord.Length - 4] == 'T') ? true : false;
                     
                    if (TING == true)
                    {
                        searchResult = "TING";
                    }   
                  
                    else if (PING == true)
                    {
                        searchResult = "PING";
                    }

                    else if (NING == true)
                    {
                        searchResult = "NING";
                    }

                    else if (MING == true)
                    {
                        searchResult = "MING";
                    }

                    else if (LING == true)
                    {
                        searchResult = "LING";
                    }
                                     
                    else if (BING == true)
                    {
                        searchResult = "BING";
                    } 
                               
                    else 
                    {
                        searchResult = "ING";
                    }          
                }
            
                else if (EST == true)
                {
                    searchResult = "EST";
                }

                else if (ABLE == true)
                {
                    bool BABLE = (currentWord[currentWord.Length - 5] == 'B') ? true : false;
                    bool LABLE = (currentWord[currentWord.Length - 5] == 'L') ? true : false;
                    bool MABLE = (currentWord[currentWord.Length - 5] == 'M') ? true : false;
                    bool NABLE = (currentWord[currentWord.Length - 5] == 'N') ? true : false;
                    bool PABLE = (currentWord[currentWord.Length - 5] == 'P') ? true : false;
                    bool TABLE = (currentWord[currentWord.Length - 5] == 'T') ? true : false;

                    if (TABLE == true)
                    {
                        searchResult = "TABLE";
                    }   
                
                    else if (PABLE == true)
                    {
                        searchResult = "PABLE";
                    }

                    else if (NABLE == true)
                    {
                        searchResult = "NABLE";
                    }

                    else if (MABLE == true)
                    {
                        searchResult = "MABLE";
                    }

                    else if (LABLE == true)
                    {
                        searchResult = "LABLE";
                    }

                    else if (BABLE == true)
                    {
                        searchResult = "BABLE";
                    }

                    else
                    {
                        searchResult = "ABLE";
                    }
                }
            }                         
            return searchResult;
        }

        string TakeFromDictionary (bool isNewWord, string sentence, int letterIndex, out string secondary)
        {
            secondary = "";
            placeholder++; //for next time

            if (isNewWord == true)
            {
                placeholder = 1;
            }
            
            if (placeholder - 1 < dictionaryMatch.Length)
            {
                return dictionaryMatch[placeholder - 1]; //im confident this will never crash since dictionaryMatch is defined in all possible uses of this method
            }

            else
            {
                return AdjacentEvaluation (sentence, letterIndex, out secondary); //reaching the end of a word in the dictionary either means i need to add an extension or i didnt put enough spaces 
            }
        }

        //AdjacentEvaluation is more efficient but its a complicated mess. catches anything not in the dictionary; including extensions.
        string AdjacentEvaluation (string sentence, int letterIndex, out string secondary)
        {
            const string VOWELS = "AEIOU";
            const string CONSONANTS = "BCDFGHJKLMNPQRSTVWXYZ";

            string primary = "";
            secondary = "";            

            int intBefore = (letterIndex - 1 >= 0) ? (letterIndex - 1) : letterIndex; //these wil prevent out-of-bounds exception.
            int intAfter = (letterIndex + 1 < sentence.Length) ? (letterIndex + 1) : letterIndex; 
            int intTwoAfter = (letterIndex + 2 < sentence.Length) ? (letterIndex + 2) : letterIndex;
            int intTwoBefore = (letterIndex - 2 >= 0) ? (letterIndex - 2) : letterIndex;

            string before = (intBefore != letterIndex) ? Convert.ToString (sentence[intBefore]) : " "; //these 4 strings ensure i can correctly identify seperate words.
            string after = (intAfter != letterIndex) ? Convert.ToString (sentence[intAfter]) : " "; //using strings instead of chars saves lines since i need strings for Contains()
            string twoBefore = (intTwoBefore != letterIndex && before != " ") ? Convert.ToString (sentence[intTwoBefore]) : " "; //the false path must return a space string because spaces signify the start/end of a word.
            string twoAfter = (intTwoAfter != letterIndex && after != " ") ? Convert.ToString (sentence[intTwoAfter]) : " ";        
            string currentLetter = Convert.ToString (sentence[letterIndex]);

            surroundingPhrase = twoBefore + before + currentLetter + after + twoAfter; //must update here before UnwantedMatchBypassed is used in this method.
           
            switch (currentLetter)
            {
                case "A": 
                    if (UnwantedMatchBypassed ("..AK.") && //!steak
                        UnwantedMatchBypassed ("REA..") && //!great
                        CONSONANTS.Contains (after) &&
                        IsMatch (".EA..")) //leaf
                    {
                        ;
                    }    
        
                    else if (IsMatch ("..AW." + //raw
                                     "|..AU." + //saul
                                     "|.WAT." //water
                                     ))
                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if (IsMatch ("REA.." + //break
                                     "|.EAK." + //steak
                                     "| TAB." + //table
                                     "| LAB." + //lable
                                     "|..APL" + //maple
                                     "|..AY." + //may
                                     "|.HASE" + //hase
                                     "|.RASE" + //phrase
                                     "|..ABL" + //able
                                     "|..ACE" //space
                                     ) ||

                            (IsMatch ("..AI.") && //faith
                             UnwantedMatchBypassed ("..A.R"))) // !fair,
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }

                    else if (IsMatch (".HAT." + //what
                                     "|. A ." + //a
                                     "|. AV." + //available
                                     "|..AST" //last
                                     ) ||

                            (IsMatch ("..AR.") && //far
                             UnwantedMatchBypassed ("..A.E")) ) //!fare
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }    
            
                    else if (IsMatch ("..ARE" + //compare                             
                                     "|..AIR" //fair
                                     ))
                    {
                        primary = PrettyScaryDictionary.EHH;
                    }

                    else    
                    {
                        primary = PrettyScaryDictionary.AHH; //plottable,
                    }
                    break;
            
                case "B":
                    if ((UnwantedMatchBypassed ("..B .") && //!bomb
                         UnwantedMatchBypassed (".BB..")) || //!cobber  
                                               
                         VOWELS.Contains (before)) //rob
                    {
                        if (IsMatch ("..BL.")) //able
                        {
                            primary = " ";
                            secondary = PrettyScaryDictionary.BIH;
                        }

                        else
                        {
                            primary = PrettyScaryDictionary.BIH;
                        }
                    }
                    break;
            
                case "C": 
                    if (IsMatch ("..CE." + //nice
                                "|..CI." + //complicit
                                "|..CY." //stacy
                                ))
                    {
                        primary = PrettyScaryDictionary.SIH; //sicily
                    }
            
                    else 
                    {
                        primary = PrettyScaryDictionary.KIH; //cat
                    } 
                    break;
            
                case "D":
                    if (IsMatch ("..DG.")) //judge
                    {
                        ; 
                    }
            
                    else if (UnwantedMatchBypassed (".DD..")) //ladder
                    {
                        primary = PrettyScaryDictionary.DIH;
                    }
                    break;
            
                case "E":
                    if (IsMatch ("THE .")) //the
                    {
                        primary = PrettyScaryDictionary.UHH;
                    } 
  
                    else if (IsMatch (".REA." + //great
                                     "|..EAK" + //break
                                     "|..EU." + //queue
                                     "|.EE.." + //speech
                                     "|..ELY" //lovely
                                     ) ||

                            (UnwantedMatchBypassed (".VE..") && //!lover 
                             UnwantedMatchBypassed (".EE..") && //!veer
                             IsMatch ("..ER.")) || //rubber                            
                            
                            (UnwantedMatchBypassed (" .E..") && //!be                         
                             IsMatch ("..E ."))) //tribe
                    {
                        ;
                    }    
        
                    else if (IsMatch ("..EW.")) //brew
                    {
                        primary = PrettyScaryDictionary.OOO; 
                    }

                    else if (IsMatch ("..EI." + //stein
                                     "|..EYE" //eye       
                                     ))                
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }         
       
                    else if (IsMatch ("..EE." + //engineer
                                     "|..EA." + //lead
                                     "|.DE.." + //deal
                                     "|..E.D" + //lead
                                     "| ME ." + //me
                                     "| HE ." + //he
                                     "| WE ." + //we
                                     "| BE ." + //be
                                     "|YBE ." + //maybe
                                     "|..ESE" + //these
                                     "|.KEY." + //key
                                     "| RE.." + //remember
                                     "|.IE. " //trekkies
                                     ))
                    {                           
                        primary = PrettyScaryDictionary.EEE;
                    }  
            
                    else if ((UnwantedMatchBypassed ("..EE.") && //!feet
                              UnwantedMatchBypassed ("..ER.") && //!later
                              UnwantedMatchBypassed ("..EW.") && //!brew
                              UnwantedMatchBypassed ("..E. ")) || //!stakes

                              IsMatch ("..ERE" + //there
                                      "|.VER." + //veto
                                      "|..ED." + //plated
                                      "|..ES " + //dresses
                                      "|..ET " //planet
                                      ))
                    {                                         
                        primary = PrettyScaryDictionary.EHH;  //such as silent E, there, fate
                    }   

                    else if (IsMatch (".REY.")) //osprey
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }
                    break;
            
                case "F": 
                    primary = PrettyScaryDictionary.FIH; //follow
                    break;
            
                case "G":
                    if (IsMatch (".GG.." + //trigger
                                "|.HG.." + //high
                                "|.NG ." + //talking
                                "|.IGN." //design
                                ))
                    {
                        ;
                    }

                    else if  ((UnwantedMatchBypassed ("..G.T") && //!git
                               IsMatch (". GI.")) || //gibberish
                                
                               IsMatch (". GY." + //gym
                                       "|.DGE." //judgement 
                                       ))
                    {   
                        primary = PrettyScaryDictionary.JIH;
                        secondary = " "; //such as "gin", judgement, 
                    }
            
                    else
                    {
                        primary = PrettyScaryDictionary.GIH; //given
                    }    
                    break;
            
                case "H":
                    if (IsMatch ("..HN." + //john
                                "|.GH.." + //dough
                                "|.OH.." + //pharoah
                                "|.PH.." //autograph
                                ) ||

                       (UnwantedMatchBypassed ("..HU.") && //!github
                        IsMatch (".TH.."))) //thigh
                    {
                        ;
                    }

                    else
                    {
                        primary = PrettyScaryDictionary.HIH;
                    }    
                    break;

                case "I":
                    if (UnwantedMatchBypassed ("FLIES") && //!flies
                        UnwantedMatchBypassed ("PLIES") && //!applies
                        IsMatch ("..IES")) //activities
                    {
                        ;
                    }

                    else if (IsMatch (".TION" + //traction
                                     "|.SION" //aggression
                                     ))
                    {
                        primary = PrettyScaryDictionary.SIH;
                        secondary = PrettyScaryDictionary.HIH;
                    }

                    else if (IsMatch ("..IKE" + //pike
                                     "|KNI.." + //knight
                                     "|..IGH" + //light
                                     "|. I ." + //I
                                     "|..IE." + //skies
                                     "|..ILE" + //filed
                                     "|..ITE" + //kite
                                     "|..IGN" //sign
                                     ) ||

                            (UnwantedMatchBypassed (".RI..") && //!ring
                             UnwantedMatchBypassed (".AI..") && //!rail
                             CONSONANTS.Contains (after) && //bike
                             VOWELS.Contains (twoAfter)) || //spite

                            (UnwantedMatchBypassed (".GI..") && //!origin
                             IsMatch ("..I.E"))) //aborigine
                    {   
                        primary = PrettyScaryDictionary.EYE;
                    }
            
                    else if (IsMatch (".OIN." + //point
                                     "|..ING" //running
                                     ))
                    {
                        primary = PrettyScaryDictionary.EEE; //such as running
                    }
    
                    else 
                    {
                        primary = PrettyScaryDictionary.IHH;  //felicity
                    }
                    break;
            
                case "J":   
                    primary = PrettyScaryDictionary.JIH; //jelly
                    break;
            
                case "K":   
                    if (UnwantedMatchBypassed (".CK..") && //!two kih's
                        UnwantedMatchBypassed ("..KN.")) //!silent K
                    {
                        primary = PrettyScaryDictionary.KIH;
                    }    
                    break;
            
                case "L": 
                    if (UnwantedMatchBypassed ("..LK.") && //!silent L
                        UnwantedMatchBypassed ("..LF.") && //!silent L
                        UnwantedMatchBypassed (".LL..")) //!double L's
                    {
                        primary = PrettyScaryDictionary.LIH; //silent L, caller,
                    }
                    break;
            
                case "M":   
                    if (UnwantedMatchBypassed (".MM..")) //!double M's
                    {                        
                        primary = " ";
                        secondary = PrettyScaryDictionary.MIH; //such as "molten", drummer,
                    }    
                    break;
            
                case "N":  
                    if (UnwantedMatchBypassed (".NN..")) //double N's
                    {    
                        primary = " ";
                        secondary = PrettyScaryDictionary.NIH;  //such as "nickel", planner, 
                    }    
                    break;
            
                case "O":    
                    if (IsMatch (".TOU." + //touch
                                "|.OO.." + //double O's
                                "|.WOR." //word
                                ))
                    {
                        ;
                    }                    
                        
                    else if (IsMatch ("..OR." + //lore
                                     "|..OI." //annoint
                                     ) ||

                             (UnwantedMatchBypassed (".SO..") && //sour
                              IsMatch ("..OUR"))) //four
                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if (IsMatch (".FOU." + //foul
                                     "|.POU." + //pouch
                                     "|.LOU." //slouch
                                     ))
                    {
                        primary = PrettyScaryDictionary.AHH; 
                    }
 
                    else if (IsMatch ("..OM." + //computer
                                     "|.ION." //champion
                                     ))
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }   
            
                    else if (IsMatch ("..O ." + //pro
                                     "|.SOU." + //soul
                                     "|..OW." + //bestow
                                     "|.BOTH" //both
                                     ) ||

                            (CONSONANTS.Contains (after) && //sole
                             VOWELS.Contains (twoAfter))) //solo
                    {
                        primary = PrettyScaryDictionary.OWE;
                    }   
            
                    else if (IsMatch ("..OHN" +  //john
                                     "|.BOT " //bot
                                     ) ||

                            (CONSONANTS.Contains (before) && 
                             IsMatch ("..OL.")) || //told

                            (UnwantedMatchBypassed ("..OO.") && //!oolacile
                             IsMatch (". O..")) || //objective

                            (UnwantedMatchBypassed (".BO..") && //!both
                             IsMatch ("..OTH"))) //sloth
                    {
                        primary = PrettyScaryDictionary.HOH;                      
                    }    
            
                    else if (IsMatch ("..OO." + //fool
                                     "|..OV." + //improve
                                     "|..OU." + //route
                                     "|.TOD." //today
                                     ))
                    {
                        primary = PrettyScaryDictionary.OOO;
                    }

                    else if (IsMatch ("..OX." + //oxygen
                                     "|..OF." //of
                                     ) ||
                        
                            (UnwantedMatchBypassed (".OO..") && //!cool
                             IsMatch ("..OL."))) //collected
                    {
                        primary = PrettyScaryDictionary.HOH;  
                    }   

                    else
                    {
                        primary = PrettyScaryDictionary.OWE;
                    }
                    break;
            
                case "P":
                    if (IsMatch ("..PH.")) //phrase
                    {
                        primary = PrettyScaryDictionary.FIH;
                    }   

                    else if (UnwantedMatchBypassed (".PP..")) //double P's
                    {
                        primary = PrettyScaryDictionary.PIH;
                    }                    
                    break;
            
                case "Q":                    
                    primary = PrettyScaryDictionary.KIH; //query    
                    secondary = PrettyScaryDictionary.WIH;
                    break;               
            
                case "R":   
                    if (UnwantedMatchBypassed (".RR..") && //!double R's
                        UnwantedMatchBypassed ("OUR..")) //!your
                    {                        
                        primary = PrettyScaryDictionary.RIH;
                    }
                    break;
            
                case "S":   
                    if (IsMatch ("..SM.")) //prism
                    {
                        primary = PrettyScaryDictionary.ZIH;
                    }

                    else if (UnwantedMatchBypassed (".SS..")) //!double S's
                    {
                        primary = PrettyScaryDictionary.SIH;
                    }                  
                    break;
            
                case "T": 
                    if (UnwantedMatchBypassed ("..T.U") && //!github
                        IsMatch ("..TH.")) //think
                    {
                        primary = " ";
                        secondary = PrettyScaryDictionary.THI;    
                    } 

                    else if (UnwantedMatchBypassed (".TT..")) //!double T's
                    {
                        if (IsMatch (".ST..")) //emphasised T
                        {
                            primary = " ";
                            secondary = PrettyScaryDictionary.TIH;
                        }
                        
                        else
                        {
                            primary = PrettyScaryDictionary.TIH;
                        }                        
                    }    
                    break;
            
                case "U":
                    if (IsMatch (".QU.." + //queue
                                "|.AU.." + //caught
                                "|.OUL." + //soul
                                "|YOU.." + //you
                                "|.AUT." + //astronaut
                                "|.AUL." //assault
                                ) ||

                       (UnwantedMatchBypassed (".OU..") && //!your
                        IsMatch ("..UR."))) //purr
                    {
                        ;
                    }    

                    else if (IsMatch (".EU.." + //eulogy
                                     "|..UE." + //cruelty
                                     "|.AU ." + //aboiteau
                                     "|.AUX " + //aboiteaux
                                     "|.RU.." + //rude
                                     "|..UI." //ruin
                                     ))
                    {
                        primary = PrettyScaryDictionary.OOO;
                    }

                    else if (IsMatch (" OUR." + //end
                                     "|.PULL" + //pull
                                     "|.OUC." + //touch
                                     "|. UN." + //undeveloped
                                     "|. UP." + //update
                                     "|.SUB." + //submit
                                     "|.HUB." //hub
                                     ) ||

                            (UnwantedMatchBypassed ("..UDE") && //!prude
                             UnwantedMatchBypassed ("..UDI") && //!ludicrous
                             IsMatch ("..UD.")) || //crud                                                    

                            (CONSONANTS.Contains (before) && //cut
                             UnwantedMatchBypassed ("..U.E") && //brute
                             IsMatch ("..UT." + //but
                                     "|..UC." //obstruct
                                     )))
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }   

                    else if (IsMatch (".BUY." + //buy
                                     "|.GUY." //guy
                                     ))
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else if (IsMatch (".PULE")) //opulence
                    {
                        primary = PrettyScaryDictionary.YIH;
                        secondary = PrettyScaryDictionary.OOO;
                    }
                    break;
            
                case "V":    
                    primary = PrettyScaryDictionary.VIH;
                    break;
            
                case "W":
                    if (UnwantedMatchBypassed ("..W .")) //narrow
                    {
                        primary = PrettyScaryDictionary.WIH;
                    }                   
                    break;
            
                case "X":   
                    if (IsMatch (". X..")) //xylophone
                    {
                        primary = PrettyScaryDictionary.ZIH; 
                    }    
        
                    else  
                    {
                        primary = PrettyScaryDictionary.KSS;                 
                    }
                    break;
            
                case "Y":
                    if (IsMatch (".UY ." + //soliloquy
                                "|.EY ." + //key
                                "|.AY.." + //maybe
                                "|.EYE." //eye
                                ))                                              
                    {
                        ; 
                    }

                    else if (IsMatch (".CYC." + //bicycle
                                     "|.MY.." + //my
                                     "|.HY.." + //hyena
                                     "|FLY.." + //fly
                                     "|.KY.." //sky
                                     ) ||

                            (UnwantedMatchBypassed ("..Y .") && //!possibility     
                             IsMatch (".TY.."))) //style                             
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else if ((IsMatch ("LLY ." + //totally
                                      "|.LY ." + //likely
                                      "|.RY ." + //chivalry
                                      "|.BY ." + //baby
                                      "|.TY ." //ability
                                      )))                                   
                    {
                        primary = PrettyScaryDictionary.EEE; //such as "flaky", negatively
                    }
            
                    else  
                    {
                        primary = PrettyScaryDictionary.YIH;  //such as "yam".
                    }
                    break;
            
                case "Z":  
                    primary = PrettyScaryDictionary.ZIH;
                    break;
            
                case " ": 
                    primary = " ";
                    break;
            }
            return primary;
        }

        //helps cut down on text needed and is easier to understand
        bool IsMatch (string pattern) //SURROUNDINGPHRASE MUST EXIST BEFORE USE.
        {
            return Regex.IsMatch (surroundingPhrase, pattern);
        }

        //returns true when the unwanted phrase
        bool UnwantedMatchBypassed (string pattern) 
        {
            return !Regex.IsMatch (surroundingPhrase, pattern);
        }
    }
}