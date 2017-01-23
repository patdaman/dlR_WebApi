using Microsoft.Web.Services3.Security.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SignalAPILib.XifinBilling
{
    public class NonceUtil
    {
        public static string UserName
        {
            get
            {
                return "signalgprod";
            }
        }

        public static string OrganizationID
        {
            get
            {
                return "signalg";
            }
        }

        private static readonly string password = "4Rrainy^&UJ";
        public static T NonceWrapper<T>(Func<object> func, OperationContextScope scope)
        {
            UsernameToken token = new UsernameToken(UserName, password);
            MessageHeader mhead = MessageHeader.CreateHeader("Security",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd",
                token.GetXml(new System.Xml.XmlDocument()));
            OperationContext.Current.OutgoingMessageHeaders.Add(mhead);
            return (T)func.Invoke();
        }


        // Credentials for use with SGNL Testing system for testing CreateAccession service
        public static string UserNameSGNLTest
        {
            get
            {
                return "signalgtest";
            }
        }
        private static readonly string passwordSGNLTest = "1oVrenr693nkKds3";
        public static T NonceWrapperSGNLTest<T>(Func<object> func, OperationContextScope scope)
        {
            UsernameToken token = new UsernameToken(UserNameSGNLTest, passwordSGNLTest);
            MessageHeader mhead = MessageHeader.CreateHeader("Security",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd",
                token.GetXml(new System.Xml.XmlDocument()));
            OperationContext.Current.OutgoingMessageHeaders.Add(mhead);
            return (T)func.Invoke();
        }
        public static string OrganizationIDSGNLTest
        {
            get
            {
                return "signalgtest";
            }
        }

    }
}
