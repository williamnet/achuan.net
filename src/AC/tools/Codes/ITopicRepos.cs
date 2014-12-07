using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// ҵ�����������ITopicRepos ��ժҪ˵����
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public interface ITopicRepos
	{
		/// <summary>
		/// ����һ����¼
		/// </summary>
		int Insert(TopicDTO topicDTO);

		/// <summary>
		/// �޸�һ����¼
		/// </summary>
		void Update(TopicDTO topicDTO);

		/// <summary>
		/// ɾ��һ����¼
		/// </summary>
		void Delete(int topicId);

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		TopicDTO GetById(int topicId);

		/// <summary>
		/// ��ѯ����
		/// </summary>
		IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO);

	}
}

