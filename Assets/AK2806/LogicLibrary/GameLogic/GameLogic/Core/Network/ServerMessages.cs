using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network
{
    public sealed class SkillMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -1L;
        public override long MessageID => MESSAGE_ID;


    }
}
