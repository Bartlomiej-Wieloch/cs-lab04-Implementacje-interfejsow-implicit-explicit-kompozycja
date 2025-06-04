using System;
using System.Reflection.Metadata;

namespace ver1
{
    public interface IDevice
    {
        enum State {on, off};

        void PowerOn(); // uruchamia urządzenie, zmienia stan na `on`
        void PowerOff(); // wyłącza urządzenie, zmienia stan na `off
        State GetState(); // zwraca aktualny stan urządzenia

        int Counter {get;}  // zwraca liczbę charakteryzującą eksploatację urządzenia,
                            // np. liczbę uruchomień, liczbę wydrukow, liczbę skanów, ...
    }

    public abstract class BaseDevice : IDevice
    {
        protected IDevice.State state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public void PowerOff()
        {
            state = IDevice.State.off;
            Console.WriteLine("... Device is off !");

        }

        public void PowerOn()
        {
            if (GetState() == IDevice.State.off)
            {
                state = IDevice.State.on;
                Counter++; // Inkrementuj licznik tylko przy faktycznym włączeniu
                Console.WriteLine("Device is on ...");
            }
        }

        public int Counter { get; private set; } = 0;
    }
    public interface IFax : IDevice
    {
        void SendFax(in IDocument document, string recipientNumber);
    }

    public interface IPrinter : IDevice
    {
        /// <summary>
        /// Dokument jest drukowany, jeśli urządzenie włączone. W przeciwnym przypadku nic się nie wykonuje
        /// </summary>
        /// <param name="document">obiekt typu IDocument, różny od `null`</param>
        void Print(in IDocument document);
    }

    public interface IScanner : IDevice
    {
        // dokument jest skanowany, jeśli urządzenie włączone
        // w przeciwnym przypadku nic się dzieje
        void Scan(out IDocument document, IDocument.FormatType formatType);
    }
    public class MultifunctionalDevice : BaseDevice, IPrinter, IScanner, IFax
    {
        public int PrintCounter { get; private set; }
        public int ScanCounter { get; private set; }
        public int FaxCounter { get; private set; }

        public void Print(in IDocument document)
        {
            if (GetState() == IDevice.State.on)
            {
                if (document == null)
                {

                    Console.WriteLine("Print: Cannot print a null document.");
                    return;
                }
                PrintCounter++;
                Console.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss} Print: {document.GetFileName()}");
            }
        }
        public void Scan(out IDocument document, IDocument.FormatType formatType)
        {
            document = null;

            if (GetState() == IDevice.State.on)
            {
                ScanCounter++;
                string actualFileName = "";

                switch (formatType)
                {
                    case IDocument.FormatType.TXT:
                        actualFileName = $"TextScan{ScanCounter:D4}.txt";
                        document = new TextDocument(actualFileName);
                        break;
                    case IDocument.FormatType.PDF:
                        actualFileName = $"PDFScan{ScanCounter:D4}.pdf";
                        document = new PDFDocument(actualFileName);
                        break;
                    case IDocument.FormatType.JPG:
                        actualFileName = $"ImageScan{ScanCounter:D4}.jpg";
                        document = new ImageDocument(actualFileName);
                        break;
                    default:
                        Console.WriteLine("Scan: Unknown format. Document not created.");
                        return;
                }

                if (document != null)
                {
                    Console.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss} Scan: {actualFileName}");
                }
            }
        }
        public void Scan(out IDocument document)
        {
            Scan(out document, IDocument.FormatType.JPG);
        }
        public void ScanAndPrint()
        {
            if (GetState() == IDevice.State.on)
            {
                Scan(out IDocument document1, IDocument.FormatType.JPG);
                Print(document1);
            }
        }
        public void SendFax(in IDocument document, string recipientNumber)
        {
            if (GetState() == IDevice.State.on && document != null && !string.IsNullOrWhiteSpace(recipientNumber))
            {
                FaxCounter++;
                Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Fax: {document.GetFileName()} sent to {recipientNumber}");
            }
        }

    }
    public class Copier : BaseDevice, IPrinter, IScanner
    {
        public int PrintCounter { get; private set; }
        public int ScanCounter { get; private set; }

    public void Print(in IDocument document)
    {
        if (GetState() == IDevice.State.on)
        {
            if (document == null)
            {
            
                Console.WriteLine("Print: Cannot print a null document.");
                return;
            }
            PrintCounter++;
            Console.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss} Print: {document.GetFileName()}");
        }
    }
        public void Scan(out IDocument document, IDocument.FormatType formatType)
        {
            document = null;

            if (GetState() == IDevice.State.on)
            {
                ScanCounter++;
                string actualFileName = "";

                switch (formatType)
                {
                    case IDocument.FormatType.TXT:
                        actualFileName = $"TextScan{ScanCounter:D4}.txt";
                        document = new TextDocument(actualFileName);
                        break;
                    case IDocument.FormatType.PDF:
                        actualFileName = $"PDFScan{ScanCounter:D4}.pdf";
                        document = new PDFDocument(actualFileName);
                        break;
                    case IDocument.FormatType.JPG:
                        actualFileName = $"ImageScan{ScanCounter:D4}.jpg";
                        document = new ImageDocument(actualFileName);
                        break;
                    default:
                        Console.WriteLine("Scan: Unknown format. Document not created.");
                        return;
                }

                if (document != null)
                {
                    Console.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss} Scan: {actualFileName}");
                }
            }
        }
        public void Scan(out IDocument document)
        {
            Scan(out document, IDocument.FormatType.JPG);
        }
        public void ScanAndPrint()
        {
            if (GetState() == IDevice.State.on)
            {
                Scan(out IDocument document1, IDocument.FormatType.JPG);
                Print(document1);
            }
        }
    }
}
