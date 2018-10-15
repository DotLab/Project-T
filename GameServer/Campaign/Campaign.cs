using GameServer.CharacterComponents;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using System;
using System.Collections.Generic;

namespace GameServer.Campaign {
	public enum ShotType {
		STORY, BATTLE, FREEDOM
	}

	public enum ContainerType {
		STORY, BATTLE
	}

	public abstract class Shot : IDescribable {
		protected List<Shot> _nexts;
		protected string _description;
		protected string _name;

		public abstract ShotType Type { get; }
		public abstract StoryShot Story { get; }
		public abstract BattleShot Battle { get; }
		public abstract FreedomShot Freedom { get; }

		public List<Shot> Nexts { get => _nexts; set => _nexts = value; }
		public string Description { get => _description; set => _description = value; }
		public string Name { get => _name; set => _name = value; }

		public Shot(string name = "", string description = "") {
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_description = description ?? throw new ArgumentNullException(nameof(description));
		}
	}

	public sealed class Campaign : IDescribable {
		private Dictionary<string, Shot> _shots;
		private Dictionary<string, Shot> _backupShots;
		private Shot _startup;
		private List<Campaign> _nexts;
		private string _description;
		private string _name;

		public Dictionary<string, Shot> Shots { get => _shots; set => _shots = value; }
		public Dictionary<string, Shot> BackupShots { get => _backupShots; set => _backupShots = value; }
		public Shot Startup { get => _startup; set => _startup = value; }
		public List<Campaign> Nexts { get => _nexts; set => _nexts = value; }
		public string Description { get => _description; set => _description = value; }
		public string Name { get => _name; set => _name = value; }

		public void JumpTo(string name) {

		}

		public void UseBackupShot(string name) {

		}

		public Campaign() {

		}

	}

	public sealed class CampaignList {
		private List<Campaign> _campaigns;
		private Campaign _startup;

		public List<Campaign> Campaigns { get => _campaigns; set => _campaigns = value; }
		public Campaign Startup { get => _startup; set => _startup = value; }


	}

	public sealed class CampaignManager : IJSContextProvider {
		private sealed class JSAPI : IJSAPI<CampaignManager> {
			private CampaignManager _outer;

			public JSAPI(CampaignManager outer) {
				_outer = outer;
			}

			public int askUser(IJSAPI<Character> playerCharacter, string promptText, string[] selections) {
				try {
					var origin_character = JSContextHelper.Instance.GetAPIOrigin(playerCharacter);
					return origin_character.Controller.Client.RequireDetermin(promptText, selections);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0;
				}
			}

			public bool requestDMCheck(IJSAPI<Character> invoker, string text) {
				try {
					var origin_invoker = JSContextHelper.Instance.GetAPIOrigin(invoker);
					return Game.DM.DMClient.RequireDMCheck(origin_invoker.Controller, text);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}
			
			public CampaignManager Origin(JSContextHelper proof) {
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

		private JSAPI _apiObj;

		private static readonly CampaignManager _instance = new CampaignManager();
		public static CampaignManager Instance => _instance;

		private ContainerType _currentContainer = ContainerType.STORY;
		private Campaign _currentCampaign = null;
		private Shot _currentShot = null;

		private CampaignManager() {
			_apiObj = new JSAPI(this);
		}

		public ContainerType CurrentContainer { get => _currentContainer; set => _currentContainer = value; }
		public Campaign CurrentCampaign => _currentCampaign;
		public Shot CurrentShot => _currentShot;

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}
