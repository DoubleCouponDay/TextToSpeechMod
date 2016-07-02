using System;

namespace SETextToSpeechMod
{
    class Pronunciation 
    {
        //pronunciation reference: http://www.englishleap.com/other-resources/learn-english-pronunciation
        const int NEW_WORD = -1;
        const int NO_MATCH = -2; 
        const int MAX_EXTENSION_SIZE = 5; 
        int placeholder = NEW_WORD;
        string[] dictionaryMatch;
        WordCounter wordCounter;   

        public Pronunciation (string inputSentence)
        {
            wordCounter = new WordCounter (inputSentence);
        }

        //first searches the ditionary, then tries the secondary pronunciation if no match found.
        public string GetLettersPronunciation (string sentence, int letterIndex, out string secondary) 
        {
            secondary = "";
            string primaryPhoneme;        
            string currentWord = wordCounter.GetCurrentWord (NEW_WORD, ref placeholder); //this update is needed every time i increment a letter.          

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

            string before = (intBefore != letterIndex) ? Convert.ToString (sentence[intBefore]) : " "; //these 4 strings ensure i can correctly detect the start and end of a sentence.
            string after = (intAfter != letterIndex) ? Convert.ToString (sentence[intAfter]) : " "; //using strings instead of chars saves lines since i need strings for Contains()
            string twoBefore = (intTwoBefore != letterIndex) ? Convert.ToString (sentence[intTwoBefore]) : " "; //the false path must return a space string.
            string twoAfter = (intTwoAfter != letterIndex) ? Convert.ToString (sentence[intTwoAfter]) : " ";        
            string currentLetter = Convert.ToString (sentence[letterIndex]);

            switch (currentLetter)
            {
                case "A": 
                    if ((before == "E" &&
                         twoBefore != "R") &&
                         CONSONANTS.Contains (after) &&
                         after != "K")
                    {
                        ; //such as leaf, meat, real
                    }    
    
                    else if (before == "E" &&
                             after == "D" &&
                             twoAfter == "E")
                    {
                        primary = PrettyScaryDictionary.EEE;  //such as leader.                
                    }
        
                    else if (before == "U" ||  
                             after == "W" ||
                             after == "U" ||
                            (before == "W" && 
                             after == "T")) 
                    {
                        primary = PrettyScaryDictionary.AWW; //such as "raw", water
                    }

                    else if ((before == "E" && //break, steak
                             (twoBefore == "R" || //great
                              after == "K")) || 
                             (after == "I" && 
                              after != "R") ||
                             (CONSONANTS.Contains (after) && //THE REPLACEMENT. space, ate, rake, grade
                              twoAfter == "E") ||
                            ((before == "T" ||
                              before == "L") && //table
                              twoBefore == " " &&
                              after == "B") ||
                             (after == "P" && //maple
                              twoAfter == "L") ||
                              after == "Y" ||
                            ((before == "H" ||
                              before == "R") && //phrase
                              after == "S" &&
                              twoAfter == "E") ||
                             (after == "B" && //able
                              twoAfter == "L"))     
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }

                    else if ((before == "H" && //what
                              after == "T") ||
                             (after == "R" &&
                              twoAfter != "E") || //far
                             (before == " " && 
                             (after == " " || //a
                              after == "V")) || //available
                             (after == "S" && //last
                              twoAfter == "T") )
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }    
            
                    else if (after == "R" && //compare, ware, hare, stare, care
                             twoAfter == "E") 
                    {
                        primary = PrettyScaryDictionary.EHH;
                    }

                    else    
                    {
                        primary = PrettyScaryDictionary.AHH; //plottable,
                    }
                    break;
            
                case "B":
                    if ((after != " " && //bomb
                         before != "B") || //cobber
                         VOWELS.Contains (before) ) //bob, silent B, cobber
                    {
                        if (after == "L")
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
                    if (after == "E" || 
                        after == "I" || 
                        after == "Y")
                    {
                        primary = PrettyScaryDictionary.SIH; //such as "sicily".
                    }
            
                    else 
                    {
                        primary = PrettyScaryDictionary.KIH; //such as "cat".
                    } 
                    break;
            
                case "D":
                    if (after == "G")
                    {
                        ; //such as judge
                    }
            
                    else if (before != "D")
                    {
                        primary = PrettyScaryDictionary.DIH; //such as ladder.
                    }
                    break;
            
                case "E":
                    if (twoBefore == "T" &&
                        after == " ")
                    {
                        primary = PrettyScaryDictionary.UHH; //such as the
                    } 
  
                    else if ((after == "A" && //late
                             (before == "R" || 
                              twoAfter == "K")) || //steak, break
                              after == "U" ||
                              before == "E" || //bee, speech
                             (after == "R" && //ber
                              before != "V" &&
                              before != "E") ||
                             (after == " " && //silent e at end
                              twoBefore != " ") ||
                             (after == "L" &&
                              twoAfter == "Y") )                              
                    {
                        ; //such as queue, requirement, speech,
                    }    
        
                    else if (after == "W")
                    {
                        primary = PrettyScaryDictionary.OOO; //such as brew
                    }

                    else if ((after == "Y" &&
                              twoAfter == "E") ||
                              after == "I")
                    {
                        primary = PrettyScaryDictionary.EYE; //such as EYE 
                    }         
       
                    else if (after == "E" ||
                             after == "A" ||
                             before == "D" ||
                             twoAfter == "D" ||
                           ((before == "M" ||
                             before == "H" ||
                             before == "W" ||
                             before == "B") &&
                             after == " " &&
                             twoBefore == " ") ||
                            (twoBefore == "Y" && //maybe
                             before == "B" &&
                             after == " ") ||
                            (after == "S" &&
                             twoAfter == "E") ||
                            (before == "I" && //ies
                             twoAfter == " ") ||
                            (before == "K" && //key
                             after == "Y") ||
                            (twoBefore == " " && //re
                             before == "R") )
                    {                           
                        primary = PrettyScaryDictionary.EEE;    //such as "engineer", speech, me
                    }  
            
                    else if ((after != "E" && 
                              after != "R" &&
                              after != "W" &&
                              twoAfter != " ") ||   
                             (after == "R" &&
                             (twoAfter == "E" ||
                              before == "V")) ||
                              after == "D" ||
                             (after == "S" && //es
                              twoAfter == " ") || 
                             (after == "T" &&
                              twoAfter == " ") )
                    {                                         
                        primary = PrettyScaryDictionary.EHH;  //such as silent E, there, fate
                    }   

                    else if (twoBefore == "R" && //prey
                              before == "E" ||
                              after == "Y")
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }
                    break;
            
                case "F": 
                    primary = PrettyScaryDictionary.FIH; //such as "follow".
                    break;
            
                case "G":
                    if (before == "G" || //trigger
                        before == "H" || //high
                       (before == "N" && //ing
                        after == " ") ||
                       (before == "I" &&
                        after == "N") ) //design
                    {
                        ;
                    }

                    else if  (after == "E" &&
                         before == "D" ||
                        (after == "I" && 
                         twoAfter != "T") || 
                         after == "Y")
                    {   
                        secondary = PrettyScaryDictionary.JIH;
                        primary = " "; //such as "gin", judgement, RNG
                    }
            
                    else
                    {
                        primary = PrettyScaryDictionary.GIH; //git,
                    }    
                    break;
            
                case "H":
                    if (after == "N" ||
                       (before == "T" && //thigh
                        after != "U") || //github
                        before == "G" || //high
                        before == "O" ||
                        before == "P")
                    {
                        ;
                    }

                    else
                    {
                        primary = PrettyScaryDictionary.HIH;
                    }    
                    break;

                case "I":
                    if ((after == "E" && //ies
                         twoAfter == "S") )
                    {
                        ;
                    }

                    else if (after == "O" && //tion
                        twoAfter == "N" &&
                        (before == "T" ||
                        before == "S") )
                    {
                        primary = PrettyScaryDictionary.SIH;
                        secondary = PrettyScaryDictionary.HIH;
                    }

                    else if (after == "K" || //pike
                            (CONSONANTS.Contains (after) && //we'll have to wait and see if this is a universal rule. if its not it has to be broken up.
                             VOWELS.Contains (twoAfter) &&
                             before != "R" &&
                             twoAfter != "L" &&
                             before != "A") || //sail
                            (twoBefore == "K" &&
                             before == "N") ||
                            (after == "G" && //light
                             twoAfter == "H") ||
                            (twoAfter == "E" &&
                             before != "G") ||
                            (before == " " &&
                             after == " ") ||
                             after == "E" || //pie
                           ((after == "L" || //filed
                             after == "T") && //kite
                             twoAfter == "E") ||
                            (after == "G" && 
                             twoAfter == "N") ) //sign 
                    {   
                        primary = PrettyScaryDictionary.EYE;
                    }
            
                    else if ((after == "N" && //running
                              twoAfter == "G") ||
                              before == "O") //point
                    {
                        primary = PrettyScaryDictionary.EEE; //such as running
                    }
    
                    else 
                    {
                        primary = PrettyScaryDictionary.IHH;  //such as 'felicity'.
                    }
                    break;
            
                case "J":   
                    primary = PrettyScaryDictionary.JIH; //such as "jelly".
                    break;
            
                case "K":   
                    if (after != "N" &&
                        before != "C")
                    {
                        primary = PrettyScaryDictionary.KIH; //such as silent K
                    }    
                    break;
            
                case "L": 
                    if (after != "K" && 
                        after != "F" &&
                        before != "L") 
                    {
                        primary = PrettyScaryDictionary.LIH; //silent L, caller,
                    }
                    break;
            
                case "M":   
                    if (before != "M")
                    {                        
                        primary = " ";
                        secondary = PrettyScaryDictionary.MIH; //such as "molten", drummer,
                    }    
                    break;
            
                case "N":  
                    if (before != "N")
                    {    
                        primary = " ";
                        secondary = PrettyScaryDictionary.NIH;  //such as "nickel", planner, 

                    }    
                    break;
            
                case "O":    
                    if ((after == "U" && //touch
                         before == "T") ||
                         before == "O" ||
                        (before == "W" && //word
                         after == "R"))
                    {
                        ;
                    }                    
                        
                    else if (after == "R" || //for, lore, or, bore, core, store, tore, support
                            (after == "U" && //four
                             twoAfter == "R" &&
                             before != "S") ||
                             after == "I") //point, annoint, soil
                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if ((after == "U" && //foul
                             (before == "F" ||
                              before == "P" || //pouch
                              before == "L")) ) //slouch
                    {
                        primary = PrettyScaryDictionary.AHH; 
                    }
 
                    else if (after == "M" ||
                            (before == "I" &&
                             after == "N"))
                    {
                        primary = PrettyScaryDictionary.UHH; //such as computer, coming.
                    }   
            
                    else if (after == " " || //pro
                            (before == "S" &&
                             after == "U") ||
                             after == "W" ||
                            (CONSONANTS.Contains (after) && //sole, solo
                             VOWELS.Contains (twoAfter)) ||
                             (before == "B" && //both
                              after == "T" && 
                              twoAfter == "H"))
                    {
                        primary = PrettyScaryDictionary.OWE; //such as hello, soul
                    }   
            
                    else if ((CONSONANTS.Contains (before) && //told, sold, hold, gold, bold 
                              after == "L") ||
                             (before == " " &&
                              after != "O") ||
                             (after == "H" && //john
                              twoAfter == "N") ||
                             ((after == "T" && //sloth, cloth
                              twoAfter == "H") &&
                              before != "B") ||
                             (before == "B" && //bot
                              after == "T" &&
                              twoAfter == " ") )
                    {
                        primary = PrettyScaryDictionary.HOH;                      
                    }    
            
                    else if (after == "O" ||
                             after == "V" ||
                             after == "U" ||
                            (before == "T" &&
                             after == "D") )
                    {
                        primary = PrettyScaryDictionary.OOO; //such as 'fool', today
                    }

                    else if (after == "X" ||
                             after == "L" &&
                             before != "O" ||
                             after == "F")
                    {
                        primary = PrettyScaryDictionary.HOH;  //such as "oxygen".    
                    }   

                    else
                    {
                        primary = PrettyScaryDictionary.OWE;
                    }
                    break;
            
                case "P":
                    if (after == "H") //phrase
                    {
                        primary = PrettyScaryDictionary.FIH;
                    }   

                    else if (before != "P") //potatoes, stoppable
                    {
                        primary = PrettyScaryDictionary.PIH;
                    }                    
                    break;
            
                case "Q":                    
                    primary = PrettyScaryDictionary.KIH; //such as "query".     
                    secondary = PrettyScaryDictionary.WIH;
                    break;               
            
                case "R":   
                    if (before != "R" /*&& //sparring
                        before != "O" && //for
                        twoBefore != "O"*/) //four
                    {                        
                        primary = PrettyScaryDictionary.RIH;
                    }
                    break;
            
                case "S":   
                    if (after == "M")
                    {
                        primary = PrettyScaryDictionary.ZIH; //such as prism
                    }

                    else if (before != "S")
                    {
                        primary = PrettyScaryDictionary.SIH; //such as "slippery".
                    }                  
                    break;
            
                case "T": 
                    if (after == "H" &&
                        twoAfter != "U") //github 
                    {
                        primary = " ";
                        secondary = PrettyScaryDictionary.THI; //such as "THInk".    
                    } 

                    else if (before != "T") //attic
                    {
                        if (before == "S") //est
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
                    if (before == "Q" || //queue
                        before == "A" ||
                       (after == "R" &&
                        before != "O") ||
                       (before == "O" && //you
                       (twoBefore == "Y" ||
                        after == "L")) )  //soul
                    {
                        ;
                    }    

                    else if (before == "O" && //your
                             after == "R")
                    {
                        primary = " ";
                    }

                    else if (before == "E" || 
                             after == "E" ||
                             before == "A" ||
                             before == "R" || 
                             after == "I" ||
                            (before == "P" &&
                             after == "L") )
                    {
                        primary = PrettyScaryDictionary.OOO;  //such as "cruelty", "eulogy".
                    }

                    else if (before == "O" ||
                             after == "D" ||
                            (after == "N" && //un
                             before == " ") ||
                             after == "P" || //update
                            (before == "S" && //submit
                             after == "B") ||
                            (CONSONANTS.Contains (before) && //but, cut
                            (after == "T" ||
                             after == "C")) ||
                            (before == "H" && //hub
                             after == "B") ) 
                    {
                        primary = PrettyScaryDictionary.UHH; //such as touch, cut
                    }   

                    else if ((before == "B" || //buy
                             before == "G") &&  //guy
                             after == "Y")
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else
                    {
                        primary = PrettyScaryDictionary.YIH; //fortune
                        secondary = PrettyScaryDictionary.OOO;
                    }
                    break;
            
                case "V":    
                    primary = PrettyScaryDictionary.VIH; //such as "vector".
                    break;
            
                case "W":
                    if (after != " ") //narrow, raw
                    {
                        primary = PrettyScaryDictionary.WIH;
                    }                   
                    break;
            
                case "X":   
                    if (before == " ") 
                    {
                        primary = PrettyScaryDictionary.ZIH;   //such as "xylophone".  
                    }    
        
                    else  
                    {
                        primary = PrettyScaryDictionary.KSS; //such as "exit".                   
                    }
                    break;
            
                case "Y":
                    if (((before == "U" || //buy
                          before == "E") && //key
                          after == " ") ||
                          before == "A" || //maybe                                     
                         (after == "E" && //eye
                          before == "E") )
                    {
                        ; 
                    }

                    else if ((before == "C" && //bicycle
                              after == "C") || 
                             (before == "T" && //style
                              after != " ") || //possibility
                              before == "M" ||
                              before == "H" ||
                             (before == "L" && //fly
                              twoBefore == "F") ||
                              before == "K")  //sky
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else if ((((twoBefore == "L" && 
                                 before == "L") || 
                                before == "L" ||
                                before == "R") &&
                                after == " ") ||
                                after == "B" ||
                                VOWELS.Contains (twoBefore) ||
                                before == "T")  
                    {
                        primary = PrettyScaryDictionary.EEE; //such as "flaky", negatively
                    }
            
                    else  
                    {
                        primary = PrettyScaryDictionary.YIH;  //such as "yam".
                    }
                    break;
            
                case "Z":  
                    primary = PrettyScaryDictionary.ZIH; //such as "zorro".
                    break;
            
                case " ": 
                    primary = " "; //such as "...dot dot dot im a pretentious wanker dot dot dot..."
                    break;
            }
            return primary;
        }
    }
}