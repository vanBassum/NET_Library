using CoreLib.Ethernet;
using FormsLib.Extentions;
using System;


namespace FormsLib.Controls
{
    public partial class TCPConnectionControl : UserControl
    {
        private TcpSocketClient? _DataSource;
        public TcpSocketClient? DataSource
        {
            get => _DataSource;
            set
            {
                _DataSource = value;
                Setup(value);
            }
        }

        private const string cancelButtonText = "Cancel";
        private const string connectedButtonText = "Disconnect";
        private const string disconnectedButtonText = "Connect";

        private CancellationTokenSource? cancellationTokenSource;
        Task? connectTask;

        public TCPConnectionControl()
        {
            InitializeComponent();
            btn_Connect.Click += Btn_Connect_Click;
            btn_Connect.Text = disconnectedButtonText;

        }

        void Setup(TcpSocketClient socket)
        {
            if (socket == null)
                return;
            socket.OnConnectionStateChanged += (sender, e) => btn_Connect.InvokeIfRequired(() => SetButtonText(e));
            SetButtonText(socket.ConnectionState);
        }


        void SetButtonText(ConnectionStates e)
        {
            switch (e)
            {
                case ConnectionStates.Connected:
                    btn_Connect.Text = connectedButtonText;
                    break;
                case ConnectionStates.Disconnected:
                    btn_Connect.Text = disconnectedButtonText;
                    break;
                case ConnectionStates.Connecting:
                    btn_Connect.Text = cancelButtonText;
                    break;
            }
        }


        private async void Btn_Connect_Click(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                int port = -1;
                string host = textBox1.Text;
                if (int.TryParse(textBox2.Text, out port))
                    textBox2.BackColor = Color.White;
                else
                {
                    textBox2.BackColor = Color.PaleVioletRed;
                    return;
                }

                switch (DataSource?.ConnectionState)
                {
                    case ConnectionStates.Connected:
                        DataSource.Disconnect();
                        break;
                    case ConnectionStates.Disconnected:
                        cancellationTokenSource = new CancellationTokenSource();
                        connectTask = Task.Run(async () => await DataSource.ConnectAsync(host, port, cancellationTokenSource.Token));
                        break;
                    case ConnectionStates.Connecting:
                        button.Enabled = false;
                        cancellationTokenSource?.Cancel();
                        if (connectTask != null) await connectTask;
                        button.Enabled = true;
                        break;
                }
            }
        }

        private Button btn_Connect;
        private TextBox textBox2;
        private TextBox textBox1;

        private void InitializeComponent()
        {
            this.btn_Connect = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(164, 3);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(75, 23);
            this.btn_Connect.TabIndex = 5;
            this.btn_Connect.Text = "button1";
            this.btn_Connect.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(109, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(49, 23);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "1001";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 23);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "192.168.35.167";
            // 
            // TCPConnectionControl
            // 
            this.Controls.Add(this.btn_Connect);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "TCPConnectionControl";
            this.Size = new System.Drawing.Size(242, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
