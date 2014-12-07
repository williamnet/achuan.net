using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// Service��TopicService ��ժҪ˵����
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	[Spring(ConstructorArgs = "topicRepos:topicRepos")]
	public class TopicService : ITopicService
	{
		private readonly ITopicRepos topicRepos;
		public TopicService(ITopicRepos topicRepos)
		{
			this.topicRepos = topicRepos;
		}
		/// <summary>
		/// ����һ����¼
		/// </summary>
		public int Create(TopicDTO topicDTO)
		{
			return topicRepos.Insert(topicDTO);
		}

		/// <summary>
		/// �޸�һ����¼
		/// </summary>
		public void Modify(TopicDTO topicDTO)
		{
			topicRepos.Update(topicDTO);
		}

		/// <summary>
		/// ɾ��һ����¼
		/// </summary>
		public void Remove(int topicId)
		{
			topicRepos.Delete(topicId);
		}

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public TopicDTO GetById(int topicId)
		{
			return topicRepos.GetById(topicId);
		}

		/// <summary>
		/// ��ѯ����
		/// </summary>
		public IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO)
		{
			return topicRepos.Query(topicQueryDTO);
		}

	}
}

