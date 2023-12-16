using System;

namespace DouyuDanmu
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class MessageAttribute: Attribute
	{
		public sbyte Opcode { get; }

		public Type AttributeType { get; }


		public MessageAttribute(sbyte opcode)
		{
			this.Opcode = opcode;
			this.AttributeType = this.GetType();
		}
	}
}