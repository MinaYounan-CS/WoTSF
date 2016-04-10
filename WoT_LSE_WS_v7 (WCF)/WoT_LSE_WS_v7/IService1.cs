using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WoT_LSE_WS_v7
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(UriTemplate = "WoTLocSearch/{x}/{y}", ResponseFormat = WebMessageFormat.Json)]//, ResponseFormat = WebMessageFormat.Xml
        List<string> WoTLocSearch(string x, string y); 
    }

}
