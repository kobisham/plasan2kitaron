using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;

namespace Plasan2Kitaron
{
    public partial class Form1 : Form
    {
        string _fileNameRead = null;
        string _fileNameWrite = null;
        string _PO = null;
        DataSet dsData=null;
        DataSet dsHeader =null;
        DialogResult _dialogResult;


        public Form1()
        {
            InitializeComponent();
          

        }

        private void showDialogForChoices()
        {
            bigMessageBox bigMessage = new bigMessageBox();
            _dialogResult = bigMessage.ShowDialog();
            
        }

        private void LoadDataToDs()
        {
            dsData = ReadExcelFile(16);
            dsHeader = ReadExcelFile(1);
            dataGridView1.DataSource = dsData.Tables[0];
            txtPO.Text = dsHeader.Tables[0].Rows[0][6].ToString().Trim();
            txtOrderDate.Text = dsHeader.Tables[0].Rows[1][6].ToString().Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //showDialogForChoices();


            

        }

        private DataSet ReadExcelFile(int firstRow)
        {
            DataSet ds = new DataSet();

            string connectionString = GetConnectionStringRead();


            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    string sheetName = dr["TABLE_NAME"].ToString();

                    if (!sheetName.EndsWith("$"))
                        continue;

                    // Get all rows from the Sheet
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "A" + firstRow.ToString() + ":z]";

                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;


                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    ds.Tables.Add(dt);
                }


                cmd = null;
                conn.Close();
            }

            return ds;
        }

        private string GetConnectionStringRead()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            //// XLSX - Excel 2007, 2010, 2012, 2013
            //props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            //props["Extended Properties"] = "Excel 12.0";
            //props["Data Source"] = @"D:\practice\Palsan2Kitaron\newPO_4057406_STANDARD.xls";

            // XLS - Excel 2003 and Older
            props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            props["Extended Properties"] = "Excel 8.0;";
            props["Data Source"] = _fileNameRead;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private string GetConnectionStringWrite ()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            _fileNameWrite = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + _PO + "_KITARON" + ".xls";
            if (File.Exists(_fileNameWrite))
            {
                File.Delete(_fileNameWrite);
            }

            //// XLSX - Excel 2007, 2010, 2012, 2013
            //props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            //props["Extended Properties"] = "Excel 12.0";
            //props["Data Source"] = @"D:\practice\Palsan2Kitaron\newPO_4057406_STANDARD.xls";

            // XLS - Excel 2003 and Older
            props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            props["Extended Properties"] = "Excel 8.0";
            props["Data Source"] = _fileNameWrite;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private void PrepareFileRead(object sender, DragEventArgs e)
        {
            string[] fileNames = null;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                {
                    fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                    // handle each file passed as needed
                    foreach (string fileName in fileNames)
                    {
                        lblDroppedFileName.Text = fileName;
                        _fileNameRead = fileName;
                    }
                }
                else if (e.Data.GetDataPresent("FileGroupDescriptor"))
                {

                    Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor");
                    byte[] fileGroupDescriptor = new byte[512];
                    theStream.Read(fileGroupDescriptor, 0, 512);
                    // used to build the filename from the FileGroupDescriptor block
                    StringBuilder fileName = new StringBuilder("");
                    // this trick gets the filename of the passed attached file
                    for (int i = 76; fileGroupDescriptor[i] != 0; i++)
                    { fileName.Append(Convert.ToChar(fileGroupDescriptor[i])); }
                    theStream.Close();
                    string path = Path.GetTempPath();
                    // put the zip file into the temp directory
                    string theFile = path + fileName.ToString();
                    // create the full-path name


                    MemoryStream ms = (MemoryStream)e.Data.GetData(
                        "FileContents", true);
                    // allocate enough bytes to hold the raw data
                    byte[] fileBytes = new byte[ms.Length];
                    // set starting position at first byte and read in the raw data
                    ms.Position = 0;
                    ms.Read(fileBytes, 0, (int)ms.Length);
                    // create a file and save the raw zip file to it
                    FileStream fs = new FileStream(theFile, FileMode.Create);
                    fs.Write(fileBytes, 0, (int)fileBytes.Length);

                    fs.Close();  // close the file

                    FileInfo tempFile = new FileInfo(theFile);

                    // always good to make sure we actually created the file
                    if (tempFile.Exists == true)
                    {
                        lblDroppedFileName.Text = tempFile.Name;
                        _fileNameRead = tempFile.FullName;
                        // for now, just delete what we created
                        //tempFile.Delete();
                    }
                    else
                    { Trace.WriteLine("File was not created!"); }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);

                // don't use MessageBox here - Outlook or Explorer is waiting !
            }
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {


            PrepareFileRead(sender,e);
            LoadDataToDs();
            showDialogForChoices();
            switch (_dialogResult)
            {

                case DialogResult.OK:
                    WriteExcelFileSerial();
                    break;


                case DialogResult.Retry:
                    WriteExcelFileDigum();
                    break;


                default:
                    MessageBox.Show("לא נבחר סוג קובץ");
                    ClearForm();
                    return;
                    

            }

            if (File.Exists(_fileNameWrite))
            {
                Process.Start("explorer.exe", _fileNameWrite);
            }

        }

        private void ClearForm()
        {
            lblDroppedFileName.Text = "";
            txtOrderDate.Text = "";
            txtPO.Text = "";
            dataGridView1.DataSource = null;
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // for this program, we allow a file to be dropped from Explorer
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            { e.Effect = DragDropEffects.Copy; }
            //    or this tells us if it is an Outlook attachment drop
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            { e.Effect = DragDropEffects.Copy; }
            //    or none of the above
            else
            { e.Effect = DragDropEffects.None; }
        }

        private void WriteExcelFileSerial()
        {

            _PO = dsHeader.Tables[0].Rows[0][6].ToString().Trim();
            string _orderDate = dsHeader.Tables[0].Rows[1][6].ToString().Trim();

            string connectionString = GetConnectionStringWrite();
            int i = 1;
            


            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                string _item, _rev, _item_descr, _need_date, _qty, _price, _total="";

                int _qtyInt;
                double _priceDouble;
              

                cmd.CommandText = "CREATE TABLE [table1] (Row_num Long, Item VARCHAR,rev VARCHAR,Item_Descr VARCHAR ,need_date VARCHAR , qty Long, price Decimal, total Decimal, PO VARCHAR, orderDate VARCHAR);";
                cmd.ExecuteNonQuery();


     

                // insert main data
                for (int index = 0; index < dsData.Tables[0].Rows.Count-1; index++)
                {
                    _item = dsData.Tables[0].Rows[index][3].ToString();
                    _item = _item.Substring(0, _item.IndexOf(" REV"));

                    _rev = dsData.Tables[0].Rows[index][3].ToString();
                    _rev = _rev.Substring(_rev.IndexOf(" REV")+5);

                    _item_descr= dsData.Tables[0].Rows[index][4].ToString();
                    _item_descr = _item_descr.Substring(_item_descr.IndexOf(" ") + 3);

                    _need_date = dsData.Tables[0].Rows[index][11].ToString();

                    _qty = dsData.Tables[0].Rows[index][7].ToString();

                    _price = dsData.Tables[0].Rows[index][9].ToString();



                    if (Int32.TryParse(_qty, out _qtyInt) && 
                        Double.TryParse(_price, out _priceDouble))
                    {
                        _total = (_qtyInt * _priceDouble).ToString();
                    }

                    

                    cmd.CommandText = "INSERT INTO [table1] " +
                        "VALUES(" +
                        i.ToString() +", '" +
                        _item + "','" +
                        _rev +  "','" +
                        _item_descr + "','" +
                        _need_date + "'," +
                        _qty + "," +
                        _price + "," +
                        _total + ",'" + 
                        _PO + "','" +
                        _orderDate + "');";
                    cmd.ExecuteNonQuery();
                    i++;

                }

                //// Insert empty line
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,null,null,null,null,null,null,null);";
                //cmd.ExecuteNonQuery();

                //// Insert PO
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,'PO','"+ _PO + "',null,null,null,null,null);";
                //cmd.ExecuteNonQuery();

                //// Insert order date
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,'Order date','"+ _orderDate +"',null,null,null,null,null);";
                //cmd.ExecuteNonQuery();


                //cmd.CommandText = "INSERT INTO [table1](id,name,datecol) VALUES(1,'AAAA','2014-01-01');";
                //cmd.ExecuteNonQuery();





                conn.Close();
            }
        }

        private void WriteExcelFileDigum()
        {

            _PO = dsHeader.Tables[0].Rows[0][6].ToString().Trim();
            string _orderDate = dsHeader.Tables[0].Rows[1][6].ToString().Trim();

            string connectionString = GetConnectionStringWrite();
            int i = 1;



            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                string _item, _rev, _item_descr, _need_date, _qty, _price, _total = "";

                int _qtyInt;
                double _priceDouble;


                cmd.CommandText = "CREATE TABLE [table1] (Row_num Long, Item VARCHAR,rev VARCHAR,Item_Descr VARCHAR ,need_date VARCHAR , qty Long, price Decimal, total Decimal, PO VARCHAR, orderDate VARCHAR);";
                cmd.ExecuteNonQuery();




                // insert main data
                for (int index = 0; index < dsData.Tables[0].Rows.Count - 1; index++)
                {


                    _rev = dsData.Tables[0].Rows[index][3].ToString();
                    _rev = _rev.Substring(_rev.IndexOf(" REV") + 5);

                    _item = dsData.Tables[0].Rows[index][4].ToString();
                    _item = _item.Substring(0, _item.IndexOf(" "));

                    _item_descr = dsData.Tables[0].Rows[index][4].ToString();
                    _item_descr = _item_descr.Substring(_item_descr.IndexOf(" ") + 1).Trim();

                    _need_date = dsData.Tables[0].Rows[index][11].ToString();

                    _qty = dsData.Tables[0].Rows[index][7].ToString();

                    _price = dsData.Tables[0].Rows[index][9].ToString();



                    if (Int32.TryParse(_qty, out _qtyInt) &&
                        Double.TryParse(_price, out _priceDouble))
                    {
                        _total = (_qtyInt * _priceDouble).ToString();
                    }



                    cmd.CommandText = "INSERT INTO [table1] " +
                        "VALUES(" +
                        i.ToString() + ", '" +
                        _item + "','" +
                        _rev + "','" +
                        _item_descr + "','" +
                        _need_date + "'," +
                        _qty + "," +
                        _price + "," +
                        _total + ",'" +
                        _PO + "','" +
                        _orderDate + "');";
                    cmd.ExecuteNonQuery();
                    i++;

                }

                //// Insert empty line
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,null,null,null,null,null,null,null);";
                //cmd.ExecuteNonQuery();

                //// Insert PO
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,'PO','"+ _PO + "',null,null,null,null,null);";
                //cmd.ExecuteNonQuery();

                //// Insert order date
                //cmd.CommandText = "INSERT INTO [table1] VALUES(null,'Order date','"+ _orderDate +"',null,null,null,null,null);";
                //cmd.ExecuteNonQuery();


                //cmd.CommandText = "INSERT INTO [table1](id,name,datecol) VALUES(1,'AAAA','2014-01-01');";
                //cmd.ExecuteNonQuery();





                conn.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}

        

  
