using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Edit
{
    public partial class Statistics : PageTemplateBase
    {
        // Timout
        private int timeOut;

        AzureTableStorageDataSource _dataSource;
        string _applicationName;

        IEnumerable<UserEntity> _users;

        int chartId = 0;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // This page is very slow so temporarily increase the timeout
            timeOut = Server.ScriptTimeout;
            Server.ScriptTimeout = 3600;    // One hour timeout
        }

        protected override void OnUnload(EventArgs e)
        {
            // Reset timout
            Server.ScriptTimeout = timeOut;
        }

        void RemoveEmptyCategories(ref string[] categories, ref object[] inData)
        {
            var processedCategories = new List<string>();
            var processedData = new List<object>();

            for (var i = 0; i < inData.Length; i++)
            {
                if ((int)inData[i] > 0)
                {
                    processedCategories.Add(categories[i]);
                    processedData.Add(inData[i]);
                }
            }
            categories = processedCategories.ToArray();
            inData = processedData.ToArray();
        }

        void AddChart(ITextControl literal, string chartTitle, string[] categories, object[] data, bool hideEmptyCategories = true)
        {
            // Should we remove all empty categories?
            if (hideEmptyCategories) RemoveEmptyCategories(ref categories, ref data);

            var chart = new Highcharts("chart" + ++chartId)
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Bar })
                .SetTitle(new Title { Text = chartTitle })
                .SetSubtitle(new Subtitle { Text = Bll.Resources.Marketing.ChartSubtitle })
                .SetXAxis(new XAxis
                {
                    Categories = categories,
                    Title = new XAxisTitle { Text = Bll.Resources.Marketing.ChartXAxisTitle }
                })
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle {
                        Text = Bll.Resources.Marketing.ChartYAxisTitle,
                        Align = AxisTitleAligns.High
                    }
                })
                .SetTooltip(new Tooltip { Formatter = Bll.Resources.Marketing.ChartToolTipJavaScript })
                .SetPlotOptions(new PlotOptions
                {
                    Bar = new PlotOptionsBar {
                        DataLabels = new PlotOptionsBarDataLabels { Enabled = true }
                    }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Right,
                    VerticalAlign = VerticalAligns.Top,
                    X = -100,
                    Y = 100,
                    Floating = true,
                    BorderWidth = 1,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml(Bll.Resources.Marketing.ChartBackgroundColor)),
                    Shadow = true
                })
                .SetCredits(new Credits { Enabled = false })
                .SetSeries(new[] {
                    new Series { Name = Bll.Resources.Marketing.BarTitle, Data = new Data(data) }
                }
            );

            literal.Text = chart.ToHtmlString();
        }

        #region Industries
        string[] GetIndustryCategories()
        {
            return new Industries().GetCategories;
        }

        object[] GetIndustryStatistics(string[] categories)
        {
            var stats = new object[categories.Length];
            for (var i = 0; i < categories.Length; i++)
            {
                var sum = _users.Count(user => user.Industry == categories[i]);
                stats[i] = sum;
            }
            return stats;
        }

        void AddIndustryChart()
        {
            var categories = GetIndustryCategories();
            AddChart(litIndustry, Bll.Resources.Marketing.Industries, categories, GetIndustryStatistics(categories));
        }
        #endregion

        #region Job categories
        string[] GetJobCategories()
        {
            return new JobCategory().GetCategories;
        }

        object[] GetJobStatistics(string[] categories)
        {
            var stats = new object[categories.Length];
            for (var i = 0; i < categories.Length; i++)
            {
                var sum = _users.Count(user => user.JobCategory == categories[i]);
                stats[i] = sum;
            }
            return stats;
        }

        void AddJobCategoryChart()
        {
            var categories = GetJobCategories();
            AddChart(litJobCategory, Bll.Resources.Marketing.JobCategories , categories, GetJobStatistics(categories));
        }
        #endregion

        #region Company sizes
        string[] GetCompanySizeCategories()
        {
            return new CompanySize().GetCategories;
        }

        object[] GetCompanySizes(string[] categories)
        {
            var stats = new object[categories.Length];
            for (var i = 0; i < categories.Length; i++)
            {
                var sum = _users.Count(user => user.CompanySize == categories[i]);
                stats[i] = sum;
            }
            return stats;
        }

        void AddCompanySizesChart()
        {
            var categories = GetCompanySizeCategories();
            AddChart(litCompanySizes, Bll.Resources.Marketing.CompanySizes , categories, GetCompanySizes(categories));
        }
        #endregion

        #region Countries
        string[] GetCountryCategories()
        {
            return Country.CountryList.Select(country => country.Split('|')[1]).ToArray();
        }

        object[] GetCountries(string[] categories)
        {
            var stats = new object[categories.Length];
            for (var i = 0; i < categories.Length; i++)
            {
                var sum = _users.Count(user => user.Country == categories[i]);
                stats[i] = sum;
            }
            return stats;
        }

        void AddCountriesChart()
        {
            var categories = GetCountryCategories();
            AddChart(litCountries, Bll.Resources.Marketing.Countries, categories, GetCountries(categories));
        }
        #endregion

        void AddCharts()
        {
            AddIndustryChart();
            AddJobCategoryChart();
            AddCompanySizesChart();
            AddCountriesChart();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _dataSource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            _users = _dataSource.GetUsers(_applicationName);

            AddCharts();
        }
    }
}