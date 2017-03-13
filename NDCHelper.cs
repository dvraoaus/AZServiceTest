﻿using Niem.Structures.v20;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using amc = Arizona.Courts.ExChanges.v20;
using aoc = Arizona.Courts.Extensions.v20;
using ecf = Oasis.LegalXml.CourtFiling.v40.Ecf;
using nc = Niem.NiemCore.v20;
using niemxsd = Niem.Proxy.xsd.v20;
using UBL21 = Oassis.UBL.v21;
using wmp = Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;
using VistaSG.Requests.DataContracts.Types;
namespace AZServiceTest
{
    public  class NDCHelper
    {
        private const string PARTIAL_ACCEPTANCE = "PARTIAL";
        private Random _randomGenerator = new Random();
        public NDCHelper()
        {
        }

        public wmp.NotifyDocketingCompleteRequest GetNDC(string documentStatusCode, string documentStatusDescription, string filingStatusCode, string filingStatusDescription, bool netZeroSubmission, decimal overPaymentAmount, bool changeDocumentType , string ajacsCaseNumber)
        {
            wmp.NotifyDocketingCompleteRequest ndcRequest = null;

            List<nc.EntityType> documentReviwer = null;
            List<nc.EntityType> documentReviwerAsReference = null;
            wmp.RecordFilingRequest rfr = GetRfrFromRvfr(documentStatusCode, documentStatusDescription, documentReviwer: out documentReviwer, documentReviwerAsReference: out documentReviwerAsReference);
            if (rfr != null)
            {
                aoc.CoreFilingMessageType filingMessage = rfr.AZRecordFilingRequest.CoreFilingMessage;
                aoc.PaymentMessageType paymentMessage = rfr.AZRecordFilingRequest.PaymentMessage;
                List<aoc.RecordDocketingMessageType> docketingMessages = rfr.AZRecordFilingRequest.RecordDocketingMessage;
                List<aoc.RecordDocketingCallbackMessageType> recordDocketingCallBackMessages = new List<aoc.RecordDocketingCallbackMessageType>();

                nc.CaseType callBackCase = new nc.CaseType();
                aoc.CivilCaseType aocCivilCase = filingMessage.Case != null ? filingMessage.Case as aoc.CivilCaseType : null;
                
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
                    // Match AJACS
                    // callBackCase.CaseCategoryText = aocCivilCase.CaseCategoryText; 
                }
                // Generate Call Back Message
                aoc.RecordDocketingCallbackMessageType recordDocketingCallBack = null;
                int documentNumber = 0;
                foreach (var dm in docketingMessages)
                {
                    documentNumber++;
                    ecf.FilingStatusType filingStatus = new ecf.FilingStatusType
                    {
                        FilingStatusCode = filingStatusCode.ToUpper(),
                        StatusDate = new List<nc.DateType> { new nc.DateType(DateTime.Now) },
                        StatusText = new List<nc.TextType> { new nc.TextType("0") }
                    };

                    if (filingStatusCode.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase))
                    {
                        if (documentNumber % 2 == 1)
                        {
                            filingStatus =  new ecf.FilingStatusType
                            {
                                FilingStatusCode = amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED.ToUpper(),
                                StatusDate = new List<nc.DateType> { new nc.DateType(DateTime.Now) },
                                StatusText = new List<nc.TextType> { new nc.TextType("0") }
                            };
                        }
                        else
                        {
                            filingStatus = new ecf.FilingStatusType
                            {
                                FilingStatusCode = amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED.ToUpper(),
                                StatusDate = new List<nc.DateType> { new nc.DateType(DateTime.Now) },
                                StatusText = new List<nc.TextType> { new nc.TextType("0") }
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
                        DocumentSubmitter = filingMessage.DocumentSubmitter, // Match AJACS
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
                if (netZeroSubmission && paymentMessage != null && paymentMessage.AllowanceCharge != null && paymentMessage.AllowanceCharge.Count > 0)
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

                if (overPaymentAmount > 0.00M && paymentMessage != null && paymentMessage.AllowanceCharge != null && paymentMessage.AllowanceCharge.Count > 0)
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
                        ID = new UBL21.cbc.IDType { Value = ecf.EcfHelper.UUID },
                        ChargeIndicator = new UBL21.cbc.ChargeIndicatorType { Value = true },
                        AllowanceChargeReasonCode = new UBL21.cbc.AllowanceChargeReasonCodeType { Value = "AFRJ" },
                        AllowanceChargeReason = new List<UBL21.cbc.AllowanceChargeReasonType> { new UBL21.cbc.AllowanceChargeReasonType { Value = "AFFIDAVIT OF RENEWAL OF JUDGMENT" } },
                        MultiplierFactorNumeric = new UBL21.cbc.MultiplierFactorNumericType { Value = 1 },
                        PrepaidIndicator = new UBL21.cbc.PrepaidIndicatorType { Value = false },
                        SequenceNumeric = new UBL21.cbc.SequenceNumericType { Value = paymentMessage.AllowanceCharge.Count + 1 },
                        Amount = new UBL21.cbc.AmountType { Value = 27.00M },
                        BaseAmount = new UBL21.cbc.BaseAmountType { Value = 27.00M },
                        AllowanceChargeCategoryCode = new nc.TextType { Value = amc.PolicyConstants.ALLOWANCE_CHARGE_CATEGORY_CODE_FILING_FEE }
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


        private  wmp.RecordFilingRequest GetRfrFromRvfr(string documentStatus, string statusDescription, out List<nc.EntityType> documentReviwer, out List<nc.EntityType> documentReviwerAsReference)
        {
            wmp.RecordFilingRequest rfr = null;
            string rvfrFile = GetFileNameToDeserialize();
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
                    documentReviwer = DocumentReviwer(out documentReviwerId);
                    if (!string.IsNullOrWhiteSpace(documentReviwerId))
                    {
                        documentReviwerAsReference = new List<nc.EntityType>
                                    {
                                        new nc.EntityType
                                        {
                                             EntityRepresentation = new ReferenceType{ Ref = documentReviwerId} ,EntityRepresentationType = nc.EntityRepresentationTpes.EntityPersonReference
                                             
                                        }
                                    };
                    }


                    if (filingMessage != null && filingMessage.FilingLeadDocument != null && filingMessage.FilingLeadDocument.Count > 0)
                    {
                        int documentNumber = 0;
                        foreach (var ld in filingMessage.FilingLeadDocument)
                        {
                            
                            documentNumber++;

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
                                ReviewedLeadDocument = ToReviewedDocument(ld as aoc.DocumentType, documentNumber, documentStatus , rfrPostDate, statusDescription),
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

        private aoc.ReviewedDocumentType ToReviewedDocument(aoc.DocumentType leadDocument , int documentNumber , string documentStatus , DateTime rfrPostDate , string statusDescription)
        {
            aoc.ReviewedDocumentType reviewedDocument = null;
            if (leadDocument != null)
            {
                string reviewedDocumentId = ecf.EcfHelper.UUID;
                bool issuanceDocument = leadDocument.DocumentTitleText != null &&
                                        !string.IsNullOrWhiteSpace(leadDocument.DocumentTitleText.Value) &&
                                        leadDocument.DocumentTitleText.Value.EndsWith("System Generated", StringComparison.OrdinalIgnoreCase) &&
                                        ( leadDocument.DocumentTitleText.Value.StartsWith("Summons", StringComparison.OrdinalIgnoreCase) ||
                                          leadDocument.DocumentTitleText.Value.StartsWith("Subpoena", StringComparison.OrdinalIgnoreCase)
                                        );

                nc.StatusType ncDocumentStatus = new nc.StatusType
                {
                    StatusText = new List<nc.TextType> { new nc.TextType(documentStatus.ToUpper()) },
                    StatusDate = new List<nc.DateType> { new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0, 0, 15))) },
                    StatusDescriptionText = new List<nc.TextType> { new nc.TextType(statusDescription.ToUpper()) }

                };

                if (documentStatus.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase))
                {
                    if (documentNumber % 2 == 1)
                    {
                        ncDocumentStatus = new nc.StatusType
                        {
                            StatusText = new List<nc.TextType> { new nc.TextType(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED.ToUpper()) },
                            StatusDate = new List<nc.DateType> { new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0, 0, 15))) },
                            StatusDescriptionText = new List<nc.TextType> { new nc.TextType(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED.ToUpper()) }
                        };
                    }
                    else
                    {
                        ncDocumentStatus = new nc.StatusType
                        {
                            StatusText = new List<nc.TextType> { new nc.TextType("REJECTED".ToUpper()) },
                            StatusDate = new List<nc.DateType> { new nc.DateType(rfrPostDate.Subtract(new TimeSpan(0, 0, 15))) },
                            StatusDescriptionText = new List<nc.TextType> { new nc.TextType(statusDescription.ToUpper()) }
                        };
                    }
                }

                reviewedDocument = new aoc.ReviewedDocumentType
                {
                    Id = reviewedDocumentId,
                    Ref = leadDocument.Id,
                    DocumentCategoryText =  leadDocument.DocumentCategoryID != null &&  !string.IsNullOrWhiteSpace( leadDocument.DocumentCategoryID.Value) ? new List<nc.TextType> { new nc.TextType(leadDocument.DocumentCategoryID.Value) } : new List<nc.TextType> { } ,
                    DocumentDescriptionText = leadDocument.DocumentDescriptionText ,
                    DocumentFiledDate = new List<nc.DateType> { new nc.DateType(rfrPostDate)} ,
                    DocumentIdentification = new List<nc.IdentificationType> { new nc.IdentificationType(string.Empty, amc.PolicyConstants.DOCUMENT_ID) },
                    DocumentTitleText = leadDocument.DocumentTitleText ,
                    DocumentStatus = new List<nc.StatusType> { ncDocumentStatus },
                    DocumentSubmitter = new List<nc.EntityType>
                                    {
                                        new nc.EntityType
                                        {
                                            EntityRepresentation = new ReferenceType{ } ,
                                            EntityRepresentationType = nc.EntityRepresentationTpes.EntityPersonReference
                                        }
                                    },
                    DocumentMetadata = new aoc.DocumentMetadataType
                    {
                        FilingAttorneyID = leadDocument.DocumentMetadata != null && leadDocument.DocumentMetadata.FilingAttorneyID != null  ? leadDocument.DocumentMetadata.FilingAttorneyID : new aoc.FilingAttorneyIDType(),
                        FilingPartyID = leadDocument.DocumentMetadata != null && leadDocument.DocumentMetadata.FilingPartyID  != null ? leadDocument.DocumentMetadata.FilingPartyID : new List<nc.IdentificationType> { new aoc.FilingPartyIdType() },
                        RegisterActionDescriptionText = new nc.TextType(string.Empty),
                        RedactionRequiredIndicator = new niemxsd.Boolean(false) ,
                        ParentDocumentReference = new ReferenceType {  Id = leadDocument.Id , Ref = leadDocument.Id , LinkMetadata = string.Empty}
                    },
                    DocumentRendition = issuanceDocument ? leadDocument.DocumentRendition : null,
                    DocumentCategoryID = leadDocument.DocumentCategoryID ,
                    DocumentCategoryName = leadDocument.DocumentCategoryName ,
                    DocumentDocketID = new  nc.IdentificationType(_randomGenerator.Next(100,99999999).ToString())

                };

            }
            return reviewedDocument;
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
                DialogResult dr = fileDialog.ShowDialog();
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

        public amc.NotifyDocketingCompleteRequestType TransformNDC(amc.NotifyDocketingCompleteRequestType originalNDC , RequestType request)
        {
            amc.NotifyDocketingCompleteRequestType transformedNDC = new amc.NotifyDocketingCompleteRequestType();
            if (originalNDC != null )
            {
                transformedNDC = originalNDC;
            }
            if (transformedNDC != null)
            {
                if (transformedNDC.RecordDocketingCallbackMessage != null && transformedNDC.RecordDocketingCallbackMessage.Count > 0)
                {
                    int callBackNumber = 0;
                    string documentReviwerId = string.Empty;
                    List<nc.EntityType> documentReviwer = DocumentReviwer(request , out documentReviwerId  );
                    List<nc.EntityType> documentReviwerAsReference = null;
                    if (!string.IsNullOrWhiteSpace(documentReviwerId))
                    {
                        documentReviwerAsReference = new List<nc.EntityType>
                                    {
                                        new nc.EntityType
                                        {
                                             EntityRepresentation = new ReferenceType{ Ref = documentReviwerId} ,EntityRepresentationType = nc.EntityRepresentationTpes.EntityPersonReference

                                        }
                                    };
                    }

                    foreach (var cb in transformedNDC.RecordDocketingCallbackMessage)
                    {
                        callBackNumber++;
                        if (callBackNumber == 1)
                        {
                            cb.DocumentSubmitter = documentReviwer;
                        }
                        else
                        {
                            cb.DocumentSubmitter = documentReviwerAsReference;
                        }
                        if (cb.ReviewedLeadDocument != null && cb.ReviewedLeadDocument is aoc.ReviewedDocumentType)
                        {
                            cb.ReviewedLeadDocument = this.FixReviewedDocument(cb.ReviewedLeadDocument as aoc.ReviewedDocumentType , documentReviwerAsReference);
                        }
                        if (cb.ReviewedConnectedDocument != null && cb.ReviewedConnectedDocument.Count > 0 )
                        {
                            for(int i = 0 ; i < cb.ReviewedConnectedDocument.Count ; i++)
                            {
                                if (cb.ReviewedConnectedDocument[i] is aoc.ReviewedDocumentType)
                                {
                                    cb.ReviewedConnectedDocument[i] = this.FixReviewedDocument(cb.ReviewedConnectedDocument[i] as aoc.ReviewedDocumentType , documentReviwerAsReference) ;
                                }
                            }
                        }

                    }
                }
            }
            return transformedNDC;
        }

        private aoc.ReviewedDocumentType FixReviewedDocument(aoc.ReviewedDocumentType reviewedDocument , List<nc.EntityType> documentReviwerAsReference)
        {
            aoc.ReviewedDocumentType fixedDocument = new aoc.ReviewedDocumentType();
            string coreFilingDocumentId = string.Empty;
            if (reviewedDocument != null)
            {
                fixedDocument = reviewedDocument;
                if (!string.IsNullOrWhiteSpace(reviewedDocument.Ref)) coreFilingDocumentId = reviewedDocument.Ref;
            }
            if (fixedDocument != null && !string.IsNullOrWhiteSpace(coreFilingDocumentId))
            {
                fixedDocument.DocumentIdentification = new List<nc.IdentificationType> { new nc.IdentificationType(coreFilingDocumentId, amc.PolicyConstants.DOCUMENT_ID) };
                fixedDocument.Ref = null;
                fixedDocument.DocumentSubmitter = documentReviwerAsReference;
                if (fixedDocument.DocumentMetadata != null && fixedDocument.DocumentMetadata is aoc.DocumentMetadataType)
                {
                    aoc.DocumentMetadataType documentMetaData = fixedDocument.DocumentMetadata as aoc.DocumentMetadataType;
                    documentMetaData.ParentDocumentReference = null;
                    documentMetaData.FilingAttorneyID = new aoc.FilingAttorneyIDType();
                    documentMetaData.FilingPartyID = new List<nc.IdentificationType> { new aoc.FilingPartyIdType() };

                }
                if (fixedDocument.DocumentRendition == null)
                {
                    fixedDocument.DocumentRendition = new List<ecf.DocumentRenditionType>();
                }
                if (fixedDocument.DocumentRendition.Count == 0)
                {
                    aoc.DocumentRenditionType blankRendition = new aoc.DocumentRenditionType
                    {
                        DocumentSignature = new List<ecf.DocumentSignatureType>
                        {
                            new ecf.DocumentSignatureType
                            {
                                SignatureProfileID = new nc.TextType("urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:NullSignature-1.0") ,
                                Signature = new ecf.SignatureType
                                {
                                    Signatures = new W3.DigitalSignature.NullSignatureType()
                                }

                            }
                        },
                        DocumentRenditionMetadata = new ecf.DocumentRenditionMetadataType
                        {
                            DocumentAttachment = new List<ecf.DocumentAttachmentType> { new ecf.DocumentAttachmentType() } 
                        }
                   
                    };
                
                    fixedDocument.DocumentRendition.Add(blankRendition);
                }
            }
            return fixedDocument;
        }

        private List<nc.EntityType> DocumentReviwer(RequestType request , out string reviewerId)
        {
            reviewerId = ecf.EcfHelper.UUID;
            string surname = "Clerk";
            string givenName = request != null && request.Information != null ? string.Format("{0}", request.Information.SubmittedToOrganizationName) : "Superior Court";
            ecf.PersonType reviewer = new ecf.PersonType
                            (
                                id: reviewerId,
                                prefix: string.Empty,
                                givenName: givenName,
                                middleName: string.Empty,
                                surName: surname,
                                suffix: string.Empty,
                                eportalUserId: string.Empty,
                                contactEntity: null,
                                address1: string.Empty,
                                address2: string.Empty,
                                city:string.Empty,
                                state: string.Empty,
                                zipCode: string.Empty,
                                phoneNumber: string.Empty,
                                extension: string.Empty,
                                emailAddress: string.Empty,
                                countryCode: string.Empty
                            );
            return new List<nc.EntityType> { new nc.EntityType(reviewer) };
        }

    }

}