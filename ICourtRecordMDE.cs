/*
	'=======================================================================
	'   Author(s):      D V Rao
	'   Module/Form:    ICourtRecordMDE.cs
	'   Created Date:   04/23/2009
	'   Description:    Filer Actions Service Contract
	'
	'   Modification History:
	'=======================================================================
	'   Author(s)       Date        Control/Procedure       Change
	'=======================================================================
	'=======================================================================
	*/

using System.ServiceModel;
using Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;


namespace Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile
{
    [ServiceContract(Namespace = "urn:oasis:names:tc:legalxml-courtfiling:wsdl:WebServiceMessagingProfile-Definitions-4.0", Name = "CourtRecordMDEPort"   )]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal )]
    public interface ICourtRecordMDE
    {
        [OperationContract() ]
        RecordFilingResponse RecordFiling(RecordFilingRequest recordFilingRequest);

        [OperationContract()]
        GetCaseResponse GetCase(GetCaseRequest getCaseRequest);

        [OperationContract()]
        GetCaseListResponse GetCaseList(GetCaseListRequest getCaseListRequest);

        [OperationContract()]
        GetServiceInformationResponse GetServiceInformation(GetServiceInformationRequest getServiceInformationRequest);

        [OperationContract()]
        GetDocumentResponse GetDocument(GetDocumentRequest getDocumentRequest);

    }
}
