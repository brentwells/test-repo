using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Alm.AssetLiability.Report.TotalReturn.Models;
using Alm.Constants.Extensions;
using MoreLinq.Extensions;

namespace Alm.AssetLiability.Report.TotalReturn.Reports
{
    public static class TrorDataExtensions
    {
       
        public static Dictionary<DateTime, Dictionary<string, object>> AllocationHistory(this List<TRORData> trorData, DateTime startDate, DateTime endDate)
        {
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            //Get Current TROR Data
            var ret = new Dictionary<DateTime, Dictionary<string,object>>();
            //Group data By EndOfMonth and H2
            var byEom = trorData.Where(e => e.EndOfMonth.Date >= startDate.Eom().Date && e.EndOfMonth.Date <= endDate.Eom().Date).GroupBy(g => g.EndOfMonth).ToList();

            //Find all allocations that are greater than startdate and have a total value greater than 0
            var allocations = trorData.GroupBy(a => a.H2)
                .Where(o => o.Sum(s => s.MarketValue) > 0).Distinct();

            byEom.ForEach((d, i) =>
            {
                //Get monthly total
                var ttlEomEndMv = d.Sum(o => o.MarketValue ?? 0);
                //Create columns for eom
                var eomAllocation = allocations.ToDictionary(a => AllocationColumnDef.FromName(a.Key), v => (object)0.0M);
                //Add End of Month column
                eomAllocation.Add(AllocationColumnDef.EndOfMonth, d.Key);

                d.GroupBy(o => o.H2).ForEach(ga =>
                {
                    var val = ga.Sum(o => o.MarketValue ?? 0).SafeDivision(ttlEomEndMv);
                    eomAllocation[AllocationColumnDef.FromName(ga.Key)] = val;
                });
                
                //set order of Groups
                var grpdEomAllocations =
                    eomAllocation.OrderBy(o => o.Key.Order).ToDictionary(k => k.Key.Name, v => v.Value);
                
                ret.Add(d.Key, grpdEomAllocations);
            });
            
            return ret.OrderBy(o => o.Key).ToDictionary();
        }

        public static IEnumerable<PerformanceByPosition> PerformanceByPosition(this List<TRORData> trorData, DateTime endDate)
        {
            //Get Current TROR Data
            //Create PerformanceByPositions dataset from TRORData
            var currentPerfData = trorData.Where(d => d.EndOfMonth.Date == endDate.Date).Select(s => new PerformanceByPosition
            {
                PositionId = s.PositionId,
                EndOfMonth = s.EndOfMonth,
                Cusip = s.Cusip,
                HoldingType = s.HoldingTypeDesc,
                EndingWeight = s.EndValue,
                TRORPercentage = s.TotalReturnPerc,
                ContributionPercentage = s.TotalReturn,
                Status = s.PositionType.Replace("NoDisposed", "").Replace("Portion", "")
            }).OrderBy(c => c.PositionId).ToList();

            //Group data to sum up Performance data
            var byPosition = currentPerfData.GroupBy(g => new { g.PositionId, g.EndOfMonth, g.Cusip, g.HoldingType, g.Status });

            var ttlTROR = currentPerfData.Sum(s => s.ContributionPercentage);
            var ttlEndMV = currentPerfData.Sum(s => s.EndingWeight);
            var ret = byPosition.Select(pos => new PerformanceByPosition
                {
                    PositionId = pos.Key.PositionId,
                    EndOfMonth = pos.Key.EndOfMonth,
                    HoldingType = pos.Key.HoldingType,
                    Cusip = pos.Key.Cusip,
                    EndingWeight = pos.Sum(d => d.EndingWeight).SafeDivision(ttlEndMV),
                    TRORPercentage = pos.Sum(s => s.TRORPercentage),
                    ContributionPercentage = pos.Sum(s => s.ContributionPercentage).SafeDivision(ttlTROR),
                    Status = pos.Key.Status
                })
                .ToList();

            return ret;
        }

        public static IEnumerable<PerformanceBySector> PerformanceBySector(this List<TRORData> trorData, DateTime endDate)
        {
            //Get Current Data and create projection
            var currentData = trorData.Where(d => d.EndOfMonth.Date == endDate.Date).Select(o => new CurrentData
            {
                Active = o.PositionType,
                ZmType = o.ALMHierarchy,
                EndMV = o.MarketValue ?? 0,
                TrorDollar = o.TotalReturn ?? 0,
                BeginingMarketValue = o.BegValue ?? 0,
                WeightedCashFlow = o.WeightedCashFlow ?? 0
            }).ToList();

            //Group data by sector
            var ttlEndMV = currentData.Sum(c => c.EndMV); ///currentData.Sum(c => c.EndMV);
            var ttlTrorDollar = currentData.Sum(c => c.TrorDollar);
            var ttlBmv = currentData.Sum(c => c.BeginingMarketValue);
            var ttlWcf = currentData.Sum(c => c.WeightedCashFlow);

            var ret = currentData.GroupBy(o => o.ZmType).Select(o =>
            {
                var filteredBalanceData = o.Where(s =>
                    s.Active.Equals("TradeNotSettled") || s.Active.Equals("ActivePortionNoDisposed"));
                var pa = new PerformanceBySector
                {
                    Order = o.Key.Equals("Cash/Equivalent") ? 0 :
                        o.Key.Equals("Equity") ? 1 :
                        99, //Puts these at the top of the ordered list. Since the sectors are nondeterministic
                    Sector = o.Key,
                    EndingMarketValue = o.Sum(se => se.EndMV),
                    TrorDollar = o.Sum(se => se.TrorDollar),
                };
                var ttlBmv = o.Sum(sd => sd.BeginingMarketValue);
                var ttlWCF = o.Sum(sd => sd.WeightedCashFlow);
                pa.TrorPercent = pa.TrorDollar.SafeDivision(ttlBmv + ttlWCF);
                pa.EndingWeight = pa.EndingMarketValue.SafeDivision(ttlEndMV);
                pa.ContributionPercent = o.Sum(sd => sd.TrorDollar).SafeDivision(ttlTrorDollar);
                return pa;
            }).Where(o => o.EndingMarketValue > 0).ToList();

            //need to add Total line
            ret.Add(new PerformanceBySector
            {
                Order = 999,
                Sector = "Total",
                EndingMarketValue = ret.Sum(o => o.EndingMarketValue),
                EndingWeight = ret.Sum(o => o.EndingWeight),
                TrorDollar = ttlTrorDollar,
                ContributionPercent = ret.Sum(o => o.ContributionPercent),
                TrorPercent = ttlTrorDollar.SafeDivision(ttlBmv + ttlWcf)
            });

            return ret.OrderBy(o => o.Order).ThenBy(o => o.Sector);
        }

        public static IEnumerable<PortfolioAnalysis> PortfolioAnalysis(this List<TRORData> trorData, DateTime endDate, List<PositionGreekData> positionGreekData)
        {
            //Get Current Data and create projection
            var currentData = trorData.Where(d => d.EndOfMonth.Date == endDate.Date).Select(o => new CurrentData
            {
                Cusip = o.Cusip,
                Active = o.PositionType,
                ZmType = o.ALMHierarchy,
                Endpar = o.EndPar ?? 0,
                EndMV = o.MarketValue ?? 0,
                EndingMarketValue = o.EndValue ?? 0
            });

            //Join current data with PositionGreek data
            var joined = currentData.Join(positionGreekData, td => td.Cusip, pg => pg.Cusip,
                (d, greekData) =>
                {
                    var isActiveTraded = d.Active.Substring(0, 6).Equals("Active") ||
                                          d.Active.Substring(0, 6).Equals("Traded");
                    var projection = new CurrentData
                    {
                        Cusip = d.Cusip,
                        Active = d.Active,
                        ZmType = d.ZmType,
                        Endpar = d.Endpar,
                        EndMV = d.EndMV,
                        EndingMarketValue = d.EndingMarketValue,
                        EffectiveDuration = isActiveTraded ? greekData.EffectiveDuration ?? 0 : 0,
                        SpreadDuration = isActiveTraded ? greekData.SpreadDuration ?? 0 : 0,
                        Convexity = greekData.Cnvx ?? 0,
                    };
                    projection.DollarEffectiveDuration = (projection.EffectiveDuration / 100) * d.EndingMarketValue;
                    projection.DollarSpreadDuration = (projection.SpreadDuration / 100) * d.EndingMarketValue;
                    projection.DollarConvex = (projection.Convexity / 100) * d.EndingMarketValue;
                    return projection;
                });
            //Group data by sector
            var ret = joined.GroupBy(o => o.ZmType).Select(o =>
            {
                var filteredBalanceData = o.Where(s =>
                    s.Active.Equals("TradeNotSettled") || s.Active.Equals("ActivePortionNoDisposed"));
                var pa = new PortfolioAnalysis
                {
                    Order = o.Key.Equals("Cash/Equivalent") ? 0 :
                        o.Key.Equals("Equity") ? 1 :
                        999, //Puts these at the top of the ordered list. Since the sectors are nondeterministic
                    Sector = o.Key,
                    Balance = filteredBalanceData.Sum(se => se.Endpar),
                    MarketValue = o.Sum(se => se.EndMV),
                };
                pa.EffectiveDuration = o.Sum(sd => sd.DollarEffectiveDuration).SafeDivision(pa.MarketValue);
                pa.SpreadDuration = o.Sum(sd => sd.DollarSpreadDuration).SafeDivision(pa.MarketValue);
                pa.EffectiveConvexity = o.Sum(sd => sd.DollarConvex).SafeDivision(pa.MarketValue);
                return pa;
            }).Where(o => o.Balance > 0).ToList();

            //need to add Total line
            ret.Add(new PortfolioAnalysis
            {
                Order = 9999,
                Sector = "Total",
                Balance = ret.Sum(o => o.Balance),
                MarketValue = ret.Sum(o => o.MarketValue),
                EffectiveDuration = ret.WeightedAverage(val => val.EffectiveDuration, weight => weight.MarketValue),
                SpreadDuration = ret.WeightedAverage(val => val.SpreadDuration, weight => weight.MarketValue),
                EffectiveConvexity = ret.WeightedAverage(val => val.EffectiveConvexity, weight => weight.MarketValue)
            });

            return ret.OrderBy(o => o.Order).ThenBy(o => o.Sector);
        }

        public static SnapShot Snapshot(this List<TRORData> currentData, SnapshotRequest req)
        {
            var snapshot = new SnapShot
            {
                CompanyName = req.Client.Name,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                PerformanceMeasure = "Total Rate of Return, Modified Dietz Method",
                Benchmark = "Duration-Matched Swaps",
                EndingMarketValue = currentData.Sum(o => o.MarketValue),
                EndingPortfolioAllocation = currentData.ToList().EndingPortfolioAllocation(),
                PortfolioPerformance =
                    PortfolioPerformanceSummary(req.EndDate, req.PortfolioPerformance.Values.ToList()),
                CumulativeReturns = new Dictionary<string, object>
                {
                    {
                        req.Client.Acronym,
                        req.CumulativeReturns.Select(o => new {X = o.EndOfMonth, Y = o.TrorCumulativePercentage})
                    },
                    {
                        "Duration-Matched Swaps",
                        req.CumulativeReturns.Select(o => new
                        {
                            X = o.EndOfMonth, Y = o.BenchmarkTrorCumulativePercentage
                        })
                    }
                },
                NetAssetValue = new Dictionary<string, object>
                {
                    {
                        "Net Asset Value",
                        req.CumulativeReturns.Select(o => new {X = o.EndOfMonth, Y = o.EndingMarketValue})
                    }
                }
            };


            return snapshot;
        }

        private static Dictionary<string, object> PortfolioPerformanceSummary(DateTime endDate, List<PortfolioPerformance> data)
        {
            //Periods are hardcoded for now
            var periods = new List<Period>
            {
                Period.Month1, Period.Month3, Period.YearToDate, Period.Year1, Period.Cumulative, Period.AvgAnnual, Period.Year3AvgAnnual,
                Period.Year5AvgAnnual
            };
            var filtered = data.Where(o => periods.Contains(o.Period)).ToDictionary(k => GetPortfolioPerfPeriodKey(k, endDate.Date),
                v => (object)new
                {
                    PortfolioReturn = v.ClientTrorPercentage,
                    BenchReturn = v.BenchmarkTrorPercentage,
                    ExcessReturn = v.ExcessPercentage,
                    v.InformationRatio
                });

            return filtered;
        }

        private static string GetPortfolioPerfPeriodKey(PortfolioPerformance p, DateTime enddate)
        {
            return p.Period switch
            {
                var e when e.Equals(Period.Month1) => enddate.ToString("MMMM yyyy"),
                var e when e.Equals(Period.Month3) => "Quarter to Date",
                var e when e.Equals(Period.Year1) => "Last 12 Months",
                var e when e.Equals(Period.Cumulative) => "Since Inception",
                _ => p.Period.Name
            };
        }


        internal static Dictionary<string, decimal> EndingPortfolioAllocation(this List<TRORData> currentData)
        {
            var ret = new List<Tuple<string, decimal>>();
            var grandTotal = currentData.Sum(o => o.MarketValue.GetValueOrDefault());
            var grouped = currentData.GroupBy(o => o.ALMHierarchy).ToList();

            foreach (var grp in grouped)
            {
                var total = grp.Sum(o => o.MarketValue.GetValueOrDefault()).SafeDivision(grandTotal);
                if (Math.Round(total*100, 0) <= 0) continue;

                var item = new Tuple<string, decimal>(grp.Key, total);
                if (grp.Key.Equals("Cash/Equivalent", StringComparison.InvariantCultureIgnoreCase))
                    ret.Insert(0, item);
                else
                    ret.Add(item);
            }
            return ret.ToDictionary(k => k.Item1, v => v.Item2);
        }
    }
}
