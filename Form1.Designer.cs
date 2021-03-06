﻿namespace AZServiceTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetCase = new System.Windows.Forms.Button();
            this.btnSubmitAllAcceptedNoChnage = new System.Windows.Forms.Button();
            this.btnSubmitAllRejected = new System.Windows.Forms.Button();
            this.btnValidateAOCCase = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCaseNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDocketId = new System.Windows.Forms.TextBox();
            this.btnGetDocument = new System.Windows.Forms.Button();
            this.tbAJACSCaseNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnNetZero = new System.Windows.Forms.Button();
            this.btngetCasesFromFile = new System.Windows.Forms.Button();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.buttonOverPayment = new System.Windows.Forms.Button();
            this.tbOverPaymentAmount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonChangeDocumentType = new System.Windows.Forms.Button();
            this.btnFixNDC = new System.Windows.Forms.Button();
            this.btnGenerateNFRCFromNDC = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.buttonmcaccepted = new System.Windows.Forms.Button();
            this.buttonmcrejected = new System.Windows.Forms.Button();
            this.buttonmcjudgereview = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetCase
            // 
            this.btnGetCase.Location = new System.Drawing.Point(413, 281);
            this.btnGetCase.Name = "btnGetCase";
            this.btnGetCase.Size = new System.Drawing.Size(119, 23);
            this.btnGetCase.TabIndex = 101;
            this.btnGetCase.Text = "Get Case";
            this.btnGetCase.UseVisualStyleBackColor = true;
            this.btnGetCase.Click += new System.EventHandler(this.btnGetCase_Click);
            // 
            // btnSubmitAllAcceptedNoChnage
            // 
            this.btnSubmitAllAcceptedNoChnage.Location = new System.Drawing.Point(27, 67);
            this.btnSubmitAllAcceptedNoChnage.Name = "btnSubmitAllAcceptedNoChnage";
            this.btnSubmitAllAcceptedNoChnage.Size = new System.Drawing.Size(211, 23);
            this.btnSubmitAllAcceptedNoChnage.TabIndex = 2;
            this.btnSubmitAllAcceptedNoChnage.Text = "Submit All Accepted NDC From RVFR";
            this.btnSubmitAllAcceptedNoChnage.UseVisualStyleBackColor = true;
            this.btnSubmitAllAcceptedNoChnage.Click += new System.EventHandler(this.btnSubmitAllAcceptedNoChnage_Click);
            // 
            // btnSubmitAllRejected
            // 
            this.btnSubmitAllRejected.Location = new System.Drawing.Point(259, 67);
            this.btnSubmitAllRejected.Name = "btnSubmitAllRejected";
            this.btnSubmitAllRejected.Size = new System.Drawing.Size(211, 23);
            this.btnSubmitAllRejected.TabIndex = 3;
            this.btnSubmitAllRejected.Text = "Submit All Rejected From RVFR";
            this.btnSubmitAllRejected.UseVisualStyleBackColor = true;
            this.btnSubmitAllRejected.Click += new System.EventHandler(this.btnSubmitAllRejected_Click);
            // 
            // btnValidateAOCCase
            // 
            this.btnValidateAOCCase.Location = new System.Drawing.Point(41, 421);
            this.btnValidateAOCCase.Name = "btnValidateAOCCase";
            this.btnValidateAOCCase.Size = new System.Drawing.Size(197, 32);
            this.btnValidateAOCCase.TabIndex = 105;
            this.btnValidateAOCCase.TabStop = false;
            this.btnValidateAOCCase.Text = "Validate NDC";
            this.btnValidateAOCCase.UseVisualStyleBackColor = true;
            this.btnValidateAOCCase.Click += new System.EventHandler(this.btnValidateAOCCase_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 281);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Case # ";
            // 
            // textBoxCaseNumber
            // 
            this.textBoxCaseNumber.Location = new System.Drawing.Point(110, 281);
            this.textBoxCaseNumber.MaxLength = 50;
            this.textBoxCaseNumber.Name = "textBoxCaseNumber";
            this.textBoxCaseNumber.Size = new System.Drawing.Size(270, 20);
            this.textBoxCaseNumber.TabIndex = 100;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 317);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Document # ";
            // 
            // tbDocketId
            // 
            this.tbDocketId.Location = new System.Drawing.Point(110, 319);
            this.tbDocketId.MaxLength = 50;
            this.tbDocketId.Name = "tbDocketId";
            this.tbDocketId.Size = new System.Drawing.Size(270, 20);
            this.tbDocketId.TabIndex = 102;
            // 
            // btnGetDocument
            // 
            this.btnGetDocument.Location = new System.Drawing.Point(413, 331);
            this.btnGetDocument.Name = "btnGetDocument";
            this.btnGetDocument.Size = new System.Drawing.Size(119, 23);
            this.btnGetDocument.TabIndex = 103;
            this.btnGetDocument.Text = "Get Document";
            this.btnGetDocument.UseVisualStyleBackColor = true;
            this.btnGetDocument.Click += new System.EventHandler(this.btnGetDocument_Click);
            // 
            // tbAJACSCaseNumber
            // 
            this.tbAJACSCaseNumber.Location = new System.Drawing.Point(110, 30);
            this.tbAJACSCaseNumber.MaxLength = 50;
            this.tbAJACSCaseNumber.Name = "tbAJACSCaseNumber";
            this.tbAJACSCaseNumber.Size = new System.Drawing.Size(270, 20);
            this.tbAJACSCaseNumber.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "AJACS Case # ";
            // 
            // btnNetZero
            // 
            this.btnNetZero.Location = new System.Drawing.Point(492, 67);
            this.btnNetZero.Name = "btnNetZero";
            this.btnNetZero.Size = new System.Drawing.Size(211, 23);
            this.btnNetZero.TabIndex = 4;
            this.btnNetZero.Text = "Submit All Accepted NET ZERO";
            this.btnNetZero.UseVisualStyleBackColor = true;
            this.btnNetZero.Click += new System.EventHandler(this.buttonNetZero_Click);
            // 
            // btngetCasesFromFile
            // 
            this.btngetCasesFromFile.Location = new System.Drawing.Point(41, 370);
            this.btngetCasesFromFile.Name = "btngetCasesFromFile";
            this.btngetCasesFromFile.Size = new System.Drawing.Size(197, 23);
            this.btngetCasesFromFile.TabIndex = 104;
            this.btngetCasesFromFile.Text = "Get Cases from File";
            this.btngetCasesFromFile.UseVisualStyleBackColor = true;
            this.btngetCasesFromFile.Click += new System.EventHandler(this.btngetCasesFromFile_Click);
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(33, 496);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ReadOnly = true;
            this.tbStatus.Size = new System.Drawing.Size(570, 138);
            this.tbStatus.TabIndex = 900;
            this.tbStatus.TabStop = false;
            // 
            // buttonOverPayment
            // 
            this.buttonOverPayment.Location = new System.Drawing.Point(383, 195);
            this.buttonOverPayment.Name = "buttonOverPayment";
            this.buttonOverPayment.Size = new System.Drawing.Size(211, 23);
            this.buttonOverPayment.TabIndex = 6;
            this.buttonOverPayment.Text = "Submit All Accepted Over Payment";
            this.buttonOverPayment.UseVisualStyleBackColor = true;
            this.buttonOverPayment.Click += new System.EventHandler(this.buttonOverPayment_Click);
            // 
            // tbOverPaymentAmount
            // 
            this.tbOverPaymentAmount.Location = new System.Drawing.Point(172, 195);
            this.tbOverPaymentAmount.MaxLength = 50;
            this.tbOverPaymentAmount.Name = "tbOverPaymentAmount";
            this.tbOverPaymentAmount.Size = new System.Drawing.Size(173, 20);
            this.tbOverPaymentAmount.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Over Payment Amount:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(72, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 9;
            // 
            // buttonChangeDocumentType
            // 
            this.buttonChangeDocumentType.Location = new System.Drawing.Point(41, 231);
            this.buttonChangeDocumentType.Name = "buttonChangeDocumentType";
            this.buttonChangeDocumentType.Size = new System.Drawing.Size(439, 23);
            this.buttonChangeDocumentType.TabIndex = 7;
            this.buttonChangeDocumentType.Text = "Remove First Apperance Fee and Change to AFFIDAVIT OF RENEWAL OF JUDGMENT";
            this.buttonChangeDocumentType.UseVisualStyleBackColor = true;
            this.buttonChangeDocumentType.Click += new System.EventHandler(this.buttonChangeDocumentType_Click);
            // 
            // btnFixNDC
            // 
            this.btnFixNDC.Location = new System.Drawing.Point(248, 421);
            this.btnFixNDC.Name = "btnFixNDC";
            this.btnFixNDC.Size = new System.Drawing.Size(141, 32);
            this.btnFixNDC.TabIndex = 106;
            this.btnFixNDC.Text = "Transform NDC";
            this.btnFixNDC.UseVisualStyleBackColor = true;
            this.btnFixNDC.Click += new System.EventHandler(this.btnFixNDC_Click);
            // 
            // btnGenerateNFRCFromNDC
            // 
            this.btnGenerateNFRCFromNDC.Location = new System.Drawing.Point(424, 421);
            this.btnGenerateNFRCFromNDC.Name = "btnGenerateNFRCFromNDC";
            this.btnGenerateNFRCFromNDC.Size = new System.Drawing.Size(222, 32);
            this.btnGenerateNFRCFromNDC.TabIndex = 107;
            this.btnGenerateNFRCFromNDC.Text = "Generate NFRC From NDC";
            this.btnGenerateNFRCFromNDC.UseVisualStyleBackColor = true;
            this.btnGenerateNFRCFromNDC.Click += new System.EventHandler(this.btnGenerateNFRCFromNDC_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 459);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(197, 32);
            this.button1.TabIndex = 901;
            this.button1.TabStop = false;
            this.button1.Text = "Validate RVFR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(618, 192);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(239, 23);
            this.button2.TabIndex = 902;
            this.button2.Text = "Reject First Document and Accept Rest";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(248, 459);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(197, 32);
            this.button3.TabIndex = 903;
            this.button3.TabStop = false;
            this.button3.Text = "Validate NFRC";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(466, 459);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(197, 32);
            this.button4.TabIndex = 904;
            this.button4.TabStop = false;
            this.button4.Text = "PaymentMessage Test";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(687, 459);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(197, 32);
            this.button5.TabIndex = 905;
            this.button5.TabStop = false;
            this.button5.Text = "Post NFRC";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(633, 331);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(197, 32);
            this.button6.TabIndex = 906;
            this.button6.TabStop = false;
            this.button6.Text = "Substring test";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // buttonmcaccepted
            // 
            this.buttonmcaccepted.Location = new System.Drawing.Point(28, 125);
            this.buttonmcaccepted.Name = "buttonmcaccepted";
            this.buttonmcaccepted.Size = new System.Drawing.Size(235, 23);
            this.buttonmcaccepted.TabIndex = 907;
            this.buttonmcaccepted.Text = "MC Accepted NFRC From CFM";
            this.buttonmcaccepted.UseVisualStyleBackColor = true;
            this.buttonmcaccepted.Click += new System.EventHandler(this.button7_Click);
            // 
            // buttonmcrejected
            // 
            this.buttonmcrejected.Location = new System.Drawing.Point(269, 125);
            this.buttonmcrejected.Name = "buttonmcrejected";
            this.buttonmcrejected.Size = new System.Drawing.Size(211, 23);
            this.buttonmcrejected.TabIndex = 908;
            this.buttonmcrejected.Text = "MC Rejected NFRC From CFM";
            this.buttonmcrejected.UseVisualStyleBackColor = true;
            this.buttonmcrejected.Click += new System.EventHandler(this.buttonmcrejected_Click);
            // 
            // buttonmcjudgereview
            // 
            this.buttonmcjudgereview.Location = new System.Drawing.Point(492, 125);
            this.buttonmcjudgereview.Name = "buttonmcjudgereview";
            this.buttonmcjudgereview.Size = new System.Drawing.Size(283, 23);
            this.buttonmcjudgereview.TabIndex = 909;
            this.buttonmcjudgereview.Text = "MC Pending Judge Review  NFRC From CFM";
            this.buttonmcjudgereview.UseVisualStyleBackColor = true;
            this.buttonmcjudgereview.Click += new System.EventHandler(this.buttonmcjudgereview_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 754);
            this.Controls.Add(this.buttonmcjudgereview);
            this.Controls.Add(this.buttonmcrejected);
            this.Controls.Add(this.buttonmcaccepted);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGenerateNFRCFromNDC);
            this.Controls.Add(this.btnFixNDC);
            this.Controls.Add(this.buttonChangeDocumentType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbOverPaymentAmount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonOverPayment);
            this.Controls.Add(this.tbStatus);
            this.Controls.Add(this.btngetCasesFromFile);
            this.Controls.Add(this.btnNetZero);
            this.Controls.Add(this.tbAJACSCaseNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnGetDocument);
            this.Controls.Add(this.tbDocketId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxCaseNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnValidateAOCCase);
            this.Controls.Add(this.btnSubmitAllRejected);
            this.Controls.Add(this.btnSubmitAllAcceptedNoChnage);
            this.Controls.Add(this.btnGetCase);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnGetCase;
        private System.Windows.Forms.Button btnSubmitAllAcceptedNoChnage;
        private System.Windows.Forms.Button btnSubmitAllRejected;
        private System.Windows.Forms.Button btnValidateAOCCase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCaseNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDocketId;
        private System.Windows.Forms.Button btnGetDocument;
        private System.Windows.Forms.TextBox tbAJACSCaseNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNetZero;
        private System.Windows.Forms.Button btngetCasesFromFile;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.Button buttonOverPayment;
        private System.Windows.Forms.TextBox tbOverPaymentAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonChangeDocumentType;
        public System.Windows.Forms.Button btnFixNDC;
        public System.Windows.Forms.Button btnGenerateNFRCFromNDC;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button buttonmcaccepted;
        private System.Windows.Forms.Button buttonmcrejected;
        private System.Windows.Forms.Button buttonmcjudgereview;
    }
}

