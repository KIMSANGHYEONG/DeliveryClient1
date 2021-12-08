using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;



namespace DeliveryClient1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Thread socketworker = new Thread(new ThreadStart(socketThread));
            socketworker.Start();
        }

        private void socketThread()
        {
            // 1. 데이타패킷 조합
            
            DataPacket packet = new DataPacket();

            packet.Id = textBox1.Text;
            packet.Phone = textBox2.Text;
            packet.Address = textBox3.Text;
            packet.Memo = textBox4.Text;


            string menu = null;
            if (checkedListBox1.CheckedItems.Count != 0)
            {

                for (int x = 0; x < checkedListBox1.Items.Count; x++)
                {
                    if (checkedListBox1.GetItemChecked(x))
                    {
                        menu = menu + " " + checkedListBox1.Items[x].ToString();
                    }
                }
            }
            packet.Menu = menu;
                        
            // 연결
            TcpClient client = new TcpClient(("127.0.0.1"), Convert.ToInt32("8888"));
            NetworkStream stream = client.GetStream();
            updateStatusInfo("Connected");

            // 전송
            byte[] buffer = GetBytes_Bind(packet);

            stream.Write(buffer, 0, buffer.Length);
            updateStatusInfo("주문 완료!");

            
            stream.Close();
            client.Close();

            // listview 에 추가하기
            Invoke((MethodInvoker)delegate
            {
                ListViewItem i = new ListViewItem();

                i.Text = textBox1.Text;
                i.SubItems.Add(textBox2.Text);
                i.SubItems.Add(textBox3.Text);
                i.SubItems.Add(packet.Menu);
                i.SubItems.Add(textBox4.Text);
                i.SubItems.Add(DateTime.Now.ToString("MM-dd HH:mm:ss"));

                listView1.Items.Add(i);
            });
        }

        private void updateStatusInfo(string content)
        {
            Action del = delegate ()
            {
                toolStripStatusLabel1.Text = content;
            };
            Invoke(del);
        }
        // 패킷 사이즈
        private const int BODY_BIND_SIZE = 20 + 20 + 20 + 100 + 100;

        // 인증 패킷 구조체를 바이트 배열로 변환하는 함수
        private byte[] GetBytes_Bind(DataPacket packet)
        {
            byte[] btBuffer = new byte[BODY_BIND_SIZE];

            MemoryStream ms = new MemoryStream(btBuffer, true);
            BinaryWriter bw = new BinaryWriter(ms);

            
            try
            {
                byte[] btName = new byte[20];
                Encoding.UTF8.GetBytes(packet.Id, 0, packet.Id.Length, btName, 0);
                bw.Write(btName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            
            try
            {
                byte[] btName = new byte[20];
                Encoding.UTF8.GetBytes(packet.Phone, 0, packet.Phone.Length, btName, 0);
                bw.Write(btName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            try
            {
                byte[] btName = new byte[20];
                Encoding.UTF8.GetBytes(packet.Address, 0, packet.Address.Length, btName, 0);
                bw.Write(btName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            try
            {
                byte[] btName = new byte[100];
                Encoding.UTF8.GetBytes(packet.Menu, 0, packet.Menu.Length, btName, 0);
                bw.Write(btName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            try
            {
                byte[] btName = new byte[100];
                Encoding.UTF8.GetBytes(packet.Memo, 0, packet.Memo.Length, btName, 0);
                bw.Write(btName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            bw.Close();
            ms.Close();

            return btBuffer;
        }



    }
}
