
using System.Collections.Generic;
using GenericAlgorithm.Model;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.Shapes.Charts;

namespace GenericAlgorithm
{
    internal static class ChartManager
    {
        public static void CreateChart(DataHolder tournamentDataHolder, DataHolder rouletteDataHolder)
        {
            Document document = new Document();

            Section section = document.AddSection();
            section.PageSetup.Margins.All = 72f;

            Paragraph paragraph = section.AddParagraph();
            paragraph.AppendText("نمودار همگرایی");
            paragraph.ApplyStyle(BuiltinStyle.Heading1);


            for (var i = 0; i < tournamentDataHolder.bestFit.Count; i+= 5)
            {
                var lastIndex = tournamentDataHolder.bestFit.Count > i + 4 ? (i + 5) : (tournamentDataHolder.bestFit.Count);
                var stages = new List<string>();
                var tournamentFitnessList = new List<double>();
                var rouletteFitnessList = new List<double>();

                paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
                paragraph = section.AddParagraph();
                paragraph.AppendText("\n\n\n");
                ShapeObject shape = paragraph.AppendChart(ChartType.Column, 400, 200);

                Chart chart = shape.Chart;
                chart.Series.Clear();

                for (int j = i; j < lastIndex; j++)
                {
                    stages.Add("دور" + (j + 1).ToString());
                    tournamentFitnessList.Add(tournamentDataHolder.bestFit[j].Fitness());
                    rouletteFitnessList.Add(rouletteDataHolder.bestFit[j].Fitness());
                }

                chart.Series.Add("tournament", stages.ToArray(), tournamentFitnessList.ToArray());
                chart.Series.Add("roulette", stages.ToArray(), rouletteFitnessList.ToArray());

                chart.Title.Text = "نمودار همگرایی دور " + (i + 1).ToString() + " تا " + (lastIndex).ToString();

                chart.AxisY.NumberFormat.FormatCode = "#,##0";
            }



            document.SaveToFile("chart.docx", FileFormat.Docx2016);
            document.Dispose();
            System.Diagnostics.Process.Start(@"chart.docx");
        }
    }
}
