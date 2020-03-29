using GitStat.Core.Contracts;
using System.Collections.Generic;
using GitStat.Core.Entities;
using System.Linq;

namespace GitStat.Persistence
{
	public class DeveloperRepository : IDeveloperRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public DeveloperRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		//public Commit[] GetAllDevoloperGroupedByName()
		//{
		//	var a = _dbContext.Commits.ToArray();
		//	return a;
		//}
	}
}