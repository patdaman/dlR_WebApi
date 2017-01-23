using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SignalAPILib.XifinBilling
{
    public static class soapAPI
    {
        public static XifinAccessionSearchService.AccessionSearchResponse AccessionSearch(
            XifinAccessionSearchService.AccessionSearchRequestPayload pl,
            string UserId,
            string SequenceNumber = null
            )
        {
            XifinAccessionSearchService.accessionSearchClient client = new XifinAccessionSearchService.accessionSearchClient();
            client.ClientCredentials.UserName.UserName = NonceUtil.UserName;

            XifinAccessionSearchService.MessageHeader mh = new XifinAccessionSearchService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationID,
                UserId = UserId,
            };
            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            XifinAccessionSearchService.AccessionSearchRequest asr = new XifinAccessionSearchService.AccessionSearchRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapper<XifinAccessionSearchService.AccessionSearchResponse>(
                () =>
                {
                    return client.AccessionSearch(asr);
                },
                new OperationContextScope(client.InnerChannel));
        }


        public static XifinAccessionService.ValidateAccessionResponse ValidateAcccession(
            XifinAccessionService.ValidateAccessionRequestPayload pl,
            string UserId,
            string SequenceNumber = null
            )
        {
            XifinAccessionService.accessionClient client = new XifinAccessionService.accessionClient();
            client.ClientCredentials.UserName.UserName = NonceUtil.UserName;

            XifinAccessionService.MessageHeader mh = new XifinAccessionService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationID,
                UserId = UserId,
            };

            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            XifinAccessionService.ValidateAccessionRequest vsr = new XifinAccessionService.ValidateAccessionRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapper<XifinAccessionService.ValidateAccessionResponse>(
                () =>
                {
                    return client.ValidateAccession(vsr);
                },
                new OperationContextScope(client.InnerChannel));
        }

        public static XifinAccessionService.GetAccessionResponse GetAcccession(
            XifinAccessionService.GetAccessionRequestPayload pl,
            string UserId,
            string SequenceNumber = null
            )
        {
            XifinAccessionService.accessionClient client = new XifinAccessionService.accessionClient();
            client.ClientCredentials.UserName.UserName = NonceUtil.UserName;

            XifinAccessionService.MessageHeader mh = new XifinAccessionService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationID,
                UserId = UserId,
            };

            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            XifinAccessionService.GetAccessionRequest gar = new XifinAccessionService.GetAccessionRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapper<XifinAccessionService.GetAccessionResponse>(
                () =>
                {
                    return client.GetAccession(gar);
                },
                new OperationContextScope(client.InnerChannel));
        }


        public static XifinAccessionService.CreateAccessionResponse CreateAcccession(
        XifinAccessionService.CreateAccessionRequestPayload pl,
        string UserId,
        string SequenceNumber = null
        )
        {
            XifinAccessionService.accessionClient client = new XifinAccessionService.accessionClient();
            client.ClientCredentials.UserName.UserName = NonceUtil.UserName; 

            XifinAccessionService.MessageHeader mh = new XifinAccessionService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationID, 
                UserId = UserId,
            };

            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            XifinAccessionService.CreateAccessionRequest car = new XifinAccessionService.CreateAccessionRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapper<XifinAccessionService.CreateAccessionResponse>( 
                () =>
                {
                    return client.CreateAccession(car);
                },
                new OperationContextScope(client.InnerChannel));
        }

        // Temporary SignalGenetics Testing system for testing CreateAccession service
        public static TestXifinAccessionSearchService.AccessionSearchResponse AccessionSearch(
        TestXifinAccessionSearchService.AccessionSearchRequestPayload pl,
            string UserId,
            string SequenceNumber = null
            )
        {
            TestXifinAccessionSearchService.accessionSearchClient client = new TestXifinAccessionSearchService.accessionSearchClient();
        client.ClientCredentials.UserName.UserName = NonceUtil.UserNameSGNLTest;

            TestXifinAccessionSearchService.MessageHeader mh = new TestXifinAccessionSearchService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationIDSGNLTest,
                UserId = UserId,
            };
            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            TestXifinAccessionSearchService.AccessionSearchRequest asr = new TestXifinAccessionSearchService.AccessionSearchRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapperSGNLTest<TestXifinAccessionSearchService.AccessionSearchResponse>(
                () =>
                {
                    return client.AccessionSearch(asr);
                },
                new OperationContextScope(client.InnerChannel));
        }


public static TestXifinAccessionService.CreateAccessionResponse CreateAcccessionSGNLTest(
        TestXifinAccessionService.CreateAccessionRequestPayload pl,
        string UserId,
        string SequenceNumber = null
        )
        {
            TestXifinAccessionService.accessionClient client = new TestXifinAccessionService.accessionClient();
            client.ClientCredentials.UserName.UserName = NonceUtil.UserNameSGNLTest;

            TestXifinAccessionService.MessageHeader mh = new TestXifinAccessionService.MessageHeader()
            {
                OrgAlias = NonceUtil.OrganizationIDSGNLTest,
                UserId = UserId,
            };

            if (SequenceNumber != null)
                mh.SequenceNumber = SequenceNumber;

            TestXifinAccessionService.CreateAccessionRequest car = new TestXifinAccessionService.CreateAccessionRequest()
            {
                MessageHeader = mh,
                Payload = pl
            };

            return NonceUtil.NonceWrapperSGNLTest<TestXifinAccessionService.CreateAccessionResponse>(
                () =>
                {
                    return client.CreateAccession(car);
                },
                new OperationContextScope(client.InnerChannel));
        }

    }
}
