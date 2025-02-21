using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Medidata.Core.Objects;
using Medidata.Core.Common;
using Medidata.Core.Common.Utilities;
using Medidata.Utilities;
using Medidata.Utilities.Interfaces;
using System.Configuration;
using System.IO;


namespace CustomFunctions
{
	#region System Class - Do Not Modify
	using Medidata.CustomFunctions.Debug;

	/// <summary>
	/// Runs the CustomFunction Development Utility.
	/// </summary>
	public class _SystemClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="_SystemClass"/> class.
		/// </summary>
		public _SystemClass()
		{
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			System.Windows.Forms.Application.Run(new MainForm());
		}

		/// <summary>
		/// Handles the UnhandledException event of the CurrentDomain control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = (Exception)e.ExceptionObject;
			if(!CacheManager.IsThreadAbortException(exception)) System.Windows.Forms.MessageBox.Show(exception.ToString());
		}
	}
	#endregion

	/// <summary>
	/// Sample CustomFunction.
	/// </summary>
	public class CustomFunction1 : CustomFunctionBase
	{
		public override object Eval(object ThisObject)
		{
return "Hello World!";
		}
        public bool CheckDptValid(DataPoint Dpt)
        {
            if (Dpt != null && Dpt.Active && Dpt.IsVisibleRaw && !Dpt.IsDataPointNonConformant)
                return true;
            else return false;
        }
        public DataPoint GetDataPoint(string fieldoid, string formoid, string folderoid, Subject currsubj, int x)
        {
            DataPoint Dpt_Target = null;
            Instance ins_tar = currsubj.Instances.FindByFolderOID(folderoid);
            if (ins_tar != null)
            {
                DataPage Dpg_tar = ins_tar.DataPages.FindByFormOID(formoid);
                if (Dpg_tar != null)
                {
                    if (x == 0)
                        Dpt_Target = Dpg_tar.MasterRecord.DataPoints.FindByFieldOID(fieldoid);
                    else
                    {
                        Record rcd = Dpg_tar.Records.FindByRecordPosition(x);
                        if (rcd != null)
                            Dpt_Target = rcd.DataPoints.FindByFieldOID(fieldoid);
                    }
                }

            }
            return Dpt_Target;
        }
        public DataPoint GetDataPoint(string fieldoid, DataPage Dpg, int x)
        {
            DataPoint Dpt_Target = null;
            DataPage Dpg_tar = Dpg;
            if (Dpg_tar != null)
            {
                if (x == 0)
                    Dpt_Target = Dpg_tar.MasterRecord.DataPoints.FindByFieldOID(fieldoid);
                else
                {
                    Record rcd = Dpg_tar.Records.FindByRecordPosition(x);
                    if (rcd != null)
                        Dpt_Target = rcd.DataPoints.FindByFieldOID(fieldoid);
                }
            }
            return Dpt_Target;
        }
        public DataPoints GetAllDataPoints(string fieldoid, string formoid, string folderoid, Subject currsubj, int recordPosition)
        {
            DataPoint Dpt_Target = null;
            DataPoints Dpts_All = new DataPoints();
            if (fieldoid == null)
                return Dpts_All;
            DataPage Dpg_Tar = null;
            const string subjectfolder = "SUBJECT";
            if ((folderoid == null || folderoid.ToUpper() == subjectfolder) && currsubj.DataPages != null && currsubj.DataPages.Count > 0)
            {
                if (formoid == null)
                {
                    for (int i = 0; i < currsubj.DataPages.Count; i++)
                    {
                        Dpg_Tar = currsubj.DataPages[i];
                        if (Dpg_Tar != null && Dpg_Tar.Active)
                        {
                            GetThatDpt(Dpg_Tar);
                        }
                    }
                }
                else if (formoid != null && formoid != string.Empty)
                {
                    for (int i = 0; i < currsubj.DataPages.Count; i++)
                    {
                        Dpg_Tar = currsubj.DataPages[i];
                        if (Dpg_Tar != null && Dpg_Tar.Active && Dpg_Tar.Form.OID == formoid)
                        {
                            GetThatDpt(Dpg_Tar);
                        }
                    }
                }
            }
            Instances Inss_All = currsubj.Instances;
            if (Inss_All != null && Inss_All.Count > 0)
            {
                for (int i = 0; i < Inss_All.Count; i++)
                {
                    if (Inss_All[i] != null && Inss_All[i].Active)
                    {
                        getdpt(Inss_All[i]);
                    }
                }
            }
            void getdpt(Instance ins)
            {
                if (ins != null && ins.Active)
                {
                    if (ins.DataPages != null && ins.DataPages.Count > 0 && (folderoid == null || ins.Folder.OID == folderoid))
                    {
                        DataPage Dpgx = null;
                        if (formoid == null)
                        {
                            for (int i = 0; i < ins.DataPages.Count; i++)
                            {
                                Dpgx = ins.DataPages[i];
                                if (Dpgx != null && Dpgx.Active)
                                {
                                    GetThatDpt(Dpgx);
                                }
                            }
                        }
                        else if (formoid != null && formoid != string.Empty)
                        {
                            for (int i = 0; i < ins.DataPages.Count; i++)
                            {
                                Dpgx = ins.DataPages[i];
                                if (Dpgx != null && Dpgx.Active && Dpgx.Form.OID == formoid)
                                {
                                    GetThatDpt(Dpgx);
                                }
                            }
                        }
                    }
                    if (ins.Instances != null && ins.Instances.Count > 0)
                    {
                        for (int i = 0; i < ins.Instances.Count; i++)
                        {
                            getdpt(ins.Instances[i]);
                        }
                    }
                }

            }
            void GetThatDpt(DataPage DPGT)
            {
                if (DPGT != null && DPGT.Active && DPGT.Records != null && DPGT.Records.Count > 0)
                {
                    if (recordPosition >= 0)
                    {
                        fromRCDtoDPT(recordPosition, DPGT);
                    }
                    else if (recordPosition == -1)
                    {
                        for (int i = 0; i < DPGT.Records.Count; i++)
                        {
                            fromRCDtoDPT(i, DPGT);
                        }
                    }

                }
            }
            void fromRCDtoDPT(int x, DataPage DPGT)
            {
                Record TarRcd = null;
                TarRcd = DPGT.Records.FindByRecordPosition(x);
                if (TarRcd != null && TarRcd.Active)
                {
                    Dpt_Target = TarRcd.DataPoints.FindByFieldOID(fieldoid);
                    if (Dpt_Target != null && Dpt_Target.Active && Dpt_Target.IsVisibleRaw)
                    {
                        Dpts_All.Add(Dpt_Target);
                        Dpt_Target = null;
                    }
                }
            }
            return Dpts_All;
        }
        public DataPoint FindTheEarliestDate(DataPoints Dpts)
        {
            DateTime MinV = DateTime.MaxValue;
            DateTime DTVAR = DateTime.MaxValue;
            DataPoint DptMin = null;
            if (Dpts != null && Dpts.Count > 0)
            {
                for (int i = 0; i < Dpts.Count; i++)
                {
                    if (CheckDptValid(Dpts[i]) && Dpts[i].StandardValue() != null && Dpts[i].StandardValue() is DateTime)
                    {
                        DTVAR = (DateTime)Dpts[i].StandardValue();
                        if (MinV > DTVAR)
                        {
                            MinV = DTVAR;
                            DptMin = Dpts[i];
                        }
                    }
                }
            }
            return DptMin;
        }
        public bool Date2EQorGTDate1(DataPoint date1, DataPoint date2)
        {
            DateTime Dt_SD_SDAT1 = DateTime.MinValue;
            DateTime Dt_SD_SDAT2 = DateTime.MinValue;
            int yr1 = 0, yr2 = 0, mn1 = 0, mn2 = 0;
            if (date1 != null && date2 != null)
            {
                if (date1.Data.ToUpper().EndsWith("UNKN") || date2.Data.ToUpper().EndsWith("UNKN"))
                    return false;
                else if ((date1.Data.Contains("UNK") || date1.Data.StartsWith("UN")) || (date1.Data.Contains("UNK") || date1.Data.StartsWith("UN")))
                {
                    if ((!date1.Data.StartsWith("UN") && date1.Data.Contains("UNK")) || (!date2.Data.StartsWith("UN") && date2.Data.Contains("UNK")))
                        return false;
                    else if ((date1.Data.StartsWith("UN") && date1.Data.Contains("UNK")) || (date2.Data.StartsWith("UN") && date2.Data.Contains("UNK")))
                    {
                        Dt_SD_SDAT1 = Convert.ToDateTime(date1.StandardValue());
                        yr1 = Dt_SD_SDAT1.Year;
                        Dt_SD_SDAT2 = Convert.ToDateTime(date2.StandardValue());
                        yr2 = Dt_SD_SDAT2.Year;
                        if (yr1 != 1800 && yr1 != 1900 && yr2 != 1800 && yr2 != 1900 && yr1 < yr2)
                            return true;
                    }
                    else if ((date1.Data.StartsWith("UN") && !date1.Data.Contains("UNK")) || (date2.Data.StartsWith("UN") && !date2.Data.Contains("UNK")))
                    {

                        Dt_SD_SDAT1 = Convert.ToDateTime(date1.StandardValue());
                        yr1 = Dt_SD_SDAT1.Year;
                        mn1 = Dt_SD_SDAT1.Month;
                        Dt_SD_SDAT2 = Convert.ToDateTime(date2.StandardValue());
                        yr2 = Dt_SD_SDAT2.Year;
                        mn2 = Dt_SD_SDAT2.Month;
                        if (yr1 != 1800 && yr1 != 1900 && yr2 != 1800 && yr2 != 1900 && yr1 == yr2)
                        {
                            if (mn1 < mn2)
                                return true;
                            else
                                return false;
                        }
                        else if (yr1 != 1800 && yr1 != 1900 && yr2 != 1800 && yr2 != 1900 && yr1 < yr2)
                            return true;
                    }
                }
                else if (date1.StandardValue() is DateTime && date2.StandardValue() is DateTime)
                {
                    Dt_SD_SDAT1 = (DateTime)date1.StandardValue();
                    Dt_SD_SDAT2 = (DateTime)date2.StandardValue();
                    if (Dt_SD_SDAT1.Year != 1800 && Dt_SD_SDAT1.Year != 1900 && Dt_SD_SDAT2.Year != 1800 && Dt_SD_SDAT1.Year != 1900)
                    {
                        if (Dt_SD_SDAT1.Year < Dt_SD_SDAT2.Year)
                            return true;
                        else if (Dt_SD_SDAT1.Year == Dt_SD_SDAT2.Year && Dt_SD_SDAT1.Month < Dt_SD_SDAT2.Month)
                            return true;
                        else if (Dt_SD_SDAT1.Year == Dt_SD_SDAT2.Year && Dt_SD_SDAT1.Month == Dt_SD_SDAT2.Month && Dt_SD_SDAT1.Day <= Dt_SD_SDAT2.Day)
                            return true;
                        else return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            return false;
        }
        public DataPoint FindTheEarliestDateForincompleteDate(DataPoints Dpts)
        {
            if (Dpts != null && Dpts.Count > 0)
            {
                DataPoint DptTemp = null;
                DataPoints Dpts_complete = new DataPoints();
                DataPoints Dpts_DayUN = new DataPoints();
                DataPoints Dpts_MonUNK = new DataPoints();
                for (int i = 0; i < Dpts.Count; i++)
                {
                    if (CheckDptValid(Dpts[i]) && Dpts[i].StandardValue() != null && Dpts[i].StandardValue() is DateTime)
                    {
                        if (!Dpts[i].Data.StartsWith("UN") && !Dpts[i].Data.Contains("UNK"))
                            Dpts_complete.Add(Dpts[i]);
                        else if (!Dpts[i].Data.Contains("UNK") && Dpts[i].Data.StartsWith("UN"))
                            Dpts_DayUN.Add(Dpts[i]);
                        else if (Dpts[i].Data.StartsWith("UN") && Dpts[i].Data.Contains("UNK") && !Dpts[i].Data.Contains("UNKN"))
                            Dpts_MonUNK.Add(Dpts[i]);
                    }
                }
                if (Dpts_complete != null && Dpts_complete.Count > 0)
                {
                    DptTemp = Dpts_complete[0];
                    for (int i = 1; i < Dpts_complete.Count; i++)
                    {
                        if (Date2GTDate1Plus(Dpts_complete[i], DptTemp, 0))
                            DptTemp = Dpts_complete[i];
                    }
                }
                if (Dpts_DayUN != null && Dpts_DayUN.Count > 0)
                {
                    if (DptTemp == null)
                        DptTemp = Dpts_DayUN[0];
                    for (int i = 0; i < Dpts_DayUN.Count; i++)
                    {
                        if (Date2GTDate1Plus(Dpts_DayUN[i], DptTemp, 0))
                            DptTemp = Dpts_DayUN[i];
                    }
                }
                if (Dpts_MonUNK != null && Dpts_MonUNK.Count > 0)
                {
                    if (DptTemp == null)
                        DptTemp = Dpts_MonUNK[0];
                    for (int i = 0; i < Dpts_MonUNK.Count; i++)
                    {
                        if (Date2GTDate1Plus(Dpts_MonUNK[i], DptTemp, 0))
                            DptTemp = Dpts_MonUNK[i];
                    }
                }
                return DptTemp;
            }
            return null;
        }
        public bool Date2EQDate1(DataPoint date1, DataPoint date2)
        {
            DateTime Dt_SD_SDAT1 = DateTime.MinValue;
            DateTime Dt_SD_SDAT2 = DateTime.MinValue;
            int yr1 = 0, yr2 = 0, mn1 = 0, mn2 = 0;
            if (date1 != null && date2 != null)
            {
                if (date1.Data.Contains("UNK") || date1.Data.StartsWith("UN") || date2.Data.Contains("UNK") || date2.Data.StartsWith("UN"))
                    return false;
                else if (date1.StandardValue() is DateTime && date2.StandardValue() is DateTime)
                {
                    Dt_SD_SDAT1 = (DateTime)date1.StandardValue();
                    Dt_SD_SDAT2 = (DateTime)date2.StandardValue();
                    if ((Dt_SD_SDAT1.Year != 1800 && Dt_SD_SDAT1.Year != 1900 && Dt_SD_SDAT2.Year != 1800 && Dt_SD_SDAT2.Year != 1900) && Dt_SD_SDAT1.Day == Dt_SD_SDAT2.Day && Dt_SD_SDAT1.Month == Dt_SD_SDAT2.Month && Dt_SD_SDAT1.Year == Dt_SD_SDAT2.Year)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            return false;
        }
        public bool Date2NEQDate1(DataPoint date1, DataPoint date2)
        {
            DateTime Dt_SD_SDAT1 = DateTime.MinValue;
            DateTime Dt_SD_SDAT2 = DateTime.MinValue;
            if (date1 != null && date2 != null)
            {
                if (date1.Data.Contains("UNK") || date1.Data.StartsWith("UN") || date2.Data.Contains("UNK") || date2.Data.StartsWith("UN"))
                    return false;
                else if (date1.StandardValue() is DateTime && date2.StandardValue() is DateTime)
                {
                    Dt_SD_SDAT1 = (DateTime)date1.StandardValue();
                    Dt_SD_SDAT2 = (DateTime)date2.StandardValue();
                    if ((Dt_SD_SDAT1.Year != 1800 && Dt_SD_SDAT1.Year != 1900 && Dt_SD_SDAT2.Year != 1800 && Dt_SD_SDAT2.Year != 1900) && Dt_SD_SDAT1.Date != Dt_SD_SDAT2.Date)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            return false;
        }
        public DataPoint FindTheLatestDate(DataPoints Dpts)
        {
            DateTime MinV = DateTime.MinValue;
            DateTime DTVAR = DateTime.MinValue;
            DataPoint DptMax = null;
            if (Dpts.Count > 0)
            {
                for (int i = 0; i < Dpts.Count; i++)
                {
                    if (CheckDptValid(Dpts[i]) && Dpts[i].StandardValue() != null && Dpts[i].StandardValue() is DateTime)
                    {
                        DTVAR = (DateTime)Dpts[i].StandardValue();
                        if (MinV < DTVAR)
                        {
                            MinV = DTVAR;
                            DptMax = Dpts[i];
                        }
                    }
                }
            }
            return DptMax;
        }
        public DataPoint FindTheLatestDateForincompleteDate(DataPoints Dpts)
        {
            if (Dpts != null && Dpts.Count > 0)
            {
                DataPoint DptTemp = null;
                DataPoints Dpts_complete = new DataPoints();
                DataPoints Dpts_DayUN = new DataPoints();
                DataPoints Dpts_MonUNK = new DataPoints();
                for (int i = 0; i < Dpts.Count; i++)
                {
                    if (CheckDptValid(Dpts[i]) && Dpts[i].StandardValue() != null && Dpts[i].StandardValue() is DateTime)
                    {
                        if (!Dpts[i].Data.StartsWith("UN") && !Dpts[i].Data.Contains("UNK"))
                            Dpts_complete.Add(Dpts[i]);
                        else if (!Dpts[i].Data.Contains("UNK") && Dpts[i].Data.StartsWith("UN"))
                            Dpts_DayUN.Add(Dpts[i]);
                        else if (Dpts[i].Data.StartsWith("UN") && Dpts[i].Data.Contains("UNK") && !Dpts[i].Data.Contains("UNKN"))
                            Dpts_MonUNK.Add(Dpts[i]);
                    }
                }
                if (Dpts_complete != null && Dpts_complete.Count > 0)
                {
                    DptTemp = Dpts_complete[0];
                    for (int i = 1; i < Dpts_complete.Count; i++)
                    {
                        if (Date2GTDate1Plus(DptTemp, Dpts_complete[i], 0))
                            DptTemp = Dpts_complete[i];
                    }
                }
                if (Dpts_DayUN != null && Dpts_DayUN.Count > 0)
                {
                    if (DptTemp == null)
                        DptTemp = Dpts_DayUN[0];
                    for (int i = 0; i < Dpts_DayUN.Count; i++)
                    {
                        if (Date2GTDate1Plus(DptTemp, Dpts_DayUN[i], 0))
                            DptTemp = Dpts_DayUN[i];
                    }
                }
                if (Dpts_MonUNK != null && Dpts_MonUNK.Count > 0)
                {
                    if (DptTemp == null)
                        DptTemp = Dpts_MonUNK[0];
                    for (int i = 0; i < Dpts_MonUNK.Count; i++)
                    {
                        if (Date2GTDate1Plus(DptTemp, Dpts_MonUNK[i], 0))
                            DptTemp = Dpts_MonUNK[i];
                    }
                }
                return DptTemp;
            }
            return null;
        }
        public bool Date2GTDate1Plus(DataPoint date1, DataPoint date2, int x)
        {

            DateTime Dt1 = DateTime.MinValue;
            DateTime Dt2 = DateTime.MinValue;
            if (CheckDptValid(date1) && CheckDptValid(date2) && date1.StandardValue() is DateTime && date2.StandardValue() is DateTime && date1.Data != "" && date2.Data != "")
            {
                Dt1 = (DateTime)date1.StandardValue();
                Dt2 = (DateTime)date2.StandardValue();
                if (date1.Data.EndsWith("UNKN") || date2.Data.EndsWith("UNKN"))
                    return false;
                else if ((!date1.Data.StartsWith("UN") && date1.Data.Contains("UNK")) || (!date2.Data.StartsWith("UN") && date2.Data.Contains("UNK")))
                    return false;
                else if ((date1.Data.Contains("UNK") && date1.Data.StartsWith("UN")) && (date2.Data.Contains("UNK") && date2.Data.StartsWith("UN")))
                {
                    if (Dt2.Year > Dt1.Year)
                        return true;
                }
                else if (x != 0)
                {
                    if (!date2.Data.Contains("UNK") && !date2.Data.StartsWith("UN"))
                    {
                        Dt2 = Dt2.AddDays(-x);
                    }
                    else if (!date1.Data.Contains("UNK") && !date1.Data.StartsWith("UN"))
                    {
                        Dt1 = Dt1.AddDays(x);
                    }

                    else if (date1.Data.StartsWith("UN") && date2.Data.StartsWith("UN"))
                    {
                        x = (int)Math.Floor((x / 30.0) + 1);
                        Dt1 = Dt1.AddMonths(x);
                    }
                }
                if (Dt1.Year != 1800 && Dt2.Year != 1900 && Dt1.Year != 1800 && Dt2.Year != 1900)
                {
                    if (Dt1.Year < Dt2.Year)
                        return true;
                    else if (Dt1.Year == Dt2.Year && Dt1.Month < Dt2.Month && !date1.Data.Contains("UNK") && !date2.Data.Contains("UNK"))
                        return true;
                    else if (Dt1.Year == Dt2.Year && Dt1.Month == Dt2.Month && Dt1.Day < Dt2.Day && (!date1.Data.StartsWith("UN") && !date2.Data.StartsWith("UN")))
                        return true;
                }
            }
            return false;
        }
    }

	}
}
