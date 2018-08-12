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
            message.gridObj.row = row;
            message.gridObj.col = col;
            message.highland = highland;
            message.gridObj.objID = gridObject.ID;
            message.view = gridObject.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveGridObject(GridObject gridObject)
        {
            BattleSceneRemoveGridObjectMessage message = new BattleSceneRemoveGridObjectMessage();
            message.gridObj.row = gridObject.GridRef.PosRow;
            message.gridObj.col = gridObject.GridRef.PosCol;
            message.gridObj.objID = gridObject.ID;
            _connection.SendMessage(message);
        }

        public void AddLadderObject(int row, int col, Direction direction, LadderObject ladderObject)
        {
            BattleSceneAddLadderObjectMessage message = new BattleSceneAddLadderObjectMessage();
            message.ladderObj.row = row;
            message.ladderObj.col = col;
            message.direction = (int)direction;
            message.ladderObj.objID = ladderObject.ID;
            message.view = ladderObject.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveLadderObject(LadderObject ladderObject)
        {
            BattleSceneRemoveLadderObjectMessage message = new BattleSceneRemoveLadderObjectMessage();
            message.ladderObj.row = ladderObject.GridRef.PosRow;
            message.ladderObj.col = ladderObject.GridRef.PosCol;
            message.ladderObj.objID = ladderObject.ID;
            _connection.SendMessage(message);
        }

        public void DisplayDicePoints(int[] dicePoints)
        {
            DisplayDicePointsMessage message = new DisplayDicePointsMessage();
            message.dicePoints = dicePoints;
            //message.userID = _owner.Id;
            _connection.SendMessage(message);
        }

        public void SetActingOrder(IEnumerable<ActableGridObject> objects)
        {
            BattleSceneSetActingOrderMessage message = new BattleSceneSetActingOrderMessage();
            //message.objsOrder = new BattleSceneSetActingOrderMessage.ActableObject[];
        }

        public void NextTurn(User who, ActableGridObject actable)
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
