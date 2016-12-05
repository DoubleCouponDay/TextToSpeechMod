namespace SETextToSpeechMod
{
    interface VoiceTemplate
    {
        string FileID { get; }
        int SpaceSize { get; }
        int ClipLength { get; }
        int SyllableSize { get; }                
    }
}
