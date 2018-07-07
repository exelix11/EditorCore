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

			internal GitHubRelease(Octokit.Release release)
			{
				CreatedAt = release.CreatedAt;
				Body = release.Body;
				Name = release.Name;
				TargetCommitish = release.TargetCommitish;
				TagName = release.TagName;
				Id = release.Id;
				Url = release.Url;
			}
		}

        public const int ReleaseID = 1;

        public static async Task<GitHubRelease> CheckForUpdates(string owner, string name)
        {
            var githubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("OdysseyEditor"));
            Octokit.Release ver = await githubClient.Repository.Release.GetLatest("exelix11", "OdysseyEditor");
			return new GitHubRelease(ver);
        }
    }
}
