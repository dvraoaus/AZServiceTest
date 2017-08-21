using ecf31 = Oasis.LegalXml.v31.CourtFiling;
using System.Windows.Forms;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using jxdm = Gjxdm;

namespace AZServiceTest
{

    public class MCNFRCHelper
    {

        public string GetNFRC( string filingStatusCode, string rejectionReason)
        {
            string nfrcText = string.Empty;
            string cfmFile = GetFileNameToDeserialize();
            if (!string.IsNullOrWhiteSpace(cfmFile))
            {
                string submissionNumber = string.Empty;
                ecf31.CoreFilingMessageType coreFilingMessage = null;
                XmlSerializer coreFilingMessageSerializer = new XmlSerializer(typeof(ecf31.CoreFilingMessageType));
                
                using (var  fs = new FileStream(cfmFile ,FileMode.Open, FileAccess.Read ))
                {
                    coreFilingMessage = coreFilingMessageSerializer.Deserialize(fs) as ecf31.CoreFilingMessageType;
                }
                if (coreFilingMessage != null && coreFilingMessage.FilingID != null && coreFilingMessage.FilingID.ID != null && !string.IsNullOrWhiteSpace(coreFilingMessage.FilingID.ID.Value))
                {
                    submissionNumber = coreFilingMessage.FilingID.ID.Value.Replace("130-" , string.Empty);
                    ecf31.FilingStatusCodeType filingStatus = null;
                    string reviewedDocumentId = string.Empty;

                    if (filingStatusCode.Equals("accepted" , StringComparison.OrdinalIgnoreCase))
                    {
                        filingStatus = new ecf31.FilingStatusCodeType { Value = ecf31.FilingStatusCodeSimpleType.Accepted };
                        reviewedDocumentId = "002020113116116115106120121125117124119107116105098100099103096098097125126";
                    }
                    else if (filingStatusCode.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                    {
                        filingStatus = new ecf31.FilingStatusCodeType { Value = ecf31.FilingStatusCodeSimpleType.Rejected };
                    }
                    else if (filingStatusCode.Equals("pending judge review", StringComparison.OrdinalIgnoreCase))
                    {
                        filingStatus = new ecf31.FilingStatusCodeType { Value = ecf31.FilingStatusCodeSimpleType.PendingJudgeReview };
                        reviewedDocumentId = "002020113116116112106120112126123127124107116105098100099099102103097125126";
                    }

                    ecf31.ReviewFilingCallbackMessageType reviewFilingCallBackMessage = new ecf31.ReviewFilingCallbackMessageType
                    {
                        ActivityCourt = new Gjxdm.CourtType { OrganizationID = new Gjxdm.IDType(string.Empty), CourtName = new Gjxdm.TextType(string.Empty) },
                        SubmissionSubmitterPerson = new Gjxdm.PersonType
                        {
                            PersonAssignedIDDetails = new Gjxdm.PersonAssignedIDDetailsType
                            {
                                PersonOtherID = new System.Collections.Generic.List<Gjxdm.PersonOtherIDType>
                            { new Gjxdm.PersonOtherIDType { ID = new Gjxdm.TextType(string.Empty) , IDTypeText = new Gjxdm.TextType(string.Empty) }
                            }
                            }
                        },
                        SubmissionSubmittedDate = new Gjxdm.Date { ValueAsText = string.Empty },
                        SubmissionSubmittedTime = new Gjxdm.Time { ValueAsText = string.Empty },
                        SubmissionReceivedDate = new jxdm.Date { ValueAsText = string.Empty } ,
                        SubmissionReceivedTime = new Gjxdm.Time {  ValueAsText = string.Empty} ,
                        SendingMDELocationID = new Gjxdm.IDType(string.Empty),
                        SendingMDEProfileCode = new ecf31.MessageProfileCodeType {  },
                        FilingID = new jxdm.IDType(submissionNumber),
                        FilingStatusCode = filingStatus  ,
                        FilingOfficialDate = new jxdm.Date {  ValueAsText = string.Empty } ,
                        FilingStatusReasonDescription= new jxdm.TextType {  Value = rejectionReason} ,
                        ReviewedDocument = this.GetReviewedDocument(reviewedDocumentId),
                        DocketedCase = this.DocketedCase,
                        FeesCalculationAmount = new Gjxdm.AmountType {  Value = 0.00M} 
                      };
                    nfrcText = this.SerializeAsString(reviewFilingCallBackMessage);
                }
                

            }
            return nfrcText;
        }

        private ecf31.ReviewedDocumentType GetReviewedDocument(string reviewedDocumentId)
        {
            return new ecf31.ReviewedDocumentType
            {
                DocumentDescriptiveMetadata = new jxdm.DocumentDescriptiveMetadataType
                {
                    DocumentID = new Gjxdm.IDType {  ID = new Gjxdm.TextType {  Value = reviewedDocumentId} } ,
                    DocumentStatus = new Gjxdm.StatusType
                    {
                        StatusText = new jxdm.TextType { Value = string.Empty } ,
                        StatusDescriptionText = new jxdm.TextType { Value = string.Empty } ,
                        StatusDate = new jxdm.Date {  ValueAsText = string.Empty} ,
                        StatusTime   = new jxdm.Time {  ValueAsText = string.Empty}
                    }
                } ,
                ExtendedDocumentDescriptiveMetadata = new ecf31.ExtendedDocumentDescriptiveMetadataType
                {
                     RegisterActionDescriptionText = new jxdm.TextType {  Value = string.Empty } ,
                     FilingAttorneyID = new jxdm.IDType {  ID = new jxdm.TextType {  Value = string.Empty} } ,
                     FilingPartyID = new System.Collections.Generic.List<jxdm.IDType>
                     {
                         new jxdm.IDType {  ID = new jxdm.TextType {  Value = string.Empty} }
                     } ,
                     DocumentAttachment = new System.Collections.Generic.List<ecf31.DocumentAttachmentType>
                     {
                         new ecf31.DocumentAttachmentType
                         {
                              AttachmentID = new jxdm.IDType { ID = new jxdm.TextType { Value = string.Empty } } ,
                              AttachmentSequenceID = new jxdm.IDType { ID = new jxdm.TextType { Value = "1" } },
                         }  
                     } ,
                     ColorRelevantIndicator = new jxdm.Boolean {  Value = false} 
                } ,
                DocumentHashText = new jxdm.TextType {  Value = string.Empty } ,
                DocumentDocketID = new jxdm.IDType {  ID =  new jxdm.TextType { Value = string.Empty } } ,
                 SealDocumentIndicator = new jxdm.Boolean () , 
            };
        }

        private ecf31.DocketedCaseType DocketedCase
        {
            get
            {
                return new ecf31.DocketedCaseType
                {
                     ActivityCourt = new Gjxdm.CourtType { OrganizationID = new Gjxdm.IDType(string.Empty), CourtName = new Gjxdm.TextType(string.Empty) },
                     CaseTitleText = new jxdm.TextType {  Value = string.Empty} ,
                     CaseTrackingID = new jxdm.IDType {  ID = new jxdm.TextType {  Value = string.Empty} } ,
                     CaseLineageCase = new jxdm.CaseType
                     {
                         ActivityCourt = new Gjxdm.CourtType { OrganizationID = new Gjxdm.IDType(string.Empty), CourtName = new Gjxdm.TextType(string.Empty) },
                         CaseTitleText = new jxdm.TextType { Value = string.Empty },
                         CaseTrackingID = new jxdm.IDType { ID = new jxdm.TextType { Value = string.Empty } },
                     } ,
                     CaseParticipants = new jxdm.CaseParticipantsType
                     {
                          CaseInitiatingPartyPerson = new System.Collections.Generic.List<jxdm.PersonType>
                          {
                              this.EmptyPerson
                          } ,
                          CaseJudge = new System.Collections.Generic.List<jxdm.CaseOfficialType>
                          {
                              this.Judge
                          } ,
                          CaseRespondentPartyPerson = new System.Collections.Generic.List<jxdm.PersonType>
                          {
                              this.EmptyPerson
                          }
                     } ,
                     CaseStatus = new jxdm.StatusType
                     {
                         StatusText = new jxdm.TextType { Value = string.Empty },
                         StatusDescriptionText = new jxdm.TextType { Value = string.Empty },
                         StatusDate = new jxdm.Date { ValueAsText = string.Empty },
                         StatusTime = new jxdm.Time { ValueAsText = string.Empty }
                     },
                     CaseShortTitleText = new jxdm.TextType { Value = string.Empty }, 
                     CaseClassification = new ecf31.CaseClassificationType
                     {
                          CaseTypeCode = new ecf31.PolicyDefinedCodeTextType {  Value = string.Empty}
                     } ,
                     ExtendedCaseParticipants = new ecf31.ExtendedCaseParticipantsType
                     {

                     } ,
                     CaseLanguageCode = new jxdm.LanguageCodeType1 {   Value = jxdm.LanguageCodeType.Eng} ,
                     CaseAttorneyRole = new System.Collections.Generic.List<ecf31.CaseAttorneyRoleType>
                     {
                         new ecf31.CaseAttorneyRoleType {  } 
                     } ,
                     CaseRelatedCase = new System.Collections.Generic.List<ecf31.RelatedCaseType>
                     {
                         new ecf31.RelatedCaseType
                         {
                            ActivityCourt = new Gjxdm.CourtType { OrganizationID = new Gjxdm.IDType(string.Empty), CourtName = new Gjxdm.TextType(string.Empty) },
                            CaseTitleText = new jxdm.TextType { Value = string.Empty },
                            CaseTrackingID = new jxdm.IDType { ID = new jxdm.TextType { Value = string.Empty } },
                            ExtendedCaseParticipants = new ecf31.ExtendedCaseParticipantsType { } ,

                         }
                     } ,
                     CaseCourtEvent = new System.Collections.Generic.List<ecf31.CourtEventType1>
                     {
                         this.CourtEvent
                     }
                };
            }
        }

        private jxdm.PersonType EmptyPerson
        {
            get
            {
                return new jxdm.PersonType
                {
                     PersonName = new jxdm.PersonNameType
                     {
                           PersonPrefixName = new jxdm.TextType {  Value = string.Empty} ,
                           PersonGivenName =  new jxdm.PersonNameTextType {  Value = string.Empty} ,
                           PersonMiddleName = new jxdm.PersonNameTextType { Value = string.Empty },
                           PersonSurName = new jxdm.PersonNameTextType { Value = string.Empty }
                     } ,
                     PrimaryContactInformation = new jxdm.ContactInformationType
                     {
                         ContactTelephoneNumber = new jxdm.TelephoneNumberType {  TelephoneNumberFullID = new jxdm.String {  Value = string.Empty} , TelephoneNumberCommentText = new jxdm.TextType {  Value = string.Empty}  } ,
                         ContactEmailID = new jxdm.IDType { ID = new jxdm.TextType { Value = string.Empty } },
                         ContactMailingAddress = new jxdm.AddressType
                         {
                              LocationStreet = new System.Collections.Generic.List<jxdm.StreetType>
                              {
                                  new jxdm.StreetType { StreetFullText =  new jxdm.TextType { Value = string.Empty }   } 
                              } ,
                              LocationCityName = new jxdm.TextType {  Value = string.Empty } ,
                              LocationCountyName = new jxdm.TextType {  Value = string.Empty} ,
                              LocationStateCodeUSPostalService = new jxdm.USStateCodeType1 { } ,
                              LocationPostalCodeID = new jxdm.IDType {  ID = new jxdm.TextType { Value = string.Empty } }  ,
                              LocationCountryCodefips104 = new jxdm.CountryCodeType1 {  } ,
                         } ,
                     } ,
                     PersonBirthDate = new jxdm.Date {  ValueAsText = string.Empty} ,
                     PersonAssignedIDDetails = new jxdm.PersonAssignedIDDetailsType
                     {
                          PersonTaxID = new jxdm.IDType { ID = new jxdm.TextType {  Value = string.Empty} } ,
                          PersonOtherID = new System.Collections.Generic.List<jxdm.PersonOtherIDType>
                          {
                              new jxdm.PersonOtherIDType { ID = new jxdm.TextType { Value = string.Empty } ,  IDTypeText = new jxdm.TextType {  Value= string.Empty} } ,

                          }
                     }
                };
            }
            
        }

        private jxdm.CaseOfficialType Judge
        {
            get
            {
                return new jxdm.CaseOfficialType
                {
                    PersonName = new jxdm.PersonNameType
                    {
                        PersonFullName = new jxdm.PersonNameTextType { Value = string.Empty }
                    },
                    PersonAssignedIDDetails = new jxdm.PersonAssignedIDDetailsType
                    {
                        PersonOtherID = new System.Collections.Generic.List<jxdm.PersonOtherIDType>
                          {
                              new jxdm.PersonOtherIDType { ID = new jxdm.TextType { Value = string.Empty } ,  IDTypeText = new jxdm.TextType {  Value= string.Empty} } ,

                          }
                    }
                };
            }

        }

        private jxdm.PersonType CourtEventActor
        {
            get
            {
                return new jxdm.PersonType
                {
                    PersonAssignedIDDetails = new jxdm.PersonAssignedIDDetailsType
                    {
                        PersonOtherID = new System.Collections.Generic.List<jxdm.PersonOtherIDType>
                          {
                              new jxdm.PersonOtherIDType { ID = new jxdm.TextType { Value = string.Empty } ,  IDTypeText = new jxdm.TextType {  Value= string.Empty} } ,

                          }
                    }
                };
            }

        }

        private ecf31.CourtEventType1 CourtEvent
        {
            get
            {
                return new ecf31.CourtEventType1
                {
                      ActivityID = new jxdm.IDType {  ID = new jxdm.TextType {  Value = string.Empty} } ,
                      ActivityDescriptionText = new jxdm.TextType {  Value = string.Empty } ,
                      ActivityDate = new jxdm.Date {   ValueAsText = string.Empty } ,
                      ActivityTime = new jxdm.Time {  ValueAsText = string.Empty } ,
                      CourtEventJudge = new System.Collections.Generic.List<jxdm.JudicialOfficialType> { this.Judge } ,
                      CourtEventEnteredOnDocketDate = new jxdm.Date {  ValueAsText = string.Empty} ,
                      CourtEventTypeCode = new ecf31.PolicyDefinedCodeTextType {  Value = string.Empty } ,
                      CourtEventDocument = new ecf31.CourtEventDocumentType
                      {
                          DocumentDescriptiveMetadata = new jxdm.DocumentDescriptiveMetadataType
                          {
                              DocumentID = new Gjxdm.IDType { ID = new Gjxdm.TextType { Value = string.Empty } },
                              DocumentSequenceID = new jxdm.IDType {  ID = new jxdm.TextType {  Value = string.Empty } } ,
                              DocumentDescriptionText = new jxdm.TextType {  Value = string.Empty} ,
                              DocumentLanguageCodeiso6392b = new jxdm.LanguageCodeType1 {  } ,
                              DocumentApplicationName = new jxdm.ApplicationNameType {  Value = string.Empty}
                          },

                      },
                      CourtEventRelatedDocketEntry = new System.Collections.Generic.List<ecf31.CourtEventRelatedDocketEntryType>
                      {
                          new ecf31.CourtEventRelatedDocketEntryType { ChildCourtEventID = new jxdm.IDType { ID = new jxdm.TextType {  Value = string.Empty} } }
                      } ,
                      CourtEventActor = this.CourtEventActor ,
                      CourtEventOnBehalfOfActor = this.CourtEventActor,
                      CourtEventSchedule = new System.Collections.Generic.List<ecf31.ScheduleDayType>
                      {
                          new ecf31.ScheduleDayType
                          {
                              ScheduleDate =  new jxdm.Date { ValueAsText = string.Empty } ,
                              ScheduleDayStartTime = new jxdm.Time { ValueAsText = string.Empty } ,
                              ScheduleDayEndTime = new jxdm.Time { ValueAsText = string.Empty } ,
                              ScheduleActivityText = new jxdm.TextType { Value = string.Empty } ,
                              ScheduleLocationCode = new ecf31.PolicyDefinedCodeTextType { Value = string.Empty } ,
                              ScheduleLocationText = new jxdm.TextType { Value = string.Empty }

                          }
                      }

                };
            } 
        }

        private string SerializeAsString(ecf31.ReviewFilingCallbackMessageType reviewFilingCallBackMessage)
        {
            string nfrcText = string.Empty;
            if (reviewFilingCallBackMessage != null )
            {
                XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add("reviewed", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:ReviewedDocumentType-3.0");
                xmlNamespaces.Add("message", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:MessageTypes-3.0");
                xmlNamespaces.Add("j-xsd", "http://www.it.ojp.gov/jxdm/3.0.3/proxy/xsd/1.0");
                xmlNamespaces.Add("jxdm", "http://www.it.ojp.gov/jxdm/3.0.3");
                xmlNamespaces.Add("common", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:CommonTypes-3.0");
                xmlNamespaces.Add("filingstatus", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:FilingStatusCode-3.0");
                xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlNamespaces.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
                xmlNamespaces.Add("document", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:DocumentType-3.0");
                xmlNamespaces.Add("docketed", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:DocketedCaseType-3.0");
                xmlNamespaces.Add("case", "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:CaseType-3.0");
                XmlSerializer serializer = new XmlSerializer(typeof(ecf31.ReviewFilingCallbackMessageType));
                using (var stream = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };

                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.Serialize(writer, reviewFilingCallBackMessage, xmlNamespaces);
                        nfrcText = stream.ToString();
                    }
                }
            }
            return nfrcText;
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
                fileDialog.Title = "Select ECF 3.0 Core Filing Message File to deserialize";
                DialogResult dr = fileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    selectedFileName = fileDialog.FileName;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Select ECF 3.0 Core Filing Message File");
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
    }
}
