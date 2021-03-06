﻿using System;
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
using wmp = Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;
using VistaSG.Requests.DataContracts.Types;
using ubl21 = Oassis.UBL.v21;


namespace AZServiceTest
{
    public partial class Form1 : Form
    {
        private const string DEFAULT_CONFIG_FILE = @".\AZCRMDE.config";
        private string _configFile = string.Empty;
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
                string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                NDCHelper ndcHelper = new NDCHelper();
                wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, 
                        documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED  ,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED, 
                        filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED ,
                        netZeroSubmission: false ,
                        overPaymentAmount : 0.00M ,
                        changeDocumentType : false ,
                        ajacsCaseNumber:ajacsCaseNumber ,
                        rejectFirstDocument:false 
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
                string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                NDCHelper ndcHelper = new NDCHelper();
                wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED, 
                        documentStatusDescription: "DO NOT HAVE TO GIVE YOU A REASON"  ,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED ,
                        filingStatusDescription: "DO NOT HAVE TO GIVE YOU A REASON" ,
                        netZeroSubmission:false ,
                        overPaymentAmount:0.00M ,
                        changeDocumentType : false ,
                        ajacsCaseNumber:ajacsCaseNumber ,
                        rejectFirstDocument:false
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
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\exchange\NotifyDocketingComplete-MessageExchange.xsd"
                        );

                    schemas.Add
                        (
                            null, 
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\common\ECF-3.0_Null_Signature_Profile_1.0.xsd"
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
                string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                NDCHelper ndcHelper = new NDCHelper();
                wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                    (
                        documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED ,
                        netZeroSubmission:true ,
                        overPaymentAmount:0.00M ,
                        changeDocumentType:false ,
                        ajacsCaseNumber:ajacsCaseNumber ,
                        rejectFirstDocument:false 
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
                    string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                    NDCHelper ndcHelper = new NDCHelper();
                    wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                        (
                            documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            netZeroSubmission: false,
                            overPaymentAmount: overPaymentAmount ,
                            changeDocumentType : false  ,
                            ajacsCaseNumber:ajacsCaseNumber ,
                            rejectFirstDocument:false
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
                string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                NDCHelper ndcHelper = new NDCHelper();
                wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                        (
                            documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            netZeroSubmission: false,
                            overPaymentAmount: overPaymentAmount ,
                            changeDocumentType:true ,
                            ajacsCaseNumber:ajacsCaseNumber ,
                            rejectFirstDocument:false
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

        private void btnFixNDC_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a NDC (NotifyDocketingCompleteRequest) to transform ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    amc.NotifyDocketingCompleteRequestType originalNDC = null;
                    XmlSerializer ndcrSerializer = new XmlSerializer(typeof(amc.NotifyDocketingCompleteRequestType));
                    using (var fs = new FileStream(opeFileDialog.FileName , FileMode.Open, FileAccess.Read))
                    {
                        originalNDC = ndcrSerializer.Deserialize(fs) as amc.NotifyDocketingCompleteRequestType;
                    }
                    if (originalNDC != null)
                    {
                        NDCHelper ndcHelper = new NDCHelper();
                        RequestType request = new RequestType
                        {
                            Information = new RequestInformationType {  SubmittedToOrganizationId = 1 , SubmittedToOrganizationName = "Yavapai County  -  Prescott" }
                        };
                    
                        amc.NotifyDocketingCompleteRequestType transformedNDCRequest = ndcHelper.TransformNDC(originalNDC: originalNDC , request:request);
                        Save(transformedNDCRequest);
                    }
                    else
                    {
                        MessageBox.Show("AOC NDC Request is null" , "Error");
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

        private void Save(amc.NotifyDocketingCompleteRequestType transformedNDC)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (transformedNDC != null )
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
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            File.Delete(saveFileDialog.FileName);
                        }
                        XmlSerializer ndcrSerializer = new XmlSerializer(typeof(amc.NotifyDocketingCompleteRequestType));
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew, FileAccess.Write))
                        {
                            ndcrSerializer.Serialize(fs, transformedNDC);
                            fs.Flush();
                            fs.Close();
                        }

                        MessageBox.Show(string.Format("Saved NDC to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("NDC is null !!!!"), "Save");
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

        private void btnGenerateNFRCFromNDC_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a NDC (NotifyDocketingCompleteRequest) to generate NFRC From ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    amc.NotifyDocketingCompleteRequestType originalNDC = null;
                    XmlSerializer ndcrSerializer = new XmlSerializer(typeof(amc.NotifyDocketingCompleteRequestType));
                    using (var fs = new FileStream(opeFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        originalNDC = ndcrSerializer.Deserialize(fs) as amc.NotifyDocketingCompleteRequestType;
                    }
                    if (originalNDC != null)
                    {
                        NDCHelper ndcHelper = new NDCHelper();

                        amc.NotifyFilingReviewCompleteRequestType nfrcRequests = ndcHelper.ToNfrc(ndcRequest: originalNDC);
                        Save(nfrcRequests);
                    }
                    else
                    {
                        MessageBox.Show("AOC NDC Request is null", "Error");
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


        private void Save(amc.NotifyFilingReviewCompleteRequestType nfrcRequest)
        {

            SaveFileDialog saveFileDialog = null;
            try
            {
                if (nfrcRequest != null)
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
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            File.Delete(saveFileDialog.FileName);
                        }
                        XmlSerializer ndcrSerializer = new XmlSerializer(typeof(amc.NotifyFilingReviewCompleteRequestType));
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew, FileAccess.Write))
                        {
                            ndcrSerializer.Serialize(fs, nfrcRequest);
                            fs.Flush();
                            fs.Close();
                        }

                        MessageBox.Show(string.Format("Saved NFRC to {0}", saveFileDialog.FileName), "Save");
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format("NDC is null !!!!"), "Save");
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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a RVFR (ReviewFilingRequest) Validate ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    XmlSchemaSet schemas = new XmlSchemaSet();

                    schemas.Add
                        (
                            null,
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\exchange\ReviewFilingRequest-MessageExchange.xsd"
                        );

                    schemas.Add
                        (
                            null,
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\common\ECF-3.0_Null_Signature_Profile_1.0.xsd"
                        );


                    XDocument doc = XDocument.Load(opeFileDialog.FileName);
                    bool errors = false;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    doc.Validate(schemas, (o, validationError) =>
                    {
                        sb.Append(string.Format("{0}\r\n", validationError.Message));
                        errors = true;
                    }, true);
                    if (errors)
                    {
                        MessageBox.Show(sb.ToString(), string.Format("document {0} {1}", opeFileDialog.FileName, "did not validate"));
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

        private void button2_Click(object sender, EventArgs e)
        {
            wmp.IFilingReviewMDE _serviceChannel = null;
            try
            {
                decimal overPaymentAmount = 0.00M;
                    string ajacsCaseNumber = !string.IsNullOrWhiteSpace(this.tbAJACSCaseNumber.Text) ? this.tbAJACSCaseNumber.Text.Trim() : "P1300CV000800";
                    NDCHelper ndcHelper = new NDCHelper();
                    wmp.NotifyDocketingCompleteRequest ndcRequest = ndcHelper.GetNDC
                        (
                            documentStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            documentStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            filingStatusDescription: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                            netZeroSubmission: false,
                            overPaymentAmount: overPaymentAmount,
                            changeDocumentType: false,
                            ajacsCaseNumber: ajacsCaseNumber,
                            rejectFirstDocument: true
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a NFRC (NotifyFilingReviewCompleteRequest) Validate ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    XmlSchemaSet schemas = new XmlSchemaSet();

                    schemas.Add
                        (
                            null,
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\exchange\NotifyFilingReviewComplete-MessageExchange.xsd"
                        );

                    schemas.Add
                        (
                            null,
                            @"C:\development\eUniversa1\5.4\ECF4.01\xsd\common\ECF-3.0_Null_Signature_Profile_1.0.xsd"
                        );


                    XDocument doc = XDocument.Load(opeFileDialog.FileName);
                    bool errors = false;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    doc.Validate(schemas, (o, validationError) =>
                    {
                        sb.Append(string.Format("{0}\r\n", validationError.Message));
                        errors = true;
                    }, true);
                    if (errors)
                    {
                        MessageBox.Show(sb.ToString(), string.Format("document {0} {1}", opeFileDialog.FileName, "did not validate"));
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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                aoc.PaymentMessageType paymentMessage = new aoc.PaymentMessageType
                {
                    FeeExceptionReasonCode = string.Empty,
                    FeeExceptionSupportingText = string.Empty,
                    PayerName = string.Empty ,
                    AllowanceCharge = new List<Oassis.UBL.v21.cac.AllowanceChargeType>
                    {
                            new aoc.AllowanceChargeType
                            {
                                ChargeIndicator = new ubl21.cbc.ChargeIndicatorType { },
                                Amount =  new ubl21.cbc.AmountType { }
                            }

                    } ,
                    Address = new ubl21.cac.AddressType() ,
                    Payment = new ubl21.cac.PaymentType() 
                };
                XmlSerializerNamespaces nameSpaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                nameSpaces.Add("aoc", "http://schema.azcourts.az.gov/aoc/efiling/ecf/extension/2.0");
                nameSpaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                XmlSerializer serializer = new XmlSerializer(typeof(aoc.PaymentMessageType));
                string paymentMessageXML = string.Empty;
                using (var ms = new MemoryStream())
                {
                    serializer.Serialize(ms, paymentMessage, nameSpaces);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(ms);
                    paymentMessageXML = sr.ReadToEnd();
                }
                MessageBox.Show(paymentMessageXML);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog opeFileDialog = null;
            Arizona.Courts.Services.v20.FilingAssemblyMDEServiceAccess famdeServiceAccess = null;
            try
            {
                opeFileDialog = new OpenFileDialog();
                opeFileDialog.CheckFileExists = false;
                opeFileDialog.CheckPathExists = true;
                opeFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                opeFileDialog.Title = "Select a NFRC (NotifyFilingReviewCompleteRequest) to Post ";
                DialogResult dr = opeFileDialog.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    amc.NotifyFilingReviewCompleteRequestType nfrc = null;
                    XmlSerializer ndcrSerializer = new XmlSerializer(typeof(amc.NotifyFilingReviewCompleteRequestType));
                    using (var fs = new FileStream(opeFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        nfrc = ndcrSerializer.Deserialize(fs) as amc.NotifyFilingReviewCompleteRequestType;
                    }
                    if (nfrc != null)
                    {
                        string configurationFile = VistaSG.Common.ConfigurationHelper.GetSetting("famdeConfigurationFile");
                        famdeServiceAccess = new Arizona.Courts.Services.v20.FilingAssemblyMDEServiceAccess { ServiceConfigurationFileName = configurationFile };
                        string response = famdeServiceAccess.NotifyReviewComplete(nfrc);
                        MessageBox.Show(string.Format("Posted NFRC File {0} Response {1}", opeFileDialog.FileName , response));
                    }
                    else
                    {
                        MessageBox.Show(string.Format("NFRC is null File name {0}", opeFileDialog.FileName));
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
                if (famdeServiceAccess != null)
                {
                    famdeServiceAccess.Dispose();
                }

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Exception ex = new Exception("Exception has been thrown by the target of an invocation.  Value can't be null");
            Exception ex = new Exception(" An error occurred while making the HTTP request to https://uvazefmuat.turbocourt.com/efsp-uvaz/NotifyFilingReviewComplete. This could be due to the fact that the server certificate is not configured properly with HTTP.SYS in the HTTPS case. This could also be caused by a mismatch of the security binding between the client and the server.");
            string exceptionMessage = ex.ToString();
            if (exceptionMessage.Length > 100)
            {
                exceptionMessage = exceptionMessage.Substring(0, 100);
            }
            MessageBox.Show(string.Format("Exception {0}", exceptionMessage));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Maricopa.Services.FilingAssemblyMDEClient serviceClient = new Maricopa.Services.FilingAssemblyMDEClient();
            try
            {
                
                MCNFRCHelper nfrcHelper = new MCNFRCHelper();
                string nfrcText = nfrcHelper.GetNFRC
                    (
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_ACCEPTED,
                        rejectionReason: string.Empty
                    );
                if (!string.IsNullOrWhiteSpace(nfrcText))
                {
                    object callReturn = serviceClient.notifyFilingReviewComplete(nfrcText);
                    MessageBox.Show("Call Complete");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (serviceClient != null)
                {
                    serviceClient.Close();
                }

            }

        }

        private void buttonmcrejected_Click(object sender, EventArgs e)
        {
            Maricopa.Services.FilingAssemblyMDEClient serviceClient = new Maricopa.Services.FilingAssemblyMDEClient();
            try
            {

                MCNFRCHelper nfrcHelper = new MCNFRCHelper();
                string nfrcText = nfrcHelper.GetNFRC
                    (
                        filingStatusCode: amc.PolicyConstants.REVIEWED_DOCUMENT_STATUS_REJECTED,
                        rejectionReason: "The document must be captioned for Maricopa County Superior Court and include the correct names of the parties. Carl Collins should be the first listed defendant."
                    );
                if (!string.IsNullOrWhiteSpace(nfrcText))
                {
                    object callReturn = serviceClient.notifyFilingReviewComplete(nfrcText);
                    MessageBox.Show("Call Complete");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (serviceClient != null)
                {
                    serviceClient.Close();
                }

            }


        }

        private void buttonmcjudgereview_Click(object sender, EventArgs e)
        {
            Maricopa.Services.FilingAssemblyMDEClient serviceClient = new Maricopa.Services.FilingAssemblyMDEClient();
            try
            {

                MCNFRCHelper nfrcHelper = new MCNFRCHelper();
                string nfrcText = nfrcHelper.GetNFRC
                    (
                        filingStatusCode: "pending judge review",
                        rejectionReason: string.Empty
                    );
                if (!string.IsNullOrWhiteSpace(nfrcText))
                {
                    object callReturn = serviceClient.notifyFilingReviewComplete(nfrcText);
                    MessageBox.Show("Call Complete");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (serviceClient != null)
                {
                    serviceClient.Close();
                }

            }


        }
    }
}
