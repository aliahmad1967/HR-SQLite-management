// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// PayrollService.cs - خدمة الرواتب
// =====================================================

using HRManagementSystem.Core.Enums;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Core.Models;
using HRManagementSystem.Services.Interfaces;
using Serilog;

namespace HRManagementSystem.Services;

/// <summary>
/// خدمة الرواتب
/// Payroll service
/// </summary>
public class PayrollService : IPayrollService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public PayrollService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = Log.ForContext<PayrollService>();
    }

    /// <summary>
    /// جلب رواتب فترة معينة
    /// Get payroll by period
    /// </summary>
    public async Task<IEnumerable<Payroll>> GetByPeriodAsync(int year, int month)
    {
        return await _unitOfWork.Payroll.GetByPeriodAsync(year, month);
    }

    /// <summary>
    /// جلب راتب مع التفاصيل
    /// Get payroll with details
    /// </summary>
    public async Task<Payroll?> GetWithDetailsAsync(int id)
    {
        return await _unitOfWork.Payroll.GetWithDetailsAsync(id);
    }

    /// <summary>
    /// توليد رواتب الشهر
    /// Generate monthly payroll
    /// </summary>
    public async Task<bool> GenerateMonthlyPayrollAsync(int year, int month, int createdBy)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var result = await _unitOfWork.Payroll.GenerateMonthlyPayrollAsync(year, month, createdBy);

            if (result)
            {
                // حساب الرواتب لكل موظف
                var payrolls = await _unitOfWork.Payroll.GetByPeriodAsync(year, month);
                foreach (var payroll in payrolls.Where(p => p.Status == PayrollStatus.Draft))
                {
                    await CalculatePayrollAsync(payroll.Id);
                }

                await _unitOfWork.CommitTransactionAsync();
                _logger.Information("تم توليد رواتب الشهر: {Year}/{Month}", year, month);
            }

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.Error(ex, "خطأ أثناء توليد رواتب الشهر: {Year}/{Month}", year, month);
            throw;
        }
    }

    /// <summary>
    /// حساب الراتب
    /// Calculate payroll
    /// </summary>
    public async Task<bool> CalculatePayrollAsync(int payrollId)
    {
        try
        {
            var payroll = await _unitOfWork.Payroll.GetWithDetailsAsync(payrollId);
            if (payroll == null)
                return false;

            // جلب عناصر راتب الموظف
            var components = await _unitOfWork.Payroll.GetEmployeeSalaryComponentsAsync(payroll.EmployeeId);

            decimal totalAllowances = 0;
            decimal totalDeductions = 0;

            foreach (var component in components)
            {
                var amount = component.Amount > 0 ? component.Amount : 
                    (payroll.BasicSalary * component.Percentage / 100);

                if (component.SalaryComponent?.Type == SalaryItemType.Allowance)
                    totalAllowances += amount;
                else
                    totalDeductions += amount;
            }

            // حساب العمل الإضافي
            var hourlyRate = payroll.BasicSalary / 30 / 8; // 30 يوم، 8 ساعات
            var overtime = await _unitOfWork.Payroll.CalculateOvertimeAsync(
                payroll.EmployeeId, payroll.Year, payroll.Month, hourlyRate);

            payroll.TotalAllowances = totalAllowances;
            payroll.TotalDeductions = totalDeductions;
            payroll.OvertimeAmount = overtime;
            payroll.NetSalary = payroll.BasicSalary + totalAllowances + overtime - totalDeductions;

            var result = await _unitOfWork.Payroll.UpdateAsync(payroll);
            _logger.Information("تم حساب الراتب: {PayrollId} - صافي: {NetSalary}", payrollId, payroll.NetSalary);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء حساب الراتب: {PayrollId}", payrollId);
            throw;
        }
    }

    /// <summary>
    /// اعتماد الراتب
    /// Approve payroll
    /// </summary>
    public async Task<bool> ApprovePayrollAsync(int payrollId, int approverId)
    {
        try
        {
            var result = await _unitOfWork.Payroll.ApprovePayrollAsync(payrollId, approverId);
            if (result)
                _logger.Information("تم اعتماد الراتب: {PayrollId}", payrollId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء اعتماد الراتب: {PayrollId}", payrollId);
            throw;
        }
    }

    /// <summary>
    /// معالجة الدفع
    /// Process payment
    /// </summary>
    public async Task<bool> ProcessPaymentAsync(int payrollId)
    {
        try
        {
            var payroll = await _unitOfWork.Payroll.GetByIdAsync(payrollId);
            if (payroll == null || payroll.Status != PayrollStatus.Approved)
                return false;

            payroll.Status = PayrollStatus.Paid;
            payroll.PaymentDate = DateTime.Now;

            var result = await _unitOfWork.Payroll.UpdateAsync(payroll);
            if (result)
                _logger.Information("تم صرف الراتب: {PayrollId}", payrollId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطأ أثناء صرف الراتب: {PayrollId}", payrollId);
            throw;
        }
    }

    /// <summary>
    /// توليد كشف الراتب PDF
    /// Generate payslip PDF
    /// </summary>
    public async Task<byte[]> GeneratePayslipAsync(int payrollId)
    {
        // سيتم تنفيذه لاحقاً باستخدام PdfSharpCore
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    /// <summary>
    /// جلب عناصر الراتب
    /// Get salary components
    /// </summary>
    public async Task<IEnumerable<SalaryComponent>> GetSalaryComponentsAsync()
    {
        return await _unitOfWork.Payroll.GetSalaryComponentsAsync();
    }
}

