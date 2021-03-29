using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TR.OGT.QueryExtract.Infrastructure
{
	public interface ISQSClient
	{
		Task AckMessage(Message message);
		Task AckMessages(IEnumerable<Message> messages);
		Task ReleaseMessages(IEnumerable<Message> messages);
		void Dispose();
		Task<ICollection<Message>> GetMessages();
	}
}