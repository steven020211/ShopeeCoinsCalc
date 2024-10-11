using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;
using System.Xml;

namespace ShopeeCoinsCalc.Models;

public class ResultViewModel
{
	public bool ShowResult { get; set; } = false;
	public bool DataError { get; set; } = false;

	public string InputText { get; set; } = string.Empty;
	public int RetrivedCount { get; set; } = 0;
	public decimal TotalAmount = 0;
	public List<IGrouping<decimal, decimal>> SummaryData = new List<IGrouping<decimal, decimal>>();

	public List<IGrouping<string, CoinInfo>> CoinInfos = null!;

    public class CoinInfo
	{
		public required string Title;
		public required string Description;
		public DateTime Time;
		public decimal Amount;
	}

	public void CountNew()
	{
        ShowResult = true;
		

		bool counting()
		{
            var list = InputText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

			if (list.Count < 4) return false;

			List<CoinInfo> all = new List<CoinInfo>();

			for(int i = 0;i <list.Count; i+=4)
			{
				//data end
				if (list.Count < i + 4) break;
				var info = new CoinInfo()
				{
					Title = list[i],
					Description = list[i + 1]
				};

				if (!DateTime.TryParse(list[i + 2], out info.Time)) return false;
                if (!decimal.TryParse(list[i + 3], out info.Amount)) return false;

				all.Add(info);
            }

			CoinInfos = all.GroupBy(x => x.Title).ToList();

			//直播 (相容舊版)
			var live_group = CoinInfos.FirstOrDefault(g => g.Key == "直播蝦幣");

            TotalAmount = live_group?.Sum(x => x.Amount) ?? 0;
			RetrivedCount = live_group?.Count() ?? 0;

			SummaryData = live_group?
				.Select(x => x.Amount)
				.GroupBy(v => v)
				.OrderByDescending(v=>v.Key)
				.ToList() ?? new List<IGrouping<decimal, decimal>>();

			return true;
        }

		DataError = !counting();
    }

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
