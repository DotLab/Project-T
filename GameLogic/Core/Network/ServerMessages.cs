using GameLogic.CharacterSystem;
using GameLogic.Core.DataSystem;
using System;

namespace GameLogic.Core.Network.ServerMessages
{
    public sealed class StorySceneResetMessage : Message
    {
        public const long MESSAGE_TYPE = -1L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class StorySceneObjectAddMessage : Message
    {
        public const long MESSAGE_TYPE = -2L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string objID;
        public CharacterView view;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteCharacterView(stream, view);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            view = InputStreamHelper.ReadCharacterView(stream);
        }
    }

    public sealed class StorySceneObjectRemoveMessage : Message
    {
        public const long MESSAGE_TYPE = -2L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
        }
    }

    public sealed class StorySceneObjectTransformMessage : Message
    {
        public const long MESSAGE_TYPE = -3L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public Layout to;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteLayout(stream, to);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            to = InputStreamHelper.ReadLayout(stream);
        }
    }

    public sealed class StorySceneObjectViewEffectMessage : Message
    {
        public const long MESSAGE_TYPE = -4L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public CharacterViewEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            effect = InputStreamHelper.ReadCharacterViewEffect(stream);
        }
    }

    public sealed class StorySceneObjectPortraitStyleMessage : Message
    {
        public const long MESSAGE_TYPE = -5L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public PortraitStyle portrait;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WritePortraitStyle(stream, portrait);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            portrait = InputStreamHelper.ReadPortraitStyle(stream);
        }
    }

    public sealed class StorySceneCameraTransformMessage : Message
    {
        public const long MESSAGE_TYPE = -6L;
        public override long MessageType => MESSAGE_TYPE;

        public Layout to;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteLayout(stream, to);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            to = InputStreamHelper.ReadLayout(stream);
        }
    }

    public sealed class StorySceneCameraEffectMessage : Message
    {
        public const long MESSAGE_TYPE = -7L;
        public override long MessageType => MESSAGE_TYPE;

        public CameraEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCameraEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            effect = InputStreamHelper.ReadCameraEffect(stream);
        }
    }

    public sealed class PlayBGMMessage : Message
    {
        public const long MESSAGE_TYPE = -8L;
        public override long MessageType => MESSAGE_TYPE;

        public string bgmID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(bgmID.Length);
            stream.WriteString(bgmID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            bgmID = stream.ReadString(length);
        }
    }

    public sealed class StopBGMMessage : Message
    {
        public const long MESSAGE_TYPE = -9L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class PlaySEMessage : Message
    {
        public const long MESSAGE_TYPE = -10L;
        public override long MessageType => MESSAGE_TYPE;

        public string seID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(seID.Length);
            stream.WriteString(seID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            seID = stream.ReadString(length);
        }
    }
    
    public sealed class ShowSceneMessage : Message
    {
        public const long MESSAGE_TYPE = -11L;
        public override long MessageType => MESSAGE_TYPE;

        public int sceneType;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(sceneType);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            sceneType = stream.ReadInt32();
        }
    }

    public sealed class TextBoxAddParagraphMessage : Message
    {
        public const long MESSAGE_TYPE = -12L;
        public override long MessageType => MESSAGE_TYPE;

        public string text;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(text.Length);
            stream.WriteString(text);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            text = stream.ReadString(length);
        }
    }

    public sealed class TextBoxAddSelectionMessage : Message
    {
        public const long MESSAGE_TYPE = -13L;
        public override long MessageType => MESSAGE_TYPE;

        public string text;
        public int selectionCode;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(text.Length);
            stream.WriteString(text);
            stream.WriteInt32(selectionCode);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            text = stream.ReadString(length);
            selectionCode = stream.ReadInt32();
        }
    }

    public sealed class TextBoxClearMessage : Message
    {
        public const long MESSAGE_TYPE = -14L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class TextBoxSetPortraitMessage : Message
    {
        public const long MESSAGE_TYPE = -15L;
        public override long MessageType => MESSAGE_TYPE;

        public CharacterView view;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCharacterView(stream, view);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            view = InputStreamHelper.ReadCharacterView(stream);
        }
    }

    public sealed class TextBoxPortraitStyleMessage : Message
    {
        public const long MESSAGE_TYPE = -16L;
        public override long MessageType => MESSAGE_TYPE;

        public PortraitStyle style;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WritePortraitStyle(stream, style);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            style = InputStreamHelper.ReadPortraitStyle(stream);
        }
    }

    public sealed class TextBoxPortraitEffectMessage : Message
    {
        public const long MESSAGE_TYPE = -17L;
        public override long MessageType => MESSAGE_TYPE;

        public CharacterViewEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            effect = InputStreamHelper.ReadCharacterViewEffect(stream);
        }
    }
    
    public sealed class CharacterInfoDataMessage : Message
    {
        public const long MESSAGE_TYPE = -18L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public IDescribable describable;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            OutputStreamHelper.WriteDescribable(stream, describable);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            describable = InputStreamHelper.ReadDescribable(stream);
        }
    }

    public abstract class CharacterPropertiesDescriptionMessage : Message
    {
        public struct Property : IStreamable
        {
            public string propertyID;
            public IDescribable describable;

            public void ReadFrom(IDataInputStream stream)
            {
                int length = stream.ReadInt32();
                propertyID = stream.ReadString(length);
                describable = InputStreamHelper.ReadDescribable(stream);
            }

            public void WriteTo(IDataOutputStream stream)
            {
                stream.WriteInt32(propertyID.Length);
                stream.WriteString(propertyID);
                OutputStreamHelper.WriteDescribable(stream, describable);
            }
        }
        
        public string characterID;
        public Property[] properties;

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            properties = new Property[length];
            for (int i = 0; i < length; ++i)
            {
                properties[i] = new Property();
                properties[i].ReadFrom(stream);
            }
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(properties.Length);
            foreach (Property property in properties)
            {
                property.WriteTo(stream);
            }
        }

    }

    public sealed class CharacterSkillsDescriptionMessage : CharacterPropertiesDescriptionMessage
    {
        public const long MESSAGE_TYPE = -19L;
        public override long MessageType => MESSAGE_TYPE;
    }
    
    public sealed class CharacterAspectsDescriptionMessage : CharacterPropertiesDescriptionMessage
    {
        public const long MESSAGE_TYPE = -20L;
        public override long MessageType => MESSAGE_TYPE;
    }

    public sealed class CharacterStuntsDescriptionMessage : CharacterPropertiesDescriptionMessage
    {
        public const long MESSAGE_TYPE = -21L;
        public override long MessageType => MESSAGE_TYPE;
    }

    public sealed class CharacterExtrasDescriptionMessage : CharacterPropertiesDescriptionMessage
    {
        public const long MESSAGE_TYPE = -22L;
        public override long MessageType => MESSAGE_TYPE;
    }

    public sealed class CharacterConsequencesDescriptionMessage : CharacterPropertiesDescriptionMessage
    {
        public const long MESSAGE_TYPE = -23L;
        public override long MessageType => MESSAGE_TYPE;
    }

    public sealed class CharacterStressDataMessage : Message
    {
        public const long MESSAGE_TYPE = -24L;
        public override long MessageType => MESSAGE_TYPE;

        public string characterID;
        public int physicsStress;
        public int physicsStressMax;
        public int mentalStress;
        public int mentalStressMax;

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            physicsStress = stream.ReadInt32();
            physicsStressMax = stream.ReadInt32();
            mentalStress = stream.ReadInt32();
            mentalStressMax = stream.ReadInt32();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(physicsStress);
            stream.WriteInt32(physicsStressMax);
            stream.WriteInt32(mentalStress);
            stream.WriteInt32(mentalStressMax);
        }
    }
    
    public sealed class CharacterFatePointDataMessage : Message
    {
        public const long MESSAGE_TYPE = -25L;
        public override long MessageType => MESSAGE_TYPE;

        public string characterID;
        public int fatePoint;
        public int refreshPoint;

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            fatePoint = stream.ReadInt32();
            refreshPoint = stream.ReadInt32();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(fatePoint);
            stream.WriteInt32(refreshPoint);
        }
    }

    public sealed class AspectDataMessage : Message
    {
        public const long MESSAGE_TYPE = -26L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string aspectID;
        public int persistenceType;
        public string benefitCharacterID;
        public int benefitTimes;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(aspectID.Length);
            stream.WriteString(aspectID);
            stream.WriteInt32(persistenceType);
            stream.WriteInt32(benefitCharacterID.Length);
            stream.WriteString(benefitCharacterID);
            stream.WriteInt32(benefitTimes);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            aspectID = stream.ReadString(length);
            persistenceType = stream.ReadInt32();
            length = stream.ReadInt32();
            benefitCharacterID = stream.ReadString(length);
            benefitTimes = stream.ReadInt32();
        }
    }
    
    public sealed class ConsequenceDataMessage : Message
    {
        public const long MESSAGE_TYPE = -27L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string consequenceID;
        public int persistenceType;
        public string benefitCharacterID;
        public int benefitTimes;
        public int counteractLevel;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(consequenceID.Length);
            stream.WriteString(consequenceID);
            stream.WriteInt32(persistenceType);
            stream.WriteInt32(benefitCharacterID.Length);
            stream.WriteString(benefitCharacterID);
            stream.WriteInt32(benefitTimes);
            stream.WriteInt32(counteractLevel);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            consequenceID = stream.ReadString(length);
            persistenceType = stream.ReadInt32();
            length = stream.ReadInt32();
            benefitCharacterID = stream.ReadString(length);
            benefitTimes = stream.ReadInt32();
            counteractLevel = stream.ReadInt32();
        }
    }

    public sealed class SkillDataMessage : Message
    {
        public const long MESSAGE_TYPE = -28L;
        public override long MessageType => MESSAGE_TYPE;

        public string characterID;
        public string skillTypeID;
        public SkillProperty skillProperty;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(skillTypeID.Length);
            stream.WriteString(skillTypeID);
            OutputStreamHelper.WriteSkillProperty(stream, skillProperty);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            skillTypeID = stream.ReadString(length);
            skillProperty = InputStreamHelper.ReadSkillProperty(stream);
        }
    }

    public sealed class StuntDataMessage : Message
    {
        public const long MESSAGE_TYPE = -29L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string stuntID;
        public string boundSkillTypeID;
        public bool needDMCheck;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(stuntID.Length);
            stream.WriteString(stuntID);
            stream.WriteInt32(boundSkillTypeID.Length);
            stream.WriteString(boundSkillTypeID);
            stream.WriteBoolean(needDMCheck);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            stuntID = stream.ReadString(length);
            length = stream.ReadInt32();
            boundSkillTypeID = stream.ReadString(length);
            needDMCheck = stream.ReadBoolean();
        }
    }

    public sealed class ExtraDataMessage : Message
    {
        public const long MESSAGE_TYPE = -30L;
        public override long MessageType => MESSAGE_TYPE;

        public string characterID;
        public string extraID;
        public string itemID;
        public bool isTool;
        public bool isLongRangeWeapon;
        public bool isVehicle;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(extraID.Length);
            stream.WriteString(extraID);
            stream.WriteInt32(itemID.Length);
            stream.WriteString(itemID);
            stream.WriteBoolean(isTool);
            stream.WriteBoolean(isLongRangeWeapon);
            stream.WriteBoolean(isVehicle);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            extraID = stream.ReadString(length);
            length = stream.ReadInt32();
            itemID = stream.ReadString(length);
            isTool = stream.ReadBoolean();
            isLongRangeWeapon = stream.ReadBoolean();
            isVehicle = stream.ReadBoolean();
        }
    }

    public sealed class SkillCheckPanelShowMessage : Message
    {
        public const long MESSAGE_TYPE = -31L;
        public override long MessageType => MESSAGE_TYPE;

        public string initiativeCharacterID;
        public CharacterView initiativeView;
        public string passiveCharacterID;
        public CharacterView passiveView;
        public int playerState;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(initiativeCharacterID.Length);
            stream.WriteString(initiativeCharacterID);
            OutputStreamHelper.WriteCharacterView(stream, initiativeView);
            stream.WriteInt32(passiveCharacterID.Length);
            stream.WriteString(passiveCharacterID);
            OutputStreamHelper.WriteCharacterView(stream, passiveView);
            stream.WriteInt32(playerState);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            initiativeCharacterID = stream.ReadString(length);
            initiativeView = InputStreamHelper.ReadCharacterView(stream);
            length = stream.ReadInt32();
            passiveCharacterID = stream.ReadString(length);
            passiveView = InputStreamHelper.ReadCharacterView(stream);
            playerState = stream.ReadInt32();
        }
    }
    
    public sealed class SkillCheckPanelHideMessage : Message
    {
        public const long MESSAGE_TYPE = -32L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class DMCheckPanelShowMessage : Message
    {
        public const long MESSAGE_TYPE = -33L;
        public override long MessageType => MESSAGE_TYPE;

        public string text;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(text.Length);
            stream.WriteString(text);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            text = stream.ReadString(length);
        }
    }

    public sealed class DMCheckPanelHideMessage : Message
    {
        public const long MESSAGE_TYPE = -34L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class DisplayDicePointsMessage : Message
    {
        public const long MESSAGE_TYPE = -35L;
        public override long MessageType => MESSAGE_TYPE;

        public string userID;
        public int[] dicePoints;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(userID.Length);
            stream.WriteString(userID);
            stream.WriteInt32(dicePoints.Length);
            foreach (int point in dicePoints)
            {
                stream.WriteInt32(point);
            }
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            userID = stream.ReadString(length);
            length = stream.ReadInt32();
            dicePoints = new int[length];
            for (int i = 0; i < length; ++i)
            {
                dicePoints[i] = stream.ReadInt32();
            }
        }
    }

    public sealed class SkillCheckPanelUpdateSumPointMessage : Message
    {
        public const long MESSAGE_TYPE = -36L;
        public override long MessageType => MESSAGE_TYPE;

        public bool isInitiative;
        public int point;

        public override void ReadFrom(IDataInputStream stream)
        {
            isInitiative = stream.ReadBoolean();
            point = stream.ReadInt32();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteBoolean(isInitiative);
            stream.WriteInt32(point);
        }
    }

    public sealed class SkillCheckPanelDisplaySkillReadyMessage : Message
    {
        public const long MESSAGE_TYPE = -37L;
        public override long MessageType => MESSAGE_TYPE;

        public bool isInitiative;
        public string skillTypeID;
        public bool bigone;

        public override void ReadFrom(IDataInputStream stream)
        {
            isInitiative = stream.ReadBoolean();
            int length = stream.ReadInt32();
            skillTypeID = stream.ReadString(length);
            bigone = stream.ReadBoolean();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteBoolean(isInitiative);
            stream.WriteInt32(skillTypeID.Length);
            stream.WriteString(skillTypeID);
            stream.WriteBoolean(bigone);
        }
    }

    public sealed class SkillCheckPanelDisplayUsingAspectMessage : Message
    {
        public const long MESSAGE_TYPE = -38L;
        public override long MessageType => MESSAGE_TYPE;

        public bool isInitiative;
        public string characterID;
        public string aspectID;

        public override void ReadFrom(IDataInputStream stream)
        {
            isInitiative = stream.ReadBoolean();
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            aspectID = stream.ReadString(length);
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteBoolean(isInitiative);
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(aspectID.Length);
            stream.WriteString(aspectID);
        }
    }

    public sealed class StorySceneAddPlayerCharacterMessage : Message
    {
        public const long MESSAGE_TYPE = -39L;
        public override long MessageType => MESSAGE_TYPE;

        public int playerIndex;
        public string characterID;
        public CharacterView view;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(playerIndex);
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            OutputStreamHelper.WriteCharacterView(stream, view);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            playerIndex = stream.ReadInt32();
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            view = InputStreamHelper.ReadCharacterView(stream);
        }
    }

    public sealed class StorySceneRemovePlayerCharacterMessage : Message
    {
        public const long MESSAGE_TYPE = -40L;
        public override long MessageType => MESSAGE_TYPE;

        public int playerIndex;
        public string characterID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(playerIndex);
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            playerIndex = stream.ReadInt32();
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
        }
    }

    public struct BattleSceneObj : IStreamable
    {
        public string objID;
        public int row;
        public int col;

        public void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            row = stream.ReadInt32();
            col = stream.ReadInt32();
        }

        public void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            stream.WriteInt32(row);
            stream.WriteInt32(col);
        }
    }

    public sealed class BattleScenePushGridObjectMessage : Message
    {
        public const long MESSAGE_TYPE = -41L;
        public override long MessageType => MESSAGE_TYPE;

        public BattleSceneObj gridObj;
        public CharacterView view;
        public bool highland;
        public int direction;
        public bool actable;
        public bool movable;

        public override void WriteTo(IDataOutputStream stream)
        {
            gridObj.WriteTo(stream);
            OutputStreamHelper.WriteCharacterView(stream, view);
            stream.WriteBoolean(highland);
            stream.WriteInt32(direction);
            stream.WriteBoolean(actable);
            stream.WriteBoolean(movable);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            gridObj.ReadFrom(stream);
            view = InputStreamHelper.ReadCharacterView(stream);
            highland = stream.ReadBoolean();
            direction = stream.ReadInt32();
            actable = stream.ReadBoolean();
            movable = stream.ReadBoolean();
        }
    }

    public sealed class BattleSceneRemoveGridObjectMessage : Message
    {
        public const long MESSAGE_TYPE = -42L;
        public override long MessageType => MESSAGE_TYPE;

        public BattleSceneObj gridObj;

        public override void WriteTo(IDataOutputStream stream)
        {
            gridObj.WriteTo(stream);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            gridObj.ReadFrom(stream);
        }
    }

    public sealed class BattleSceneAddLadderObjectMessage : Message
    {
        public const long MESSAGE_TYPE = -43L;
        public override long MessageType => MESSAGE_TYPE;

        public BattleSceneObj ladderObj;
        public CharacterView view;
        public int direction;

        public override void WriteTo(IDataOutputStream stream)
        {
            ladderObj.WriteTo(stream);
            OutputStreamHelper.WriteCharacterView(stream, view);
            stream.WriteInt32(direction);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            ladderObj.ReadFrom(stream);
            view = InputStreamHelper.ReadCharacterView(stream);
            direction = stream.ReadInt32();
        }
    }

    public sealed class BattleSceneRemoveLadderObjectMessage : Message
    {
        public const long MESSAGE_TYPE = -44L;
        public override long MessageType => MESSAGE_TYPE;

        public BattleSceneObj ladderObj;

        public override void WriteTo(IDataOutputStream stream)
        {
            ladderObj.WriteTo(stream);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            ladderObj.ReadFrom(stream);
        }
    }
    
    public sealed class BattleSceneResetMessage : Message
    {
        public const long MESSAGE_TYPE = -45L;
        public override long MessageType => MESSAGE_TYPE;

        public int rows;
        public int cols;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(rows);
            stream.WriteInt32(cols);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            rows = stream.ReadInt32();
            cols = stream.ReadInt32();
        }
    }

    public sealed class BattleSceneSetActingOrderMessage : Message
    {
        public const long MESSAGE_TYPE = -46L;
        public override long MessageType => MESSAGE_TYPE;

        public struct ActableObject : IStreamable
        {
            public BattleSceneObj gridObj;
            public CharacterView view;

            public void WriteTo(IDataOutputStream stream)
            {
                gridObj.WriteTo(stream);
                OutputStreamHelper.WriteCharacterView(stream, view);
            }

            public void ReadFrom(IDataInputStream stream)
            {
                gridObj.ReadFrom(stream);
                view = InputStreamHelper.ReadCharacterView(stream);
            }
        }

        public ActableObject[] objsOrder;

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objsOrder = new ActableObject[length];
            for (int i = 0; i < length; ++i)
            {
                objsOrder[i] = new ActableObject();
                objsOrder[i].ReadFrom(stream);
            }
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objsOrder.Length);
            foreach (ActableObject obj in objsOrder)
            {
                obj.WriteTo(stream);
            }
        }

    }


}
