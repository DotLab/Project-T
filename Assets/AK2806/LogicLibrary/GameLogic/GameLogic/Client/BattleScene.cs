using GameLogic.CharacterSystem;
using GameLogic.Container.BattleComponent;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class BattleScene : ClientComponent
    {
        public override void MessageReceived(ulong timestamp, Message message)
        {
            
        }

        protected BattleScene(Connection connection, User owner) :
            base(connection, owner)
        {

        }
        
        public void Reset(int rows, int cols)
        {
            BattleSceneResetMessage message = new BattleSceneResetMessage();
            message.rows = rows;
            message.cols = cols;
            _connection.SendMessage(message);
        }

        public void PushGridObject(int row, int col, bool highland, GridObject gridObject)
        {
            BattleScenePushGridObjectMessage message = new BattleScenePushGridObjectMessage();
            message.row = row;
            message.col = col;
            message.highland = highland;
            message.objID = gridObject.ID;
            message.view = gridObject.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveGridObject(GridObject gridObject)
        {
            BattleSceneRemoveGridObjectMessage message = new BattleSceneRemoveGridObjectMessage();
            message.row = gridObject.GridRef.PosRow;
            message.col = gridObject.GridRef.PosCol;
            message.objID = gridObject.ID;
            _connection.SendMessage(message);
        }

        public void AddLadderObject(int row, int col, Direction direction, LadderObject ladderObject)
        {
            BattleSceneAddLadderObjectMessage message = new BattleSceneAddLadderObjectMessage();
            message.row = row;
            message.col = col;
            message.direction = (int)direction;
            message.objID = ladderObject.ID;
            message.view = ladderObject.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveLadderObject(LadderObject ladderObject)
        {
            BattleSceneRemoveLadderObjectMessage message = new BattleSceneRemoveLadderObjectMessage();
            message.row = ladderObject.GridRef.PosRow;
            message.col = ladderObject.GridRef.PosCol;
            message.objID = ladderObject.ID;
            _connection.SendMessage(message);
        }

        public void DisplayDicePoints(int[] dicePoints)
        {
            DisplayDicePointsMessage message = new DisplayDicePointsMessage();
            message.dicePoints = dicePoints;
            //message.userID = _owner.Id;
            _connection.SendMessage(message);
        }

        public void ShowActionTurn(IEnumerable<Character> characters)
        {

        }

        public void GridObjectMove(GridObject gridObject, Direction direction, bool stairway)
        {

        }

        public void GridObjectSkillEffect(GridObject gridObject, SkillType skillType, bool bigone)
        {

        }

    }

    public sealed class DMBattleScene : BattleScene
    {
        public DMBattleScene(Connection connection, User owner) :
            base(connection, owner)
        {

        }
    }

    public sealed class PlayerBattleSceme : BattleScene
    {
        public PlayerBattleSceme(Connection connection, User owner) :
            base(connection, owner)
        {

        }
    }

}
