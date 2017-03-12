using Niem.Structures.v20;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using amc = Arizona.Courts.ExChanges.v20;
using aoc = Arizona.Courts.Extensions.v20;
using azs = Arizona.Courts.Services.v20;
using caseQuery = Oasis.LegalXml.CourtFiling.v40.CaseQuery;
using docQuery = Oasis.LegalXml.CourtFiling.v40.DocumentQuery;
using ecf = Oasis.LegalXml.CourtFiling.v40.Ecf;
using j = Niem.Domains.Jxdm.v40;
using nc = Niem.NiemCore.v20;
using niemxsd = Niem.Proxy.xsd.v20;
using UBL21 = Oassis.UBL.v21;
using wmp = Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;

namespace AZServiceTest
{
    public partial class Form1 : Form
    {
        private const string DEFAULT_CONFIG_FILE = @".\AZCRMDE.config";
        private string _configFile = string.Empty;
        private const string PARTIAL_ACCEPTANCE = "PARTIAL";
        private XmlSerializer _civilCaseSerializer = null;
        private XmlSerializerNamespaces _civilCaseNamespaces = null;

        private XmlSerializer _getCaseResponseSerializer = null;
        private XmlSerializerNamespaces _getCaseResponseNameSpaces = null;


        public Form1()
        {
            InitializeComponent();
            _configFile = ConfigurationManager.AppSettings["ServiceConfigurationFile"];
            if (string.IsNullOrEmpty(_configFile))
            {
                _configFile = DEFAULT_CONFIG_FILE;
            }
        }

        

        private void btnGetCase_Click(object sender, EventArgs e)
        {
            azs.ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<azs.ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        _configFile
                    );

                wmp.GetCaseRequest request = new wmp.GetCaseRequest
                (
                      new amc.GetCaseRequestType { CaseQueryMessage = this.SampleCaseQuery }
                );
                wmp.GetCaseResponse response = _serviceChannel.GetCase(request);
                Save(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

            }

        }

        private caseQuery.CaseQueryMessageType SampleCaseQuery
        {
            get
            {
                return new caseQuery.CaseQueryMessageType
                {
                    CaseTrackingID = new Niem.Proxy.xsd.v20.String(this.textBoxCaseNumber.Text.Trim()),
                    CaseCourt = this.CaseCourt,
                    QuerySubmitter = new Niem.NiemCore.v20.EntityType
                    {
                        EntityRepresentation = this.PersonAtKeyBoard,
                        EntityRepresentationType = Niem.NiemCore.v20.EntityRepresentationTpes.EcfPerson
                    },
                    CaseQueryCriteria = new caseQuery.CaseQueryCriteriaType
                    {
                         IncludeParticipantsIndicator = new niemxsd.Boolean(true) ,
                         IncludeCalendarEventIndicator = new niemxsd.Boolean(true) ,
                          IncludeDocketEntryIndicator = new niemxsd.Boolean(true)
                    }
                };
            }
        }


       private docQuery.DocumentQueryMessageType SampleDocumentQuery
        {
            get
            {
                return new docQuery.DocumentQueryMessageType
                {
                    CaseTrackingID = new Niem.Proxy.xsd.v20.String(this.textBoxCaseNumber.Text.Trim()),
                    CaseDocketID = new niemxsd.String(this.tbDocketId.Text.Trim()) ,
                    CaseCourt = this.CaseCourt,
                    QuerySubmitter = new Niem.NiemCore.v20.EntityType
                    {
                        EntityRepresentation = this.PersonAtKeyBoard,
                        EntityRepresentationType = Niem.NiemCore.v20.EntityRepresentationTpes.EcfPerson
                    },
                    SendingMDELocationID = new nc.IdentificationType("FAMDE TEST"),
                     SendingMDEProfileCode = amc.PolicyConstants.WEB_SERVICES_SIP_CODE 

                };
            }
        }

        private ecf.PersonType PersonAtKeyBoard
        {
            get
            {
                ecf.PersonType personAtKeyBoard = new ecf.PersonType
                                (
                                    id: string.Empty,
                                    prefix: string.Empty,
                                    givenName: "Jason",
                                    middleName: string.Empty,
                                    surName: "Alexander",
                                    suffix: string.Empty,
                                    eportalUserId: "6420",
                                    contactEntity: null,
                                    address1: "400 West Congress South Building",
                                    address2: "Suite 315",
                                    city: "Tuscon",
                                    state: "AZ",
                                    zipCode: "85701",
                                    phoneNumber: "5206286504",
                                    extension: string.Empty,
                                    emailAddress: "jalexander@udalllaw.com",
                                    countryCode: "US"
                                );
                return personAtKeyBoard;
            }

        }
        private j.CourtType CaseCourt
        {
            get
            {
                return new j.CourtType
                {
                    // OrganizationIdentification = new List<nc.IdentificationType> { new nc.IdentificationType("courts.az.gov:1300") },
                    OrganizationIdentification = new List<nc.IdentificationType> { new nc.IdentificationType("courts.az.gov:1000") },
                    OrganizationLocation = new List<nc.LocationType>
                     {
                         new nc.LocationType
                         {
                              LocationAddress = new List<nc.AddressType>
                              {
                                  new nc.AddressType
                                  (
                                        address1: "110 West Congress Street" ,
                                        address2 : string.Empty ,
                                        city : "Tucson" ,
                                        state: "AZ" ,
                                        zipCode:"85701" ,
                                        countryCode:"US"
                                  ) 
                                  
                              }
                         }
                     },
                    CourtName = new List<nc.TextType>
                     {
                         new  nc.TextType("Pima County Superior Court")
                     }
                };
            }

        }
        private void Save(wmp.GetCaseResponse response)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (response != null && response.CaseResponseMessage != null && response.CaseResponseMessage.Case != null && response.CaseResponseMessage.Case is aoc.CivilCaseType )
                {

                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.CheckFileExists = false;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog.Title = "Select a file to save  to ";
                    saveFileDialog.FileName = this.textBoxCaseNumber.Text;
                    DialogResult dr = saveFileDialog.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        SaveGetCaseResponse(response, saveFileDialog.FileName);
                        MessageBox.Show(
                            string.Format("Saved GetCase Response to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Response is null !!!!"), "Save");
                }
            }
            finally
            {
                if (saveFileDialog != null)
                {
                    saveFileDialog.Dispose();
                    saveFileDialog = null;
                }

            }
        }

        private void SaveGetCaseResponse(wmp.GetCaseResponse response, string fileName)
        {
            if (response != null && !string.IsNullOrWhiteSpace(fileName))
            {

                if (_civilCaseSerializer == null)
                {
                    _civilCaseNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                    ecf.EcfHelper.AddNameSpaces(_civilCaseNamespaces);
                    _civilCaseSerializer = new XmlSerializer(typeof(aoc.CivilCaseType));

                }
                if (File.Exists(fileName)) File.Delete(fileName);
                using (var fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
                {
                    _civilCaseSerializer.Serialize(fs, response.CaseResponseMessage.Case, _civilCaseNamespaces);
                    fs.Flush();
                    fs.Close();
                }
            }
        }

        private void SaveCompleteGetCaseResponse(wmp.GetCaseResponse response, string fileName)
        {
            if (response != null && !string.IsNullOrWhiteSpace(fileName))
            {

                if (_getCaseResponseSerializer == null)
                {
                    _getCaseResponseNameSpaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                    ecf.EcfHelper.AddNameSpaces(_getCaseResponseNameSpaces);
                    _getCaseResponseSerializer = new XmlSerializer(typeof(wmp.GetCaseResponse));

                }
                if (File.Exists(fileName)) File.Delete(fileName);
                using (var fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
                {
                    _getCaseResponseSerializer.Serialize(fs, response, _getCaseResponseNameSpaces);
                    fs.Flush();
                    fs.Close();
                }
            }
        }


        private void Save(wmp.GetDocumentResponse response)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (response != null)
                {

                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.CheckFileExists = false;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog.Title = "Select a file to save  to ";
                    DialogResult dr = saveFileDialog.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew, FileAccess.Write))
                        {
                            XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                            ecf.EcfHelper.AddNameSpaces(namespaces);
                            XmlSerializer serializer = new XmlSerializer(typeof(wmp.GetDocumentResponse));
                            serializer.Serialize(fs, response, namespaces);
                            fs.Flush();
                            fs.Close();
                        }
                        MessageBox.Show(
                            string.Format("Saved GetDocument Response to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Response is null !!!!"), "Save");
                }
            }
            finally
            {
                if (saveFileDialog != null)
                {
                    saveFileDialog.Dispose();
                    saveFileDialog = null;
                }

            }
        }


        private void Save(wmp.ReviewFilingResponse response)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (response != null)
                {

                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.CheckFileExists = false;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog.Title = "Select a file to save  to ";
                    DialogResult dr = saveFileDialog.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew, FileAccess.Write))
                        {
                            XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                            ecf.EcfHelper.AddNameSpaces(namespaces);
                            XmlSerializer serializer = new XmlSerializer(typeof(wmp.ReviewFilingResponse));
                            serializer.Serialize(fs, response, namespaces);
                            fs.Flush();
                            fs.Close();
                        }
                        MessageBox.Show(
                            string.Format("Saved Review Filing Response to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Response is null !!!!"), "Save");
                }
            }
            finally
            {
                if (saveFileDialog != null)
                {
                    saveFileDialog.Dispose();
                    saveFileDialog = null;
                }

            }
        }

        

        private void btnSubmitAllAcceptedNoChnage_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, 
                        documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED  ,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, 
                        filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED ,
                        netZeroSubmission: false ,
                        overPaymentAmount : 0.00M ,
                        changeDocumentType : false
                    );
                if (ndcRequest != null)
                {
                    _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                        (
                            "FilingReviewMDEService",
                            _configFile
                        );

                    wmp.NotifyDocketingCompleteResponse response = _serviceChannel.NotifyDocketingComplete(ndcRequest);
                    Save(response);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

            }
       }

        private wmp.NotifyDocketingCompleteRequest GetNDC(string documentStatusCode , string documentStatusDescription , string filingStatusCode , string filingStatusDescription , bool netZeroSubmission, decimal overPaymentAmount , bool changeDocumentType)
        {
            wmp.NotifyDocketingCompleteRequest ndcRequest = null;

            List<nc.EntityType> documentReviwer = null;
            List<nc.EntityType> documentReviwerAsReference = null;
            wmp.RecordFilingRequest rfr = this.GetRfrFromRvfr(documentStatusCode, documentStatusDescription , documentReviwer:out documentReviwer, documentReviwerAsReference:out documentReviwerAsReference);
            if (rfr != null)
            {
                    aoc.CoreFilingMessageType filingMessage = rfr.AZRecordFilingRequest.CoreFilingMessage;
                    aoc.PaymentMessageType paymentMessage = rfr.AZRecordFilingRequest.PaymentMessage;
                    List<aoc.RecordDocketingMessageType> docketingMessages = rfr.AZRecordFilingRequest.RecordDocketingMessage;
                    List<aoc.RecordDocketingCallbackMessageType> recordDocketingCallBackMessages = new List<aoc.RecordDocketingCallbackMessageType>();

                    nc.CaseType callBackCase = new nc.CaseType();
                    aoc.CivilCaseType aocCivilCase = filingMessage.Case != null ? filingMessage.Case as aoc.CivilCaseType : null;
                    string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                    if (aocCivilCase != null && !string.IsNullOrWhiteSpace(filingStatusCode) && 
                         (filingStatusCode.Equals(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, StringComparison.OrdinalIgnoreCase) ||
                           filingStatusCode.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase) ||
                           filingStatusCode.Equals(amc.PolicyConstants.FILING_STATUS_NET_ZERO, StringComparison.OrdinalIgnoreCase)
                         )

                        )
                    {
                        aocCivilCase.CaseTrackingID = new List<niemxsd.String> { new niemxsd.String(ajacsCaseNumber) };
                        callBackCase.CaseTrackingID = new List<niemxsd.String> { new niemxsd.String(ajacsCaseNumber) };
                        callBackCase.CaseTitleText = aocCivilCase.CaseTitleText;
                        callBackCase.CaseCategoryText = aocCivilCase.CaseCategoryText;
                    }
                    // Generate Call Back Message
                    aoc.RecordDocketingCallbackMessageType recordDocketingCallBack = null;
                    int documentNumber = 0;
                    foreach (var dm in docketingMessages)
                    {
                        documentNumber++;
                        ecf.FilingStatusType filingStatus = new ecf.FilingStatusType
                           {
                               FilingStatusCode = filingStatusCode,
                               StatusDescriptionText = nc.NiemStringHelper<nc.TextType>.ToList(filingStatusDescription)
                           };

                        if (filingStatusCode.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase))
                        {
                            if ( documentNumber % 2 == 1)
                            {
                                filingStatus = new ecf.FilingStatusType
                                {
                                    FilingStatusCode = amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                                    StatusDescriptionText = nc.NiemStringHelper<nc.TextType>.ToList(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED)
                                };
                            }
                            else
                            {
                                filingStatus = new ecf.FilingStatusType
                                {
                                    FilingStatusCode = amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED,
                                    StatusDescriptionText = nc.NiemStringHelper<nc.TextType>.ToList(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED)
                                };
                            }
                        }
                        recordDocketingCallBack = new aoc.RecordDocketingCallbackMessageType
                        {
                            DocumentFiledDate = new List<nc.DateType> { new nc.DateType(DateTime.Now) },
                            DocumentIdentification = filingMessage.DocumentIdentification,
                            DocumentPostDate = filingMessage.DocumentPostDate,
                            DocumentReceivedDate = filingMessage.DocumentReceivedDate,
                            DocumentInformationCutOffDate = filingMessage.DocumentInformationCutOffDate,
                            DocumentSubmitter = documentNumber == 1 ?  documentReviwer : documentReviwerAsReference,
                            SendingMDELocationID = new nc.IdentificationType("CRMDE ADDRESS"),
                            SendingMDEProfileCode = nc.Constants.ECF4_WEBSERVICES_SIP_CODE,
                            FilingStatus = filingStatus,
                            ReviewedLeadDocument = dm.ReviewedLeadDocument,
                            ReviewedConnectedDocument = dm.ReviewedConnectedDocument,
                            Case = callBackCase,
                            CaseTypeSelection = ecf.CaseTypeSelectionType.NiemCase
                        };
                        recordDocketingCallBackMessages.Add(recordDocketingCallBack);

                    }
                if (netZeroSubmission  && paymentMessage != null && paymentMessage.AllowanceCharge != null && paymentMessage.AllowanceCharge.Count > 0)
                {
                    List<aoc.AllowanceChargeType> deleteList = new List<aoc.AllowanceChargeType>();
                    foreach (aoc.AllowanceChargeType charge in paymentMessage.AllowanceCharge)
                    {
                        string summaryDescription = charge.AllowanceChargeCategoryCode != null && !string.IsNullOrEmpty(charge.AllowanceChargeCategoryCode.Value) ? charge.AllowanceChargeCategoryCode.Value : string.Empty;
                        if (summaryDescription.Equals(amc.PolicyConstants.ALLOWANCE_CHARGE_CATEGORY_CODE_FILING_FEE, StringComparison.OrdinalIgnoreCase))
                        {
                            deleteList.Add(charge);
                        }
                    }
                    foreach (var charge in deleteList)
                    {
                        paymentMessage.AllowanceCharge.Remove(charge);
                    }

                }

                if (overPaymentAmount > 0.00M  && paymentMessage != null && paymentMessage.AllowanceCharge != null && paymentMessage.AllowanceCharge.Count > 0)
                {
                    foreach (aoc.AllowanceChargeType charge in paymentMessage.AllowanceCharge)
                    {
                        string summaryDescription = charge.AllowanceChargeCategoryCode != null && !string.IsNullOrEmpty(charge.AllowanceChargeCategoryCode.Value) ? charge.AllowanceChargeCategoryCode.Value : string.Empty;
                        decimal itemAmount = charge.Amount != null ? charge.Amount.Value : 0.00M;

                        if (itemAmount > overPaymentAmount && summaryDescription.Equals(amc.PolicyConstants.ALLOWANCE_CHARGE_CATEGORY_CODE_FILING_FEE, StringComparison.OrdinalIgnoreCase))
                        {
                            charge.Amount.Value = charge.Amount.Value - overPaymentAmount;
                        }
                    }

                }

                if (changeDocumentType && paymentMessage != null && paymentMessage.AllowanceCharge != null && paymentMessage.AllowanceCharge.Count > 0)
                {
                    List<aoc.AllowanceChargeType> deleteList = new List<aoc.AllowanceChargeType>();
                    foreach (aoc.AllowanceChargeType charge in paymentMessage.AllowanceCharge)
                    {
                        string summaryDescription = charge.AllowanceChargeCategoryCode != null && !string.IsNullOrEmpty(charge.AllowanceChargeCategoryCode.Value) ? charge.AllowanceChargeCategoryCode.Value : string.Empty;
                        string chargeId = charge.ID != null && !string.IsNullOrWhiteSpace(charge.ID.Value) ? charge.ID.Value : string.Empty;
                        if (!string.IsNullOrWhiteSpace(chargeId) &&
                             chargeId.Equals("P2", StringComparison.OrdinalIgnoreCase) &&
                            summaryDescription.Equals(amc.PolicyConstants.ALLOWANCE_CHARGE_CATEGORY_CODE_FILING_FEE, StringComparison.OrdinalIgnoreCase))
                        {
                            deleteList.Add(charge);
                        }
                    }
                    foreach (var charge in deleteList)
                    {
                        paymentMessage.AllowanceCharge.Remove(charge);
                    }
                    aoc.AllowanceChargeType changedCharge = new aoc.AllowanceChargeType
                    {
                         ID = new UBL21.cbc.IDType {  Value = ecf.EcfHelper.UUID } ,
                         ChargeIndicator = new UBL21.cbc.ChargeIndicatorType { Value = true } ,
                         AllowanceChargeReasonCode = new UBL21.cbc.AllowanceChargeReasonCodeType {  Value  = "AFRJ" } ,
                         AllowanceChargeReason = new List<UBL21.cbc.AllowanceChargeReasonType> { new UBL21.cbc.AllowanceChargeReasonType { Value = "AFFIDAVIT OF RENEWAL OF JUDGMENT" }  }   ,
                         MultiplierFactorNumeric = new UBL21.cbc.MultiplierFactorNumericType {  Value = 1} ,
                         PrepaidIndicator = new UBL21.cbc.PrepaidIndicatorType {  Value = false} ,
                         SequenceNumeric = new UBL21.cbc.SequenceNumericType {  Value = paymentMessage.AllowanceCharge.Count + 1  } ,
                         Amount = new UBL21.cbc.AmountType {  Value = 27.00M},
                         BaseAmount  = new UBL21.cbc.BaseAmountType {  Value = 27.00M} ,
                         AllowanceChargeCategoryCode = new nc.TextType {  Value = amc.PolicyConstants.ALLOWANCE_CHARGE_CATEGORY_CODE_FILING_FEE }
                    };

                    paymentMessage.AllowanceCharge.Add(changedCharge);
                }


                ndcRequest = new wmp.NotifyDocketingCompleteRequest
                    (
                         new amc.NotifyDocketingCompleteRequestType
                         {
                             RecordDocketingCallbackMessage = recordDocketingCallBackMessages,
                             PaymentMessage = paymentMessage,
                         }
                    );
                }

            return ndcRequest;
        } 


        private List<nc.EntityType> DocumentReviwer(out string reviewerId)
        {
            reviewerId = ecf.EcfHelper.UUID;
            ecf.PersonType reviewer = new ecf.PersonType
                            (
                                id: reviewerId,
                                prefix: string.Empty,
                                givenName: "Summer",
                                middleName: string.Empty,
                                surName: "Roberts",
                                suffix: string.Empty,
                                eportalUserId: string.Empty,
                                contactEntity: null,
                                address1: "2500 Syracuse St",
                                address2: string.Empty,
                                city: "Tampa",
                                state: "FL",
                                zipCode: "81444",
                                phoneNumber: "6041451441",
                                extension: string.Empty,
                                emailAddress: "sroberts@geico.com",
                                countryCode: "US"
                            );
            reviewer.PersonOtherIdentification = new List<nc.IdentificationType>
                    {
                        new nc.IdentificationType("1415_AJACS" , amc.PolicyConstants.REVIEWREFMUSERID)
                    };
            return new List<nc.EntityType> { new nc.EntityType(reviewer) };
        }

        private string GetFileNameToDeserialize()
        {
            OpenFileDialog fileDialog = null;
            string selectedFileName = string.Empty;
            try
            {
                // Prompt for BulkReviewFilingRequest XML File
                fileDialog = new OpenFileDialog();
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                fileDialog.Title = "Select XML File to deserialize";
                DialogResult dr = fileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    selectedFileName = fileDialog.FileName;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Select File");
            }
            finally
            {
                if (fileDialog != null)
                {
                    fileDialog.Dispose();
                }

            }
            return selectedFileName;
        }
        private wmp.RecordFilingRequest GetRfrFromRvfr(string documentStatus ,  string statusDescription , out List<nc.EntityType> documentReviwer , out List<nc.EntityType> documentReviwerAsReference)
        {
            wmp.RecordFilingRequest rfr = null;
            string rvfrFile = this.GetFileNameToDeserialize();
            documentReviwer = null;
            documentReviwerAsReference = null;
            if (!string.IsNullOrWhiteSpace(rvfrFile))
            {
                wmp.ReviewFilingRequest rvfrRequest = null;
                using (var fs = new FileStream(rvfrFile, FileMode.Open, FileAccess.Read))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(wmp.ReviewFilingRequest));
                    object rvfrObject = serializer.Deserialize(fs);
                    rvfrRequest = rvfrObject != null && rvfrObject is wmp.ReviewFilingRequest ? rvfrObject as wmp.ReviewFilingRequest : null;
                }
                if (rvfrRequest != null)
                {
                    aoc.CoreFilingMessageType filingMessage = rvfrRequest.CoreFilingMessage as aoc.CoreFilingMessageType;
                    aoc.PaymentMessageType paymentMessage = rvfrRequest.PaymentMessage;

                    List<aoc.RecordDocketingMessageType> docketingMessages = new List<aoc.RecordDocketingMessageType>();
                    DateTime rfrPostDate = DateTime.Now;
                    string documentReviwerId = string.Empty;
                    documentReviwer = this.DocumentReviwer(out documentReviwerId);
                    if (!string.IsNullOrWhiteSpace(documentReviwerId))
                    {
                        documentReviwerAsReference = new List<nc.EntityType>
                                    {
                                        new nc.EntityType
                                        {
                                             EntityRepresentation = new ReferenceType{ Ref = documentReviwerId} ,
                                             EntityRepresentationType = nc.EntityRepresentationTpes.EntityPersonReference
                                        }
                                    };
                    }
                 

                    if (filingMessage != null && filingMessage.FilingLeadDocument != null && filingMessage.FilingLeadDocument.Count > 0)
                    {
                        int documentNumber = 0;
                        foreach (var ld in filingMessage.FilingLeadDocument)
                        {
                            string reviewedDocumentId = ecf.EcfHelper.UUID;
                            documentNumber++;
                            nc.StatusType ncDocumentStatus = new nc.StatusType
                            {
                              StatusText = new List<nc.TextType>{new nc.TextType(documentStatus)},
                              StatusDate = new List<nc.DateType>{ new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0,0,15)))} ,
                              StatusDescriptionText = new List<nc.TextType>{ new nc.TextType(statusDescription) }

                            };

                            if (documentStatus.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase))
                            {
                                if (documentNumber % 2 == 1)
                                {
                                    ncDocumentStatus = new nc.StatusType
                                    {
                                        StatusText = new List<nc.TextType> { new nc.TextType(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED) },
                                        StatusDate = new List<nc.DateType> { new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0, 0, 15))) },
                                        StatusDescriptionText = new List<nc.TextType> { new nc.TextType(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED) }
                                    };
                                }
                                else
                                {
                                    ncDocumentStatus = new nc.StatusType
                                    {
                                        StatusText = new List<nc.TextType> { new nc.TextType("REJECTED") },
                                        StatusDate = new List<nc.DateType> { new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0, 0, 15))) },
                                        StatusDescriptionText = new List<nc.TextType> { new nc.TextType(statusDescription) }
                                    };
                                }
                            }

                            aoc.RecordDocketingMessageType docketingMessage = new aoc.RecordDocketingMessageType
                            {
                                DocumentIdentification = new List<nc.IdentificationType>
                                {
                                    new nc.IdentificationType(ecf.EcfHelper.UUID , amc.PolicyConstants.REVIEWED_SUBMISSION_ID) 
                                },
                                DocumentPostDate = new List<nc.DateType> { new nc.DateType(rfrPostDate) },
                                DocumentStatus = new List<nc.StatusType>
                                {
                                    new nc.StatusType
                                    {
                                         StatusText = new List<nc.TextType>{new nc.TextType(amc.PolicyConstants.RDM_SUBMISSION_STATUS_SUBMISSION_FULLY_REVIEWED)},
                                         StatusDate = new List<nc.DateType>{ new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0,0,15)))}
                                    } 
                                },
                                DocumentSubmitter = documentReviwer,
                                SendingMDELocationID = new nc.IdentificationType("http://az.gov/FRMDE:xxxx"),
                                SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesProfile-2.0",
                                // CaseCourt = civilCase != null && civilCase.CaseAugmentation != null && civilCase.CaseAugmentation.CaseCourt.Count > 0 ? civilCase.CaseAugmentation.CaseCourt[0] : null,
                                ReviewedLeadDocument = new aoc.ReviewedDocumentType
                                {
                                    Id = reviewedDocumentId,
                                    Ref = ld.Id,
                                    DocumentIdentification = new List<nc.IdentificationType> { new nc.IdentificationType(reviewedDocumentId, amc.PolicyConstants.DOCUMENT_ID) },
                                    DocumentStatus = new List<nc.StatusType>
                                    {
                                        ncDocumentStatus
                                    },
                                    DocumentSubmitter = new List<nc.EntityType>
                                    {
                                        new nc.EntityType
                                        {
                                             EntityRepresentation = new ReferenceType{ Ref = documentReviwerId} ,
                                             EntityRepresentationType = nc.EntityRepresentationTpes.EntityPersonReference
                                        }
                                    },
                                    DocumentMetadata = new aoc.DocumentMetadataType
                                    {
                                        FilingAttorneyID = new aoc.FilingAttorneyIDType(),
                                        FilingPartyID = new List<nc.IdentificationType> { new aoc.FilingPartyIdType() },
                                        RegisterActionDescriptionText = new nc.TextType(string.Empty),
                                        RedactionRequiredIndicator = new niemxsd.Boolean(false)
                                    },
                                    DocumentRendition = new List<ecf.DocumentRenditionType>
                                    {
                                        new ecf.DocumentRenditionType
                                        {
                                             DocumentRenditionMetadata = new ecf.DocumentRenditionMetadataType
                                             {
                                                  DocumentAttachment = new List<ecf.DocumentAttachmentType>
                                                  {
                                                      new ecf.DocumentAttachmentType{}
                                                  }
                                             }
                                        }
                                    }

                                },
                                FilingReviewCommentsText = new nc.TextType(string.Empty)
                            };
                            docketingMessages.Add(docketingMessage);

                        }
                    }
                    amc.RecordFilingRequestType aocrfr = new amc.RecordFilingRequestType
                    {
                        CoreFilingMessage = filingMessage,
                        PaymentMessage = paymentMessage,
                        RecordDocketingMessage = docketingMessages
                    };
                    rfr = new wmp.RecordFilingRequest(aocrfr);
                }
                else
                {
                    MessageBox.Show("Rvfr Request is null");
                }

            }
            return rfr;
        }

        private void Save(wmp.NotifyDocketingCompleteResponse response)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (response != null)
                {

                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.CheckFileExists = false;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog.Title = "Select a file to save  to ";
                    DialogResult dr = saveFileDialog.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew, FileAccess.Write))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(wmp.NotifyDocketingCompleteResponse));
                            serializer.Serialize(fs, response);
                            fs.Flush();
                            fs.Close();
                        }
                        MessageBox.Show(
                            string.Format("Saved NDC Response to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Response is null !!!!"), "Save");
                }
            }
            finally
            {
                if (saveFileDialog != null)
                {
                    saveFileDialog.Dispose();
                    saveFileDialog = null;
                }

            }
        }

        private void btnSubmitAllRejected_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED, 
                        documentStatusDescription: "DO NOT HAVE TO GIVE YOU A REASON"  ,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED ,
                        filingStatusDescription: "DO NOT HAVE TO GIVE YOU A REASON" ,
                        netZeroSubmission:false ,
                        overPaymentAmount:0.00M ,
                        changeDocumentType : false
                    );
                if (ndcRequest != null)
                {
                    _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                        (
                            "FilingReviewMDEService",
                            _configFile
                        );

                    wmp.NotifyDocketingCompleteResponse response = _serviceChannel.NotifyDocketingComplete(ndcRequest);
                    Save(response);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

            }

        }

        

        

        private void btnValidateAOCCase_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a NDC (NotifyDocketingCompleteRequest) Validate ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    XmlSchemaSet schemas = new XmlSchemaSet();
                    
                    schemas.Add
                        (
                            null,
                            @"C:\AZBuildFiles54\WebServer\ECF4.01\xsd\exchange\NotifyDocketingComplete-MessageExchange.xsd"
                        );

                    XDocument doc = XDocument.Load(opeFileDialog.FileName);
                    bool errors = false;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    doc.Validate(schemas, (o, validationError) =>
                    {
                        sb.Append(string.Format("{0}\r\n", validationError.Message ));
                        errors = true;
                    }, true);
                    if (errors)
                    {
                        MessageBox.Show(sb.ToString() , string.Format("document {0} {1}", opeFileDialog.FileName, "did not validate" ));
                    }
                    else
                    {
                        MessageBox.Show(string.Format("document {0} {1}", opeFileDialog.FileName, "validated"));
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (opeFileDialog != null)
                {
                    opeFileDialog.Dispose();
                }
            }

        }

        

        private void btnGetDocument_Click(object sender, EventArgs e)
        {
            azs.ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<azs.ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        _configFile
                    );

                wmp.GetDocumentRequest request = new wmp.GetDocumentRequest
                (
                      new amc.GetDocumentRequestType { DocumentQueryMessage = this.SampleDocumentQuery }
                );
                wmp.GetDocumentResponse response = _serviceChannel.GetDocument(request);
                
                Save(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

            }

        }

        

        

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBoxCaseNumber.Focus();
        }

        private void buttonNetZero_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED ,
                        netZeroSubmission:true ,
                        overPaymentAmount:0.00M ,
                        changeDocumentType:false
                    );
                if (ndcRequest != null)
                {
                    _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                        (
                            "FilingReviewMDEService",
                            _configFile
                        );

                    wmp.NotifyDocketingCompleteResponse response = _serviceChannel.NotifyDocketingComplete(ndcRequest);
                    Save(response);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }


            }
        }

        private void btngetCasesFromFile_Click(object sender, EventArgs e)
        {
            string errorFileName = string.Empty;
            StreamWriter errorFileWriter = null;
            azs.ICourtRecordMDE _serviceChannel = null;

            try
            {
                errorFileName = ConfigurationManager.AppSettings["resultsFile"];

                if (string.IsNullOrEmpty(errorFileName))
                {
                    errorFileName = @".\GetCaseResults.log";
                }
                if (File.Exists(errorFileName)) File.Delete(errorFileName);
                errorFileWriter = new StreamWriter(errorFileName);
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<azs.ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        _configFile
                    );
                string caseNumbersFile = ConfigurationManager.AppSettings["caseNumbersFile"];
                if (File.Exists(caseNumbersFile))
                {
                    int numberOfCases = 0;
                    int numberFound = 0;
                    int numberErrors = 0;
                    using (StreamReader sr = new StreamReader(caseNumbersFile))
                    {
                        string caseNumber;
                        while ((caseNumber = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(caseNumber))
                            {
                                caseNumber = caseNumber.Trim().Replace("\t", "");
                            }
                            if (!string.IsNullOrWhiteSpace(caseNumber))
                            {
                                
                                this.tbStatus.Text = string.Format("Fetching Information for case {0}", caseNumber);
                                Application.DoEvents();
                                numberOfCases++;
                                try
                                {
                                    wmp.GetCaseRequest request = new wmp.GetCaseRequest
                                    (
                                          new amc.GetCaseRequestType { CaseQueryMessage = this.SampleCaseQuery }
                                    );
                                    request.CaseQueryMessage.CaseTrackingID = new niemxsd.String(caseNumber);
                                    wmp.GetCaseResponse response = _serviceChannel.GetCase(request);
                                    string responseFileName = @".\" + caseNumber + ".xml";
                                    if (response != null && response.CaseResponseMessage != null && response.CaseResponseMessage.Error != null && response.CaseResponseMessage.Error.Count > 0)
                                    {
                                        string errorCode = response.CaseResponseMessage.Error[0].ErrorCode != null && !string.IsNullOrWhiteSpace(response.CaseResponseMessage.Error[0].ErrorCode.Value) ? response.CaseResponseMessage.Error[0].ErrorCode.Value : "NA";
                                        string errorText = response.CaseResponseMessage.Error[0].ErrorText != null && !string.IsNullOrWhiteSpace(response.CaseResponseMessage.Error[0].ErrorText.Value) ? response.CaseResponseMessage.Error[0].ErrorText.Value : "NA";
                                        if (!string.IsNullOrWhiteSpace(errorCode) && errorCode.Equals("0"))
                                        {
                                            numberFound++;
                                        }
                                        else
                                        {
                                            numberErrors++;
                                        }
                                        errorFileWriter.WriteLine(string.Format("{0} {1} {2}", caseNumber, errorCode,errorText ));
                                        errorFileWriter.Flush();
                                    }
                                    SaveCompleteGetCaseResponse(response , responseFileName);
                                }
                                catch (Exception gcex)
                                {
                                    errorFileWriter.WriteLine(string.Format("Case # {0} {1}", caseNumber, gcex.ToString())) ;
                                }
                            }
                        }
                        this.tbStatus.Text = string.Format("Done. # Cases {0} # Found {1} # Errors {2}" , numberOfCases , numberFound , numberErrors);
                        MessageBox.Show(string.Format("Done. # Cases {0} # Found {1} # Errors {2}", numberOfCases, numberFound, numberErrors) , "Done");
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("FIle {0} not found", caseNumbersFile, "Exception")) ;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception");

            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

                if (errorFileWriter != null)
                {
                    errorFileWriter.Flush();
                    errorFileWriter.Close();
                    errorFileWriter.Dispose();
                    errorFileWriter = null;
                }

            }
        }

        private void buttonOverPayment_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                decimal overPaymentAmount = 0.00M;
                if (!string.IsNullOrWhiteSpace(this.tbOverPaymentAmount.Text))
                {
                    decimal.TryParse(this.tbOverPaymentAmount.Text, out overPaymentAmount);
                }
                if (overPaymentAmount > 0)
                {
                    wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC
                        (
                            documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            netZeroSubmission: false,
                            overPaymentAmount: overPaymentAmount ,
                            changeDocumentType : false 
                        );
                    if (ndcRequest != null)
                    {
                        _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                            (
                                "FilingReviewMDEService",
                                _configFile
                            );

                        wmp.NotifyDocketingCompleteResponse response = _serviceChannel.NotifyDocketingComplete(ndcRequest);
                        Save(response);

                    }
                }
                else
                {
                    MessageBox.Show("Over payment amount is required", "error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }


            }

        }

        private void buttonChangeDocumentType_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                decimal overPaymentAmount = 0.00M;
                    wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC
                        (
                            documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            netZeroSubmission: false,
                            overPaymentAmount: overPaymentAmount ,
                            changeDocumentType:true
                        );
                    if (ndcRequest != null)
                    {
                        _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                            (
                                "FilingReviewMDEService",
                                _configFile
                            );

                        wmp.NotifyDocketingCompleteResponse response = _serviceChannel.NotifyDocketingComplete(ndcRequest);
                        Save(response);

                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }


            }

        }
    }
}
