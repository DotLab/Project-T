using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network.ClientMessages
{
    public sealed class StorySceneObjectActionMessage : Streamable
    {
        public enum PlayerAction
        {
            INTERACT,
            CREATE_ASPECT,
            ATTACK,
            HINDER
        }

        public static readonly long MESSAGE_ID = 0L;
        public override long MessageID => MESSAGE_ID;

        public string objectID;
        public PlayerAction action;

    }

    public sealed class TextSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 1L;
        public override long MessageID => MESSAGE_ID;

        public int selection;

    }

    public sealed class SkillSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 2L;
        public override long MessageID => MESSAGE_ID;

        public string skillTypeID;

    }

    public sealed class AspectsSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 3L;
        public override long MessageID => MESSAGE_ID;

        public struct AspectGroup
        {
            public string characterID;
            public string[] aspectsID;
        }

        public AspectGroup[] aspectGroups;
        
    }

    public sealed class StuntSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 4L;
        public override long MessageID => MESSAGE_ID;

        public string stuntID;
        
    }

    public sealed class StorySceneNextActionMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 5L;
        public override long MessageID => MESSAGE_ID;
        
    }

    public sealed class ClientInitMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 6L;
        public override long MessageID => MESSAGE_ID;
        
    }


}
