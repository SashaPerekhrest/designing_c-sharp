using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Delegates.Reports
{
    public delegate string ReportDelegate(ReportMaker printer, List<Measurement> pData);

    public abstract class ReportMaker
    {
        public abstract string MakeCaption(string caption);
        public abstract string BeginList();
        public abstract string MakeItem(string valueType, string entry);
        public abstract string EndList();
        public abstract string Caption { get; }
        public string MakeReport(ReportDelegate reportMethod, ReportMaker printer, List<Measurement> pData)
        => reportMethod(printer, pData);
    }

    public class Calculations
    {
        public static List<Measurement> Data;

        public static IEnumerable<double> GetTemperature()
        => Data.Select(z => z.Temperature);

        public static IEnumerable<double> GetHumidity()
        => Data.Select(z => z.Humidity);

        public static double GetAverageTemperature()
        => Data.Select(z => z.Temperature).Average();

        public static double GetAverageHumidity()
        => Data.Select(z => z.Humidity).Average();

        public static double GetTemperatureStandartDeviation()
        => Math.Sqrt(Data.Select(z => Math.Pow(z.Temperature - GetAverageTemperature(), 2)).Sum() / (Data.Count - 1));

        public static double GetHumidityStandartDeviation()
        => Math.Sqrt(Data.Select(z => Math.Pow(z.Humidity - GetAverageHumidity(), 2)).Sum() / (Data.Count - 1));

        private static double GetMedian(IEnumerable<double> source)
        {
	        var temp = source.ToArray();
            Array.Sort(temp);

            var count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
	        if (count % 2 == 0)
            {
	            var a = temp[count / 2 - 1];
                var b = temp[count / 2];
                return (a + b) / 2.0;
            }
            return temp[count / 2];
        }

        public static double GetHumidityMedian() => GetMedian(Data.Select(z => z.Humidity));

        public static double GetTemperatureMedian() => GetMedian(Data.Select(z => z.Temperature));
    }

    public class Printing{
        public static string PrintMeanAndStd(ReportMaker printer, List<Measurement> pData)
        {
            Calculations.Data = pData;

            var avTmp = Calculations.GetAverageTemperature();
            var avHum = Calculations.GetAverageHumidity();
            var devTmp = Calculations.GetTemperatureStandartDeviation();
            var devHum = Calculations.GetHumidityStandartDeviation();
            
			return MakeString(
				"Mean and Std",
				String.Format(CultureInfo.InvariantCulture, "{0}±{1}", avTmp, devTmp),
				String.Format(CultureInfo.InvariantCulture, "{0}±{1}", avHum, devHum),
				printer);
        }

        public static string PrintMedian(ReportMaker printer, List<Measurement> pData)
        {
            Calculations.Data = pData;

            var medTmp = Calculations.GetTemperatureMedian();
            var medHum = Calculations.GetHumidityMedian();
            
            return MakeString(
	            "Median",
	            String.Format(CultureInfo.InvariantCulture,"{0}", medTmp),
	            String.Format(CultureInfo.InvariantCulture,"{0}", medHum),
	            printer);
        }

        private static string MakeString(string name, string temperature, string humidity, ReportMaker printer)
        {
	        var result = new StringBuilder();
	        result.Append(printer.MakeCaption(name));
	        result.Append(printer.BeginList());
	        result.Append(printer.MakeItem("Temperature", temperature));
	        result.Append(printer.MakeItem("Humidity", humidity));
	        result.Append(printer.EndList());
	        return result.ToString();
        }
    }

	public class HtmlReportMaker : ReportMaker
	{
        public override string Caption => "Mean and Std";

        public override string MakeCaption(string caption) => $"<h1>{caption}</h1>";

        public override string BeginList() => "<ul>";

        public override string EndList() => "</ul>";

        public override string MakeItem(string valueType, string entry) => $"<li><b>{valueType}</b>: {entry}";
	}

	public class MarkdownReportMaker : ReportMaker
	{
		public override string Caption => "Median"; 
		
		public override string BeginList() => "";

		public override string EndList() => "";

        public override string MakeCaption(string caption) => $"## {caption}\n\n";
        
        public override string MakeItem(string valueType, string entry) => $" * **{valueType}**: {entry}\n\n";
	}

	public static class ReportMakerHelper
	{   
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
            var hm = new HtmlReportMaker();

            return hm.MakeReport(Printing.PrintMeanAndStd, hm, data.ToList<Measurement>());
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
            var mm = new MarkdownReportMaker();

            return mm.MakeReport(Printing.PrintMedian, mm, data.ToList<Measurement>());
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
            var mm = new MarkdownReportMaker();

            return mm.MakeReport(Printing.PrintMeanAndStd, mm, measurements.ToList<Measurement>());
        }

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
            var hm = new HtmlReportMaker();

            return hm.MakeReport(Printing.PrintMedian, hm, measurements.ToList<Measurement>());
        }
	}
}