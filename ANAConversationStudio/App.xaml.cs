using ANAConversationStudio.Helpers;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat.Sections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Windows;

namespace ANAConversationStudio
{
	public partial class App : Application
	{
		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			Logger.Write(e.Exception);
			MessageBox.Show(e.Exception.Message, "Oops! Something went wrong.");
		}
		public static string Cryptio { get; set; }

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			#region Initialize Mongo Parser Maps
			ConventionRegistry.Register(typeof(IgnoreExtraElementsConvention).Name, new ConventionPack { new IgnoreExtraElementsConvention(true) }, t => true);
			ConventionRegistry.Register("EnumStringConvention", new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, t => true);
			if (!BsonClassMap.IsClassMapRegistered(typeof(GifSection)))
				BsonClassMap.RegisterClassMap<GifSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(TextSection)))
				BsonClassMap.RegisterClassMap<TextSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(ImageSection)))
				BsonClassMap.RegisterClassMap<ImageSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(AudioSection)))
				BsonClassMap.RegisterClassMap<AudioSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(VideoSection)))
				BsonClassMap.RegisterClassMap<VideoSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(EmbeddedHtmlSection)))
				BsonClassMap.RegisterClassMap<EmbeddedHtmlSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselSection)))
				BsonClassMap.RegisterClassMap<CarouselSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(NodeContent)))
				BsonClassMap.RegisterClassMap<NodeContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(SectionContent)))
				BsonClassMap.RegisterClassMap<SectionContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(TextSectionContent)))
				BsonClassMap.RegisterClassMap<TextSectionContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(TitleCaptionSectionContent)))
				BsonClassMap.RegisterClassMap<TitleCaptionSectionContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(GraphSectionContent)))
				BsonClassMap.RegisterClassMap<GraphSectionContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(ButtonContent)))
				BsonClassMap.RegisterClassMap<ButtonContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(PrintOTPSection)))
				BsonClassMap.RegisterClassMap<PrintOTPSection>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselButtonContent)))
				BsonClassMap.RegisterClassMap<CarouselButtonContent>();

			if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselItemContent)))
				BsonClassMap.RegisterClassMap<CarouselItemContent>();
			#endregion

			#region This applies the default font
			FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
			{
				DefaultValue = FindResource(typeof(Window))
			});
			#endregion
		}
	}
}
