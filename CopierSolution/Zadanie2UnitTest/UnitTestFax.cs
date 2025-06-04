using Microsoft.VisualStudio.TestPlatform.Utilities;
using ver1;

namespace Zadanie2UnitTest
{
    public class ConsoleRedirectionToStringWriter : IDisposable
    {
        private StringWriter stringWriter;
        private TextWriter originalOutput;

        public ConsoleRedirectionToStringWriter()
        {
            stringWriter = new StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(stringWriter);
        }

        public string GetOutput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }
    }

    [TestClass]
    public sealed class UnitTestFax
    {
        [TestMethod]
        public void MultifunctionalDevice_SendFax_DeviceOn_ValidDocumentAndRecipient()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOn();
            IDocument doc = new PDFDocument("fax_content.pdf");
            string recipient = "123456789";

            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                fax.SendFax(in doc, recipient);
                string output = consoleOutput.GetOutput();

                Assert.IsTrue(output.Contains("Fax:"), "Output should contain 'Fax:' prefix.");
                Assert.IsTrue(output.Contains(doc.GetFileName()), "Output should contain document file name.");
                Assert.IsTrue(output.Contains(recipient), "Output should contain recipient number.");
                Assert.AreEqual(1, fax.FaxCounter, "FaxCounter should be incremented.");
            }
        }

        [TestMethod]
        public void MultifunctionalDevice_SendFax_DeviceOff()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOff(); // Urządzenie wyłączone
            IDocument doc = new PDFDocument("fax_content.pdf");
            string recipient = "123456789";

            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                fax.SendFax(in doc, recipient);
                string output = consoleOutput.GetOutput();

                Assert.IsFalse(output.Contains("Fax:"), "Output should not contain 'Fax:' when device is off.");
                Assert.AreEqual(0, fax.FaxCounter, "FaxCounter should not be incremented when device is off.");
            }
        }

        [TestMethod]
        public void MultifunctionalDevice_SendFax_NullDocument()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOn();
            IDocument doc = null; // Dokument jest null
            string recipient = "123456789";

            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                fax.SendFax(in doc, recipient);
                string output = consoleOutput.GetOutput();

                Assert.IsFalse(output.Contains("Fax:"), "Output should not contain 'Fax:' for null document.");
                // Jeśli dodałeś logowanie dla null dokumentu:
                // Assert.IsTrue(output.Contains("Fax: Document is null."), "Output should indicate null document.");
                Assert.AreEqual(0, fax.FaxCounter, "FaxCounter should not be incremented for null document.");
            }
        }

        [TestMethod]
        public void MultifunctionalDevice_SendFax_EmptyRecipientNumber()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOn();
            IDocument doc = new PDFDocument("fax_content.pdf");
            string recipient = ""; // Pusty numer odbiorcy

            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                fax.SendFax(in doc, recipient);
                string output = consoleOutput.GetOutput();

                Assert.IsFalse(output.Contains("Fax:"), "Output should not contain 'Fax:' for empty recipient number.");
                // Jeśli dodałeś logowanie dla pustego numeru:
                // Assert.IsTrue(output.Contains("Fax: Recipient number is empty."), "Output should indicate empty recipient.");
                Assert.AreEqual(0, fax.FaxCounter, "FaxCounter should not be incremented for empty recipient number.");
            }
        }

        [TestMethod]
        public void MultifunctionalDevice_SendFax_WhitespaceRecipientNumber()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOn();
            IDocument doc = new PDFDocument("fax_content.pdf");
            string recipient = "   "; // Numer odbiorcy składający się tylko z białych znaków

            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                fax.SendFax(in doc, recipient);
                string output = consoleOutput.GetOutput();

                Assert.IsFalse(output.Contains("Fax:"), "Output should not contain 'Fax:' for whitespace recipient number.");
                Assert.AreEqual(0, fax.FaxCounter, "FaxCounter should not be incremented for whitespace recipient number.");
            }
        }

        [TestMethod]
        public void MultifunctionalDevice_FaxCounter_MultipleFaxes()
        {
            var fax = new MultifunctionalDevice();
            fax.PowerOn();
            IDocument doc1 = new PDFDocument("fax1.pdf");
            IDocument doc2 = new TextDocument("fax2.txt");
            string recipient1 = "111222333";
            string recipient2 = "444555666";

            fax.SendFax(in doc1, recipient1); // Fax 1
            fax.SendFax(in doc2, recipient2); // Fax 2

            // Wyłączamy, próbujemy wysłać, włączamy, wysyłamy ponownie
            fax.PowerOff();
            fax.SendFax(in doc1, recipient1); // Nie powinno wysłać
            fax.PowerOn();
            fax.SendFax(in doc1, recipient2); // Fax 3

            Assert.AreEqual(3, fax.FaxCounter, "FaxCounter should count only successful faxes.");
        }
    }
}
