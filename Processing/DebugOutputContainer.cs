using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    public class DebugOutputContainer
    {
        public IEnumerable <KeyValuePair <string, List <string>>> DictionaryWords
        {
            get
            {
                return dictionaryWords.AsEnumerable();
            }
        }

        public IEnumerable <KeyValuePair <string, List <string>>> RuleBasedWords
        {
            get
            {
                return ruleBasedWords.AsEnumerable();
            }
        }
        private Dictionary <string, List <string>> dictionaryWords = new Dictionary <string, List <string>>();
        private Dictionary <string, List <string>> ruleBasedWords = new Dictionary <string, List <string>>();

        /// <summary>
        /// Creates new entry or appends to existing entry.
        /// </summary>
        public void AddDictionaryWord (string key, List <string> value)
        {
            AddOrAppendToCollection (dictionaryWords, key, value);
        }

        /// <summary>
        /// Creates new entry or appends to existing entry.
        /// </summary>
        public void AddRuleBasedWord (string key, List <string> value)
        {
            AddOrAppendToCollection (ruleBasedWords, key, value);
        }

        private void AddOrAppendToCollection (Dictionary <string, List <string>> collection, string key, List <string> value)
        {
            if (collection.ContainsKey (key))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    collection[key].Add (value[i]);
                }                
            }

            else
            {
                collection.Add (key, value);
            }
        }

        public void Clear()
        {
            dictionaryWords.Clear();
            ruleBasedWords.Clear();
        }
    }
}
