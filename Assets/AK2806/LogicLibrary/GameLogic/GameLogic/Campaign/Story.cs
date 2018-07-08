using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Campaign
{
    public interface IStory
    {
        IAction NextAction();
        IAction CurrentAction();
        bool Next();
    }

    public abstract class AbstractStory : IStory, ICampaignBlock
    {
        public CBType Type => CBType.Story;
        public IStory Story => this;
        public IBattle Battle => null;
        public abstract List<ICampaignBlock> Nexts { get; }

        public abstract IAction NextAction();
        public abstract IAction CurrentAction();
        public abstract bool Next();
    }

    public class Story : AbstractStory
    {
        protected List<IAction> _actions;
        protected int _currentActionIndex;
        protected List<ICampaignBlock> _nexts;

        public List<IAction> Actions { get => _actions; set => _actions = value; }
        public int CurrentActionIndex => _currentActionIndex;
        public override List<ICampaignBlock> Nexts => _nexts;

        public Story(List<IAction> actions = null, List<ICampaignBlock> nexts = null)
        {
            _actions = actions;
            _currentActionIndex = -1;
            _nexts = nexts;
        }
        
        public override IAction NextAction()
        {
            if (_actions != null)
            {
                int nextIndex = _currentActionIndex + 1;
                if (nextIndex >= 0 && nextIndex < _actions.Count)
                {
                    return _actions[nextIndex];
                }
            }
            return null;
        }

        public override IAction CurrentAction()
        {
            if (_actions != null)
            {
                if (_currentActionIndex >= 0 && _currentActionIndex < _actions.Count)
                {
                    return _actions[_currentActionIndex];
                }
            }
            return null;
        }

        public override bool Next()
        {
            if (_actions != null)
            {
                int nextIndex = _currentActionIndex + 1;
                if (nextIndex >= 0 && nextIndex < _actions.Count)
                {
                    _currentActionIndex = nextIndex;
                    return true;
                }
            }
            return false;
        }
    }

    public interface IAction
    {
        void DoAction();
        string Comment { get; }
    }

    public class Action : IAction
    {
        protected ICommand _command;
        protected string _comment;

        public ICommand Command { get => _command; set => _command = value; }
        public string Comment { get => _comment; set => _comment = value; }

        public Action(ICommand command = null, string comment = "")
        {
            _command = command;
            _comment = comment;
        }
        
        public void DoAction()
        {
            if (_command != null)
            {
                JSEngineManager.Run(_command);
            }
        }
    }
}
