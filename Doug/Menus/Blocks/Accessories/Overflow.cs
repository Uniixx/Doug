﻿using System.Collections.Generic;
using Doug.Menus.Blocks.Text;

namespace Doug.Menus.Blocks.Accessories
{
    public class Overflow : Accessory
    {
        public List<Option> Options { get; set; }
        public string ActionId { get; set; }

        public Overflow(List<Option> options, string action) : base("overflow")
        {
            Options = options;
            ActionId = action;
        }
    }

    public class Option
    {
        public PlainText Text { get; set; }
        public string Value { get; set; }

        public Option(string text, string value)
        {
            Text = new PlainText(text);
            Value = value;
        }
    }
}
