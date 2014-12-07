using System.Collections.Generic;

namespace AC.Tools.Codes
{
	/// <summary>
	/// ҵ���߼���ITopicService ��ժҪ˵����
	/// Generate By: www.wangyuchuan.com
	/// Generate Time: 2014-11-08 00:49:09
	/// </summary>
	public interface ITopicService
	{
		/// <summary>
		/// ����һ����¼
		///<param name="topicDTO">Ҫ�����Ķ���</param>
		/// </summary>
		int Create(TopicDTO topicDTO);

		/// <summary>
		/// �޸�һ����¼
		///<param name="topicDTO">Ҫ�޸ĵĶ���</param>
		/// </summary>
		void Modify(TopicDTO topicDTO);

		/// <summary>
		/// ɾ��һ����¼
		/// </summary>
		void Remove(int topicId);

		/// <summary>
		/// ����Id�õ�һ������ʵ��
		/// </summary>
		TopicDTO GetById(int topicId);

		/// <summary>
		/// ��ѯ����
		/// </summary>
		IList<TopicDTO> Query(TopicQueryDTO topicQueryDTO);

	}
}

