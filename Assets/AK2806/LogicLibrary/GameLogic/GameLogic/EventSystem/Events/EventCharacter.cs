using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.CharacterSystem;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.EventSystem.Events
{
    public class EventCharacter : Event
    {
        private static readonly string[] _idList = {
            "event.character"
        };
        
        public override string[] NotifyList => _idList;

        public override object GetContext()
        {
            throw new NotImplementedException();
        }
        
        public override void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }

    public class EventGetSkillLevel : EventCharacter
    {
        private static readonly string[] _idList = {
            "event.character",
            "event.character.get_skill_level"
        };

        public override string[] NotifyList => _idList;

        private TemporaryCharacter _character;
        private SkillType _skillType;

        public TemporaryCharacter Character => _character;
        public SkillType SkillType => _skillType;

        private EventGetSkillLevel()
        {
        }
        
    }
}
