// Copyright 2008 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Resources;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("DNS for .NET 3.5")]
[assembly: AssemblyDescription("Domain Name System Library for .NET 3.5.")]
[assembly: AssemblyCompany("Andy Kernahan")]
[assembly: AssemblyProduct("AK.Net.Dns")]
[assembly: AssemblyCopyright("Copyright © Andy Kernahan 2008")]
[assembly: AssemblyTrademark("Andy Kernahan")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-GB")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
[assembly: AssemblyVersion("0.9.58.0")]
[assembly: AssemblyFileVersion("0.9.58.0")]
#if BUILD_DEBUG
[assembly: AssemblyConfiguration("BUILD_DEBUG")]
#elif BUILD_RELEASE
[assembly: AssemblyConfiguration("BUILD_RELEASE")]
#elif BUILD_DOCS
[assembly: AssemblyConfiguration("BUILD_DOCS")]
#else
#error "Unknown build configuration"
#endif