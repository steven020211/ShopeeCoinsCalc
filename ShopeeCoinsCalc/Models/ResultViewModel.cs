using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;

namespace ShopeeCoinsCalc.Models;

public class ResultViewModel
{
	public bool ShowResult { get; set; } = false;

	public string InputText { get; set; } = string.Empty;
	public int RetrivedCount { get; set; } = 0;
	public decimal TotalAmount = 0;
	public List<IGrouping<decimal, decimal>> SummaryData = new List<IGrouping<decimal, decimal>>();

	public void Count()
	{
		ShowResult = true;

		TotalAmount = 0;
		RetrivedCount = Regex.Matches(InputText, "直播蝦幣").Count;
		List<(Match, bool)> list = new List<(Match, bool)>();

		foreach (Match m in Regex.Matches(InputText, "直播蝦幣"))
			list.Add((m, true));


		string pattern = @"\+\d+(\.\d+)?";
		foreach (Match m in Regex.Matches(InputText, pattern))
		{
			list.Add((m, false));
			//Console.WriteLine(m.Index);
			//total+=decimal.Parse( m.Value);
		}

		list = list.OrderBy(x => x.Item1.Index).ToList();

		bool need = false;
		List<decimal> values = new List<decimal>();

		for (int i = 0; i < list.Count; i++)
		{
			var x = list[i];
			var m = x.Item1;
			if (need)
			{
				var v = decimal.Parse(m.Value);
				values.Add(v);
				TotalAmount += v;
				need = false;
				continue;
			}
			if (x.Item2)
			{
				need = true;
				continue;
			}
		}

		SummaryData = values.GroupBy(v => v)
			.OrderByDescending(v => v.Key)
			.ToList();

		
	}
}
