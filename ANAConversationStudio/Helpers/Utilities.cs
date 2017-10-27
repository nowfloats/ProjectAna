using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using Serilog;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Converters;
using ANAConversationStudio.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace ANAConversationStudio.Helpers
{
	public static class Utilities
	{
		public static Settings Settings { get; set; }

		public static bool IsDesignMode()
		{
			return DesignerProperties.GetIsInDesignMode(new DependencyObject());
		}

		public static bool ValidateStrings(params string[] strings)
		{
			foreach (var item in strings)
				if (string.IsNullOrWhiteSpace(item)) return false;
			return true;
		}

		public static async Task<AutoUpdateResponse> GetLatestVersionInfo()
		{
			if (Settings.UpdateDetails != null && !string.IsNullOrWhiteSpace(Settings.UpdateDetails.StudioUpdateUrl))
			{
				using (var wc = new WebClient())
				{
					var resp = await wc.DownloadStringTaskAsync(Settings.UpdateDetails.StudioUpdateUrl);
					return JsonConvert.DeserializeObject<AutoUpdateResponse>(resp);
				}
			}
			return null;
		}

		public static string Encrypt(string encryptString, string encryptionKey)
		{
			byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					encryptString = Convert.ToBase64String(ms.ToArray());
				}
			}
			return encryptString;
		}

		public static string Decrypt(string cipherText, string encryptionKey)
		{
			cipherText = cipherText.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return cipherText;
		}

		public static readonly JsonSerializerSettings StrictTypeHandlingJsonSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			Converters = new List<JsonConverter> { new StringEnumConverter() }
		};

		public static List<BaseContent> ExtractContentFromChatNodes(List<ChatNode> chatNodes)
		{
			var contentBank = new List<BaseContent>();
			foreach (var node in chatNodes)
			{
				if (node.Buttons != null)
					foreach (var btn in node.Buttons)
					{
						contentBank.Add(new ButtonContent
						{
							ButtonId = btn._id,
							ButtonText = btn.ButtonText,
							ButtonName = btn.ButtonName,
							NodeId = node.Id,
							_id = btn.ContentId,
							Emotion = btn.ContentEmotion,
						});
					}

				if (node.Sections != null)
					foreach (var section in node.Sections)
					{
						switch (section.SectionType)
						{
							case SectionTypeEnum.Image:
							case SectionTypeEnum.Gif:
							case SectionTypeEnum.Audio:
							case SectionTypeEnum.Video:
							case SectionTypeEnum.EmbeddedHtml:
								{
									var titleCaptionSection = section as TitleCaptionSection;
									if (titleCaptionSection != null)
									{
										contentBank.Add(new TitleCaptionSectionContent
										{
											Caption = titleCaptionSection.Caption,
											Title = titleCaptionSection.Title,
											_id = titleCaptionSection.ContentId,
											SectionId = titleCaptionSection._id,
											NodeId = node.Id,
											Emotion = titleCaptionSection.ContentEmotion
										});
									}
								}
								break;
							case SectionTypeEnum.Text:
								{
									var ts = section as TextSection;
									if (ts != null)
									{
										contentBank.Add(new TextSectionContent
										{
											Emotion = ts.ContentEmotion,
											NodeId = node.Id,
											SectionId = ts._id,
											SectionText = ts.Text,
											_id = ts.ContentId
										});
									}
								}
								break;
							case SectionTypeEnum.Carousel:
								{
									var car = section as CarouselSection;
									if (car != null)
									{
										contentBank.Add(new TitleCaptionSectionContent
										{
											_id = car.ContentId,
											Caption = car.Caption,
											Title = car.Title,
											Emotion = car.ContentEmotion,
											NodeId = node.Id,
											SectionId = car._id,
										});

										foreach (var carItem in car.Items)
										{
											if (carItem != null)
											{
												contentBank.Add(new CarouselItemContent
												{
													Caption = carItem.Caption,
													CarouselItemId = carItem._id,
													_id = carItem.ContentId,
													Emotion = car.ContentEmotion,
													NodeId = node.Id,
													Title = carItem.Title,
												});

												foreach (var carItemButton in carItem.Buttons)
												{
													if (carItemButton != null)
													{
														contentBank.Add(new CarouselButtonContent
														{
															_id = carItemButton.ContentId,
															ButtonText = carItemButton.Text,
															CarouselButtonId = carItemButton._id,
															Emotion = car.ContentEmotion,
															NodeId = node.Id
														});
													}
												}
											}
										}
									}
								}
								break;
							case SectionTypeEnum.PrintOTP:
								break;
							default:
								break;
						}
					}
			}
			return contentBank;
		}

		public static string TrimText(string text, int length = Constants.GroupHeaderTextMaxLength, bool removeNewLines = true)
		{
			if (text == null) return null;

			if (removeNewLines)
				text = text.Replace("\r\n", " ");

			bool trimNeeded = text.Length > length;
			if (trimNeeded)
				text = text.Substring(0, length) + "...";
			return text;
		}

		public static void FillConnectionsFromButtonsOfChatNode(NodeViewModel node)
		{
			if (node.Network != null)
				foreach (var btn in node.ChatNode.Buttons)
				{
					if (!string.IsNullOrWhiteSpace(btn.NextNodeId))
					{
						var destNode = node.Network.Nodes.FirstOrDefault(x => x.ChatNode.Id == btn.NextNodeId);
						if (destNode != null)
						{
							var conn = new ConnectionViewModel
							{
								SourceConnector = node.OutputConnectors.FirstOrDefault(x => x.Button._id == btn._id),
								DestConnector = destNode.InputConnectors.FirstOrDefault()
							};
							conn.ConnectionChanged += ConnectionViewModelConnectionChanged;
							if (conn.SourceConnector == null || conn.DestConnector == null) continue;
							node.Network.Connections.Add(conn);
						}
					}
				}
		}

		public static void ConnectionViewModelConnectionChanged(object s, EventArgs e)
		{
			var connection = s as ConnectionViewModel;
			if (connection.SourceConnector != null)
			{
				var sourceBtn = connection.SourceConnector.Button;
				if (connection.DestConnector != null)
				{
					var destinationNode = connection.DestConnector.ParentNode.ChatNode;
					sourceBtn.NextNodeId = destinationNode.Id;
				}
			}
		}

		public static string GenerateHash(string data)
		{
			using (var hashAlgo = MD5.Create())
			{
				hashAlgo.Initialize();
				return Convert.ToBase64String(hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(data)));
			}
		}
	}

	public static class Exts
	{
		public static Models.ChatFlowSearchItem SearchNode(this ChatNode node, string searchKeywords)
		{
			if (node.ToString().IsMatch(searchKeywords) || node.Id.IsMatch(searchKeywords))
			{
				return new Models.ChatFlowSearchItem
				{
					NodeId = node.Id,
					NodeText = node.ToString() ?? node.Alias
				};
			}

			foreach (var btn in node.Buttons)
			{
				if (btn.ToString().IsMatch(searchKeywords) || btn._id.IsMatch(searchKeywords))
				{
					return (new Models.ChatFlowSearchItem
					{
						NodeId = node.Id,
						NodeText = node.ToString() ?? node.Alias,
						ButtonText = btn.ToString()
					});
				}
			}

			foreach (var sec in node.Sections)
			{
				if (sec.ToString().IsMatch(searchKeywords) || sec._id.IsMatch(searchKeywords))
				{
					return (new Models.ChatFlowSearchItem
					{
						NodeId = node.Id,
						NodeText = node.ToString() ?? node.Alias,
						SectionText = sec.ToString()
					});
				}
			}

			return null;
		}
		public static bool IsMatch(this string text, string searchKeywords)
		{
			if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchKeywords)) return false;
			return Regex.IsMatch(text,
				string.Join("|", searchKeywords.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())), RegexOptions.IgnoreCase);
		}

		public static T DeepCopy<T>(this T source)
		{
			var json = source.ToJson();
			return BsonSerializer.Deserialize<T>(json);
		}

		public static Point? GetPointForNode(this Dictionary<string, LayoutPoint> nodeLocations, string chatNodeId)
		{
			if (nodeLocations.ContainsKey(chatNodeId) && nodeLocations[chatNodeId] != null)
				return new Point(nodeLocations[chatNodeId].X, nodeLocations[chatNodeId].Y);
			return null;
		}
	}
	public static class Logger
	{
		static Logger()
		{
			Log.Logger = new LoggerConfiguration().WriteTo.RollingFile(Settings.LoggingDirectory + "\\Log-{Date}.log").CreateLogger();
		}

		public static void Write(Exception ex, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = null)
		{
			Log.Error(ex, $"Ex in File: {Path.GetFileName(fileName ?? "")}->Line: {lineNumber}->Method: {caller}");
		}
		public static void Write(string message)
		{
			Log.Debug(message);
		}
	}
	public static class Constants
	{
		public const int GroupHeaderTextMaxLength = 50;
	}
}
