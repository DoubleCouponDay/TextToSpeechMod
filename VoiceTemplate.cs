namespace SETextToSpeechMod
{
    interface VoiceTemplate
    {
        string Name { get; } 
        string FileID { get; }
        int SpaceSize { get; }
        int ClipLength { get; }
        int SyllableSize { get; }
        
        
    }
}
