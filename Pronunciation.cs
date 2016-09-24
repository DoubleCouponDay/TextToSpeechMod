using System;
using System.Text.RegularExpressions; //needed to match wildcard string matches simplify the rule based phoneme approach AdjacentEvaluation().
using System.Collections.Generic;

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
        public bool usedDictionary {get; private set;}
        public int wrongFormatMatchers {get; private set;}
        public int wrongFormatNonMatchers {get; private set;}

        public WordCounter wordCounter {get; private set;}

        public Pronunciation (string inputSentence)
        {
            this.wordCounter = new WordCounter (inputSentence);
        }

        //first searches the ditionary, then tries the secondary pronunciation if no match found.
        public List <string> GetLettersPronunciation (string sentence, int letterIndex) 
        {
            List <string> results = new List <string>();
            wordCounter.IncrementCurrentPosition (placeholder); //this update is needed every time i increment a letter.      
            placeholder = wordCounter.placeholder;         
            string currentWord = wordCounter.currentWord;

            if (currentWord != " ")
            {                
                if (placeholder == NEW_WORD)
                {                    
                    usedDictionary = PrettyScaryDictionary.TTSdictionary.TryGetValue (currentWord, out dictionaryMatch);

                    if (usedDictionary == true)
                    {
                        results = TakeFromDictionary (true, sentence, letterIndex);
                    }
            
                    else //if no match is found, use secondary pronunciation.
                    {
                        placeholder = NO_MATCH;
                        results = AdjacentEvaluation (sentence, letterIndex);
                    }
                }

                else if (placeholder != NO_MATCH) //takes over reading once a match is found in the dictionary.
                {
                    results = TakeFromDictionary (false, sentence, letterIndex);
                }

                else
                {
                    results = AdjacentEvaluation (sentence, letterIndex);
                }
            }

            else
            {
                results.Insert (0, " "); //avoids setting placeholder in this scenario since an empty space cant reset it when needed.
                results.Insert (1, " ");
            }
            placeholder = wordCounter.CheckForEnd (placeholder, NEW_WORD); //script needed to reset to default state but the mod did not.
            return results;
        }

        List <string> TakeFromDictionary (bool isNewWord, string sentence, int letterIndex)
        {
            List <string> output = new List <string>();

            if (isNewWord == true)
            {
                placeholder = 0;
            }

            if (placeholder < dictionaryMatch.Length)
            {
                if (wordCounter.dumpRemainingLetters == true)
                {
                    int counter = 0;

                    for (int i = placeholder; i < dictionaryMatch.Length; i++)
                    {                        
                        output.Insert (counter, dictionaryMatch[placeholder]);
                        counter++;
                        placeholder++;
                    }
                }
                
                else
                {
                    string extract = dictionaryMatch[placeholder];
                    output.Insert (0, extract);
                    placeholder++;
                }                
            }
            return output;
        }

        //AdjacentEvaluation is more efficient but its a complicated mess. catches anything not in the dictionary
        List <string> AdjacentEvaluation (string sentence, int letterIndex)
        {
            const string VOWELS = "AEIOU";
            const string CONSONANTS = "BCDFGHJKLMNPQRSTVWXYZ";

            List <string> output = new List <string>();
            string primary = "";
            string secondary = "";

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
#region case A
                case "A":
                    if (UnwantedMatchBypassed ("..AK.") && //!steak
                        UnwantedMatchBypassed ("REAT.") && //!great
                        IsMatch (".EAF." + //leaf
                                "|.EAL." + //real
                                "|.EAD." + //lead
                                "|.EAT." //meat
                                ) ||

                       (IsMatch (".OAH."))) //pharoah 
                    {
                        ;
                    }

                    else if (IsMatch ("..AW." + //raw
                                     "|.WAT." + //water
                                     "|. AUT" + //autograph
                                     "|..AUT" + //astronaut
                                     "| CALL" + //caller
                                     "| TALL" + //tallest
                                     "|..AUG" + //caught
                                     "|..ALK" //talking
                                     ) ||

                            (UnwantedMatchBypassed ("SSAUL") && //!assault
                            IsMatch ("..AUL"))) //saul                                 

                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if (IsMatch ("REA.." + //break
                                     "|.EAK." + //steak
                                     "| TAB." + //table
                                     "| LAB." + //lable
                                     "|..APL" + //maple
                                     "|..AY." + //may
                                     "|.HAZE" + //haze
                                     "|.RASE" + //phrase
                                     "|. ABL" + //able
                                     "|..ACE" + //space
                                     "|..ATE" + //activate
                                     "| BABY" + //baby
                                     "|..ADY" + //lady
                                     "|..AKY" + //flaky
                                     "|..ACY" + //stacy
                                     "|..AME" //same
                                     ) ||
                            
                            (UnwantedMatchBypassed (".PATI") && //!patio
                             UnwantedMatchBypassed (".GATI") && //!negatively
                             IsMatch ("..ATI")) || //station

                            (IsMatch ("..AI.") && //faith
                             UnwantedMatchBypassed ("..A.R"))) // !fair,
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }

                    else if (IsMatch (". A ." + //a
                                     "|. AV." + //available
                                     "|..ABL" + //available
                                     "|. AG." + //aggression
                                     "|. ANN" + //annoint
                                     "|. ASS" + //assault
                                     "|. ABI" + //ability
                                     "|OTAL." + //totally
                                     "|..A ." + //hyena
                                     "|..ARG" + //bargain
                                     "|.GATI" + //negatively
                                     "|..AMA" + //proclamation
                                     "|ISA.." //elisabeth
                                     ) ||

                            (UnwantedMatchBypassed ("..ACT") && //!activites
                             IsMatch (". AC.")) || //acoustic
                            
                            (IsMatch ("..AR ") && //far
                             UnwantedMatchBypassed ("..A.E"))) //!fare
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }

                    else if (IsMatch ("..ARE" + //compare                             
                                     "|..AIR" + //fair
                                     "|..ARO" //pharoah
                                     ))
                    { 
                        primary = PrettyScaryDictionary.EHH;
                    }

                    else if (IsMatch ("SSAUL" + //assault
                                     "|WHAT." //what        
                                     )) 
                    {
                        primary = PrettyScaryDictionary.HOH;
                    }

                    else if (IsMatch ("TEAU.")) //chateau
                    {
                        primary = PrettyScaryDictionary.OWE;
                    }

                    else if (IsMatch (".TAGE")) //advantage
                    {
                        primary = PrettyScaryDictionary.IHH;
                    }

                    else    
                    {
                        primary = PrettyScaryDictionary.AHH; //plottable
                    }
                    break;
#endregion case A

#region case B
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
#endregion case B

#region case C
                case "C": 
                    if (IsMatch ("..CE." + //nice
                                "|..CI." + //complicit
                                "|..CY." + //stacy
                                "|..CH " //touch
                                ))
                    {
                        primary = PrettyScaryDictionary.SIH; //sicily
                    }
            
                    else if (UnwantedMatchBypassed (".CC..")) //!double C's 
                    {
                        primary = PrettyScaryDictionary.KIH; //cat
                    } 
                    break;
#endregion case C

#region case D
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
#endregion case D

#region case E
                case "E":
                    if (IsMatch ("THE .")) //the
                    {
                        primary = PrettyScaryDictionary.UHH;
                    } 
  
                    else if (IsMatch ("..EAK" + //break
                                     "|..EU." + //queue
                                     "|.EE.." + //speech
                                     "|..ELY" + //lovely
                                     "|.TEAU" + //aboiteau
                                     "|OPED " + //undeveloped
                                     "|.LED " + //filed
                                     "|DGE.." + //judgement
                                     "|.VES " + //ourselves
                                     "|.BEL " //label
                                     ) ||

                            (UnwantedMatchBypassed (".REAL") && //!real
                             IsMatch (".REA.")) || //great

                            (UnwantedMatchBypassed ("TIES.") && //!activities
                            IsMatch (".IES.")) || //flies

                            (UnwantedMatchBypassed (".VE..") && //!lover 
                             UnwantedMatchBypassed (".EE..") && //!veer
                             IsMatch ("..ER.")) || //rubber                            
                            
                            (UnwantedMatchBypassed (" .E..") && //!He man    
                             UnwantedMatchBypassed ("YBE..") && //!maybe                     
                             IsMatch ("..E ."))) //tribe
                    {
                        ;
                    }    
        
                    else if (IsMatch ("..EW.")) //brew
                    {
                        primary = PrettyScaryDictionary.OOO; 
                    }

                    else if (IsMatch ("..EYE")) //eye               
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }         
       
                    else if (IsMatch ("..EE." + //engineer
                                     "|.DEA." + //deal
                                     "|..E.D" + //lead
                                     "| ME ." + //me
                                     "| HE ." + //he
                                     "| WE ." + //we
                                     "| BE ." + //be
                                     "|YBE ." + //maybe
                                     "|..ESE" + //these
                                     "|.KEY." + //key
                                     "|.IE. " + //trekkies
                                     "|.VETO" + //veto
                                     "|..ENA" + //hyena
                                     "|.TEI." //stein
                                     ) ||

                            (UnwantedMatchBypassed (".RES.") && //!respite
                             UnwantedMatchBypassed (".REI.") && //!rein
                             IsMatch (" RE..")) || //remember
                                     
                            (UnwantedMatchBypassed ("ITE..") && //!aboiteau
                             IsMatch (".LEA."))) //lead
                    {                           
                        primary = PrettyScaryDictionary.EEE;
                    }  
            
                    else if ((UnwantedMatchBypassed (".VES ") && //!ourselves
                              IsMatch ("..ES ")) || //dresses

                              IsMatch ("..ERE" + //there
                                      "|.VER." + //veto
                                      "|.TED." + //plated
                                      "|.NET " + //planet
                                      "|..EN." //given
                                      ))
                    {                                         
                        primary = PrettyScaryDictionary.EHH;
                    }   

                    else if (IsMatch (".REY." + //osprey
                                     "|.REI." //rein
                                     ))
                    {
                        primary = PrettyScaryDictionary.AEE;
                    }

                    else
                    {
                        primary = PrettyScaryDictionary.EHH;
                    }
                    break;
#endregion case E

#region case F
                case "F": 
                    primary = PrettyScaryDictionary.FIH; //follow
                    break;
#endregion case F

#region case G
                case "G":
                    if (IsMatch (".GG.." + //trigger
                                "|..GH." + //high
                                "|.NG ." + //talking
                                "|.IGN." + //design
                                "|..GHT" //caught
                                ))
                    {
                        ;
                    }

                    else if  (

                              (UnwantedMatchBypassed ("..G .") && //rig
                               IsMatch (".IGI.")) || //aborigine
                                
                               IsMatch (". GY." + //gym
                                       "|.DGE." + //judgement 
                                       "|ENGI." + //engineer
                                       "|.OGY " + //eulogy
                                       "|. GIN" + //gin
                                       "|.AGE." + //advantage
                                       "|..GEN" //oxygen
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
#endregion case G

#region case H
                case "H":
                    if (IsMatch ("..HN." + //john
                                "|.GH.." + //dough
                                "|.PH.." + //autograph
                                "|.WH.." //what
                                ) ||

                       (UnwantedMatchBypassed (".CH..") && //!pouch
                        IsMatch ("..H .")) || //pharoah

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
#endregion case H

#region case I
                case "I":
                    if (UnwantedMatchBypassed (".LIES") && //!flies
                        UnwantedMatchBypassed (" PIES") && //!pies
                        UnwantedMatchBypassed (".KIES") && //!skies
                        IsMatch ("..IES") || //activities

                        IsMatch ("VAILA" + //available
                                "|GAIN." + //bargain
                                "|.AITH" + //faith
                                "|.AIR." + //fair
                                "|.EI.." //stein
                                ))
                    {
                        ;
                    }

                    else if (IsMatch (".TION" + //traction
                                     "|.SION" //accession
                                     ))
                    {
                        if (UnwantedMatchBypassed ("SSION") == true) //!double S's
                        {
                            primary = PrettyScaryDictionary.SIH;
                            secondary = PrettyScaryDictionary.HIH;
                        }

                        else
                        {
                            primary = PrettyScaryDictionary.HIH;
                        }                        
                    }

                    else if (IsMatch ("..IKE" + //pike
                                     "|KNI.." + //knight
                                     "|..IGH" + //light
                                     "|. I ." + //I
                                     "|..ILE" + //filed                                 
                                     "|..IGN" + //sign
                                     "| VITA" + //vitality                                                                   
                                     "|..ICY" + //bicycle
                                     "| TITA" + //titanite
                                     "|OVISA" + //improvisation
                                     "|OVISE" + //improvise
                                     "|.LIE." + //flies
                                     "| .IE." + //pies
                                     "| .IK." //bikie
                                     ) ||

                            (UnwantedMatchBypassed (".GINE") && //!aborigine
                             UnwantedMatchBypassed ("ENICE") && //!venice
                             UnwantedMatchBypassed (".OITE") && //!aboiteau
                             UnwantedMatchBypassed (".GIVE") && //!give
                             UnwantedMatchBypassed (".TIVE") && //!negatively
                             IsMatch ("..I.E")) || //nice
                            
                            (CONSONANTS.Contains (before) && //respite
                             IsMatch ("..ITE"))) //titanite
                    {   
                        primary = PrettyScaryDictionary.EYE;
                    }
            
                    else if (IsMatch (".OIN." + //point
                                     "|..ING" + //running
                                     "|.OITE" + //aboiteaux
                                     "|..ION" + //champion
                                     "|..IO." + //patio                                     
                                     "|SKIE." + //huskies
                                     "|.KIE." + //trekkies
                                     "| .ISA" //visa
                                     ))
                    {
                        primary = PrettyScaryDictionary.EEE;
                    }
    
                    else 
                    {
                        primary = PrettyScaryDictionary.IHH;  //felicity
                    }
                    break;
#endregion case I

#region case J
                case "J":   
                    primary = PrettyScaryDictionary.JIH; //jelly
                    break;
#endregion case J

#region case K
                case "K":   
                    if (UnwantedMatchBypassed (".CK..") && //!two kih's
                        UnwantedMatchBypassed ("..KN.") && //!silent K
                        UnwantedMatchBypassed (".KK..")) //!double K's
                    {
                        primary = PrettyScaryDictionary.KIH;
                    }    
                    break;
#endregion case K

#region case L
                case "L": 
                    if (UnwantedMatchBypassed ("..LK.") && //!silent L
                        UnwantedMatchBypassed ("..LF.") && //!silent L
                        UnwantedMatchBypassed (".LL..")) //!double L's
                    {
                        primary = PrettyScaryDictionary.LIH; //silent L, caller,
                    }
                    break;
#endregion case L

#region case M
                case "M":   
                    if (UnwantedMatchBypassed (".MM..")) //!double M's
                    {                        
                        primary = " ";
                        secondary = PrettyScaryDictionary.MIH; //such as "molten", drummer,
                    }    
                    break;
#endregion case M

#region case N
                case "N":      
                    if (UnwantedMatchBypassed (".NN..")) //double N's
                    {
                        if (IsMatch ("GINE ")) //aborigine
                        {
                            primary = PrettyScaryDictionary.NIH;
                            secondary = PrettyScaryDictionary.EEE;
                        }   
                        
                        else
                        {
                            primary = " ";
                            secondary = PrettyScaryDictionary.NIH;  //such as nickel,
                        } 
                        
                    }    
                    break;
#endregion case N

#region case O
                case "O":    
                    if (IsMatch (".TOU." + //touch
                                "|.OO.." + //double O's
                                "|.WOR." //word
                                ) ||

                       (UnwantedMatchBypassed (".COUS") && //acoustic
                        UnwantedMatchBypassed (".HOUS") && //house
                        IsMatch (".IOUS" + //abstentious
                                "|.ROUS" + //ludicrous
                                "|.POUS" + //acarpous
                                "|.EOUS" + //advantageous
                                "|.LOUS" //acaulous
                                )))
                    {
                        ;
                    }                    
                        
                    else if (IsMatch ("..OI." + //annoint
                                     "|.FOUR" //four
                                     ) ||

                            (UnwantedMatchBypassed ("ABORI") && //aborigine
                             IsMatch ("..OR."))) //lore
                    {
                        primary = PrettyScaryDictionary.AWW;
                    }

                    else if (IsMatch (".FOUL" + //foul
                                     "|.POUC" + //pouch
                                     "|.LOUC" + //slouch
                                     "|.HOUS" + //house
                                     "|..OUT" + //out
                                     "|..OUR" //our
                                     ))
                    {
                        primary = PrettyScaryDictionary.AHH; 
                        secondary = PrettyScaryDictionary.HOH;
                    }
 
                    else if (IsMatch (".ION." + //champion
                                     "|.DONE" + //done
                                     "|.LOVE" //lovely
                                     ))
                    {
                        primary = PrettyScaryDictionary.UHH;
                    }   
            
                    else if (IsMatch ("..O ." + //pro
                                     "|.SOU." + //soul
                                     "|..OW." + //bestow
                                     "|.BOTH" + //both
                                     "|ABORI" //aborigine
                                     ) ||

                            (UnwantedMatchBypassed (".LOV.") && //!love
                             UnwantedMatchBypassed ("PROVE") && //!improve
                             UnwantedMatchBypassed ("..OPU") && //!opulence
                             UnwantedMatchBypassed ("..OLE") && //!opulence
                             IsMatch (".SOLO"))) //solo
                    {
                        primary = PrettyScaryDictionary.OWE;
                    }   
            
                    else if (IsMatch ("..OHN" +  //john
                                     "|.BOT " + //bot
                                     "|..OM." + //computer
                                     "|..OGY" + //eulogy
                                     "|..OPU" + //opulence
                                     "|..OTT" + //plottable
                                     "|..OC." + //proclamation
                                     "|..OB " + //rob
                                     "|..OL." //told
                                     ) ||

                            (UnwantedMatchBypassed ("..OO.") && //!oolacile
                             IsMatch (". O..")) || //objective

                            (UnwantedMatchBypassed (".BO..") && //!both
                             IsMatch ("..OTH"))) //sloth
                    {
                        primary = PrettyScaryDictionary.HOH;                      
                    }    
            
                    else if (IsMatch ("..OO." + //fool
                                     "|PROVE" + //improve
                                     "|.TOD." + //today
                                     "|PROVE" + //improve
                                     "|.COUS" + //acoustic
                                     "|..OU " //you
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
#endregion case O

#region case P
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
#endregion case P

#region case Q
                case "Q": 
                    if (IsMatch ("..QUE"))
                    {
                        primary = PrettyScaryDictionary.KIH;
                        secondary = PrettyScaryDictionary.YIH;
                    }                   

                    else
                    {
                        primary = PrettyScaryDictionary.KIH; //query    
                        secondary = PrettyScaryDictionary.WIH;
                    }                    
                    break;
#endregion case Q

#region case R
                case "R":   
                    if (UnwantedMatchBypassed (".RR..")) //!double R's
                    {                        
                        primary = PrettyScaryDictionary.RIH;
                    }
                    break;
#endregion case R

#region case S
                case "S":   
                    if (IsMatch ("..SM " + //prism
                                "|VIS.." //improvise
                                ))
                    {
                        primary = PrettyScaryDictionary.ZIH;
                    }

                    else if (UnwantedMatchBypassed (".SS..")) //!double S's
                    {
                        primary = PrettyScaryDictionary.SIH;
                    }                  
                    break;
#endregion case S

#region case T
                case "T":
                    if (UnwantedMatchBypassed ("PATIO") && //!patio
                        IsMatch (".ATIO" + //proclamation                        
                                "|.CTIO" //instructional 
                                ))
                    {
                        ;
                    }
                             
                    else if (UnwantedMatchBypassed ("..T.U") && //!github
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
#endregion case T

#region case U
                case "U":
                    if (IsMatch (".QU.." + //queue
                                "|.AU.." + //caught
                                "|.OUL." + //soul
                                "|YOU.." + //you
                                "|.AUT." + //astronaut
                                "|.AUL." + //assault
                                "|.OUGH" + //dough
                                "|.OUR." + //four
                                "|COUS." + //accoustic
                                "|HOUS." + //hous
                                "|.OUT." //out
                                ) ||
                        
                       (UnwantedMatchBypassed ("TOUCH") && //!touch
                        IsMatch (".OUCH")) || //pouch

                       (UnwantedMatchBypassed (".OU..") && //!your
                        IsMatch ("..URR"))) //purr
                    {
                        ;
                    }    

                    else if (IsMatch (".EU.." + //sleuth
                                     "|..UE." + //cruelty
                                     "|.AU ." + //aboiteau
                                     "|.AUX " + //aboiteaux
                                     "|.RU.E" + //rude
                                     "|..UI." + //ruin
                                     "|..UDI" + //ludicrous
                                     "|.PULL" //pull
                                     ))
                    {
                        if (IsMatch (" EU..")) //eulogy
                        {
                            primary = PrettyScaryDictionary.YIH;
                            secondary = PrettyScaryDictionary.OOO;
                        }

                        else
                        {
                            primary = PrettyScaryDictionary.OOO;
                        }                        
                    }

                    else if (IsMatch (" OUR." + //end
                                     "|.OUC." + //touch
                                     "|. UN." + //undeveloped
                                     "|. UP." + //update
                                     "|.SUB." + //submit
                                     "|.HUB." + //hub
                                     "|..UMM" //drummer                                     
                                     ) ||

                            (UnwantedMatchBypassed (".OUSE") && //!house
                             IsMatch (".OUS.")) || //ludicrous

                            (UnwantedMatchBypassed ("..UDE") && //!prude
                             UnwantedMatchBypassed ("..UDI") && //!ludicrous
                             IsMatch ("..UD.")) || //crud                                                    

                            (CONSONANTS.Contains (after) && //rubber
                             CONSONANTS.Contains (twoAfter)) || //instructional

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

                    else
                    {
                        primary = PrettyScaryDictionary.YIH;
                        secondary = PrettyScaryDictionary.OOO;
                    }
                    break;
#endregion case U

#region case V
                case "V":    
                    primary = PrettyScaryDictionary.VIH;
                    break;
#endregion case V

#region case W
                case "W":
                    if (UnwantedMatchBypassed ("..W .")) //narrow
                    {
                        primary = PrettyScaryDictionary.WIH;
                    }                   
                    break;
#endregion case W

#region case X
                case "X":   
                    if (IsMatch (". X..")) //xylophone
                    {
                        primary = PrettyScaryDictionary.ZIH; 
                    }    
        
                    else if (UnwantedMatchBypassed ("AUX .")) //aboitaux
                    {
                        primary = PrettyScaryDictionary.KSS;                 
                    }
                    break;
#endregion case X

#region case Y
                case "Y":
                    if (IsMatch (".EY ." + //key
                                "|.AY.." + //maybe
                                "|.EYE." //eye
                                ) ||
                                
                        (UnwantedMatchBypassed ("QUY .") && //!soliloquy 
                         IsMatch (".UY ."))) //buy                                              
                    {
                        ; 
                    }

                    else if (IsMatch (".CYC." + //bicycle
                                     "|.MY.." + //my
                                     "|.HY.." + //hyena
                                     "|FLY.." + //fly
                                     "| BY ." + //by
                                     "| XYL." //xylophone 
                                     ) ||              

                            (UnwantedMatchBypassed ("..Y .") && //!possibility     
                             IsMatch (".TY.."))) //style                             
                    {
                        primary = PrettyScaryDictionary.EYE;
                    }

                    else if (IsMatch ("..Y ." + //ability                                  
                                     "|QUY ." //soliloquy                            
                                     ))
                    {
                        primary = PrettyScaryDictionary.EEE;
                    }

                    else if (IsMatch (" XYS." + //xyster
                                     "|.GYM." + //gym
                                     "|.XYG." //oxygen
                                    ))   
                    {
                        primary = PrettyScaryDictionary.IHH;
                    }   
                           
                    else
                    {
                        primary = PrettyScaryDictionary.YIH;  //yam
                    }
                    break;
#endregion case Y

#region case Z
                case "Z":  
                    primary = PrettyScaryDictionary.ZIH;
                    break;
#endregion case Z
            }            
            output.Insert (0, primary);
            output.Insert (1, secondary);
            return output;
        }

        //helps cut down on text needed and is easier to understand
        bool IsMatch (string pattern) //SURROUNDINGPHRASE MUST EXIST BEFORE USE.
        {
            string[] analyseDivisions = pattern.Split ('|');

            for (int i = 0; i < analyseDivisions.Length; i++)
            {
                if (analyseDivisions[i].Length != 5)
                {
                    wrongFormatMatchers++;
                }
            }           
            return Regex.IsMatch (surroundingPhrase, pattern);
        }

        //returns true when the unwanted phrase
        bool UnwantedMatchBypassed (string pattern) 
        {
            if (pattern.Length != 5)
            {
                wrongFormatNonMatchers++;
            }
            return !Regex.IsMatch (surroundingPhrase, pattern);
        }
    }
}