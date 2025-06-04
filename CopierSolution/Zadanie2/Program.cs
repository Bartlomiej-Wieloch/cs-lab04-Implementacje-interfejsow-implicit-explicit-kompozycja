using ver1;

namespace Zadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            var faxMachine = new MultifunctionalDevice();
            IDocument doc1 = new PDFDocument("contract_v1.pdf");
            faxMachine.PowerOn();
            faxMachine.SendFax(in doc1, "555-0002");
            faxMachine.SendFax(in doc1, "");
            Console.WriteLine($"Faxes sent: {faxMachine.FaxCounter}");
            IDocument scannedDoc;
            faxMachine.Scan(out scannedDoc);
            if (scannedDoc != null) Console.WriteLine($"Scanned to: {scannedDoc.GetFileName()}");
            Console.WriteLine($"Scans: {faxMachine.ScanCounter}");
            faxMachine.PowerOff();
            Console.WriteLine($"Device state: {faxMachine.GetState()}");

            Console.WriteLine($"Total PowerUps: {faxMachine.Counter}");
            Console.WriteLine($"Total Faxes Sent: {faxMachine.FaxCounter}");
            Console.WriteLine($"Total Scans: {faxMachine.ScanCounter}");
            Console.WriteLine($"Total Prints: {faxMachine.PrintCounter}");

        }
    }
}
