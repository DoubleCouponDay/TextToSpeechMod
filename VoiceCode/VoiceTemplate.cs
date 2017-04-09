namespace SETextToSpeechMod
{
    interface VoiceTemplate
    {
        string VoiceId { get; }
        int SpaceSize { get; }  
        int ClipLength { get; }
        int SyllableSize { get; }                     
    }
}
