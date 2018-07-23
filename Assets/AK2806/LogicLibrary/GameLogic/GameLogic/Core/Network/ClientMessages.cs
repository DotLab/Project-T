using GameLogic.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network
{
    public sealed class StoryboardObjectMessage : Streamable
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

        private readonly string _objID;
        private readonly PlayerAction _action;

        public string ObjectID => _objID;
        public PlayerAction Action => _action;

        public StoryboardObjectMessage(string objID, PlayerAction action)
        {
            _objID = objID;
            _action = action;
        }
        
    }

    public sealed class TextSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 1L;
        public override long MessageID => MESSAGE_ID;

        private readonly int _selection;

        public int Selection => _selection;

        public TextSelectedMessage(int selection)
        {
            _selection = selection;
        }
    }

    public sealed class SkillSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 2L;
        public override long MessageID => MESSAGE_ID;

        private readonly SkillType _skillType;
        
        public SkillType SkillType => _skillType;

        public SkillSelectedMessage(SkillType skillType)
        {
            _skillType = skillType;
        }
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

        private readonly AspectGroup[] _aspectGroups;

        public AspectGroup[] AspectGroups => _aspectGroups;

        public AspectsSelectedMessage(AspectGroup[] aspectGroups)
        {
            _aspectGroups = aspectGroups;
        }
    }

    public sealed class StuntSelectedMessage : Streamable
    {
        public static readonly long MESSAGE_ID = 4L;
        public override long MessageID => MESSAGE_ID;

        private readonly string _stuntID;

        public string StuntID => _stuntID;

        public StuntSelectedMessage(string stuntID)
        {
            _stuntID = stuntID;
        }
    }
}
