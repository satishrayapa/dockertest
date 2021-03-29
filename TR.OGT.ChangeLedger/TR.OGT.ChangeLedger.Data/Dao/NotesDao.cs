using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Options;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    public class NotesDao : BaseDao, IChildEntityDao
    {
        private const string NotesEntityName = "tPcNotes";

        private static readonly string _query = @"
                    select distinct
	                    pc.ParentNoteGuid as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
						join [Content_InProcess]..[tPcNotes] pc with (nolock)
							on ct.EntityID = pc.NoteGuid
                    where
	                    ct.TableName = 'tPcNotes'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion;
                    ";


        public NotesDao(ILogger<NotesDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => NotesEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}