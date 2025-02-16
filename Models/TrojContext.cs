using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TrojWebApp.Models;

namespace TrojWebApp.Models
{
    public class TrojContext : DbContext
    {
        public TrojContext(DbContextOptions<TrojContext> options) : base(options)
        {
        }

        public DbSet<CasesModel> Cases { get; set; } = null!;
        public DbSet<CasesViewModel> CasesView { get; set; } = null!;
        public DbSet<CasesClientViewModel> CasesClientView { get; set; } = null!;
        public DbSet<ActiveCasesWorkingTimesViewModel> ActiveCasesWorkingTimesView { get; set; } = null!;
        public DbSet<CaseTypesModel> CaseTypes { get; set; } = null!;
        public DbSet<EmployeesModel> Employees { get; set; } = null!;
        public DbSet<PersonsModel> Persons { get; set; } = null!;
        public DbSet<PersonCasesModel> PersonCases { get; set; } = null!;
        public DbSet<PersonCasesViewModel> PersonCasesView { get; set; } = null!;
        public DbSet<PersonsAtCaseViewModel> PersonAtCasesView { get; set; } = null!;
        public DbSet<PersonAddressesModel> PersonAddresses { get; set; } = null!;
        public DbSet<WorkingTimesModel> WorkingTimes { get; set; } = null!;
        public DbSet<WorkingTimesViewModel> WorkingTimesView { get; set; } = null!;
        public DbSet<WorkingTimesSummarysModel> WorkingTimesSummaries { get; set; } = null!;
        public DbSet<TariffTypesModel> TariffTypes { get; set; } = null!;
        public DbSet<ActiveTariffTypesModel> ActiveTariffTypes { get; set; } = null!;
        public DbSet<BackgroundColorsModel> BackgroundColors { get; set; } = null!;
        public DbSet<TariffLevelsModel> TariffLevels { get; set; } = null!;
        public DbSet<TariffLevelsViewModel> TariffLevelsView { get; set; } = null!;
        public DbSet<CourtsModel> Courts { get; set; } = null!;
        public DbSet<PersonTypesModel> PersonTypes { get; set; } = null!;
        public DbSet<PhoneNumberTypesModel> PhoneNumberTypes { get; set; } = null!;
        public DbSet<PhoneNumbersModel> PhoneNumbers { get; set; } = null!;
        public DbSet<PhoneNumbersViewModel> PhoneNumbersView { get; set; } = null!;
        public DbSet<CaseNumbersModel> CaseNumbers { get; set; } = null!;
        public DbSet<CaseNumbersViewModel> CaseNumbersView { get; set; } = null!;
        public DbSet<CaseLogsModel> CaseLogs { get; set; } = null!;
        public DbSet<CaseLogsViewModel> CaseLogsView { get; set; } = null!;
        public DbSet<ClientFundingsModel> ClientFundings { get; set; } = null!;
        public DbSet<ClientFundingsViewModel> ClientFundingsView { get; set; } = null!;
        public DbSet<TotalSumModel> ClientFundingTotalSum { get; set; } = null!;
        public DbSet<InvoiceUnderlaysModel> InvoiceUnderlays { get; set; } = null!;
        public DbSet<InvoiceUnderlaysViewModel> InvoiceUnderlaysView { get; set; } = null!;
        public DbSet<InvoiceUnderlaysPartialViewModel> InvoiceUnderlaysPartialView { get; set; } = null!;
        public DbSet<InvoiceUnderlaySummarysModel> InvoiceUnderlaySummaries { get; set; } = null!;
        public DbSet<TotalSumModel> UnderlaySummariesTotalSum { get; set; } = null!;
        public DbSet<InvoiceWorkingTimesModel> InvoiceWorkingTimes { get; set; } = null!;
        public DbSet<InvoiceWorkingTimesViewModel> InvoiceWorkingTimesView { get; set; } = null!;
        public DbSet<UnderlayWorkingTimeSummaryModel> UnderlayWorkingTimeSummary { get; set; } = null!;
        public DbSet<InvoicesModel> Invoices { get; set; } = null!;
        public DbSet<InvoicesViewModel> InvoicesView { get; set; } = null!;
        public DbSet<InvoicesFortknoxModel> InvoicesFortknoxView { get; set; } = null!;
        public DbSet<InvoicesPartialViewModel> InvoicesPartialView { get; set; } = null!;
        public DbSet<InvoiceSummarysModel> InvoiceSummarys { get; set; } = null!;
        public DbSet<InvoiceSummarysViewModel> InvoiceSummarysView { get; set; } = null!;
        public DbSet<InvoiceClientFundsModel> InvoiceClientFunds { get; set; } = null!;
        public DbSet<ConfigurationsModel> Configurations { get; set; } = null!;
        public DbSet<NumberOfModel> NumberOf { get; set; } = null!;
        public DbSet<SumOfModel> SumOf { get; set; } = null!;
        public DbSet<TotalSumAndHoursModel> TotalSumAndHours { get; set; } = null!;
        public DbSet<WorkingTimesEconomyModel> WorkingTimesEconomy { get; set; } = null!;
        public DbSet<WorkingTimesPeriodEconomyModel> WorkingTimesPeriodEconomy { get; set; } = null!;
        public DbSet<SumOfWorkingTimesModel> SumOfWorkingTimes { get; set; } = null!;
        public DbSet<InvoiceEconomyModel> InvoiceEconomy { get; set; } = null!;
        public DbSet<MaxMinDateModel> MaxMinDate { get; set; } = null!;


        public DbSet<Pages3Model> Pages3 { get; set; } = null!;
        public DbSet<PageUsers3Model> PageUsers3 { get; set; } = null!;
        public DbSet<PageUsersView3Model> PageUsersView3 { get; set; } = null!;


        public DbSet<SubPages3Model> SubPages3 { get; set; } = null!;
        public DbSet<SubPagesView3Model> SubPagesView3 { get; set; } = null!;
        public DbSet<SubPageUsers3Model> SubPageUsers3 { get; set; } = null!;
        public DbSet<SubPageUsersView3Model> SubPageUsersView3 { get; set; } = null!;


        public DbSet<SubPageMenusModel> SubPageMenus { get; set; } = null!;
        public DbSet<SubPageMenusViewModel> SubPageMenusView { get; set; } = null!;
        public DbSet<SubPageMenusChildViewModel> SubPageMenusChildView { get; set; } = null!;
        

        public DbSet<IdModel> Id { get; set; } = null!;


        public DbSet<MenuPagesModel> MenuPages { get; set; }
        public DbSet<MenuPagesViewModel> MenuPagesView { get; set; }

        public DbSet<LoadTimesModel> LoadTimes { get; set; }


    }
}

