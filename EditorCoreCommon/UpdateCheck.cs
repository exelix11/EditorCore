using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore
{
	public class GitHubUpdateCheck
	{
		public class GitHubRelease
		{
			public DateTimeOffset CreatedAt { get; protected set; }
			public string Body { get; protected set; }
			public string Name { get; protected set; }
			public string TargetCommitish { get; protected set; }
			public string TagName { get; protected set; }
			public int Id { get; protected set; }
			public string Url { get; protected set; }

			public int Index { get; protected set; }

			internal GitHubRelease(Octokit.Release release, int index)
			{
				CreatedAt = release.CreatedAt;
				Body = release.Body;
				Name = release.Name;
				TargetCommitish = release.TargetCommitish;
				TagName = release.TagName;
				Id = release.Id;
				Url = release.Url;
				Index = index;
			}
		}
		
        public static async Task<GitHubRelease> CheckForUpdates(string owner, string name)
        {
            var githubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("EditorCore"));
            var ver = await githubClient.Repository.Release.GetAll(owner, name);
			return new GitHubRelease(ver[ver.Count - 1],ver.Count -1);
        }
    }
}
