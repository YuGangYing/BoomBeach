using System;
using UnityEngine.Events;
using UnityEngine;

namespace WWWNetwork
{
	public class BaseAPI
	{
		public string api;
		public byte[] bytedata;
		public virtual void Send(UnityAction<WWW> complete){
			WWWNetworkManager.GetInstance.Send (api, bytedata, complete);
		}

	}
}

