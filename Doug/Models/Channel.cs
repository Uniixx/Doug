﻿namespace Doug.Models
{
    public enum ChannelType
    {
        Default,
        Pvp,
        Casino,
        Coffee,
        Marketplace
    }

    public class Channel
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
