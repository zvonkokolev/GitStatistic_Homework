using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;

namespace GitStat.ImportConsole
{
	public class ImportController
	{
		const string Filename = "commits.csv";
		public static Commit[] ReadFromCsv()
		{
			string[][] lines = MyFile.ReadStringMatrixFromCsv(Filename, false);
			List<Commit> commits = new List<Commit>();
			List<Commit> returnList = new List<Commit>();
			List<Developer> devs = new List<Developer>();
			Dictionary<Developer, Commit[]> developers = new Dictionary<Developer, Commit[]>();
			var c = lines.Select(l => l.ElementAt(0));
			Commit commit;
			var developersStrings = c.Distinct();
			Developer d;
			foreach (var item in developersStrings)
			{
				d = new Developer()
				{
					Name = item
				};
				var v = lines
					.Where(l => l.ElementAt(0).Equals(item))
					.Select(s => new
					{
						Date = DateTime.Parse(s.ElementAt(1)),
						Message = s.ElementAt(2),
						HashCode = s.ElementAt(3),
						FileChanges = int.Parse(s.ElementAt(4)),
						Insertions = int.Parse(s.ElementAt(5)),
						Deletions = int.Parse(s.ElementAt(6))
					}
					);
				foreach (var anonym in v)
				{
					commit = new Commit
					{
						Developer = d,
						Date = anonym.Date,
						Message = anonym.Message,
						HashCode = anonym.HashCode,
						FilesChanges = anonym.FileChanges,
						Insertions = anonym.Insertions,
						Deletions = anonym.Deletions
					};
					commits.Add(commit);
					returnList.Add(commit);
				}
				developers.Add(d, commits.ToArray());
				commits = new List<Commit>();
			}
			List<Commit> ret = new List<Commit>();
			foreach (var item in developers)
			{
				ret.AddRange(item.Value);
			}
			return ret.ToArray();
		}
	}
}
