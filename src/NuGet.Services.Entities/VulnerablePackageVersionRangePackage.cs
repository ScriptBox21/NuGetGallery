// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NuGet.Services.Entities
{
    /// <summary>
    /// Join entity - represents a join of a <see cref="VulnerablePackageVersionRange"/> and a <see cref="Package"/>.
    /// </summary>
    public class VulnerablePackageVersionRangePackage
    {
        [Required]
        public int VulnerablePackageVersionRange_Key { get; set; }

        public VulnerablePackageVersionRange VulnerablePackageVersionRange { get; set; }

        [Required]
        public int Package_Key { get; set; }

        public Package Package { get; set; }
    }
}