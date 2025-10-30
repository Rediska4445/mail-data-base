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
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.console = new System.Windows.Forms.RichTextBox();
            this.request = new System.Windows.Forms.RichTextBox();
            this.buttosList = new System.Windows.Forms.ListBox();
            this.right_side = new System.Windows.Forms.Panel();
            this.update = new System.Windows.Forms.Button();
            this.commandLine = new System.Windows.Forms.RichTextBox();
            this.remove = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.sqlDataAdapter1 = new Microsoft.Data.SqlClient.SqlDataAdapter();
            this.left_side.SuspendLayout();
            this.right_side.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // left_side
            // 
            this.left_side.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.left_side.Controls.Add(this.richTextBox2);
            this.left_side.Controls.Add(this.label2);
            this.left_side.Controls.Add(this.label1);
            this.left_side.Controls.Add(this.console);
            this.left_side.Controls.Add(this.request);
            this.left_side.Controls.Add(this.buttosList);
            this.left_side.Location = new System.Drawing.Point(12, 12);
            this.left_side.Name = "left_side";
            this.left_side.Size = new System.Drawing.Size(202, 637);
            this.left_side.TabIndex = 0;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.BackColor = System.Drawing.SystemColors.Menu;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Location = new System.Drawing.Point(3, 12);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(196, 24);
            this.richTextBox2.TabIndex = 7;
            this.richTextBox2.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 446);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Консоль";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 387);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Прямой запрос SQL";
            // 
            // console
            // 
            this.console.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.console.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.console.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.console.ForeColor = System.Drawing.SystemColors.InfoText;
            this.console.Location = new System.Drawing.Point(3, 462);
            this.console.Name = "console";
            this.console.Size = new System.Drawing.Size(196, 169);
            this.console.TabIndex = 7;
            this.console.Text = "";
            // 
            // request
            // 
            this.request.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.request.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.request.EnableAutoDragDrop = true;
            this.request.Location = new System.Drawing.Point(3, 403);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(196, 34);
            this.request.TabIndex = 3;
            this.request.Text = "";
            this.request.TextChanged += new System.EventHandler(this.request_TextChanged);
            this.request.KeyUp += new System.Windows.Forms.KeyEventHandler(this.request_KeyUp);
            // 
            // buttosList
            // 
            this.buttosList.BackColor = System.Drawing.SystemColors.Control;
            this.buttosList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.buttosList.FormattingEnabled = true;
            this.buttosList.Items.AddRange(new object[] {
            "main",
            "printing_house",
            "newspaper"});
            this.buttosList.Location = new System.Drawing.Point(3, 42);
            this.buttosList.Name = "buttosList";
            this.buttosList.Size = new System.Drawing.Size(196, 338);
            this.buttosList.TabIndex = 1;
            this.buttosList.SelectedIndexChanged += new System.EventHandler(this.buttosList_SelectedIndexChanged);
            // 
            // right_side
            // 
            this.right_side.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.right_side.Controls.Add(this.update);
            this.right_side.Controls.Add(this.commandLine);
            this.right_side.Controls.Add(this.remove);
            this.right_side.Controls.Add(this.save);
            this.right_side.Controls.Add(this.dataGridView1);
            this.right_side.Location = new System.Drawing.Point(220, 12);
            this.right_side.Name = "right_side";
            this.right_side.Size = new System.Drawing.Size(654, 637);
            this.right_side.TabIndex = 1;
            // 
            // update
            // 
            this.update.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.update.Location = new System.Drawing.Point(13, 520);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(629, 33);
            this.update.TabIndex = 5;
            this.update.Text = "Обновить";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // commandLine
            // 
            this.commandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandLine.BackColor = System.Drawing.SystemColors.Menu;
            this.commandLine.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commandLine.Location = new System.Drawing.Point(13, 12);
            this.commandLine.Name = "commandLine";
            this.commandLine.Size = new System.Drawing.Size(629, 24);
            this.commandLine.TabIndex = 4;
            this.commandLine.Text = "";
            this.commandLine.TextChanged += new System.EventHandler(this.commandLine_TextChanged);
            // 
            // remove
            // 
            this.remove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.remove.Location = new System.Drawing.Point(13, 559);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(629, 33);
            this.remove.TabIndex = 2;
            this.remove.Text = "Удалить";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(13, 598);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(629, 33);
            this.save.TabIndex = 1;
            this.save.Text = "Добавить";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonShadow;
            this.dataGridView1.Location = new System.Drawing.Point(13, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(629, 472);
            this.dataGridView1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(886, 661);
            this.Controls.Add(this.right_side);
            this.Controls.Add(this.left_side);
            this.Name = "Form1";
            this.Text = "Form1";
            this.left_side.ResumeLayout(false);
            this.left_side.PerformLayout();
            this.right_side.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel left_side;
        private System.Windows.Forms.Panel right_side;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListBox buttosList;
        private System.Windows.Forms.RichTextBox request;
        private System.Windows.Forms.RichTextBox commandLine;
        private System.Windows.Forms.Button remove;
        private System.Windows.Forms.Button save;
        private Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter1;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}

