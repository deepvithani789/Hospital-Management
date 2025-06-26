using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public AppointmentService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FindAsync(appointment.PatientId);
            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);

            if (patient != null && doctor != null && !string.IsNullOrEmpty(patient.Email))
            {
                string subject = "Your Appointment Details";
                string body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; background-color: #f8f8f8; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                            <div style='background-color: #007bff; padding: 20px; text-align: center; color: white; border-radius: 10px 10px 0 0;'>
                                <h2 style='margin: 0;'>Appointment Confirmation</h2>
                                <p style='margin: 0;'>Your appointment is scheduled!</p>
                            </div>
                            <div style='padding: 20px; font-size: 16px; color: #333;'>
                                <p>Dear <strong>{patient.Name}</strong>,</p>
                                <p>We are happy to confirm your appointment with <strong>Dr.{doctor.Name}</strong>.</p>
                                <p style='font-weight: bold;'>Appointment Details:</p>

                                <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                                    <tr>
                                        <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Appointment Date</td>
                                        <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.AppointmentDate}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Doctor</td>
                                        <td style='padding: 10px; border: 1px solid #ddd;'>Dr. {doctor.Name} - {doctor.Specialization}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Appointment Type</td>
                                        <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.AppointmentType}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Reason for Visit</td>
                                        <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.ReasonForVisit}</td>
                                    </tr>
                                </table>

                                <p style='margin-top: 20px;'>We look forward to seeing you! Please contact us if you need any further assistance or wish to reschedule.</p>
                                <div class=""col-md-6 mb-4"">
                                    <div class=""contact-info"">
                                        <h3 class=""text-primary mb-4"">Contact Information</h3>
                                        <div class=""contact-item mb-3"">
                                            <i class=""bi bi-envelope-fill me-3""></i>
                                            <strong>Email:</strong> <a href=""mailto:deepvithani789@gmail.com"" class=""text-decoration-none"">deepvithani789@gmail.com</a>
                                        </div>
                                        <div class=""contact-item mb-3"">
                                            <i class=""bi bi-telephone-fill me-3""></i>
                                            <strong>Phone:</strong> <a href=""tel:+918799146248"" class=""text-decoration-none"">+91 8799146248</a>
                                        </div>
                                    </div>
                                </div>
                                <div style='text-align: center; margin-top: 30px;'>
                                    <p style='font-size: 14px; color: #777;'>Thank you for choosing our healthcare services.</p>
                                </div>
                            </div>
                        </div>
                    </body>
                </html>";

                _emailService.SendEmail(patient.Email, subject, body);
            }

            return appointment;
        }

        public async Task<Appointment> DeleteAsync(int id)
        {
            var exists = await _context.Appointments.FindAsync(id);
            if (exists != null)
            {
                _context.Appointments.Remove(exists);
                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments.Include(p => p.Patient).Include(d => d.Doctor).OrderBy(a => a.Id).ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments.Include(p => p.Patient).Include(d => d.Doctor).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment, int id)
        {
            var exists = await _context.Appointments.FindAsync(id);
            if (exists != null)
            {
                exists.DoctorId = appointment.DoctorId;
                exists.PatientId = appointment.PatientId;
                exists.AppointmentDate = appointment.AppointmentDate;
                exists.Status = appointment.Status;
                exists.AppointmentType = appointment.AppointmentType;
                exists.ReasonForVisit = appointment.ReasonForVisit;

                var patient = await _context.Patients.FindAsync(appointment.PatientId);
                var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);

                if (patient != null && doctor != null && !string.IsNullOrEmpty(patient.Email))
                {
                    string subject = "Updated Appointment Details";
                    string body = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f8f8f8; padding: 20px;'>
                            <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                                <div style='background-color: #28a745; padding: 20px; text-align: center; color: white; border-radius: 10px 10px 0 0;'>
                                    <h2 style='margin: 0;'>Updated Appointment Details</h2>
                                    <p style='margin: 0;'>Your appointment has been updated!</p>
                                </div>
                                <div style='padding: 20px; font-size: 16px; color: #333;'>
                                    <p>Dear <strong>{patient.Name}</strong>,</p>
                                    <p>Your appointment with <strong>Dr. {doctor.Name}</strong> has been updated. Please find the new details below:</p>
                                    <p style='font-weight: bold;'>Updated Appointment Details:</p>

                                    <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                                        <tr>
                                            <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Appointment Date</td>
                                            <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.AppointmentDate}</td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Doctor</td>
                                            <td style='padding: 10px; border: 1px solid #ddd;'>Dr. {doctor.Name} - {doctor.Specialization}</td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Appointment Type</td>
                                            <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.AppointmentType}</td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 10px; background-color: #f4f4f4; border: 1px solid #ddd;'>Reason for Visit</td>
                                            <td style='padding: 10px; border: 1px solid #ddd;'>{appointment.ReasonForVisit}</td>
                                        </tr>
                                    </table>

                                    <p style='margin-top: 20px;'>If you need to reschedule or have any questions, feel free to contact us.</p>

                                    <div class=""col-md-6 mb-4"">
                                        <div class=""contact-info"">
                                            <h3 class=""text-primary mb-4"">Contact Information</h3>
                                            <div class=""contact-item mb-3"">
                                                <i class=""bi bi-envelope-fill me-3""></i>
                                                <strong>Email:</strong> <a href=""mailto:deepvithani789@gmail.com"" class=""text-decoration-none"">deepvithani789@gmail.com</a>
                                            </div>
                                            <div class=""contact-item mb-3"">
                                                <i class=""bi bi-telephone-fill me-3""></i>
                                                <strong>Phone:</strong> <a href=""tel:+918799146248"" class=""text-decoration-none"">+91 8799146248</a>
                                            </div>
                                        </div>
                                    </div>

                                    <div style='text-align: center; margin-top: 30px;'>
                                        <p style='font-size: 14px; color: #777;'>Thank you for choosing our healthcare services. We look forward to your visit!</p>
                                    </div>
                                </div>
                            </div>
                        </body>
                    </html>";

                    _emailService.SendEmail(patient.Email, subject, body);
                }



                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }
    }
}
