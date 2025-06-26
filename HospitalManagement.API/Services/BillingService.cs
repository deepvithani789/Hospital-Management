using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _context;
        public BillingService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Billing> AddASync(Billing billing)
        {
            billing.FinalAmount = billing.TotalAmount - (billing.Discount ?? 0) - (billing.InsuranceCoverage ?? 0);
            await _context.Billings.AddAsync(billing);
            await _context.SaveChangesAsync();
            return billing;
        }

        public async Task<Billing> DeleteASync(int id)
        {
            var exists = await _context.Billings.FindAsync(id);
            if (exists != null)
            {
                _context.Billings.Remove(exists);
                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }

        public async Task<byte[]> GenerateInvoice(int id)
        {
            var billing = await _context.Billings
                .Include(p => p.Patient)
                .Include(a => a.Appointment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billing == null)
            {
                throw new Exception("Billing not found!!");
            }

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Hospital Branding
            var title = new Paragraph("Hospital Management")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(24)
                .SimulateBold()
                .SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE);

            var subTitle = new Paragraph("INVOICE")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(18)
                .SimulateBold();

            document.Add(title);
            document.Add(subTitle);
            document.Add(new Paragraph($"Date: {DateTime.Now:dd MMM yyyy}")
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10));

            // Billing Info Table
            var table = new Table(2).UseAllAvailableWidth();
            table.SetMarginTop(20);
            table.AddCell("Invoice ID:").AddCell(billing.Id.ToString());
            table.AddCell("Patient Name:").AddCell(billing.Patient?.Name ?? "N/A");
            table.AddCell("Appointment Date:").AddCell(billing.Appointment?.AppointmentDate.ToShortDateString() ?? "N/A");
            table.AddCell("Billing Date:").AddCell(billing.BillingDate.ToShortDateString());
            table.AddCell("Payment Method:").AddCell(billing.PaymentMethod);
            table.AddCell("Is Paid:").AddCell(billing.IsPaid ? "Yes" : "No");

            document.Add(table);

            // Charges Table
            var chargeTable = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth();
            chargeTable.SetMarginTop(30);
            chargeTable.AddHeaderCell("Description").SimulateBold();
            chargeTable.AddHeaderCell("Amount (Rs.)").SimulateBold();

            chargeTable.AddCell("Total Amount").AddCell(billing.TotalAmount.ToString("F2"));
            chargeTable.AddCell("Discount").AddCell((billing.Discount ?? 0).ToString("F2"));
            chargeTable.AddCell("Insurance Coverage").AddCell((billing.InsuranceCoverage ?? 0).ToString("F2"));
            chargeTable.AddCell("Final Amount").AddCell(billing.FinalAmount.ToString("F2")).SimulateBold();

            document.Add(chargeTable);

            // Footer
            document.Add(new Paragraph("Thank you for choosing this Hospital.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SimulateItalic()
                .SetMarginTop(40));

            document.Close();
            return ms.ToArray();
        }

        public async Task<IEnumerable<Billing>> GetAllASync()
        {
            return await _context.Billings
                .Include(p => p.Patient)
                .Include(a => a.Appointment)
                .OrderBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<Billing> GetByIdAsync(int id)
        {
            var exists = await _context.Billings.FindAsync(id);
            if(exists != null)
            {
                return await _context.Billings
                    .Include(p => p.Patient)
                    .Include(a => a.Appointment)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
            return null;
        }

        public async Task<Billing> UpdateASync(Billing billing, int id)
        {
            var exists = await _context.Billings.FindAsync(id);
            if (exists != null)
            {
                exists.PatientId = billing.PatientId;
                exists.AppointmentId = billing.AppointmentId;
                exists.TotalAmount = billing.TotalAmount;
                exists.Discount = billing.Discount;
                exists.InsuranceCoverage = billing.InsuranceCoverage;
                exists.FinalAmount = billing.TotalAmount - (billing.Discount ?? 0) - (billing.InsuranceCoverage ?? 0);
                exists.PaymentMethod = billing.PaymentMethod;
                exists.IsPaid = billing.IsPaid;
                //exists.BillingDate = billing.BillingDate;

                await _context.SaveChangesAsync();
                return billing;
            }
            return null;
        }
    }
}
