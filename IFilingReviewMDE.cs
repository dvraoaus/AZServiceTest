/*
	'=======================================================================
	'   Author(s):      D V Rao
	'   Module/Form:    IFilerActions.cs
	'   Created Date:   04/23/2009
	'   Description:    Filer Actions Service Contract
	'
	'   Modification History:
	'=======================================================================
	'   Author(s)       Date        Control/Procedure       Change
	'=======================================================================
    '   Rao             04/25/2012  Adding NotifyFilingStatusChange Operation 
	'=======================================================================
	*/

using System.ServiceModel;
using Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;


namespace Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile
{
    [ServiceContract(Namespace = "urn:oasis:names:tc:legalxml-courtfiling:wsdl:WebServiceMessagingProfile-Definitions-4.0", Name = "FilingReviewMDEPort")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    public interface IFilingReviewMDE
    {
        [OperationContract()]
        ReviewFilingResponse ReviewFiling(ReviewFilingRequest reviewFilingRequest);

        [OperationContract()]
        NotifyDocketingCompleteResponse NotifyDocketingComplete(NotifyDocketingCompleteRequest notifyDocketingCompleteRequest);

        [OperationContract()]
        NotifyFilingStatusChangeResponse NotifyFilingStatusChange(NotifyFilingStatusChangeRequest notifyFilingStatusChangeRequest);

        [OperationContract()]
        GetFilingStatusResponse GetFilingStatus(GetFilingStatusRequest getFilingStatusRequest);

    }


}
