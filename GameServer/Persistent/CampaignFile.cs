using System.Collections.Generic;

namespace GameServer.Persistent {

	public sealed class CampaignFile { // persistent
		public struct AddChara {

		}

		public struct CharaBehave {

		}

		public struct RmChara {

		}

		public struct PlayBGM {

		}

		public struct StopBGM {

		}

		public struct PlaySE {

		}

		public struct ShowMessage {

		}

		public struct Battle {

		}

		public struct Action {
			public enum ActionType {
				AddObj, RmObj,
				Behave, Focus,
				Backgroud, Envir,
				PlayBGM, StopBGM, PlaySE,
				ShowMessage, RunJS
			}

			public struct Next {
				public string condition;
				public int index;
				public string tip;
			}

			public ActionType actionType;
			public string param;
			public bool wait;
			public List<Next> nexts;
		}

		public string fileType;
		public List<Action> actions;
	}

	public sealed class CampaignListFile { // persistent
		public struct Router {
			public struct From {
				public string scene;
				public string outcome; // reference to the outcome’s name in scene file
			}

			public From from;
			public string to; // reference to the name of scene
		}

		public struct SceneFileInfo {
			public string name;
			public string path;
		}

		public string fileType;
		public List<SceneFileInfo> scenes;
		public string startup; // reference to the name of scene
		public List<Router> routers;
	}
}
