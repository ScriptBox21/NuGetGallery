// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Services.Entities;
using NuGetGallery.Auditing;

namespace NuGetGallery
{
    public class PackageVulnerabilitiesService : IPackageVulnerabilitiesService
    {
        private readonly IEntitiesContext _entitiesContext;

        public PackageVulnerabilitiesService(IEntitiesContext entitiesContext)
        {
            _entitiesContext = entitiesContext ?? throw new ArgumentNullException(nameof(entitiesContext));
        }

        public IReadOnlyDictionary<int, IReadOnlyList<PackageVulnerability>> GetVulnerabilitiesById(string id)
        {
            var result = new Dictionary<int, List<PackageVulnerability>>();
            var vulnerableRanges = _entitiesContext.VulnerableRanges.Where(vr => vr.PackageId == id);
            if (vulnerableRanges == null || !vulnerableRanges.Any())
            {
                return null;
            }

            foreach (var vulnerableRange in vulnerableRanges)
            {
                var vulnerability = _entitiesContext.Vulnerabilities.Where(v => v.Key == vulnerableRange.VulnerabilityKey).FirstOrDefault();
                if (vulnerability == null)
                {
                    continue;
                }

                var vulnerableRangePackages = _entitiesContext.VulnerableRangePackages
                    .Where(vrp => vrp.VulnerablePackageVersionRange_Key == vulnerableRange.Key);
                if (vulnerableRangePackages == null || !vulnerableRangePackages.Any())
                {
                    continue;
                }

                foreach (var vulnerableRangePackage in vulnerableRangePackages)
                {
                    var vulnerablePackage = vulnerableRangePackage.Package;
                    if (vulnerablePackage == null)
                    {
                        continue; // sanity check
                    }

                    if (result.TryGetValue(vulnerablePackage.Key, out var packageVulnerabilities))
                    {
                        packageVulnerabilities.Add(vulnerableRange.Vulnerability);
                    }
                    else
                    {
                        result.Add(vulnerablePackage.Key, new List<PackageVulnerability> { vulnerableRange.Vulnerability });
                    }
                }
            }

            if (result.Count == 0)
            {
                return null;
            }

            return result.ToDictionary(kv => kv.Key, kv => kv.Value as IReadOnlyList<PackageVulnerability>);
        }
    }
}