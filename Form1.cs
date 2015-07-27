using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using j = Niem.Domains.Jxdm.v40;
using nc = Niem.NiemCore.v20;
using UBL21 = Oassis.UBL.v21;
using niemxsd = Niem.Proxy.xsd.v20;
using wmp = Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;
using amc = Arizona.Courts.ExChanges.v20;
using aoc = Arizona.Courts.Extensions.v20;
using ecf = Oasis.LegalXml.CourtFiling.v40.Ecf;
using caseQuery = Oasis.LegalXml.CourtFiling.v40.CaseQuery;
using caseResponse = Oasis.LegalXml.CourtFiling.v40.CaseResponse;
using System.Configuration;
using core = Oasis.LegalXml.CourtFiling.v40.Core;
using Niem.Structures.v20;
namespace AZServiceTest
{
    public partial class Form1 : Form
    {
        private const string DEFAULT_CONFIG_FILE = @".\AZCRMDE.config";
        private string _configFile = string.Empty;
        private const string PARTIAL_ACCEPTANCE = "PARTIAL";

        public Form1()
        {
            InitializeComponent();
            _configFile = ConfigurationManager.AppSettings["ServiceConfigurationFile"];
            if (string.IsNullOrEmpty(_configFile))
            {
                _configFile = DEFAULT_CONFIG_FILE;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wmp.ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        _configFile
                    );

                wmp.GetDocumentRequest request = new wmp.GetDocumentRequest
                {
                    DocumentQueryMessage = new Oasis.LegalXml.CourtFiling.v40.DocumentQuery.DocumentQueryMessageType
                     {
                         CaseTrackingID = new Niem.Proxy.xsd.v20.String("CV 2015 000100"),
                         CaseDocketID = new Niem.Proxy.xsd.v20.String("133551"),
                         CaseCourt = this.CaseCourt,
                         QuerySubmitter = new Niem.NiemCore.v20.EntityType
                         {
                             EntityRepresentation = new Oasis.LegalXml.CourtFiling.v40.Ecf.PersonType
                             (
                                eportalUserId: string.Empty,
                                prefix: string.Empty,
                                givenName: "Jim",
                                middleName: string.Empty,
                                suffix: string.Empty,
                                surName: "Price",
                                id: string.Empty
                             ),
                             EntityRepresentationType = Niem.NiemCore.v20.EntityRepresentationTpes.EcfPerson
                         }
                     }
                };
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

        private void button2_Click(object sender, EventArgs e)
        {
            wmp.ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.ICourtRecordMDE>
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
                    CaseTrackingID = new Niem.Proxy.xsd.v20.String("P-1300-CV-20020106"),
                    CaseCourt = this.CaseCourt,
                    QuerySubmitter = new Niem.NiemCore.v20.EntityType
                    {
                        EntityRepresentation = this.PersonAtKeyBoard,
                        EntityRepresentationType = Niem.NiemCore.v20.EntityRepresentationTpes.EcfPerson
                    }
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
                    OrganizationIdentification = new List<nc.IdentificationType> { new nc.IdentificationType("courts.az.gov:1000") },
                    OrganizationLocation = new List<nc.LocationType>
                     {
                         new nc.LocationType
                         {
                              LocationAddress = new List<nc.AddressType>
                              {
                                  new nc.AddressType
                                  (
                                        address1: "120 S Cortez St" ,
                                        address2 : string.Empty ,
                                        city : "Prescott" ,
                                        state: "AZ" ,
                                        zipCode:"86302" ,
                                        countryCode:"US"
                                  ) 
                                  
                              }
                         }
                     },
                    CourtName = new List<nc.TextType>
                     {
                         new  nc.TextType("Yavapai County Superior Court ")
                     }
                };
            }

        }
        private void Save(wmp.GetCaseResponse response)
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
                            XmlSerializer serializer = new XmlSerializer(typeof(wmp.GetCaseResponse));
                            serializer.Serialize(fs, response, namespaces);
                            fs.Flush();
                            fs.Close();
                        }
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

        private void button3_Click(object sender, EventArgs e)
        {
            wmp.ReviewFilingRequest reviewFilingRequest = new wmp.ReviewFilingRequest
                 {
                     ReviewFilingRequestMessage = new wmp.ReviewFilingRequestMessageType
                     {
                         CoreFilingMessage = new aoc.CoreFilingMessageType
                         {
                             DocumentIdentification = new List<nc.IdentificationType>
                             {
                                 new nc.IdentificationType("14454")
                             },
                             Case = this.AZCivilCase,
                             // CaseTypeSelection = ecf.CaseTypeSelectionType.AZCivilCase ,
                             ExtendedMetadata = new List<aoc.MetadataType>
                            {
                                new  aoc.MetadataType{ Id="CORE_COMMENT1" , CommentText = new nc.TextType("Hello World")}
                            }
                         },
                         // PaymentMessage = this.PaymentMessage
                     }
                 };
            SubmitFiling(reviewFilingRequest);
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

        private aoc.PaymentMessageType PaymentMessage
        {
            get
            {
                List<UBL21.cac.AllowanceChargeType> allowanceCharges = new List<UBL21.cac.AllowanceChargeType> { };
                allowanceCharges.Add
                    (
                        new aoc.AllowanceChargeType
                        {
                            ID = new UBL21.cbc.IDType { Value = "P1" },
                            ChargeIndicator = new UBL21.cbc.ChargeIndicatorType { Value = true },
                            AllowanceChargeReasonCode = new UBL21.cbc.AllowanceChargeReasonCodeType { Value = "SERVICE_FEE" },
                            AllowanceChargeReason = new List<UBL21.cbc.AllowanceChargeReasonType> { new UBL21.cbc.AllowanceChargeReasonType { Value = "Application Fee" } },
                            MultiplierFactorNumeric = new UBL21.cbc.MultiplierFactorNumericType { Value = 1 },
                            PrepaidIndicator = new UBL21.cbc.PrepaidIndicatorType { Value = false },
                            SequenceNumeric = new UBL21.cbc.SequenceNumericType { Value = 1 },
                            Amount = new UBL21.cbc.AmountType { Value = 6.00M, CurrencyID = "USD" },
                            AllowanceChargeCategoryCode = new nc.TextType("APPLICATION_FEE"),
                            BaseAmount = new UBL21.cbc.BaseAmountType { Value = 6.00M, CurrencyID = "USD" }
                        }
                     );
                allowanceCharges.Add
                    (
                        new aoc.AllowanceChargeType
                        {
                            ID = new UBL21.cbc.IDType { Value = "P2" },
                            ChargeIndicator = new UBL21.cbc.ChargeIndicatorType { Value = true },
                            AllowanceChargeReasonCode = new UBL21.cbc.AllowanceChargeReasonCodeType { Value = "CLASSA" },
                            AllowanceChargeReason = new List<UBL21.cbc.AllowanceChargeReasonType> { new UBL21.cbc.AllowanceChargeReasonType { Value = "CLASSA Fee" } },
                            MultiplierFactorNumeric = new UBL21.cbc.MultiplierFactorNumericType { Value = 1 },
                            PrepaidIndicator = new UBL21.cbc.PrepaidIndicatorType { Value = false },
                            SequenceNumeric = new UBL21.cbc.SequenceNumericType { Value = 2 },
                            Amount = new UBL21.cbc.AmountType { Value = 280.00M, CurrencyID = "USD" },
                            AllowanceChargeCategoryCode = new nc.TextType("FILING_FEE"),
                            BaseAmount = new UBL21.cbc.BaseAmountType { Value = 280.00M, CurrencyID = "USD" }
                        }

                     );



                aoc.PaymentMessageType paymentMessage = new aoc.PaymentMessageType
                {
                    PayerName = "Gretchen Crates",
                    AllowanceCharge = allowanceCharges,
                    Payment = new UBL21.cac.PaymentType
                    {
                        ID = new UBL21.cbc.IDType { Value = "4HP88229KK651203L" },
                        PaidAmount = new UBL21.cbc.PaidAmountType { Value = 286.00M, CurrencyID = "USD" },
                        PaidDate = new UBL21.cbc.PaidDateType { Value = DateTime.Today },
                        PaidTime = new UBL21.cbc.PaidTimeType { Value = DateTime.Now },
                        ReceivedDate = new UBL21.cbc.ReceivedDateType { Value = DateTime.Today },
                        InstructionID = new UBL21.cbc.InstructionIDType { Value = "Payment for Filing # 781333" }
                    },
                    Address = new UBL21.cac.AddressType
                    {
                        CityName = new UBL21.cbc.CityNameType { Value = "Denver" },
                        PostalZone = new UBL21.cbc.PostalZoneType { Value = "80238" },
                        Country = new UBL21.cac.CountryType
                        {
                            IdentificationCode = new UBL21.cbc.IdentificationCodeType { Value = "US" },
                            Name = new UBL21.cbc.NameType { Value = "United States of America" }
                        },
                        AddressLine = new List<UBL21.cac.AddressLineType>
                        {
                            new UBL21.cac.AddressLineType{ Line = new UBL21.cbc.LineType{Value="2644 Roslyn St"}} , 
                            new UBL21.cac.AddressLineType{ Line = new UBL21.cbc.LineType{Value="Unit # 1"}}  
                        },
                        CountrySubentity = new UBL21.cbc.CountrySubentityType { Value = "AZ" },
                        CountrySubentityCode = new UBL21.cbc.CountrySubentityCodeType { Value = "AZ" },

                    },
                    Metadata = new List<aoc.MetadataType>
                    {
                        new aoc.MetadataType{ Id="PAYMNET_CARD_TYPE" , CommentText= new nc.TextType("VISA") },
                        new aoc.MetadataType{ Id="PAYMNET_CARD_EXPIRATION" , CommentText= new nc.TextType("0816") },
                        new aoc.MetadataType{ Id="PAYMNET_CARD_LAST4" , CommentText= new nc.TextType("7881") },

                    }
                };
                return paymentMessage;
            }
        }

        private aoc.CivilCaseType AZCivilCase
        {
            get
            {
                aoc.OrganizationType aocOrganization = new aoc.OrganizationType
                               (
                                 id: "PTY0003",
                                 eportalOrganizationId: string.Empty,
                                 name: "SURETY ACCEPTANCE CO",
                                 eportalUnitId: string.Empty,
                                 unitName: string.Empty,
                                 eportalSubUnitId: string.Empty,
                                 subUnitName: string.Empty,
                                 address1: "400 East Broadway Boulevard",
                                 address2: string.Empty,
                                 city: "Tucson",
                                 state: "AZ",
                                 zipCode: "85711",
                                 phoneNumber: "520-790-7181",
                                 extension: string.Empty,
                                 emailAddress: string.Empty
                               );
                aocOrganization.OrganizationAugmentation = new j.OrganizationAugmentationType();
                aocOrganization.EcfOrganizationAugmentation = new ecf.OrganizationAugmentationType();
                List<ecf.CaseParticipantType> caseParticipant = new List<ecf.CaseParticipantType>
                        {
                            new aoc.CaseParticipantType
                            {
                                 EntityRepresentation = new ecf.PersonType
                                 ( 
                                   id : "PTY0001" ,
                                   prefix: string.Empty,
                                   givenName: "CASSANDRA",
                                   middleName:string.Empty,
                                   surName: "PRICE" ,
                                   suffix: string.Empty ,
                                   eportalUserId: string.Empty 
                                 ) ,
                                 EntityRepresentationType = nc.EntityRepresentationTpes.EcfPerson,
                                 CaseParticipantRoleCode = new nc.TextType("PL")  
                            } ,
                            new aoc.CaseParticipantType
                            {
                                 EntityRepresentation = new ecf.PersonType
                                 ( 
                                   id : "PTY0002" ,
                                   prefix: string.Empty,
                                   givenName: "CHRISTOPHER",
                                   middleName: string.Empty,
                                   surName: "PRICE" ,
                                   suffix: string.Empty ,
                                   eportalUserId: string.Empty 
                                 ) ,
                                 EntityRepresentationType = nc.EntityRepresentationTpes.EcfPerson,
                                 CaseParticipantRoleCode = new nc.TextType("PL")  
                            } ,
                            new aoc.CaseParticipantType
                            {
                                 EntityRepresentation = aocOrganization ,                                  
                                 EntityRepresentationType = nc.EntityRepresentationTpes.AZAOCOrganization,
                                 CaseParticipantRoleCode = new nc.TextType("DE")  
                            } ,

                        };

                return new aoc.CivilCaseType
                {
                    CaseTitleText = new List<nc.TextType> { new nc.TextType("CCPACKING ET AL. Vs. Lamex") },
                    CaseCategoryText = new List<nc.TextType> { new nc.TextType("Civil") },
                    CaseTrackingID = new List<niemxsd.String> { new niemxsd.String("C20117066") },
                    ClassActionIndicator = new niemxsd.Boolean(false),
                    JuryDemandIndicator = new niemxsd.Boolean(true),
                    CaseGeneralCategoryText = new nc.TextType("Civil"),
                    CaseSubCategoryText = new nc.TextType("Default"),
                    CaseAugmentation = new j.CaseAugmentationType
                    {
                        CaseCourt = new List<j.CourtType> { this.CaseCourt }
                    },
                    EcfCaseAugmentation = new aoc.CaseAugmentationType
                    {
                        CaseParticipant = caseParticipant
                    }

                };
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a file to Submit ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    wmp.ReviewFilingRequest reviewFilingRequest = null;
                    using (var fs = new FileStream(opeFileDialog.FileName, FileMode.Open))
                    {
                        XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                        ecf.EcfHelper.AddNameSpaces(namespaces);
                        XmlSerializer serializer = new XmlSerializer(typeof(wmp.ReviewFilingRequest));
                        reviewFilingRequest = serializer.Deserialize(fs) as wmp.ReviewFilingRequest;
                        fs.Flush();
                        fs.Close();
                    }

                    SubmitFiling(reviewFilingRequest);
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

        private void SubmitFiling(wmp.ReviewFilingRequest reviewFilingRequest)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;

            try
            {
                if (reviewFilingRequest != null)
                {
                    _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<wmp.IFilingReviewMDE>
                        (
                            "FilingReviewMDEService",
                            _configFile
                        );

                    wmp.ReviewFilingResponse response = _serviceChannel.ReviewFiling(reviewFilingRequest);
                    Save(response);
                }
                else
                {
                    MessageBox.Show("Review filing request is null", "Submit Filing");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Submit Filing : Error");
            }
            finally
            {
                if (_serviceChannel != null && _serviceChannel is IClientChannel)
                {
                    VistaSG.Services.ServicesFactory.CloseChannel(_serviceChannel as IClientChannel);
                }

            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a file to Submit ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    wmp.ReviewFilingRequest reviewFilingRequest = null;
                    using (var fs = new FileStream(opeFileDialog.FileName, FileMode.Open))
                    {
                        XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                        ecf.EcfHelper.AddNameSpaces(namespaces);
                        XmlSerializer serializer = new XmlSerializer(typeof(wmp.ReviewFilingRequest));
                        reviewFilingRequest = serializer.Deserialize(fs) as wmp.ReviewFilingRequest;
                        fs.Flush();
                        fs.Close();
                    }
                    SubmitFiling(reviewFilingRequest);
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

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a file to Submit ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    wmp.ReviewFilingRequest reviewFilingRequest = null;
                    using (var fs = new FileStream(opeFileDialog.FileName, FileMode.Open))
                    {
                        XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                        ecf.EcfHelper.AddNameSpaces(namespaces);
                        XmlSerializer serializer = new XmlSerializer(typeof(wmp.ReviewFilingRequest));
                        reviewFilingRequest = serializer.Deserialize(fs) as wmp.ReviewFilingRequest;
                        fs.Flush();
                        fs.Close();
                    }
                    core.CoreFilingMessageType coreFilingMessage = reviewFilingRequest != null ? reviewFilingRequest.CoreFilingMessage : null;
                    aoc.CoreFilingMessageType filingMessage = coreFilingMessage != null && coreFilingMessage is aoc.CoreFilingMessageType ? coreFilingMessage as aoc.CoreFilingMessageType : null;
                    if (filingMessage != null)
                    {
                        DateTime? postTime = null;
                        if (filingMessage.DocumentPostDate != null && filingMessage.DocumentPostDate.Count > 0)
                        {
                            postTime = filingMessage.DocumentPostDate[0].DateValue;
                        }
                        MessageBox.Show(string.Format("Post Time {0} Received Time {1} ", postTime, filingMessage.ReceivedTime));

                    }
                    else
                    {
                        MessageBox.Show("Filing Message is null !!!!");
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

        private void button7_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC(statusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, statusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED);
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

        private wmp.NotifyDocketingCompleteRequest GetNDC(string statusCode , string statusDescription)
        {
            wmp.NotifyDocketingCompleteRequest ndcRequest = null;

            wmp.RecordFilingRequest rfr = this.GetRfrFromRvfr(statusCode , statusDescription);
            if (rfr != null)
            {
                    aoc.CoreFilingMessageType filingMessage = rfr.AZRecordFilingRequest.CoreFilingMessage;
                    aoc.PaymentMessageType paymentMessage = rfr.AZRecordFilingRequest.PaymentMessage;
                    List<aoc.RecordDocketingMessageType> docketingMessages = rfr.AZRecordFilingRequest.RecordDocketingMessage;
                    List<aoc.RecordDocketingCallbackMessageType> recordDocketingCallBackMessages = new List<aoc.RecordDocketingCallbackMessageType>();
                    aoc.CivilCaseType civilCase = filingMessage.Case != null ? filingMessage.Case as aoc.CivilCaseType : null;
                    if (civilCase != null && !string.IsNullOrWhiteSpace(statusCode) && 
                         ( statusCode.Equals(amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, StringComparison.OrdinalIgnoreCase) ||
                           statusCode.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase)
                         )

                        )
                    {
                        civilCase.CaseTrackingID = new List<niemxsd.String> { new niemxsd.String("P1300CV000800") };
                    }
                    // Generate Call Back Message
                    aoc.RecordDocketingCallbackMessageType recordDocketingCallBack = null;
                    int documentNumber = 0;
                    foreach (var dm in docketingMessages)
                    {
                        documentNumber++;
                        ecf.FilingStatusType filingStatus = new ecf.FilingStatusType
                           {
                               FilingStatusCode = statusCode,
                               StatusDescriptionText = nc.NiemStringHelper<nc.TextType>.ToList(statusDescription)
                           };

                        if (statusCode.Equals(PARTIAL_ACCEPTANCE, StringComparison.OrdinalIgnoreCase))
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
                                    FilingStatusCode = "REJECTED",
                                    StatusDescriptionText = nc.NiemStringHelper<nc.TextType>.ToList(statusDescription)
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
                            DocumentSubmitter = filingMessage.DocumentSubmitter,
                            SendingMDELocationID = new nc.IdentificationType("CRMDE ADDRESS"),
                            SendingMDEProfileCode = Niem.NiemCore.v20.Constants.ECF4_WEBSERVICES_SIP_CODE,
                            FilingStatus = filingStatus,
                            ReviewedLeadDocument = dm.ReviewedLeadDocument,
                            ReviewedConnectedDocument = dm.ReviewedConnectedDocument,
                            Case = filingMessage.Case,
                            CaseTypeSelection = filingMessage.CaseTypeSelection
                        };
                        recordDocketingCallBackMessages.Add(recordDocketingCallBack);

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
                        new nc.IdentificationType("1415" , amc.PolicyConstants.REVIEWREFMUSERID)
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
        private wmp.RecordFilingRequest GetRfrFromRvfr(string documentStatus ,  string statusDescription)
        {
            wmp.RecordFilingRequest rfr = null;
            string rvfrFile = this.GetFileNameToDeserialize();
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
                    aoc.CivilCaseType civilCase = filingMessage.Case != null && filingMessage.Case is aoc.CivilCaseType ? filingMessage.Case as aoc.CivilCaseType : null;
                    List<aoc.RecordDocketingMessageType> docketingMessages = new List<aoc.RecordDocketingMessageType>();
                    DateTime rfrPostDate = DateTime.Now;
                    string documentReviwerId = string.Empty;
                    List<nc.EntityType> documentReviwer = this.DocumentReviwer(out documentReviwerId);

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
                                CaseCourt = civilCase != null && civilCase.CaseAugmentation != null && civilCase.CaseAugmentation.CaseCourt.Count > 0 ? civilCase.CaseAugmentation.CaseCourt[0] : null,
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

        private void button8_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC(statusCode: "REJECTED", statusDescription: "DO NOT HAVE TO GIVE YOU A REASON");
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

        private void button9_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                wmp.NotifyDocketingCompleteRequest ndcRequest = this.GetNDC(statusCode: PARTIAL_ACCEPTANCE, statusDescription: "DO NOT REJECT COMPLAINT");
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
