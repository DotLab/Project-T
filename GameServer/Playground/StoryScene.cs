using GameServer.CharacterComponents;
using GameServer.Playground.StoryComponent;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameUtil;
using System;
using System.Collections.Generic;

namespace GameServer.Playground {
	public sealed class StoryScene : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<StoryScene> {
			private readonly StoryScene _outer;

			public JSAPI(StoryScene outer) {
				_outer = outer;
			}

			public IJSAPI<SceneObject> createStoryObject(IJSAPI<Character> character) {
				try {
					Character originCharacter = JSContextHelper.Instance.GetAPIOrigin(character);
					SceneObject sceneObject = _outer.CreateStoryObject(originCharacter);
					return (IJSAPI<SceneObject>)sceneObject.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public bool addToScene(IJSAPI<ISceneObject> sceneObj) {
				try {
					ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
					return _outer.AddIntoScene(originSceneObj);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public IJSAPI<ISceneObject> getObject(string id) {
				try {
					ISceneObject sceneObject = _outer.ObjList[id];
					return (IJSAPI<ISceneObject>)sceneObject.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public bool isInScene(IJSAPI<ISceneObject> sceneObj) {
				try {
					ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
					return _outer.ObjList.Contains(originSceneObj);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public bool isInScene(string id) {
				try {
					return _outer.ObjList.Contains(id);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public bool removeFromScene(IJSAPI<ISceneObject> sceneObj) {
				try {
					ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
					return _outer.RemoveFromScene(originSceneObj);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public bool removeFromScene(string id) {
				try {
					return _outer.RemoveFromScene(id);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void resetScene() {
				try {
					_outer.ResetScene();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Camera> getCamera() {
				try {
					return (IJSAPI<Camera>)_outer.Camera.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<TextBox> getTextBox(int index) {
				try {
					return (IJSAPI<TextBox>)_outer.TextBoxes[index].GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public StoryScene Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private static readonly StoryScene _instance = new StoryScene();
		public static StoryScene Instance => _instance;

		private readonly IdentifiedObjectList<ISceneObject> _objList;
		private readonly IdentifiedObjectList<Character> _playerCharacters;
		private readonly Camera _camera;
		private readonly List<TextBox> _textBoxes;

		public IReadonlyIdentifiedObjectList<ISceneObject> ObjList => _objList;
		public IReadonlyIdentifiedObjectList<Character> PlayerCharacters => _playerCharacters;
		public Camera Camera => _camera;
		public List<TextBox> TextBoxes => _textBoxes;

		public StoryScene() {
			_objList = new IdentifiedObjectList<ISceneObject>();
			_playerCharacters = new IdentifiedObjectList<Character>();
			_camera = new Camera();
			_textBoxes = new List<TextBox>();
			_apiObj = new JSAPI(this);
		}

		public Character FindCharacter(string id) {
			if (_playerCharacters.TryGetValue(id, out Character character)) {
				return character;
			} else if (_objList.TryGetValue(id, out ISceneObject storyObject)) {
				return storyObject.CharacterRef;
			} else return null;
		}

		private SceneObject CreateStoryObject(Character character) {
			SceneObject ret = new SceneObject(character);
			return ret;
		}

		public void AddPlayerCharacter(Character playerCharacter) {
			_playerCharacters.Add(playerCharacter);
			throw new NotImplementedException();
		}

		public bool RemovePlayerCharacter(string id) {
			bool ret = _playerCharacters.Remove(id);
			throw new NotImplementedException();
			return ret;
		}

		public bool RemovePlayerCharacter(Character playerCharacter) {
			return this.RemovePlayerCharacter(playerCharacter.ID);
		}

		public bool AddIntoScene(ISceneObject sceneObject) {
			_objList.Add(sceneObject);
			throw new NotImplementedException();
			return true;
		}

		public bool RemoveFromScene(string id) {
			bool ret = _objList.Remove(id);
			throw new NotImplementedException();
			return ret;
		}

		public bool RemoveFromScene(ISceneObject sceneObject) {
			return this.RemoveFromScene(sceneObject.ID);
		}

		public void ResetScene() {
			_objList.Clear();
			_camera.Reset();
			foreach (TextBox box in _textBoxes) {
				box.Clear();
				box.DisplayText();
			}
			throw new NotImplementedException();
		}
		
		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}

namespace GameServer.Playground.StoryComponent {
	public interface ISceneObject : IIdentifiable {
		void OnInteract();
		void OnCreateAspect();
		void OnAttack();
		void OnHinder();
		Character CharacterRef { get; }
		Layout Layout { get; }
		PortraitStyle Style { get; }
		void TransTo(Layout layout);
		void ChangeStyle(PortraitStyle style);
		void ApplyEffect(CharacterViewEffect effect);
	}

	public class SceneObject : ISceneObject {
		#region Javascript API class
		protected class JSAPI : IJSAPI<SceneObject> {
			private readonly SceneObject _outer;

			public JSAPI(SceneObject outer) {
				_outer = outer;
			}

			public string getID() {
				try {
					return _outer.ID;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setInteractReaction(Action callback) {
				try {
					_outer.Interact = new Command(callback);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void setCreateAspectReaction(Action callback) {
				try {
					_outer.CreateAspect = new Command(callback);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void setAttackReaction(Action callback) {
				try {
					_outer.Attack = new Command(callback);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void setHinderReaction(Action callback) {
				try {
					_outer.Hinder = new Command(callback);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Character> getCharacter() {
				try {
					return (IJSAPI<Character>)_outer.CharacterRef.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void transTo(Layout layout) {
				try {
					_outer.TransTo(layout);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public Vec3 getPosition() {
				try {
					return _outer.Layout.pos;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new Vec3();
				}
			}

			public float getRotation() {
				try {
					return _outer.Layout.rot;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0.0f;
				}
			}

			public Vec2 getScale() {
				try {
					return _outer.Layout.sca;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new Vec2();
				}
			}

			public void changeStyle(PortraitStyle style) {
				try {
					_outer.ChangeStyle(style);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getAction() {
				try {
					return _outer.Style.action;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public int getEmotion() {
				try {
					return _outer.Style.emotion;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void applyEffect(CharacterViewEffect effect) {
				try {
					_outer.ApplyEffect(effect);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public SceneObject Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		protected Command _interact;
		protected Command _createAspect;
		protected Command _attack;
		protected Command _hinder;
		protected readonly Character _characterRef;
		protected Layout _layout;
		protected PortraitStyle _style;
		protected CharacterViewEffect _effect;

		public string ID => _characterRef.ID;
		public string Name { get => _characterRef.Name; set { } }
		public string Description { get => _characterRef.Description; set { } }
		public Command Interact { get => _interact; set => _interact = value; }
		public Command CreateAspect { get => _createAspect; set => _createAspect = value; }
		public Command Attack { get => _attack; set => _attack = value; }
		public Command Hinder { get => _hinder; set => _hinder = value; }
		public Character CharacterRef => _characterRef;
		public Layout Layout => _layout;
		public PortraitStyle Style => _style;
		public CharacterViewEffect Effect => _effect;

		public SceneObject(Character character) {
			_characterRef = character ?? throw new ArgumentNullException(nameof(character));
			_layout = Layout.INIT;
			_style = PortraitStyle.INIT;
			_effect = CharacterViewEffect.INIT;
			_apiObj = new JSAPI(this);
		}

		public virtual void ApplyEffect(CharacterViewEffect effect) {
			_effect = effect;
			throw new NotImplementedException();
		}

		public virtual void TransTo(Layout layout) {
			_layout = layout;
			throw new NotImplementedException();
		}

		public virtual void ChangeStyle(PortraitStyle style) {
			_style = style;
			throw new NotImplementedException();
		}

		public virtual void OnAttack() {
			if (_attack != null) {
				_attack.DoAction();
			}
		}

		public virtual void OnCreateAspect() {
			if (_createAspect != null) {
				_createAspect.DoAction();
			}
		}

		public virtual void OnInteract() {
			if (_interact != null) {
				_interact.DoAction();
			}
		}

		public virtual void OnHinder() {
			if (_hinder != null) {
				_hinder.DoAction();
			}
		}

		public virtual IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public class PictureObject : SceneObject {
		#region Javascript API class
		protected new class JSAPI : SceneObject.JSAPI, IJSAPI<PictureObject> {
			private readonly PictureObject _outer;

			public JSAPI(PictureObject outer) :
				base(outer) {
				_outer = outer;
			}

			PictureObject IJSAPI<PictureObject>.Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		protected readonly Camera _cameraRef;

		public PictureObject(Character character) :
			base(character) {
			_apiObj = new JSAPI(this);
		}

		public override void TransTo(Layout layout) {
			_layout = layout;
			throw new NotImplementedException();
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}

	}

	public sealed class Camera : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Camera> {
			private readonly Camera _outer;

			public JSAPI(Camera outer) {
				_outer = outer;
			}

			public void TransTo(Layout layout) {
				try {
					_outer.TransTo(layout);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public Vec3 getPosition() {
				try {
					return _outer.Layout.pos;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new Vec3();
				}
			}

			public float getRotation() {
				try {
					return _outer.Layout.rot;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0.0f;
				}
			}

			public Vec2 getScale() {
				try {
					return _outer.Layout.sca;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return new Vec2();
				}
			}

			public void applyEffect(CharacterViewEffect effect) {
				try {
					_outer.ApplyEffect(effect);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void reset() {
				try {
					_outer.Reset();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public Camera Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private Layout _layout;
		private CharacterViewEffect _effect;

		public Layout Layout => _layout;
		public CharacterViewEffect Effect => _effect;

		public Camera() {
			_layout = Layout.INIT;
			_effect = CharacterViewEffect.INIT;
			_apiObj = new JSAPI(this);
		}

		public void TransTo(Layout layout) {
			_layout = layout;
			throw new NotImplementedException();
		}

		public void ApplyEffect(CharacterViewEffect effect) {
			_effect = effect;
			throw new NotImplementedException();
		}

		public void Reset() {
			_layout = Layout.INIT;
			_effect = CharacterViewEffect.INIT;
			throw new NotImplementedException();
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public interface ITextItem : IJSContextProvider {
		bool IsSelection { get; }
		int SelectionCode { get; }
		string Text { get; }
	}

	public sealed class TextItem : ITextItem {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<TextItem> {
			private readonly TextItem _outer;

			public JSAPI(TextItem outer) {
				_outer = outer;
			}

			public void setText(string text) {
				try {
					_outer.Text = text;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public string getText() {
				try {
					return _outer.Text;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public TextItem Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private string _text;

		public bool IsSelection => false;
		public int SelectionCode => -1;
		public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }

		public TextItem(string text) {
			_text = text ?? throw new ArgumentNullException(nameof(text));
			_apiObj = new JSAPI(this);
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class SelectionItem : ITextItem {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<SelectionItem> {
			private readonly SelectionItem _outer;

			public JSAPI(SelectionItem outer) {
				_outer = outer;
			}

			public void setSelectionCode(int code) {
				try {
					_outer.SelectionCode = code;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void setText(string text) {
				try {
					_outer.Text = text;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public string getText() {
				try {
					return _outer.Text;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public SelectionItem Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private string _text;
		private int _selectionCode;

		public bool IsSelection => true;
		public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }
		public int SelectionCode { get => _selectionCode; set => _selectionCode = value; }

		public SelectionItem(string text, int selectionCode) {
			_text = text ?? throw new ArgumentNullException(nameof(text));
			_apiObj = new JSAPI(this);
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class TextBox : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<TextBox> {
			private readonly TextBox _outer;

			public JSAPI(TextBox outer) {
				_outer = outer;
			}

			public IJSAPI<TextItem> generateText(string text) {
				try {
					ITextItem textItem = _outer.GenerateText(text);
					return (IJSAPI<TextItem>)textItem.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<SelectionItem> generateSelection(string text, int selectionCode) {
				try {
					ITextItem buttonItem = _outer.GenerateSelection(text, selectionCode);
					return (IJSAPI<SelectionItem>)buttonItem.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getPlayerIndex() {
				try {
					return _outer.PlayerIndex;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void clear() {
				try {
					_outer.Clear();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void display() {
				try {
					_outer.DisplayText();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public TextBox Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private readonly List<ITextItem> _textItems;
		private CharacterView _portrait;
		private PortraitStyle _portraitStyle;
		private readonly int _playerIndex;

		public int PlayerIndex => _playerIndex;
		public CharacterView Portrait => _portrait;
		public PortraitStyle PortraitStyle => _portraitStyle;

		public TextBox(int index) {
			_playerIndex = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(index), "Player index is less than 0.");
			_textItems = new List<ITextItem>();
			_apiObj = new JSAPI(this);
		}

		public ITextItem GenerateText(string text) {
			TextItem ret = new TextItem(text);
			_textItems.Add(ret);
			return ret;
		}

		public ITextItem GenerateSelection(string text, int selectionCode) {
			SelectionItem ret = new SelectionItem(text, selectionCode);
			_textItems.Add(ret);
			return ret;
		}

		public void Clear() {
			_textItems.Clear();
		}

		public void DisplayText() {
			throw new NotImplementedException();
		}

		public void UpdatePortrait(CharacterView view) {
			_portrait = view;
			throw new NotImplementedException();
		}

		public void UpdatePortraitStyle(PortraitStyle style) {
			_portraitStyle = style;
			throw new NotImplementedException();
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

}