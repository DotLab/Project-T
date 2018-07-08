using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Character;
using GameLogic.Core.ScriptSystem;
using GameLogic.Core.ScriptSystem.Event;

namespace GameLogic.GameEvent
{
    public class EventCharacter : IEvent
    {

        public string EventID => "";

        public virtual void RetrieveContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }

        public virtual void SendContext(JSEngine engine)
        {
            throw new NotImplementedException();
        }
    }

    public class EventGetSkillLevel : EventCharacter
    {
        private BaseCharacter _character;
        private SkillType _skillType;

        public BaseCharacter Character => _character;
        public SkillType SkillType => _skillType;

        private EventGetSkillLevel()
        {
        }

        public override void SendContext(JSEngine engine)
        {
            
        }
    }
}
