// Copyright (C) 2008 Andy Kernahan
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

namespace AK.Net.Dns.Configuration
{
#if BUILD_DOCS
/// <summary>
/// Contains classes used to provide configuration information for
/// <see cref="AK.Net.Dns.IDnsTransport"/>,
/// <see cref="AK.Net.Dns.IDnsResolver"/> and 
/// <see cref="AK.Net.Dns.IDnsCache"/> implementations.
/// </summary>
/// <example>
/// <para>
/// The application configuration file below provides configuration for
/// most of the sections contained within the <see cref="AK.Net.Dns.Configuration"/>
/// namespace. It also shows a simple log4net configuration which appends
/// all messages to <see cref="System.Diagnostics.Trace"/>.
/// </para>
/// <para>    
/// <b>Note</b> that most applications will not need to provide configuration
/// information for the library as each section has sensible defaults. 
/// </para>
/// <para>
/// Please refer to the documentation for each section for further information.
/// </para>
/// <code lang="xml">
/// <![CDATA[
/// <?xml version="1.0" encoding="utf-8"?>
/// <configuration>
///   <configSections>
///     <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
///     <sectionGroup name="ak.net.dns">
///       <sectionGroup name="transports">
///         <section name="tcp" type="AK.Net.Dns.Configuration.DnsTcpTransportSection, AK.Net.Dns"/>  
///         <section name="udp" type="AK.Net.Dns.Configuration.DnsUdpTransportSection, AK.Net.Dns"/>
///         <section name="smart" type="AK.Net.Dns.Configuration.DnsSmartTransportSection, AK.Net.Dns"/>
///       </sectionGroup>      
///       <sectionGroup name="resolvers">
///         <section name="stub" type="AK.Net.Dns.Configuration.DnsStubResolverSection, AK.Net.Dns"/>  
///       </sectionGroup>
///     </sectionGroup>
///   </configSections>
///   <ak.net.dns>
///     <transports>
///       <tcp receiveTimeout="10000" sendTimeout="10000" maxIncomingMessageSize="1048576"/>
///       <udp receiveTimeout="10000" sendTimeout="10000" transmitRetries="4"/>
///       <smart receiveTimeout="10000" sendTimeout="10000"
///              udpTransport="AK.Net.Dns.IO.DnsUdpTransport, AK.Net.Dns"
///              tcpTransport="AK.Net.Dns.IO.DnsTcpTransport, AK.Net.Dns"/>
///     </transports>
///     <resolvers>
///       <stub discoverServers="true" nameSuffix="example.com."
///              transport="AK.Net.Dns.IO.DnsSmartTransport, AK.Net.Dns"
///              cache="AK.Net.Dns.Caching.DnsNullCache, AK.Net.Dns">
///         <!-- OpenDNS servers -->
///         <endpoint address="208.67.222.222"/>
///         <endpoint address="208.67.220.220"/>
///       </stub>
///     </resolvers>
///   </ak.net.dns>
///   <log4net>
///     <appender name="TraceAppender" type="log4net.Appender.TraceAppender">
///       <layout type="log4net.Layout.PatternLayout">
///         <conversionPattern value="%date{yyyy-MM-dd HH:mm:ss.ffff} %level %logger{1}[%thread]: %message%newline" />
///       </layout>
///     </appender>
///     <root>
///       <level value="DEBUG" />
///       <appender-ref ref="TraceAppender" />
///     </root>
///   </log4net>
/// </configuration>
/// ]]>
/// </code>
/// </example>
    internal sealed class NamespaceDoc { }
#endif
}
