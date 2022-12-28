using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using TextConverter;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace xEncode
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lbFile;
		private System.Windows.Forms.Button btnAddFile;
		private System.Windows.Forms.Button btnAddFolder;
		private System.Windows.Forms.Button butRemoveSelected;
		private System.Windows.Forms.Button btnRemoveAll;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox tbPreview;
		private System.Windows.Forms.ComboBox cbDefault;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbChangeTo;
		private System.Windows.Forms.TextBox tbDefault;
		private System.Windows.Forms.TextBox tbChangeTo;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.TextBox tbAddFolder;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox cbWithSub;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.OpenFileDialog ofdAddFile;
		private System.Windows.Forms.FolderBrowserDialog fbdAddFolder;
		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox cbPreserveDate;
		private System.Windows.Forms.CheckBox cbPreserveAtri;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnCancel;

		//��� ����
		Encoder encoder = new Encoder();
		private System.Windows.Forms.Label lblEncoding;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox gbFileList;
		private System.Windows.Forms.CheckBox cbSimulaton;
		bool stopped = false;
		Thread convertThread;

		//������
		public frmMain()
		{
			InitializeComponent();
			foreach(string str in Encoder.encodingNames)
			{
				this.cbChangeTo.Items.Add(str);
				this.cbDefault.Items.Add(str);
			}
			this.cbDefault.SelectedItem = "Default";
			this.cbChangeTo.SelectedItem = "UTF-8";

            //2021.10.05
            RegistryKey reg; 
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("xEncode");
            this.cbDefault.SelectedItem = reg.GetValue("cbDefault", "Default").ToString();
            this.cbChangeTo.SelectedItem = reg.GetValue("cbChangeTo", "UTF-8").ToString();
            this.tbAddFolder.Text = reg.GetValue("tbAddFolder", "*.txt").ToString();
            
            FormClosing += new FormClosingEventHandler(closing);//2021.10.05
        }
        private void closing(object sender, FormClosingEventArgs e)//2021.10.05
        {
            //2021.10.05
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("xEncode");
            reg.SetValue("cbDefault", this.cbDefault.SelectedItem);
            reg.SetValue("cbChangeTo", this.cbChangeTo.SelectedItem);
            reg.SetValue("tbAddFolder", this.tbAddFolder.Text);

            //Application.Exit();
        }
        /// <summary>
        /// ��� ���� ��� ���ҽ��� �����մϴ�.
        /// </summary>
        protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form �����̳ʿ��� ������ �ڵ�
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.gbFileList = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbAddFolder = new System.Windows.Forms.TextBox();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.cbWithSub = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.lbFile = new System.Windows.Forms.ListBox();
            this.butRemoveSelected = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSimulaton = new System.Windows.Forms.CheckBox();
            this.cbPreserveAtri = new System.Windows.Forms.CheckBox();
            this.cbPreserveDate = new System.Windows.Forms.CheckBox();
            this.tbDefault = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDefault = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbChangeTo = new System.Windows.Forms.ComboBox();
            this.tbChangeTo = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbPreview = new System.Windows.Forms.TextBox();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ofdAddFile = new System.Windows.Forms.OpenFileDialog();
            this.fbdAddFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.gbFileList.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFileList
            // 
            this.gbFileList.Controls.Add(this.groupBox4);
            this.gbFileList.Controls.Add(this.btnAddFile);
            this.gbFileList.Controls.Add(this.lbFile);
            this.gbFileList.Controls.Add(this.butRemoveSelected);
            this.gbFileList.Controls.Add(this.btnRemoveAll);
            this.gbFileList.Controls.Add(this.label5);
            this.gbFileList.Location = new System.Drawing.Point(10, 9);
            this.gbFileList.Name = "gbFileList";
            this.gbFileList.Size = new System.Drawing.Size(720, 267);
            this.gbFileList.TabIndex = 0;
            this.gbFileList.TabStop = false;
            this.gbFileList.Text = "���ϸ��";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbAddFolder);
            this.groupBox4.Controls.Add(this.btnAddFolder);
            this.groupBox4.Controls.Add(this.cbWithSub);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(10, 207);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(576, 51);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "�����߰�";
            // 
            // tbAddFolder
            // 
            this.tbAddFolder.Location = new System.Drawing.Point(106, 17);
            this.tbAddFolder.Name = "tbAddFolder";
            this.tbAddFolder.Size = new System.Drawing.Size(200, 21);
            this.tbAddFolder.TabIndex = 2;
            this.tbAddFolder.Text = "*.txt";
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(451, 17);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(106, 26);
            this.btnAddFolder.TabIndex = 1;
            this.btnAddFolder.Text = "�����߰�";
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // cbWithSub
            // 
            this.cbWithSub.Checked = true;
            this.cbWithSub.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWithSub.Location = new System.Drawing.Point(321, 17);
            this.cbWithSub.Name = "cbWithSub";
            this.cbWithSub.Size = new System.Drawing.Size(125, 26);
            this.cbWithSub.TabIndex = 4;
            this.cbWithSub.Text = "�������� ����";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 28);
            this.label3.TabIndex = 3;
            this.label3.Text = "�߰��� ���� :  ( ; �� ���� )";
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(595, 26);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(106, 26);
            this.btnAddFile.TabIndex = 1;
            this.btnAddFile.Text = "�����߰�";
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // lbFile
            // 
            this.lbFile.ItemHeight = 12;
            this.lbFile.Location = new System.Drawing.Point(10, 26);
            this.lbFile.Name = "lbFile";
            this.lbFile.ScrollAlwaysVisible = true;
            this.lbFile.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFile.Size = new System.Drawing.Size(576, 172);
            this.lbFile.TabIndex = 0;
            this.lbFile.SelectedIndexChanged += new System.EventHandler(this.lbFile_SelectedIndexChanged);
            // 
            // butRemoveSelected
            // 
            this.butRemoveSelected.Location = new System.Drawing.Point(595, 60);
            this.butRemoveSelected.Name = "butRemoveSelected";
            this.butRemoveSelected.Size = new System.Drawing.Size(106, 26);
            this.butRemoveSelected.TabIndex = 1;
            this.butRemoveSelected.Text = "�������ϻ���";
            this.butRemoveSelected.Click += new System.EventHandler(this.butRemoveSelected_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(595, 95);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(106, 26);
            this.btnRemoveAll.TabIndex = 1;
            this.btnRemoveAll.Text = "��ü����";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(634, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 17);
            this.label5.TabIndex = 6;
            this.label5.Text = "���� : v1.1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSimulaton);
            this.groupBox2.Controls.Add(this.cbPreserveAtri);
            this.groupBox2.Controls.Add(this.cbPreserveDate);
            this.groupBox2.Controls.Add(this.tbDefault);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbDefault);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbChangeTo);
            this.groupBox2.Controls.Add(this.tbChangeTo);
            this.groupBox2.Location = new System.Drawing.Point(346, 284);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(384, 173);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "����";
            // 
            // cbSimulaton
            // 
            this.cbSimulaton.Location = new System.Drawing.Point(259, 138);
            this.cbSimulaton.Name = "cbSimulaton";
            this.cbSimulaton.Size = new System.Drawing.Size(106, 26);
            this.cbSimulaton.TabIndex = 5;
            this.cbSimulaton.Text = "�ùķ��̼�";
            // 
            // cbPreserveAtri
            // 
            this.cbPreserveAtri.Checked = true;
            this.cbPreserveAtri.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPreserveAtri.Location = new System.Drawing.Point(134, 138);
            this.cbPreserveAtri.Name = "cbPreserveAtri";
            this.cbPreserveAtri.Size = new System.Drawing.Size(125, 26);
            this.cbPreserveAtri.TabIndex = 4;
            this.cbPreserveAtri.Text = "���� �Ӽ� ����";
            // 
            // cbPreserveDate
            // 
            this.cbPreserveDate.Checked = true;
            this.cbPreserveDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPreserveDate.Location = new System.Drawing.Point(10, 138);
            this.cbPreserveDate.Name = "cbPreserveDate";
            this.cbPreserveDate.Size = new System.Drawing.Size(124, 26);
            this.cbPreserveDate.TabIndex = 3;
            this.cbPreserveDate.Text = "���� ��¥ ����";
            // 
            // tbDefault
            // 
            this.tbDefault.Location = new System.Drawing.Point(269, 52);
            this.tbDefault.Name = "tbDefault";
            this.tbDefault.Size = new System.Drawing.Size(105, 21);
            this.tbDefault.TabIndex = 2;
            this.tbDefault.TextChanged += new System.EventHandler(this.tbDefault_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "�����ڵ尡 �ƴҶ� �⺻ ���ڵ� : ";
            // 
            // cbDefault
            // 
            this.cbDefault.DropDownHeight = 300;
            this.cbDefault.IntegralHeight = false;
            this.cbDefault.Location = new System.Drawing.Point(134, 26);
            this.cbDefault.Name = "cbDefault";
            this.cbDefault.Size = new System.Drawing.Size(240, 20);
            this.cbDefault.TabIndex = 0;
            this.cbDefault.Text = "cbDefault";
            this.cbDefault.SelectedIndexChanged += new System.EventHandler(this.cbDefault_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "��ȯ�� ���ڵ� :";
            // 
            // cbChangeTo
            // 
            this.cbChangeTo.DropDownHeight = 300;
            this.cbChangeTo.IntegralHeight = false;
            this.cbChangeTo.Location = new System.Drawing.Point(134, 86);
            this.cbChangeTo.Name = "cbChangeTo";
            this.cbChangeTo.Size = new System.Drawing.Size(240, 20);
            this.cbChangeTo.TabIndex = 0;
            this.cbChangeTo.Text = "cbChangeTo";
            this.cbChangeTo.SelectedIndexChanged += new System.EventHandler(this.cbChangeTo_SelectedIndexChanged);
            // 
            // tbChangeTo
            // 
            this.tbChangeTo.Location = new System.Drawing.Point(269, 112);
            this.tbChangeTo.Name = "tbChangeTo";
            this.tbChangeTo.Size = new System.Drawing.Size(105, 21);
            this.tbChangeTo.TabIndex = 2;
            this.tbChangeTo.TextChanged += new System.EventHandler(this.tbChangeTo_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbPreview);
            this.groupBox3.Controls.Add(this.lblEncoding);
            this.groupBox3.Location = new System.Drawing.Point(10, 284);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 173);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "�̸�����";
            // 
            // tbPreview
            // 
            this.tbPreview.Location = new System.Drawing.Point(10, 17);
            this.tbPreview.Multiline = true;
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPreview.Size = new System.Drawing.Size(297, 129);
            this.tbPreview.TabIndex = 0;
            // 
            // lblEncoding
            // 
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(10, 146);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(81, 12);
            this.lblEncoding.TabIndex = 1;
            this.lblEncoding.Text = "���� ���ڵ� : ";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(624, 465);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(106, 26);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "����";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(10, 465);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(105, 26);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "��ȯ����";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(125, 465);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 26);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "��ȯ����";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ofdAddFile
            // 
            this.ofdAddFile.Filter = "�ؽ�Ʈ ����|*.txt";
            this.ofdAddFile.Multiselect = true;
            this.ofdAddFile.Title = "�����߰�";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(394, 474);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "���� : �ѽ���(numseal@gmail.com)";
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(743, 506);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbFileList);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "�ؽ�Ʈ ���ڵ� �ϰ� ��ȯ��";
            this.gbFileList.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

        private void btnAddFile_Click(object sender, System.EventArgs e)
        {
            //���̾�α� ����
            DialogResult result = ofdAddFile.ShowDialog();
            if (!(result == DialogResult.OK))
                return;

            //�߰�
            foreach (string filename in ofdAddFile.FileNames)
            {
                this.AddToList(filename);
                //TextEncoding.Get(filename);
            }


        }

		private void butRemoveSelected_Click(object sender, System.EventArgs e)
		{
			foreach(object item in lbFile.SelectedItems)
				lbFile.Items.Remove(item);
		}

		private void btnRemoveAll_Click(object sender, System.EventArgs e)
		{
			lbFile.Items.Clear();
		}

		private void btnAddFolder_Click(object sender, System.EventArgs e)
		{
			ArrayList paths = new ArrayList();
			string path; //�߰��� ���� ��ġ
			string[] searchPattens;

			DialogResult result = fbdAddFolder.ShowDialog();
			if(!(result == DialogResult.OK))
				return;

			path = fbdAddFolder.SelectedPath;

			//���� ���� ���
			if(this.cbWithSub.Checked)
			{
				paths = FileClass.GetSubFolders(path);
				paths.Insert(0, path);//2021.10.05
			}
			else
			{
				paths.Add(path);
			}

			//serchPattens �м�
			searchPattens = this.tbAddFolder.Text.Split(';');


			//���� �߰�
			for(int i=0;i<paths.Count;i++)
			{
				string tempPath = paths[i].ToString();
				foreach(string patten in searchPattens)
				{
					string[] files = Directory.GetFiles(tempPath,patten);
					foreach(string file in files)
						this.lbFile.Items.Add(file);
				}
			}
		}

		//�ߺ� ���θ� �˻��� �߰�
		private void AddToList(object item)
		{
			foreach(object obj in lbFile.Items)
			{
				if(obj.ToString() == item.ToString())
					return;
			}
			this.lbFile.Items.Add(item);
		}

		private void cbDefault_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.tbDefault.Text = Encoder.encodingCodePages[cbDefault.SelectedIndex].ToString();
		}

		private void cbChangeTo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.tbChangeTo.Text = Encoder.encodingCodePages[cbChangeTo.SelectedIndex].ToString();
		}
		private void tbDefault_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				int code = Convert.ToInt32(tbDefault.Text);
				ComboBox cb = this.cbDefault;

				try
				{
					TextEncoding.SetDefEncoding(System.Text.Encoding.GetEncoding(code));

					for(int i=0;i<Encoder.encodingCodePages.Length;i++)
					{
						if(Encoder.encodingCodePages[i] == code)
						{
							cb.SelectedIndex = i;
							cb.Text=cb.SelectedItem.ToString();
							return;
						}
					}
					cb.Text="Unknown";
				}
				catch(Exception)
				{
					cb.Text="Unusable";
				}
			}
			catch(Exception)
			{
			}
		}
		private void tbChangeTo_TextChanged(object sender, System.EventArgs e)
		{
			int code = Convert.ToInt32(tbChangeTo.Text);
			ComboBox cb = this.cbChangeTo;

			for(int i=0;i<Encoder.encodingCodePages.Length;i++)
			{
				if(Encoder.encodingCodePages[i] == code)
				{
					cb.SelectedIndex = i;
					cb.Text=cb.SelectedItem.ToString();
					return;
				}
			}
			cb.Text="Unknown";
		}

		private void lbFile_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				string path = lbFile.SelectedItem.ToString();
				this.tbPreview.Text = TextEncoding.ReadTextFile(path);

				System.Text.Encoding encoding =TextEncoding.Get(path);
				string currentEnco = Encoder.EncodingToString(encoding);
				this.lblEncoding.Text=String.Format("���� ���ڵ� : {0}", currentEnco);
			}
			catch(Exception)
			{
			}
		}

		private void btnStart_Click(object sender, System.EventArgs e)
		{
			btnStart.Enabled=false;
			btnCancel.Enabled=true;
			convertThread = new Thread(new ThreadStart(StartConvert));
			convertThread.Priority=ThreadPriority.Lowest;

			convertThread.Start();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			stopped = true;
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void StartConvert()
		{
			int succeed =0;
			int skiped = 0;
			int error = 0;


			DialogResult dr = MessageBox.Show("�ؽ�Ʈ ������ ���ڵ��� ��ȯ�մϴ�. \r\n����ϽÁٽ��ϱ�?",
				"���ڵ� ��ȯ", MessageBoxButtons.YesNo);
			if(!(dr == DialogResult.Yes))
				goto Final;

			string result ="";
			System.Text.Encoding encoding;
			try
			{
				encoding = System.Text.Encoding.GetEncoding(Convert.ToInt32(tbChangeTo.Text));
			}
			catch(Exception)
			{
				MessageBox.Show("��ȯ�� ���ڵ��� ������ �ֽ��ϴ�.");
				goto Final;
			}
			
			for(int i=0;i<lbFile.Items.Count;i++)
			{
				string path = lbFile.Items[i].ToString();
				if(stopped)
					result+=String.Format("{0} : ��� �Ǿ����ϴ�.\r\n",path);
				try
				{
                    //2022.12.28 - ���� ���ڵ��� �ٽ� ��ȯ UTF-8 BOM
                    bool pass = false;
					if(encoding == TextEncoding.Get(path))
					{
                        if (encoding == System.Text.Encoding.UTF8)
                        {
                            pass = false;
                        }
                        else
                        {
                            pass = true;
                            skiped++;
                        }
					}
					
                    if(pass==false)
					{
						string temp=Encoder.Convert(path,encoding
							,this.cbPreserveDate.Checked,this.cbPreserveAtri.Checked,this.cbSimulaton.Checked);
						if(temp !="")
						{
							result+=String.Format("{0} : {1}\r\n",path,temp);
							error++;
						}
						else
						{
							succeed++;
						}
					}
				}
				catch(Exception)
				{
					result+=String.Format("{0} : ��ȯ�� ������ �߻��Ͽ����ϴ�.\r\n",path);
					error++;
				}
				btnStart.Text=String.Format("[{0:N1}% �Ϸ�]",((double)i/(double)(lbFile.Items.Count))*100);
			}

			result = String.Format("�� ���� : {0}\r\n��ȯ�� ���� : {1}\r\n������ ���� : {2}\r\n\r\n== ���� �޽��� ==\r\n{3}",
				lbFile.Items.Count,succeed,error,result);

			frmResult frmresult = new frmResult();
			frmresult.tbResult.Text = result;
			frmresult.tbResult.Select(0,0);
			frmresult.ShowDialog();
			Final:;
			btnStart.Text="��ȯ����";
			btnStart.Enabled=true;
			btnCancel.Enabled=false;
			stopped = false;
		}

	}
}
