using System.Collections.Generic;
using System.ComponentModel;

namespace ANAConversationPlatform.Models.Sections
{
    public class GraphSection : Section
	{
		public List<Coordinates> CoordinatesSet { get; set; } = new List<Coordinates>();
		public Axis X { get; set; } = new Axis();
		public Axis Y { get; set; } = new Axis(); 

		public string Caption { get; set; }
		public GraphTypeEnum GraphType { get; set; }

		public GraphSection(string id, int delayInMs, List<Coordinates> coordinatesSet, Axis x, Axis y, string caption, GraphTypeEnum graphType) : base(id, SectionTypeEnum.Graph, delayInMs)
		{
			this.CoordinatesSet = coordinatesSet;
			this.X = x;
			this.Y = y;
			this.Caption = caption;
			this.GraphType = graphType;
		}

		public GraphSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Graph, delayInMs)
		{ 
		}
	}

	public enum GraphTypeEnum
	{
		Bar, Line, ScatterPlot, Histogram
	}

	public class Axis : BaseEntity
	{
		public string Label { get; set; }

		public AxisTypeEnum AxisType
		{
			get; set;
		}
	}

	public enum AxisTypeEnum
	{
		[Description("Integer")]
		Integer,

		[Description("Double")]
		Double,

		[Description("String")]
		String,

		//[EnumMember(Value = "yyyy-MM-dd")]
		[Description("yyyy-MM-dd")]
		YearMonthDay,

		[Description("yyyy-MM")]
		YearMonth,

		[Description("yyyy-MonthName")]
		YearMonthName,

		[Description("yyyy-MM-dd hh:mm:ss")]
		YearMonthDayHourMinuteSecond,

		[Description("hh:mm:ss")]
		HourMinuteSecond,

		[Description("WeekDay")]
		WeekDay,

		[Description("yy")]
		Year,

		[Description("MM")]
		Month,

		[Description("MonthName")]
		MonthName,

		[Description("dd")]
		Day,

		[Description("DayName")]
		DayName,
	}

	public class Coordinates
	{
		public string CoordinateListId { get; set; }
		public List<Coordinate> CoordinateList { get; set; } = new List<Coordinate>();
		public string LegendName { get; set; }

		public void AddXYCoordinates(string x, string y)
		{
			if (CoordinateList == null)
				CoordinateList = new List<Coordinate>();

			CoordinateList.Add(new Coordinate(x, y));
		}

		public void AddXYCoordinates(string x, string y, string coordinateName)
		{
			if (CoordinateList == null)
				CoordinateList = new List<Coordinate>();

			CoordinateList.Add(new Coordinate(x, y, coordinateName));
		}
	}

	public class Coordinate
	{
		public Coordinate(string x, string y)
		{
			this.X = x;
			this.Y = y;
		}

		public Coordinate(string x, string y, string coordinateText) : this(x, y)
		{
			this.CoordinateText = coordinateText;
		}

		public string X { get; private set; }
		public string Y { get; private set; }
		public string CoordinateText { get; private set; }
	}
}