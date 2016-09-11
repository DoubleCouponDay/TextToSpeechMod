using System.Text;
using System.IO; //filewriter

namespace SETextToSpeechMod
{
    class OptionalDebugger
    {
        static void Main()
        {
            MessageEventHandler handlers = new MessageEventHandler();
            Encoding test_encode = Encoding.Unicode;
            string testString = "[ AACHENER AACHENER";
            string upper = testString.ToUpper();
            byte[] testBytes = test_encode.GetBytes (upper);
            handlers.OnReceivedPacket (testBytes);

            while (handlers.speeches[0].finished == false)
            {
                handlers.speeches[0].Load();
            }
            handlers.speeches.RemoveAt (0);

            PrintAdjacentEvalutionWords();
        }

        static void PrintAdjacentEvalutionWords()
        {
            const string resultsAddress = @"C:\Users\power\Desktop\scripting\SpaceEngineersTextToSpeechMod\AdjacentWordsListDebugger.txt";
            File.WriteAllText ();
        }
    }
}
