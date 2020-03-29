using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using GitStat.Persistence;

namespace GitStat.ImportConsole
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("Import der Commits in die Datenbank");
			using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
			{
				Console.WriteLine("Datenbank löschen");
				unitOfWorkImport.DeleteDatabase();
				Console.WriteLine("Datenbank migrieren");
				unitOfWorkImport.MigrateDatabase();
				Console.WriteLine("Commits werden von commits.txt eingelesen");
				var commits = ImportController.ReadFromCsv();
				if (commits.Length == 0)
				{
					Console.WriteLine("!!! Es wurden keine Commits eingelesen");
					return;
				}
				Console.WriteLine(
					 $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
				unitOfWorkImport.CommitRepository.AddRange(commits);
				int countDevelopers = commits.GroupBy(c => c.Developer).Count();
				int savedRows = unitOfWorkImport.SaveChanges();
				Console.WriteLine(
					 $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
				Console.WriteLine();
				var csvCommits = commits.Select(c =>
					 $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
				File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
			}
			Console.WriteLine("Datenbankabfragen");
			Console.WriteLine("=================");
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				var g = unitOfWork.CommitRepository.GetAllCommitsGroupedByDevoloper();
				var group = g.GroupBy(g => g.Developer).OrderByDescending(g => g.Key.Commits.Count());
				Console.WriteLine("Statistik der Commits der Developer");
				Console.WriteLine("-----------------------------------");
				Console.WriteLine($"Developer	Commits	FileChanges	Insertions	Deletions");
				foreach (var pair in group)
				{
						Console.WriteLine($"{pair.Key.Name, -16} " +
							$"{pair.Key.Commits.Count(), 4} " +
							$"{pair.Key.Commits.Sum(s => s.FilesChanges), 12} " +
							$"{ pair.Key.Commits.Sum(s => s.Insertions), 13} " +
							$"{ pair.Key.Commits.Sum(s => s.Deletions), 14}");
				}
				var v = unitOfWork.CommitRepository.GetCommitByDeveloperID(4);
				Console.WriteLine("Commit mit ID 4");
				Console.WriteLine("-----------------------------------");
				Console.WriteLine($"{v.Developer.Name,-16} {v.Date.ToShortDateString(),4} {v.FilesChanges}\t {v.Insertions}\t {v.Deletions}");
				var s = unitOfWork.CommitRepository.GetCommitsByDate();
				Console.WriteLine("Commit der letzten 4 Wochen");
				Console.WriteLine("---------------------------");
				foreach (var item in s)
				{
					Console.WriteLine($"{item.Developer.Name,-16} {item.Date.ToShortDateString(),4} {item.FilesChanges}\t {item.Insertions}\t {item.Deletions}");
				}
			}
			Console.Write("Beenden mit Eingabetaste ...");
			Console.ReadLine();
		}

	}
}
