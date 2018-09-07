using GameLib.Utilities.Network.ClientMessages;
using GameLib.Utilities.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextyClient {
	public partial class CharacterInfo : Form {
		public CharacterInfo() {
			InitializeComponent();
		}

		public void RequestData(string characterID) {
			var request = new GetCharacterDataMessage() {
				characterID = characterID,
				dataType = GetCharacterDataMessage.DataType.INFO
			};
			Program.connection.Request(request, result => {
				var resp = result as CharacterInfoDataMessage;
				if (resp != null) {
					this.nameTbx.Text = resp.describable.name;
					this.descriptionTbx.Text = resp.describable.description;
				}
			});
			request.dataType = GetCharacterDataMessage.DataType.FATEPOINT;
			Program.connection.Request(request, result => {
				var resp = result as CharacterFatePointDataMessage;
				if (resp != null) {
					this.fatePointLbl.Text = resp.fatePoint.ToString();
					this.refreshPointLbl.Text = resp.refreshPoint.ToString();
				}
			});
			request.dataType = GetCharacterDataMessage.DataType.ASPECTS;
			Program.connection.Request(request, result => {
				var resp = result as CharacterAspectsDescriptionMessage;
				if (resp != null) {
					foreach (var aspect in resp.properties) {
						this.aspectListBox.Items.Add(aspect.describable.name);
					}
				}
			});
			foreach (var skillType in Program.skillTypes) {
				var skillReq = new GetSkillLevelMessage() {
					characterID = characterID,
					skillTypeID = skillType.propertyID
				};
				Program.connection.Request(skillReq, result => {
					var resp = result as SkillLevelMessage;
					if (resp != null) {
						this.skillListBox.Items.Add(skillType.name + " " + (resp.level >= 0 ? "+" : "") + resp.level);
					}
				});
			}
			request.dataType = GetCharacterDataMessage.DataType.STUNTS;
			Program.connection.Request(request, result => {
				var resp = result as CharacterStuntsDescriptionMessage;
				if (resp != null) {
					foreach (var stunt in resp.properties) {
						this.stuntListBox.Items.Add(stunt.describable.name + "：" + stunt.describable.description);
					}
				}
			});
			request.dataType = GetCharacterDataMessage.DataType.EXTRAS;
			Program.connection.Request(request, result => {
				var resp = result as CharacterExtrasDescriptionMessage;
				if (resp != null) {
					foreach (var extra in resp.properties) {
						this.extraListBox.Items.Add(extra.describable.name + "：" + extra.describable.description);
					}
				}
			});
			request.dataType = GetCharacterDataMessage.DataType.STRESS;
			Program.connection.Request(request, result => {
				var resp = result as CharacterStressDataMessage;
				if (resp != null) {
					this.physicsStressLbl.Text = resp.physicsStress + "/" + resp.physicsStressMax;
					this.mentalStressLbl.Text = resp.mentalStress + "/" + resp.mentalStressMax;
				}
			});
			request.dataType = GetCharacterDataMessage.DataType.CONSEQUENCES;
			Program.connection.Request(request, result => {
				var resp = result as CharacterConsequencesDescriptionMessage;
				if (resp != null) {
					foreach (var consequence in resp.properties) {
						var req2 = new GetConsequenceDataMessage();
						req2.characterID = resp.characterID;
						req2.consequenceID = consequence.propertyID;
						Program.connection.Request(req2, result2 => {
							var resp2 = result2 as ConsequenceDataMessage;
							if (resp2 != null) {
								this.consequenceListBox.Items.Add(consequence.describable.name + " 伤痕槽-" + resp2.counteractLevel);
							}
						});
					}
				}
			});
		}
	}
}
