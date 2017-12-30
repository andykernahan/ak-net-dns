# DNS for .NET

Very old, half completed, personal project intended to provide a complete .NET DNS API.

## AK.NDig

Demo project which provides a very basic `dig` like clone.

```
$ mono AK.NDig.exe
> ----------------------------------------------------------------------
> NDig for .NET 3.5 - v1.3.39.0
> Copyright © Andy Kernahan 2008
> ----------------------------------------------------------------------
> @ns1.google.com. aaaa maps.google.co.uk.
> ----------------------------------------------------------------------
> HEADER:
> 
> Opcode: QUERY, Rcode: NOERROR, Id: 26150, Flags: qr aa rd
> Question: 1, Answer: 2, Authority: 0, Additional: 0
> 
> QUESTION SECTION:
> 
> maps.google.co.uk.               IN     AAAA  
> 
> ANSWER SECTION:
> 
> maps.l.google.com.        300    IN     AAAA   2a00:1450:4009:802::200e
> maps.google.co.uk.        345600 IN     CNAME  maps.l.google.com.
> 
> QUERY STATS:
> 
> Elapsed: 33.62ms
> Server:  216.239.32.10:53 (ns1.google.com.)
> When:    Sat, 30 Dec 2017 08:28:58 GMT
> 
> ----------------------------------------------------------------------
```
