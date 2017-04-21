using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml;
namespace Blending
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public PictureBox ThePicture
        {
            get { return this.pictureBox1; }
        }

        static int imagenumber = 0;
        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            StringBuilder sb = new StringBuilder();
            sb.Append("image");
            
            sb.Append(imagenumber);
            imagenumber++;
            dialog.FileName = sb.ToString();
            dialog.Filter = "Images (*.bmp)|*.bmp|All files (*.*)|*.*";

            dialog.DefaultExt = "bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                Bitmap bmp = new Bitmap(pictureBox1.Image);
                bmp.Save(dialog.FileName, ImageFormat.Bmp);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void addToLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("image");

            sb.Append(imagenumber+".bmp");
            imagenumber++;
            String file = sb.ToString();
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            bmp.Save(file, ImageFormat.Bmp);

            Form1 form = this.Owner as Form1;
            form.list.BeginUpdate();
           
                form.il.Images.Add(Image.FromFile(file));
                ListViewItem item = new ListViewItem();

                item.ImageIndex = form.il.Images.Count - 1;
                form.list.Items.Add(item);
                item.Name = file;


                XmlDocument doc = new XmlDocument();
                doc.Load("imgLibrary.xml");
                XmlElement el = doc.CreateElement("Image");
                el.SetAttribute("path", file);

                doc.DocumentElement.AppendChild(el);
                doc.Save("imgLibrary.xml");


            
            form.list.EndUpdate();
        }
    }
}
