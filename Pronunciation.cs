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
            
                if (S == true)
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

            int int_before = (letterIndex - 1 >= 0) ? (letterIndex - 1) : letterIndex; //these wil prevent out-of-bounds exception.
            int int_after = (letterIndex + 1 < sentence.Length) ? (letterIndex + 1) : letterIndex; 
            int int_two_after = (letterIndex + 2 < sentence.Length) ? (letterIndex + 2) : letterIndex;
            int int_two_before = (letterIndex - 2 >= 0) ? (letterIndex - 2) : letterIndex;

            string before = (int_before != letterIndex) ? Convert.ToString (sentence[int_before]) : " "; //these 4 strings ensure i can correctly detect the start and end of a sentence.
            string after = (int_after != letterIndex) ? Convert.ToString (sentence[int_after]) : " "; //using strings instead of chars saves lines since i need strings for Contains()
            string two_before = (int_two_before != letterIndex) ? Convert.ToString (sentence[int_two_before]) : " "; //the false path must return a space string.
            string two_after = (int_two_after != letterIndex) ? Convert.ToString (sentence[int_two_after]) : " ";        
            string current_letter = Convert.ToString (sentence[letterIndex]);

            switch (current_letter)
            {
                case "A": 
                    if (before == "E" &&
                        CONSONANTS.Contains (after) &&
                        after != "K")
                    {
                        ; //such as leaf, meat, real
                    }    
    
                    else if (before == "E" &&
                             after == "D" &&
                             two_after == "E")
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
                              after == "K") || 
                             (after == "I" && 
                              after != "R") ||
                              after == "Y" ||
                            ((after == "C" || //space, ate, rake
                              after == "T" ||
                              after == "K" ||
                              after == "L") && 
                              two_after == "E") ||
                            ((before == "T" ||
                              before == "L") && //table
                              two_before == " " &&
                              after == "B") ||
                             (after == "P" && //maple
                              two_after == "L") ||
                              after == "Y")     
                    {
                        primary = PrettyScaryDictionary.AEE; // "break",
                    }

                    else if ((before == "H" && //what
                              after == "T") ||
                             (after == "R" &&
                              two_after != "E") || //far
                             (before == " " &&
                              after == " ") || //a
                             (after == "S" && //last
                              two_after == "T") )
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }    
            
                    else if (after == "R" && //compare, ware, hare, stare, care
                             two_after == "E") 
                    {
                        primary = PrettyScaryDictionary.EHH;
                    }

                    else    
                    {
                        primary = PrettyScaryDictionary.AHH; //plottable,
                    }
                    break;
            
                case "B":
                    if (before != "M" && //silent B
                        before != "B") //cobber
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
                    if (two_before == "T" &&
                        after == " ")
                    {
                        primary = PrettyScaryDictionary.UHH; //such as the
                    } 
  
                    else if ((after == "A" &&
                              two_after == "K") ||
                              after == "U" ||
                             (after == "M" &&
                              before == "R") ||
                              before == "E" ||
                             (after == "R" &&
                              before != "V" &&
                              before != "E") ||
                             (after == " " &&
                              before != "B" &&
                              before != "H" &&
                              before != "M" &&
                              before != "W") ||
                             (after == "L" &&
                              two_after == "Y") )                              
                    {
                        ; //such as late, cake, break, steak, queue, requirement, speech,
                    }    
        
                    else if (after == "W")
                    {
                        primary = PrettyScaryDictionary.OOO; //such as brew
                    }

                    else if ((after == "Y" &&
                              two_after == "E") ||
                              after == "I")
                    {
                        primary = PrettyScaryDictionary.EYE; //such as EYE 
                    }         
       
                    else if (after == "E" ||
                             after == "A" ||
                             before == "D" ||
                             two_after == "D" ||
                           ((before == "M" ||
                             before == "H" ||
                             before == "W" ||
                             before == "B") &&
                             after == " ") ||
                            (after == "S" &&
                             two_after == "E") ||
                            (before == "I" && //ies
                             two_after == " ") ||
                            (before == "K" && //key
                             after == "Y"))
                    {                           
                        primary = PrettyScaryDictionary.EEE;    //such as "engineer", speech, me
                    }  
            
                    else if ((after != "E" && 
                              after != "R" &&
                              after != "W" &&
                              two_after != " ") ||   
                             (after == "R" &&
                             (two_after == "E" ||
                              before == "V")) ||
                              after == "D" ||
                             (after == "S" && //es
                              two_after == " ") || 
                             (after == "T" &&
                              two_after == " ") )
                    {                                         
                        primary = PrettyScaryDictionary.EHH;  //such as silent E, there, fate
                    }   

                    else if (two_before == "R" && //prey
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
                    if  (after == "E" || 
                         before == "D" ||
                         after == "I" || 
                         after == "Y")
                    {   
                        secondary = PrettyScaryDictionary.JIH;
                        primary = " "; //such as "gin", high, judgement, RNG
                    }
            
                    else if (after != "H" && //high
                             before != "G" && //lagg
                            (before != "N" &&
                             after != " ")) //ing
                    {
                        primary = PrettyScaryDictionary.GIH;
                    }    
                    break;
            
                case "H":
                    if (after != "N" &&
                        before != "T" &&
                        before != "G" &&
                        before != "O")
                    {
                        primary = PrettyScaryDictionary.HIH; //such as sign, THIgh, highest
                    }    
                    break;

                case "I":
                    if ((after == "E" && //ies
                         two_after == "S") )
                    {
                        ;
                    }

                    else if (after == "O" &&
                        two_after == "N")
                    {
                        primary = PrettyScaryDictionary.SIH;
                        secondary = PrettyScaryDictionary.HIH;
                    }

                    else if (after == "K" ||
                            (before != "R" &&
                             two_after != "L" && 
                             after == "S" &&   
                             two_after == "D" &&
                            (CONSONANTS.Contains (after) && 
                             VOWELS.Contains (two_after)) ||
                            (two_before == "K" &&
                             before == "N") ||
                             after == "G") ||
                            (two_after == "E" &&
                             before != "G") ||
                            (before == " " &&
                             after == " ") ||
                             after == "E") 
                    {   
                        primary = PrettyScaryDictionary.EYE; //such as "filed", light, kite
                    }
            
                    else if (two_after == "G")
                    {
                        primary = PrettyScaryDictionary.EEE; //such as running
                    }
    
                    else if (after != "E")  
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
                        
                    else if ((after == "R" && //for, lore, or, bore, core, store, tore,
                             (before == "F" || 
                              before == "L" ||
                              before == " " ||
                              before == "B" ||
                              before == "C" ||
                              before == "T")) || 
                             (after == "U" && //four
                              two_after == "R" &&
                              before != "S"))
                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if (after == "U" &&
                             before == "F")
                    {
                        primary = PrettyScaryDictionary.AHH; //foul
                    }
 
                    else if (after == "M" ||
                            (before == "I" &&
                             after == "N"))
                    {
                        primary = PrettyScaryDictionary.UHH; //such as computer, coming.
                    }   
            
                    else if (after == " " ||
                            (before == "S" &&
                             after == "U") ||
                             after == "H" ||
                            (after == "W"))
                    {
                        primary = PrettyScaryDictionary.OWE; //such as hello, soul
                    }   
            
                    else if ((CONSONANTS.Contains (before) &&
                              before != "Y" &&
                             (after != "V" ||
                              before == "H")) )
                    {
                        primary = PrettyScaryDictionary.HOH; //such as told
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
                    break;
            
                case "P":   
                    if (before != "P") //potatoes, stoppable
                    {
                        primary = PrettyScaryDictionary.PIH;
                    }                    
                    break;
            
                case "Q":                    
                    primary = PrettyScaryDictionary.KIH; //such as "query".     
                    secondary = PrettyScaryDictionary.WIH;
                    break;               
            
                case "R":   
                    if (before != "R" && //sparring
                        before != "O" && //for
                        two_before != "O") //four
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
                    if (after == "H") 
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
                       (two_before == "Y" ||
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
                             after == "L"))
                    {
                        primary = PrettyScaryDictionary.OOO;  //such as "cruelty", "eulogy".
                    }

                    else if (after == "C" ||
                             before == "O" ||
                             after == "D" ||
                             after == "N") //un
                    {
                        primary = PrettyScaryDictionary.UHH; //such as touch, cut
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
                    if (after == "E" || //eye
                        before == "A" || //maybe
                        before == "E") //key                                      
                    {
                        ; 
                    }

                    else if ((before == "C" && //bicycle
                              after == "C") || 
                             (before == "T" && //style
                              after != " ") || //possibility
                              before == "M" ||
                              before == "H" ||
                              before == "L" || //fly
                              before == "K")  //sky
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else if ((((two_before == "L" && 
                                 before == "L") || 
                                before == "L" ||
                                before == "R") &&
                                after == " ") ||
                                after == "B" ||
                                VOWELS.Contains (two_before) ||
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