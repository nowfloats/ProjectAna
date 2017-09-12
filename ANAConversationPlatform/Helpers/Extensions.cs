using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ANAConversationPlatform.Helpers
{
	public static class Extensions
	{
		public static SectionContent GetFor(this IEnumerable<BaseContent> contentCollection, Section section)
		{
			if (string.IsNullOrWhiteSpace(section._id)) return null;
			return contentCollection.FirstOrDefault(x => x is SectionContent && !string.IsNullOrWhiteSpace((x as SectionContent).SectionId) && (x as SectionContent).SectionId == section._id) as SectionContent;
		}
		public static CarouselItemContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselItem carouselItem)
		{
			if (string.IsNullOrWhiteSpace(carouselItem._id)) return null;
			return contentCollection.FirstOrDefault(x => x is CarouselItemContent && !string.IsNullOrWhiteSpace((x as CarouselItemContent).CarouselItemId) && (x as CarouselItemContent).CarouselItemId == carouselItem._id) as CarouselItemContent;
		}
		public static CarouselButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselButton carouselBtn)
		{
			if (string.IsNullOrWhiteSpace(carouselBtn._id)) return null;
			return contentCollection.FirstOrDefault(x => x is CarouselButtonContent && !string.IsNullOrWhiteSpace((x as CarouselButtonContent).CarouselButtonId) && (x as CarouselButtonContent).CarouselButtonId == carouselBtn._id) as CarouselButtonContent;
		}
		public static ButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, Button btn)
		{
			return contentCollection.FirstOrDefault(x => x is ButtonContent && !string.IsNullOrWhiteSpace((x as ButtonContent).ButtonId) && (x as ButtonContent).ButtonId == btn._id) as ButtonContent;
		}
		public static string GetDescriptionOrDefault(this Enum value)
		{
			var field = value.GetType().GetField(value.ToString());
			var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes.Where(x => x is DescriptionAttribute).FirstOrDefault() is DescriptionAttribute descAttr)
				return descAttr.Description;
			else
				return value.ToString();
		}
		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static ChatFlowPack ToPack(this ChatFlowPackRequest request)
		{
			return new ChatFlowPack
			{
				ChatContent = request.ChatContent,
				ChatNodes = request.ChatNodes.Select(x => x.ToNode()).ToList(),
				CreatedOn = request.CreatedOn,
				NodeLocations = request.NodeLocations,
				ProjectId = request.ProjectId,
				UpdatedOn = request.UpdatedOn,
				WebNodeLocations = request.WebNodeLocations,
				_id = request._id
			};
		}

		public static ChatNode ToNode(this ChatNodeRequest request)
		{
			return new ChatNode
			{
				ApiMethod = request.ApiMethod,
				ApiResponseDataRoot = request.ApiResponseDataRoot,
				ApiUrl = request.ApiUrl,
				Buttons = request.Buttons,
				CardFooter = request.CardFooter,
				CardHeader = request.CardHeader,
				Emotion = request.Emotion,
				FlowId = request.FlowId,
				GroupName = request.FlowId,
				Id = request.Id,
				IsStartNode = request.IsStartNode,
				Name = request.Name,
				NextNodeId = request.NextNodeId,
				NodeType = request.NodeType,
				Placement = request.Placement,
				RequiredVariables = request.RequiredVariables,
				TimeoutInMs = request.TimeoutInMs,
				VariableName = request.VariableName,
				Sections = request.Sections.Select(x => (Section)Utils.ToTypedSection(x)).ToList()
			};
		}

	}
}
