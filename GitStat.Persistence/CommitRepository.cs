using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
	public class CommitRepository : ICommitRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public CommitRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public void AddRange(Commit[] commits)
		{
			_dbContext.Commits.AddRange(commits);
		}
		public Commit[] GetAllCommitsGroupedByDevoloper() => _dbContext.Commits.Include(d => d.Developer).ToArray();
		public Commit GetCommitByDeveloperID(int commit)
		{
			var v = _dbContext.Commits.Where(w => w.Id == commit).FirstOrDefault();
			return v;
		}
		public Commit[] GetCommitsByDate()
		{
			var maxSec = _dbContext.Commits.OrderByDescending(d => d.Date).Select(s => s.Date.AddDays(-28)).FirstOrDefault();
			var v = _dbContext.Commits.Include(d => d.Developer).OrderByDescending(d => d.Date).Where(w => w.Date > maxSec).ToArray();
			return v;
		}

	}
}