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
using System.Xml;

namespace Blending
{

   
    public partial class Form1 : Form
    {
        public ImageList il = new ImageList();
        public ListView list
        {
            get
            {
                return listView1;
            }
            set
            {
                listView1 = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true; listView1.Clear();
            
            listView1.View = View.SmallIcon;
           
            XmlDocument doc = new XmlDocument();
            doc.Load("imgLibrary.xml");
            il.ImageSize = new Size(150,150);
            listView1.SmallImageList = il;
            int j = 0;
           foreach(XmlNode node in doc.DocumentElement.ChildNodes)
            {
                

                String path = node.Attributes["path"].InnerText;
                if (!File.Exists(path))
                {
                    node.ParentNode.RemoveChild(node);
                    doc.Save("imgLibrary.xml");
                    continue;
                }
                il.Images.Add(Image.FromFile(path));
                
                ListViewItem item = new ListViewItem();

                item.ImageIndex = j;
                this.listView1.Items.Add(item);
                item.Name = path;
                
                j++;
            }
           

        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in listView1.Items)
            {
                if(item.Selected== true)
                {
                    if(sender as PictureBox == pictureBox1)
                    pictureBox1.Image=il.Images[item.ImageIndex];
                    else
                    pictureBox2.Image = il.Images[item.ImageIndex];

                    if (pictureBox1.Image != null && pictureBox2.Image != null)
                        button1.Enabled = true;

                    return;
                }

            }

            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "image files | *.bmp;*.jpg;*.png ";
            string filename = null;
            if (od.ShowDialog() == DialogResult.OK)
            {
                filename = od.FileName;
            }
            if (filename != null)
                if (sender as PictureBox == pictureBox1)
                {
                    try
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Image = Image.FromFile(filename);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        MessageBox.Show("WrongFile", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    try
                    {

                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox2.Image = Image.FromFile(filename);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        MessageBox.Show("WrongFile", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            if (pictureBox1.Image != null && pictureBox2.Image != null)
                button1.Enabled = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                Bitmap memoryImage;
                memoryImage = new Bitmap(resolution.Width, resolution.Height);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

                if (pictureBox1.Image == null)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = memoryImage;
                }
                else
                {

                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox2.Image = memoryImage;
                }

                if (pictureBox1.Image != null && pictureBox2.Image != null)
                    button1.Enabled = true;
            }
        }
        //http://www.mini.pw.edu.pl/~swiechowskim/Zadanie4A_lab.html

        BackgroundWorker[] bw=new BackgroundWorker [2];
        bool [] busy = new bool[2];
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Visible = true;
            if (!button1.Enabled) return;
            int ind=0;
            if(busy[0]==false)
            {
               busy[0] = true;
                ind = 0;
            }
            else
            {
                busy[1] = true;
                ind = 1;
            }
            double alfa = (double)trackBar1.Value / 10;
            if (busy[0] == true && busy[1] == true)
            {
                button1.Enabled = false;
            }
            bw[ind] = new BackgroundWorker();
            bw[ind].WorkerReportsProgress = true;
            bw[ind].DoWork += new DoWorkEventHandler(bw_DoWork);
            bw[ind].RunWorkerCompleted += new RunWorkerCompletedEventHandler(runWorkCompleted);
            bw[ind].ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            bw[ind].RunWorkerAsync(alfa);

        }
        //Bitmap result;
        Color addColors(Color a, Color b,double alfa)
        {
            //return Color.FromArgb(255,255,255);

            return Color.FromArgb((int)(alfa*a.R+(1-alfa)*b.R), (int)(alfa * a.G + (1 - alfa) * b.G), (int)(alfa * a.B + (1 - alfa) * b.B));
           
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap im1 = new Bitmap(pictureBox1.Image);
            Bitmap im2 = new Bitmap(pictureBox2.Image);
            Bitmap result = new Bitmap(Math.Max(im1.Width, im2.Width), Math.Max(im1.Height, im2.Height));
            BackgroundWorker bw = sender as BackgroundWorker;
            object alfa =(e.Argument as object);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    
                    result.SetPixel(x, y, addColors(im1.GetPixel(x, y), im2.GetPixel(x, y),(double)alfa)); //addColors(im1.GetPixel(x, y), im2.GetPixel(x, y))
                }
                
                bw.ReportProgress((int)((double)(x)/(double)result.Width*100));
            }

            e.Result = result as object;
        }
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (sender as BackgroundWorker == bw[0])
            {
                progressBar1.Visible = true;
                progressBar1.Value = e.ProgressPercentage;
            }
            else
            {
                progressBar2.Visible = true;
                progressBar2.Value = e.ProgressPercentage;
            }


        }
        
        void runWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender as BackgroundWorker == bw[0])
            {
                progressBar1.Visible = false;
                busy[0] = false;
            }
            else
            {
                progressBar2.Visible = false;
                busy[1] = false;
            }
            if (busy[0] == false && busy[1] == false)
            {
                label2.Visible = false;
            }
            if (!(busy[0] == true && busy[1] == true))
                button1.Enabled = true;

            Form2 newForm = new Form2();

            newForm.ThePicture.Image = e.Result as Bitmap;
            newForm.Show(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        ListViewItem last; //= new ListViewItem();
        

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListViewItem lv = listView1.FocusedItem as ListViewItem;
                if (lv != null)
                {

                    // il.Images[item.ImageIndex];
                    XmlDocument doc = new XmlDocument();
                    doc.Load("imgLibrary.xml");
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        String path = node.Attributes["path"].InnerText;
                        if (path == lv.Name)
                        node.ParentNode.RemoveChild(node);
                        doc.Save("imgLibrary.xml");
                    }


                }
                

                lv.Remove();
            }
            
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            listView1.BeginUpdate();
            foreach (string file in files)
            {
                bool contains = false;
                foreach(ListViewItem lv in listView1.Items)
                {
                    if (lv.Name == file)
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains) continue;
                il.Images.Add(Image.FromFile(file));
                ListViewItem item = new ListViewItem();
           
                item.ImageIndex = il.Images.Count-1;
                this.listView1.Items.Add(item);
                item.Name = file;

                
                XmlDocument doc = new XmlDocument();
                doc.Load("imgLibrary.xml");
                XmlElement el = doc.CreateElement("Image");
                el.SetAttribute("path",file);

                doc.DocumentElement.AppendChild(el);
                doc.Save("imgLibrary.xml");


            }
            listView1.EndUpdate();

        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

       
        
    }
}
