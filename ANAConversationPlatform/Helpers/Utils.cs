using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ANAConversationPlatform.Helpers
{
	public static class Utils
	{
		public static Settings Settings { get; set; }
		public static BasicAuth BasicAuth { get; set; }

		public static List<BaseContent> ExtractContentFromChatNodes_OLD(List<ChatNode> chatNodes)
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
							case SectionTypeEnum.Graph:
								break;
							case SectionTypeEnum.Link:
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
		public static List<BaseContent> ExtractContentFromChatNodes(List<ChatNodeRequest> chatNodes)
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
							NodeName = node.Name,
							Emotion = btn.ContentEmotion,
						});
					}

				if (node.Sections != null)
					foreach (var section in node.Sections)
					{
						switch ((SectionTypeEnum)section.SectionType)
						{
							case SectionTypeEnum.Image:
							case SectionTypeEnum.Gif:
							case SectionTypeEnum.Audio:
							case SectionTypeEnum.Video:
							case SectionTypeEnum.EmbeddedHtml:
								{
									var titleCaptionSection = section;
									if (titleCaptionSection != null)
									{
										contentBank.Add(new TitleCaptionSectionContent
										{
											Caption = titleCaptionSection.Caption,
											Title = titleCaptionSection.Title,
											_id = titleCaptionSection.ContentId,
											SectionId = titleCaptionSection._id,
											NodeId = node.Id,
											Emotion = titleCaptionSection.ContentEmotion,
											NodeName = node.Name
										});
									}
								}
								break;
							case SectionTypeEnum.Text:
								{
									var ts = section;
									if (ts != null)
									{
										contentBank.Add(new TextSectionContent
										{
											Emotion = ts.ContentEmotion,
											NodeId = node.Id,
											SectionId = ts._id,
											SectionText = ts.Text,
											_id = ts.ContentId,
											NodeName = node.Name
										});
									}
								}
								break;
							case SectionTypeEnum.Graph:
								break;
							case SectionTypeEnum.Link:
								break;
							case SectionTypeEnum.Carousel:
								{
									var car = section;
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
											NodeName = node.Name
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
													NodeName = node.Name
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
															NodeId = node.Id,
															NodeName = node.Name
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
		public static Section ToTypedSection(dynamic section)
		{
			switch ((SectionTypeEnum)section.SectionType)
			{
				case SectionTypeEnum.Image:
					return JsonConvert.DeserializeObject<ImageSection>(section.ToString());
				case SectionTypeEnum.Text:
					var t = JsonConvert.DeserializeObject<TextSection>(section.ToString());
					return t;
				case SectionTypeEnum.Gif:
					return JsonConvert.DeserializeObject<GifSection>(section.ToString());
				case SectionTypeEnum.Audio:
					return JsonConvert.DeserializeObject<AudioSection>(section.ToString());
				case SectionTypeEnum.Video:
					return JsonConvert.DeserializeObject<VideoSection>(section.ToString());
				case SectionTypeEnum.EmbeddedHtml:
					return JsonConvert.DeserializeObject<EmbeddedHtmlSection>(section.ToString());
				case SectionTypeEnum.Carousel:
					return JsonConvert.DeserializeObject<CarouselSection>(section.ToString());
				case SectionTypeEnum.PrintOTP:
					return JsonConvert.DeserializeObject<PrintOTPSection>(section.ToString());
				default:
					return null;
			}
		}

	}
	public class BasicAuth
	{
		public string APIKey { get; set; }
		public string APISecret { get; set; }

		public string GetBase64()
		{
			if (string.IsNullOrWhiteSpace(APIKey) || string.IsNullOrWhiteSpace(APISecret))
				return null;
			return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{APIKey}:{APISecret}"));
		}
	}
	public class Settings
	{
		public string StudioDistFolder { get; set; }
		public int MaxCapTimeTakenToType { get; set; }
		public int BaseTimeTakenToTypePerChar { get; set; }
		public int VariableTimeTakenToTypePerChar { get; set; }
	}
	public class APIResponse
	{
		public bool Status { get; set; }
		public string Message { get; set; }
	}
	public class DataResponse<T> : APIResponse
	{
		public T Data { get; set; }
	}
}
