using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;

namespace TR.OGT.QueryExtract.Domain
{
	public interface IExtractService
	{
		Task<Result> HandleCreateEvent(IEnumerable<Guid> ids);
		Task<Result> HandleUpdateEvent(IEnumerable<Guid> ids);
		Task<Result> HandleDeleteEvent(IEnumerable<Guid> ids);
	}
}