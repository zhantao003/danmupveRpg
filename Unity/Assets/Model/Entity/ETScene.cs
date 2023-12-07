namespace ETModel
{
	public static class SceneType
	{
		public const string Share = "Share";
		public const string Game = "Game";
		public const string Login = "Login";
		public const string Lobby = "Lobby";
		public const string Map = "Map";
		public const string Launcher = "Launcher";
		public const string Robot = "Robot";
		public const string RobotClient = "RobotClient";
		public const string Realm = "Realm";
	}
	
	public sealed class ETScene: Entity
	{
		public string Name { get; set; }

		public ETScene()
		{
		}

		public ETScene(long id): base(id)
		{
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}