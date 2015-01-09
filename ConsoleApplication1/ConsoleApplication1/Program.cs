using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;
using System.Data;

namespace ConsoleApplication1
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            string _username = Console.ReadLine();
            Console.Write("Password: ");
            string _password = Console.ReadLine();


            
            SAPSystemConnect sapcfg = new SAPSystemConnect();
            sapcfg.ApplicationServer = "rb3pa786.server.bosch.com";
            sapcfg.Username = _username;
            sapcfg.Password = _password;

            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapcfg);
                RfcDestination rfcDest = RfcDestinationManager.GetDestination("P78");
                RfcRepository repo = rfcDest.Repository;
                Console.WriteLine("Connected!");

                IRfcFunction COPAQuery = repo.CreateFunction("BAPI_COPAQUERY_GETCOST_ACTDATA");
                COPAQuery.SetValue("OPERATINGCONCERN", "8290");
                COPAQuery.SetValue("CURRENCYTYPE", "B0");
                COPAQuery.SetValue("MAXRECORDS", "99999999");

                IRfcTable SELECTION = COPAQuery.GetTable("SELECTION");

                SELECTION.Append();
                SELECTION.SetValue("FIELDNAME", "PERIO");
                SELECTION.SetValue("SIGN", "I");
                SELECTION.SetValue("OPTION", "BT");
                SELECTION.SetValue("LOW", "2015001");
                SELECTION.SetValue("HIGH", "2015001");

                SELECTION.Append();
                SELECTION.SetValue("FIELDNAME", "VRGAR");
                SELECTION.SetValue("SIGN", "I");
                SELECTION.SetValue("OPTION", "EQ");
                SELECTION.SetValue("LOW", "F");
                SELECTION.SetValue("HIGH", "F");

                SELECTION.Append();
                SELECTION.SetValue("FIELDNAME", "VRGAR");
                SELECTION.SetValue("SIGN", "I");
                SELECTION.SetValue("OPTION", "EQ");
                SELECTION.SetValue("LOW", "F");
                SELECTION.SetValue("HIGH", "F");

                IRfcTable SELECTEDFIELDS = COPAQuery.GetTable("SELECTEDFIELDS");

                SELECTEDFIELDS.Append();
                SELECTEDFIELDS.SetValue("FIELDNAME", "BELNR");
                SELECTEDFIELDS.Append();
                SELECTEDFIELDS.SetValue("FIELDNAME", "PERIO");
                SELECTEDFIELDS.Append();
                SELECTEDFIELDS.SetValue("FIELDNAME", "PAPH3");

                IRfcTable RESULTDATA = COPAQuery.GetTable("RESULTDATA");
                IRfcTable RETURN = COPAQuery.GetTable("RETURN");

                COPAQuery.Invoke(rfcDest);

                DataTable rstData = new DataTable();

                if (RESULTDATA.RowCount > 0)
                {
                    for (int tempFname = 0; tempFname < SELECTEDFIELDS.RowCount; tempFname++)
                    {
                        SELECTEDFIELDS.CurrentIndex = tempFname;
                        rstData.Columns.Add(SELECTEDFIELDS.GetString("FIELDNAME"), typeof(string));
                    }
                }

                string kkk = "";

                for (int DATAPTR = 1; DATAPTR < RESULTDATA.RowCount; DATAPTR++)
                {
                    string[] ttt = new string[SELECTEDFIELDS.RowCount];
                    for (int DATAPTR2 = 0; DATAPTR2 < SELECTEDFIELDS.RowCount; DATAPTR2++)
                    {
                        RESULTDATA.CurrentIndex = DATAPTR - 1;
                        kkk = RESULTDATA.GetString("VALUE").Trim().Replace(".", ",");
                        ttt[DATAPTR2] = ((kkk.Substring(kkk.Length - 1) == "-" ? "-" + kkk.Replace("-", "") : kkk));
                        if ((DATAPTR % (SELECTEDFIELDS.RowCount) == 0))
                        {
                            break;
                        }
                        DATAPTR++;
                    }
                    rstData.Rows.Add(ttt);
                    ttt = null;
                }

                rstData.DefaultView.Sort = "BELNR";

                foreach (DataRow row in rstData.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.WriteLine(item);
                    }
                }

                Console.WriteLine("Read COPA - " + (RESULTDATA.RowCount / SELECTEDFIELDS.RowCount).ToString() + " row(s) read");




            }
            catch (RfcLogonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
