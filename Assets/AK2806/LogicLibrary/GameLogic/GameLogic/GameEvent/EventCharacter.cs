using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.CharacterSystem;
using GameLogic.Core.ScriptSystem;
using GameLogic.Core.ScriptSystem.Event;

namespace GameLogic.GameEvent
{
    public class EventCharacter : IEvent
    {
        private static readonly string[] _idList = {
            "event.character"
        };
        
        public virtual string[] NotifyList => _idList;

        public object GetContext()
        {
            throw new NotImplementedException();
        }

        public virtual void RetrieveContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }

        public virtual void SendContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }

        public void SetContext(object context)
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

        public override void RetrieveContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }

        public override void SendContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
