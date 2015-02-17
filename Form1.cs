using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Xml.Linq;
using System.ServiceModel;
using System.Xml;
using Oasis.LegalXml.CourtFiling.v40.WebServiceMessagingProfile;
using System.Xml.Serialization;
using ecf = Oasis.LegalXml.CourtFiling.v40.Ecf;

namespace AZServiceTest
{
    public partial class Form1 : Form
    {
        private const string CONFIG_FILE = @"C:\temp\AZCRMDE.config";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        CONFIG_FILE 
                    );

                GetDocumentRequest request = new  GetDocumentRequest
                {
                   DocumentQueryMessage    = new  Oasis.LegalXml.CourtFiling.v40.DocumentQuery.DocumentQueryMessageType
                    {
                        CaseTrackingID = new Niem.Proxy.xsd.v20.String("CV 2015 000100"),
                        CaseDocketID = new Niem.Proxy.xsd.v20.String("133551") ,
                        CaseCourt = new Niem.Domains.Jxdm.v40.CourtType
                            (
                                eportalCourtId: string.Empty,
                                courtName: "Pima County",
                                eportalUnitId: string.Empty,
                                ePortalSubUnitId: string.Empty,
                                unitName: string.Empty,
                                subUnitName: string.Empty
                            ),
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
                GetDocumentResponse response = _serviceChannel.GetDocument(request);
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
            ICourtRecordMDE _serviceChannel = null;

            try
            {
                _serviceChannel = VistaSG.Services.ServicesFactory.CreateServiceChannel<ICourtRecordMDE>
                    (
                        "CourtRecordMDEService",
                        CONFIG_FILE
                    );

                GetCaseRequest request = new GetCaseRequest
                {
                    CaseQueryMessage = new Oasis.LegalXml.CourtFiling.v40.CaseQuery.CaseQueryMessageType
                    {
                        CaseTrackingID = new Niem.Proxy.xsd.v20.String( "CV 2015 000100"),
                        CaseCourt = new Niem.Domains.Jxdm.v40.CourtType
                            (
                                eportalCourtId: string.Empty,
                                courtName: "Pima County",
                                eportalUnitId: string.Empty,
                                ePortalSubUnitId: string.Empty,
                                unitName: string.Empty,
                                subUnitName: string.Empty
                            ),
                        QuerySubmitter = new Niem.NiemCore.v20.EntityType
                        {
                             EntityRepresentation = new  Oasis.LegalXml.CourtFiling.v40.Ecf.PersonType
                             (
                                eportalUserId: string.Empty,
                                prefix: string.Empty,
                                givenName: "Jim",
                                middleName: string.Empty,
                                suffix: string.Empty ,
                                surName:"Price",
                                id : string.Empty
                             ) ,
                              EntityRepresentationType = Niem.NiemCore.v20.EntityRepresentationTpes.EcfPerson 
                        }
                    }
                };
                GetCaseResponse response = _serviceChannel.GetCase(request);
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


        private void Save(GetCaseResponse response)
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
                            XmlSerializer serializer = new XmlSerializer(typeof(GetCaseResponse));
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

        private void Save(GetDocumentResponse response)
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
                            XmlSerializer serializer = new XmlSerializer(typeof(GetDocumentResponse));
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
    }
}
