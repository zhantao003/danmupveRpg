using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class SecureRequest:ABJsonSerializeObject
    {
        public string Data { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("Data", Data);
            return msg;
        }
    }

    public abstract class ABJsonSerializeObject {
        public abstract CLocalNetMsg GetJsonMsg();
      
    }
    public abstract class ABJsonDeSerializeObject {
        public abstract void FillDatas(string json);
    }
}
