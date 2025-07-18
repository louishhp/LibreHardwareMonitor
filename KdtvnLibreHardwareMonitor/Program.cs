using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;


namespace KdtvnLibreHardwareMonitor
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KdtvnIT_LibreHardwareMonitor is running...");
            Console.WriteLine("The software will automatically shut down when completed.");
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            string report = computer.GetReport();
            computer.Close();

            string directory = AppDomain.CurrentDomain.BaseDirectory; // Lấy thư mục hiện tại của file .exe
            //string filename = $"LibreHardwareLog.txt";
            string machineName = Environment.MachineName; // Lấy tên máy tính
            string filename = $"{machineName}_CpuLibreHardwareLog.txt"; // Sử dụng tên máy tính trong tên file
            string fullPath = Path.Combine(directory, filename);

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(fullPath, report);
                Console.WriteLine($"Đã lưu file log vào: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu file log: {ex.Message}");
            }


        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

}
