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
        private bool _canOperate = false;
        private bool _passiveChecking = false;

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

        public void DisplayDicePoints(User who, int[] dicePoints)
        {
            DisplayDicePointsMessage message = new DisplayDicePointsMessage();
            message.dicePoints = dicePoints;
            message.userID = who.Id;
            _connection.SendMessage(message);
        }

        public void SetActingOrder(IEnumerable<ActableGridObject> objects, int count)
        {
            var message = new BattleSceneSetActingOrderMessage();
            message.objsOrder = new BattleSceneObj[count];
            int i = 0;
            foreach (ActableGridObject actableGridObject in objects)
            {
                var msgActableObj = message.objsOrder[i++];
                msgActableObj.objID = actableGridObject.ID;
                msgActableObj.row = actableGridObject.GridRef.PosRow;
                msgActableObj.col = actableGridObject.GridRef.PosCol;
            }
            _connection.SendMessage(message);
        }

        public void NextTurn(ActableGridObject actable)
        {
            _canOperate = actable.CharacterRef.Controller == _owner;
            var message = new BattleSceneNextTurnMessage();
            message.canOperate = _canOperate;
            message.gridObj = new BattleSceneObj(actable);
            message.actionPoint = actable.ActionPoint;
            _connection.SendMessage(message);
        }

        public void NotifyPassiveCheck(GridObject passive, GridObject initiative, SkillType initiativeSkillType)
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
