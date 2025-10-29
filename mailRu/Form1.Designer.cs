namespace mailRu
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.left_side = new System.Windows.Forms.Panel();
            this.right_side = new System.Windows.Forms.Panel();
            this.console = new System.Windows.Forms.RichTextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttosList = new System.Windows.Forms.ListBox();
            this.request = new System.Windows.Forms.RichTextBox();
            this.save = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.commandLine = new System.Windows.Forms.RichTextBox();
            this.sqlDataAdapter1 = new Microsoft.Data.SqlClient.SqlDataAdapter();
            this.left_side.SuspendLayout();
            this.right_side.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // left_side
            // 
            this.left_side.Controls.Add(this.buttosList);
            this.left_side.Location = new System.Drawing.Point(12, 12);
            this.left_side.Name = "left_side";
            this.left_side.Size = new System.Drawing.Size(202, 310);
            this.left_side.TabIndex = 0;
            // 
            // right_side
            // 
            this.right_side.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.right_side.Controls.Add(this.commandLine);
            this.right_side.Controls.Add(this.close);
            this.right_side.Controls.Add(this.save);
            this.right_side.Controls.Add(this.dataGridView1);
            this.right_side.Location = new System.Drawing.Point(220, 12);
            this.right_side.Name = "right_side";
            this.right_side.Size = new System.Drawing.Size(589, 424);
            this.right_side.TabIndex = 1;
            // 
            // console
            // 
            this.console.Location = new System.Drawing.Point(12, 328);
            this.console.Name = "console";
            this.console.Size = new System.Drawing.Size(202, 110);
            this.console.TabIndex = 2;
            this.console.Text = "";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(583, 351);
            this.dataGridView1.TabIndex = 0;
            // 
            // buttosList
            // 
            this.buttosList.FormattingEnabled = true;
            this.buttosList.Items.AddRange(new object[] {
            "Mail",
            "Print House",
            "Newspapers"});
            this.buttosList.Location = new System.Drawing.Point(3, 3);
            this.buttosList.Name = "buttosList";
            this.buttosList.Size = new System.Drawing.Size(196, 264);
            this.buttosList.TabIndex = 1;
            this.buttosList.SelectedIndexChanged += new System.EventHandler(this.buttosList_SelectedIndexChanged);
            // 
            // request
            // 
            this.request.Location = new System.Drawing.Point(15, 285);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(196, 34);
            this.request.TabIndex = 3;
            this.request.Text = "";
            // 
            // save
            // 
            this.save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.save.Location = new System.Drawing.Point(13, 388);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(280, 33);
            this.save.TabIndex = 1;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // close
            // 
            this.close.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.close.Location = new System.Drawing.Point(299, 388);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(276, 33);
            this.close.TabIndex = 2;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // commandLine
            // 
            this.commandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandLine.Location = new System.Drawing.Point(3, 3);
            this.commandLine.Name = "commandLine";
            this.commandLine.Size = new System.Drawing.Size(583, 24);
            this.commandLine.TabIndex = 4;
            this.commandLine.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 448);
            this.Controls.Add(this.request);
            this.Controls.Add(this.console);
            this.Controls.Add(this.right_side);
            this.Controls.Add(this.left_side);
            this.Name = "Form1";
            this.Text = "Form1";
            this.left_side.ResumeLayout(false);
            this.right_side.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel left_side;
        private System.Windows.Forms.Panel right_side;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListBox buttosList;
        private System.Windows.Forms.RichTextBox request;
        private System.Windows.Forms.RichTextBox commandLine;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button save;
        private Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter1;
    }
}

