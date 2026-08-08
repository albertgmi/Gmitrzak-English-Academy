namespace inzBackend.Helpers
{
    public readonly struct WavAudioInfo
    {
        public int SampleRate { get; init; }
        public short Channels { get; init; }
        public short BitsPerSample { get; init; }
        public short BlockAlign { get; init; }
        public int DataOffset { get; init; }
        public int DataLength { get; init; }
    }
}
