using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using OfficeOpenXml;

class Program
{
    class Customer
    {
        //<cartId, Purchase>
        public Dictionary<int,List<Purchase>> Purchase { get; set; }
        public int CustomerId { get; set; }

    }
    class Purchase
    {
        public int CartId { get; set; }
        public string itemName { get; set; }
        public decimal price { get; set; }
        public DateTime date { get; set; }
        public double itemId { get; set; }
        public string customerName { get; set; }
    }
    public static void Main()
    {
        string filePath = "C:\\Users\\User\\Desktop\\BIProk\\Database.xlsx";
        Dictionary<int, Customer> dataDictionary = ReadCsvToDictionary(filePath);
        List<Purchase> purchases6to10 = new List<Purchase>();
        List<Purchase> purchases10to14 = new List<Purchase>();
        List<Purchase> purchases14to18 = new List<Purchase>();
        List<Purchase> purchases18to22 = new List<Purchase>();
        //פעם אחת לפצל את הטבלה ל-2 
        //טבלת רוכשים
        //טבלת רכישות


        TimeSpan sixAM = new TimeSpan(6, 0, 0); 
        TimeSpan tenAM = new TimeSpan(10, 0, 0); 
        TimeSpan twoPM = new TimeSpan(14, 0, 0); 
        TimeSpan sixPM = new TimeSpan(18, 0, 0); 
        TimeSpan tenPM = new TimeSpan(22, 0, 0);

        //itemId,ALL PURCHASES
        Dictionary<double, List<Purchase>> Question6to10 = new Dictionary<double, List<Purchase>>();
        Dictionary<double, List<Purchase>> Question10to14 = new Dictionary<double, List<Purchase>>();
        Dictionary<double, List<Purchase>> Question14to18 = new Dictionary<double, List<Purchase>>();
        Dictionary<double, List<Purchase>> Question18to22 = new Dictionary<double, List<Purchase>>();


        //לבדוק האם יש קשר בין טווח המכירות
        //למול סוג המוצר

        foreach (var item in dataDictionary)
        {
            foreach (var row in item.Value.Purchase)
            {
                for (int i = 0; i < row.Value.Count; i++)
                {
                    TimeSpan currentTime = row.Value[i].date.TimeOfDay;
                    //Console.WriteLine(currentTime);
                    if (currentTime >= sixAM && currentTime <= tenAM)
                    {
                        purchases6to10.Add(row.Value[i]);
                        if (!Question6to10.ContainsKey(row.Value[i].itemId))
                        {
                            List<Purchase> list = new List<Purchase>();
                            list.Add(row.Value[i]);
                            Question6to10.Add(row.Value[i].itemId, list);
                        }
                        else
                        {
                            Question6to10.GetValueOrDefault(row.Value[i].itemId).Add(row.Value[i]);
                        }
                    }
                    if (currentTime >= tenAM && currentTime <= twoPM)
                    {
                        purchases10to14.Add(row.Value[i]);
                        if (!Question10to14.ContainsKey(row.Value[i].itemId))
                        {
                            List<Purchase> list = new List<Purchase>();
                            list.Add(row.Value[i]);
                            Question10to14.Add(row.Value[i].itemId, list);
                        }
                        else
                        {
                            Question10to14.GetValueOrDefault(row.Value[i].itemId).Add(row.Value[i]);
                        }
                    }
                    if (currentTime >= twoPM && currentTime <= sixPM)
                    {
                        purchases14to18.Add(row.Value[i]);
                        if (!Question14to18.ContainsKey(row.Value[i].itemId))
                        {
                            List<Purchase> list = new List<Purchase>();
                            list.Add(row.Value[i]);
                            Question14to18.Add(row.Value[i].itemId, list);
                        }
                        else
                        {
                            Question14to18.GetValueOrDefault(row.Value[i].itemId).Add(row.Value[i]);
                        }
                    }
                    if (currentTime >= sixPM && currentTime <= tenPM)
                    {
                        purchases18to22.Add(row.Value[i]);
                        if (!Question18to22.ContainsKey(row.Value[i].itemId))
                        {
                            List<Purchase> list = new List<Purchase>();
                            list.Add(row.Value[i]);
                            Question18to22.Add(row.Value[i].itemId, list);
                        }
                        else
                        {
                            Question18to22.GetValueOrDefault(row.Value[i].itemId).Add(row.Value[i]);
                        }
                    }
                }
                
            }
            

        }

        string newFilePath = "C:\\Users\\User\\Desktop\\BIProk\\Answer.csv";

        using (StreamWriter writer = new StreamWriter(newFilePath, false, Encoding.UTF8))
        {
            writer.WriteLine("Time,ItemName,ItemID,COUNT");

            foreach (var item in Question6to10)
            {
                writer.WriteLine($"06:00-10:00,{item.Value[0].itemName},{item.Key},{item.Value.Count}");
            }
            foreach (var item in Question10to14)
            {
                writer.WriteLine($"10:00-14:00,{item.Value[0].itemName},{item.Key},{item.Value.Count}");
            }
            foreach (var item in Question14to18)
            {
                writer.WriteLine($"14:00-18:00,{item.Value[0].itemName},{item.Key},{item.Value.Count}");
            }
            foreach (var item in Question18to22)
            {
                writer.WriteLine($"18:00-22:00,{item.Value[0].itemName},{item.Key},{item.Value.Count}");
            }
        }

    }

     static Dictionary<int, Customer> ReadCsvToDictionary(string filePath)
    {
        Dictionary<int, Customer> dataDictionary = new Dictionary<int, Customer>();

        using (StreamReader reader = new StreamReader(filePath))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    DateTime date;
                    if (DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out date))
                    {
                        string value = worksheet.Cells[row, 2].Value?.ToString();
                        string itemName = worksheet.Cells[row, 7].Value.ToString();
                        decimal price = Convert.ToDecimal(worksheet.Cells[row, 9].Value.ToString());
                        int customerId = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                        string customerName = worksheet.Cells[row, 2].Value.ToString();
                        double itemId = Convert.ToDouble(worksheet.Cells[row, 6].Value);
                        int cartId = Convert.ToInt32(worksheet.Cells[row, 4].Value);
                        if (!dataDictionary.ContainsKey(customerId))
                        {
                            Dictionary<int, List<Purchase>> purchase = new Dictionary<int, List<Purchase>>();
                            Customer customer = new Customer() { CustomerId = customerId, Purchase = purchase };
                            List<Purchase> list = new List<Purchase>();
                            Purchase dataBaseRow = new Purchase() { date = date, itemName = itemName, price = price,itemId = itemId,customerName = customerName };
                            list.Add(dataBaseRow);
                            //Console.WriteLine("time is " + date);
                            customer.Purchase.Add(cartId, list);
                            dataDictionary.Add(customerId, customer);
                        }
                        else
                        {
                            Purchase dataBaseRow = new Purchase() { date = date, itemName = itemName, price = price, itemId = itemId, customerName = customerName };
                            //Console.WriteLine("time is " + date);
                            if (dataDictionary.GetValueOrDefault(customerId).Purchase.ContainsKey(cartId))
                                dataDictionary.GetValueOrDefault(customerId).Purchase.GetValueOrDefault(cartId).Add(dataBaseRow);

                            else
                            {
                                List<Purchase> list = new List<Purchase>();
                                list.Add(dataBaseRow);
                                dataDictionary.GetValueOrDefault(customerId).Purchase.Add(cartId, list);
                            }
                        }   
                    }
                }
            }
        }

        return dataDictionaryPURCHASES;
    }
}
