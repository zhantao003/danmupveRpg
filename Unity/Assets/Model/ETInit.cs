using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
	public class ETInit : MonoBehaviour
	{
		public bool bDontDes;

		private void Start()
		{
			//this.StartAsync().Coroutine();
		}
		
		public async ETVoid StartAsync(Action callSuc = null)
		{
			try
			{
				Debug.Log("初始化ET框架");

				SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

				if(bDontDes)
                {
					DontDestroyOnLoad(gameObject);
				}
				
				ETGame.EventSystem.Add(DLLType.Model, typeof(ETInit).Assembly);

				ETGame.Scene.AddComponent<TimerComponent>();
				ETGame.Scene.AddComponent<NetOuterComponent>();
				ETGame.Scene.AddComponent<ResourcesComponent>();

				// 加载配置
				ETGame.Scene.AddComponent<OpcodeTypeComponent>();
				ETGame.Scene.AddComponent<MessageDispatcherComponent>();

                //自定义组件
                //Game.Scene.AddComponent<CRoomMgrComponent>();
				//ETGame.EventSystem.Run(EventIdType.ETFrameWorkInitFinish);

				callSuc?.Invoke();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Update()
		{
			OneThreadSynchronizationContext.Instance.Update();
			ETGame.EventSystem.Update();
		}

		private void LateUpdate()
		{
			ETGame.EventSystem.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			ETGame.Close();
		}
	}
}