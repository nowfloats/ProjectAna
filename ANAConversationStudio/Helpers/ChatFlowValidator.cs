using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CVR = ANAConversationStudio.Helpers.ChatFlowValidationResult;
using CVS = ANAConversationStudio.Helpers.ChatFlowValidationStatus;
namespace ANAConversationStudio.Helpers
{
	public static class ChatFlowValidator
	{
		private static ButtonTypeEnum[] InputTextButtonTypes = new[]
		{
			ButtonTypeEnum.GetText,
			ButtonTypeEnum.GetPhoneNumber,
			ButtonTypeEnum.GetNumber,
			ButtonTypeEnum.GetItemFromSource,
			ButtonTypeEnum.GetEmail,
		};

		private static List<Func<ChatFlowPack, CVR>> ChatFlowValidatorList = new List<Func<ChatFlowPack, CVR>>
		{
			EmptyValidation_ERROR,
			AllNodesShouldHaveAName_WARNING,
			NodesWithGetXTypeButtonsShouldHaveVariableName_ERROR,
			CarouselSectionsValidations_ERROR,
			OnlyOneTextInputButtonIsAllowedPerNode_ERROR,
			MandatoryFieldsPerButtonType_ERROR,
			APICallNodeValidations_ERROR
		};

		public static List<CVR> Validate(ChatFlowPack pack)
		{
			var fails = new List<CVR>();
			foreach (var validation in ChatFlowValidatorList)
			{
				var res = validation(pack);
				if (res.Status != CVS.Valid)
					fails.Add(res);
			}
			return fails;
		}

		/// <summary>
		/// Should be the first in the validator's list
		/// </summary>
		/// <param name="pack"></param>
		/// <returns></returns>
		private static CVR EmptyValidation_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			if (pack == null || pack.ChatNodes == null || pack.ChatNodes.Count == 0)
				return res.SetMsg("Error: The chat flow is empty! Please add at least one chat node").SetStatus(CVS.Error);
			return res.Valid();
		}

		public static CVR AllNodesShouldHaveAName_WARNING(ChatFlowPack pack)
		{
			var res = new CVR();
			var emptyNameNodes = pack.ChatNodes.Where(x => string.IsNullOrWhiteSpace(x.Name)).ToList();
			if (emptyNameNodes.Count > 0)
				return res.SetMsg($"Warning: {emptyNameNodes.Count} chat node(s) have empty name(s). It's recommended to give each node a name which can help you identify it. These are the node ids, search them and give them a name. ({emptyNameNodes.Select(x => x.Id).Join(", ")})").SetStatus(ChatFlowValidationStatus.Warning);
			return res.Valid();
		}

		public static CVR NodesWithGetXTypeButtonsShouldHaveVariableName_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			var nonEmptyVarNameBtnTypes = new HashSet<ButtonTypeEnum>(new[]
			{
				ButtonTypeEnum.GetText,
				ButtonTypeEnum.GetTime,
				ButtonTypeEnum.GetVideo,
				ButtonTypeEnum.GetFile,
				ButtonTypeEnum.GetImage,
				ButtonTypeEnum.GetItemFromSource,
				ButtonTypeEnum.GetLocation,
				ButtonTypeEnum.GetNumber,
				ButtonTypeEnum.GetPhoneNumber,
				ButtonTypeEnum.GetDate,
				ButtonTypeEnum.GetDateTime,
				ButtonTypeEnum.GetAudio
			});
			var respMessage = new List<string>();
			foreach (var node in pack.ChatNodes)
			{
				if (node.Buttons.Any(x => nonEmptyVarNameBtnTypes.Contains(x.ButtonType)) && string.IsNullOrWhiteSpace(node.VariableName))
					respMessage.Add($"Error: Node '{node.Identifer()}' must not have Variable Name field empty as it is being used to capture information. Variable Name will allow you to refer the captured information later in the flow to display or to send it to any APIs");
			}
			if (respMessage.Count > 0)
				return res.SetMsg(string.Join("\r\n", respMessage)).SetStatus(CVS.Error);
			return res.Valid();
		}

		public static CVR CarouselSectionsValidations_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			var carouselSectionNodes = pack.ChatNodes.Where(x => x.Sections.Any(y => y.SectionType == SectionTypeEnum.Carousel));
			var msgs = new List<string>();
			foreach (var node in carouselSectionNodes)
			{
				var nodeMsg = $"Error: In node '{node.Identifer()}' ";

				foreach (var sec in node.Sections.Where(x => x.SectionType == SectionTypeEnum.Carousel))
				{
					var carSec = sec as CarouselSection;
					var carSecMsg = $"{nodeMsg} in Carousel Section '{carSec}' ";
					if (carSec.Items == null || carSec.Items.Count == 0)
					{
						msgs.Add($"{carSecMsg} has no items!");
						res.SetStatus(CVS.Error);
						continue;
					}
					foreach (var carItem in carSec.Items)
					{
						var carItemMsg = $"in {carSecMsg} Carousel Item '{carItem}' ";
						if (carItem.Buttons == null || carItem.Buttons.Count == 0)
						{
							msgs.Add($"{carItemMsg} has no buttons!");
							res.SetStatus(CVS.Error);
							continue;
						}
						if (carItem.Buttons.Count > 2)
						{
							msgs.Add($"{carItemMsg} has more than 2 buttons! A carousel item can have maximum of 2 buttons.");
							res.SetStatus(CVS.Error);
						}

						foreach (var carBtn in carItem.Buttons)
						{
							var carItemBtnMsg = $"in {carItemMsg} Carousel Button '{carBtn}' ";
							if (string.IsNullOrWhiteSpace(carBtn.Text))
							{
								msgs.Add($"{carItemBtnMsg}, text is empty!");
								res.SetStatus(CVS.Error);
							}
							if ((carBtn.Type == CardButtonType.DeepLink || carBtn.Type == CardButtonType.OpenUrl) && string.IsNullOrWhiteSpace(carBtn.Url))
							{
								msgs.Add($"{carItemBtnMsg} is of type '{carBtn.Type}' but the '{nameof(carBtn.Url)}' field is empty! Set it to the target url.");
								res.SetStatus(CVS.Error);
							}
							if (carBtn.Type == CardButtonType.NextNode && string.IsNullOrWhiteSpace(carBtn.NextNodeId))
							{
								msgs.Add($"{carItemBtnMsg} is of type '{carBtn.Type}' but the '{nameof(carBtn.NextNodeId)}' field is empty! Set it to the target Next Node Id.");
								res.SetStatus(CVS.Error);
							}
						}
					}
				}
			}

			if (msgs.Count > 0)
				return res.SetMsg(msgs.Join("\r\n")).SetStatus(CVS.Error);
			return res.Valid();
		}

		public static CVR OnlyOneTextInputButtonIsAllowedPerNode_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			var msgs = new List<string>();
			foreach (var node in pack.ChatNodes.Where(x => x.Buttons != null && x.Buttons.Count > 0))
			{
				if (node.Buttons.Count(x => InputTextButtonTypes.Contains(x.ButtonType)) > 1)
					msgs.Add($"Error: Node '{node.Identifer()}' has multiple text input buttons. Only one text input button is allowed per node.");
			}
			if (msgs.Count > 0)
				return res.SetMsg(msgs.Join("\r\n")).SetStatus(CVS.Error);
			return res.Valid();
		}

		public static CVR MandatoryFieldsPerButtonType_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			var msgs = new List<string>();
			foreach (var node in pack.ChatNodes.Where(x => x.Buttons != null && x.Buttons.Count > 0))
			{
				var nodeMsg = $"Error: In node '{node.Identifer()}', ";
				foreach (var btn in node.Buttons)
				{
					if (node.NodeType == NodeTypeEnum.Combination && btn.ButtonType == ButtonTypeEnum.NextNode && !btn.Hidden && string.IsNullOrWhiteSpace(btn.NextNodeId))
						msgs.Add($"{nodeMsg} button '{btn}' is of type '{btn.ButtonType}' and is not hidden. So, it must be mapped to a node. Any NextNode button which is not hidden must be mapped to a next node.");
					if (node.NodeType == NodeTypeEnum.Combination && btn.ButtonType == ButtonTypeEnum.NextNode && !btn.Hidden && (string.IsNullOrWhiteSpace(btn.ButtonName) && string.IsNullOrWhiteSpace(btn.ButtonText)))
						msgs.Add($"{nodeMsg} button '{btn}' is of type '{btn.ButtonType}' and is not hidden. So, it must have text! Any NextNode button which is not hidden must have text which is displayed on the button.");
					if (btn.ButtonType == ButtonTypeEnum.DeepLink && string.IsNullOrWhiteSpace(btn.DeepLinkUrl))
						msgs.Add($"{nodeMsg} button '{btn}' is of type '{btn.ButtonType}' but the field '{nameof(btn.DeepLinkUrl)}' is empty! Set it to the target deeplink url.");
					if ((btn.ButtonType == ButtonTypeEnum.FetchChatFlow || btn.ButtonType == ButtonTypeEnum.OpenUrl) && string.IsNullOrWhiteSpace(btn.Url))
						msgs.Add($"{nodeMsg} button '{btn}' is of type '{btn.ButtonType}' but the field '{nameof(btn.Url)}' is empty! Set it to the target url.");
					if (btn.ButtonType == ButtonTypeEnum.FetchChatFlow && string.IsNullOrWhiteSpace(btn.NextNodeId))
						msgs.Add($"{nodeMsg} button '{btn}' is of type '{btn.ButtonType}' but the field '{nameof(btn.NextNodeId)}' is empty! Set it to the target next node id.");
				}
			}
			if (msgs.Count > 0)
				return res.SetMsg(msgs.Join("\r\n")).SetStatus(CVS.Error);
			return res.Valid();
		}

		public static CVR APICallNodeValidations_ERROR(ChatFlowPack pack)
		{
			var res = new CVR();
			var msgs = new List<string>();
			var apicallNodes = pack.ChatNodes.Where(x => x.NodeType == NodeTypeEnum.ApiCall).ToList();
			if (apicallNodes.Count > 0)
			{
				foreach (var node in apicallNodes)
				{
					var nodeMsg = $"In node '{node.Identifer()}', ";
					if (node.ApiMethod == null)
					{
						msgs.Add($"{nodeMsg} '{nameof(node.ApiMethod)}' field is not set! Set it to the Method(HTTP Verb) of the api you want to use.");
						res.SetStatus(CVS.Error);
					}

					if (string.IsNullOrWhiteSpace(node.ApiUrl))
					{
						msgs.Add($"{nodeMsg} '{nameof(node.ApiUrl)}' field is not set! Set it to the api you want to use.");
						res.SetStatus(CVS.Error);
					}
					else if (!Uri.TryCreate(node.ApiUrl, UriKind.Absolute, out Uri uri))
					{
						msgs.Add($"{nodeMsg} '{nameof(node.ApiUrl)}' field has invalid URL! Set it to a valid URL. Did you forget to start with http:// or https://");
						res.SetStatus(CVS.Error);
					}
				}
			}
			if (msgs.Count > 0)
				return res.SetMsg(msgs.Join("\r\n")).SetStatus(CVS.Error);
			return res.Valid();
		}
	}

	public enum ANATargetChannel
	{
		Generic,
		ANA,
		FacebookMessenger
	}

	public class ChatFlowValidationResult
	{
		public string ValidationName { get; set; }
		public string Message { get; set; }
		public ChatFlowValidationStatus Status { get; set; }

		public ChatFlowValidationResult SetMsg(string msg)
		{
			this.Message = msg;
			return this;
		}
		public ChatFlowValidationResult SetValidationName(string name)
		{
			this.ValidationName = name;
			return this;
		}
		public ChatFlowValidationResult SetStatus(ChatFlowValidationStatus status)
		{
			this.Status = status;
			return this;
		}

		public ChatFlowValidationResult Valid()
		{
			this.Status = ChatFlowValidationStatus.Valid;
			return this;
		}

		public ChatFlowValidationResult([CallerMemberName] string validationName = null)
		{
			this.ValidationName = validationName;
		}
	}

	public enum ChatFlowValidationStatus
	{
		Warning, Error, Valid
	}

	public static class ChatFlowExts
	{
		public static string Identifer(this ChatNode node)
		{
			if (!string.IsNullOrWhiteSpace(node.Name))
				return node.Name + " - " + node.Id;
			return node.Id;
		}

		public static string Join<T>(this IEnumerable<T> items, string saperator)
		{
			return string.Join(saperator, items);
		}
	}
}
