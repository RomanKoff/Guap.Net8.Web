namespace Guap.Net8.Web.Models
{

	public interface IGuapUserProfile
	{
		string NameIdentifier { get; set; }
		int Right { get; set; }
		string Roles { get; set; }
		string Actions { get; set; }
		string Resources { get; set; }
	}



	public class GuapUserProfileModel
		: IGuapUserProfile
	{
		public string NameIdentifier { get; set; }
		public int Right { get; set; }
		public string Roles { get; set; }
		public string Actions { get; set; }
		public string Resources { get; set; }
	}

}
